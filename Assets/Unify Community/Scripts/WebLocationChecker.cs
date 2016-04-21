using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace OmiyaGames
{
    ///-----------------------------------------------------------------------
    /// <copyright file="WebLocationChecker.cs">
    /// Original code by andyman from Github:
    /// https://gist.github.com/andyman/e58dea85cce23cccecff
    /// Extra modifications by jcx from Github:
    /// https://gist.github.com/jcx/93a3fc93531911add8a8
    /// Taro Omiya made a couple of changes as well.
    /// </copyright>
    /// <author>andyman</author>
    /// <author>jcx</author>
    /// <author>Taro Omiya</author>
    ///-----------------------------------------------------------------------
    /// <summary>
    /// Add this script to an object in the first scene of your game.
    /// It doesn't do anything for non-webplayer builds. For webplayer
    /// builds, it checks the domain to make sure it contains at least
    /// one of the strings, or it will redirect the page to the proper
    /// URL for the game.
    /// </summary>
    public class WebLocationChecker : ISingletonScript
    {
        public enum State
        {
            NotUsed = -1,
            InProgress,
            EncounteredError,
            DomainMatched,
            DomainDidntMatch
        }

        ///<summary>
        /// If it is a webplayer, then the domain must contain any
        /// one or more of these strings, or it will be redirected.
        /// This array is ignored if empty (i.e. no redirect will occur).
        ///</summary>
        [SerializeField]
        List<string> domainMustContain;

        ///<summary>
        /// (optional) or fetch the domain list from this URL
        ///</summary>
        [Header("Getting Domain List Remotely")]
        [SerializeField]
        string remoteDomainListUrl;
        ///<summary>
        /// (optional) or fetch the domain list from this URL
        ///</summary>
        [SerializeField]
        char[] splitRemoteDomainListUrlBy = new char[] { ',' };
        ///<summary>
        /// (optional) game objects to deactivate while the domain checking is happening
        ///</summary>
        [SerializeField]
        GameObject[] waitObjects;

        ///<summary>
        /// If true, the game will force the webplayer to redirect to
        /// the URL below
        ///</summary>
        [Header("Redirect Options")]
        [SerializeField]
        bool forceRedirectIfDomainDoesntMatch = true;
        ///<summary>
        /// This is where to redirect the webplayer page if none of
        /// the strings in domainMustContain are found.
        ///</summary>
        [SerializeField]
        string redirectURL;

        State currentState = State.NotUsed;
        bool downloadedRemoteDomainList = false;

        #region Properties
        public State CurrentState
        {
            get
            {
                return currentState;
            }
            private set
            {
                currentState = value;
            }
        }

        public bool IsRemoteDomainListSuccessfullyDownloaded
        {
            get
            {
                return downloadedRemoteDomainList;
            }
            private set
            {
                downloadedRemoteDomainList = value;
            }
        }
        #endregion

        public override void SingletonAwake(Singleton instance)
        {
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
#if !UNITY_EDITOR
            // Shoot a coroutine if not in editor, and making Webplayer or WebGL
            StartCoroutine(CheckDomainList());
#else
            // Do a little bit of debugging
            StringBuilder buf = new StringBuilder();
            if (string.IsNullOrEmpty(remoteDomainListUrl) == false)
            {
                // Print remote domain list URL
                Debug.Log("Example URL to grab remote Domain List will look like:\n" + GenerateRemoteDomainList(buf));
            }
            if (string.IsNullOrEmpty(redirectURL) == false)
            {
                // Print redirect javascript
                Debug.Log("Redirect javascript will look like:\n" + GenerateRedirect(buf));
            }
#endif
#endif
        }

        public override void SceneAwake(Singleton instance)
        {
            // Do nothing
        }

        public void ForceRedirect()
        {
            ForceRedirect(new StringBuilder());
        }

        #region Helper Methods
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
        IEnumerator CheckDomainList()
        {
            // Update state
            CurrentState = State.InProgress;

            // Deactivate any objects
            int index = 0;
            for (index = 0; index < waitObjects.Length; ++index)
            {
                waitObjects[index].SetActive(false);
            }

            // Grab a domain list remotely
            StringBuilder buf = new StringBuilder();
            if (string.IsNullOrEmpty(remoteDomainListUrl) == false)
            {
                // Grab remote domain list
                WWW www = new WWW(GenerateRemoteDomainList(buf));
                yield return www;

                // Check if there were any errors
                if (string.IsNullOrEmpty(www.error) == true)
                {
                    // If none, split the text file we've downloaded, and add it to the list
                    domainMustContain.AddRange(www.text.Split(splitRemoteDomainListUrlBy));
                    IsRemoteDomainListSuccessfullyDownloaded = true;
                }
            }

            // Make sure there's at least one domain we need to check
            if (domainMustContain.Count > 0)
            {
                // parse the page's address
                bool isErrorEncountered = false;
                if (IsHostMatchingListedDomain(domainMustContain, out isErrorEncountered) == true)
                {
                    // Update state
                    CurrentState = State.DomainMatched;
                }
                else
                {
                    // Update state
                    if(isErrorEncountered == true)
                    {
                        CurrentState = State.EncounteredError;
                    }
                    else
                    {
                        CurrentState = State.DomainDidntMatch;
                    }

                    // Check if we should force redirecting the player
                    if (forceRedirectIfDomainDoesntMatch == true)
                    {
                        ForceRedirect(buf);
                    }
                }
            }
            else
            {
                // Update state
                CurrentState = State.NotUsed;
            }

            // Reactivate any objects
            for (index = 0; index < waitObjects.Length; ++index)
            {
                waitObjects[index].SetActive(true);
            }
        }

        string GenerateRemoteDomainList(StringBuilder buf)
        {
            buf.Length = 0;
            buf.Append(remoteDomainListUrl);
            buf.Append("?r=");
            buf.Append(UnityEngine.Random.value);
            return buf.ToString();
        }

        bool IsHostMatchingListedDomain(List<string> domainList, out bool encounteredError)
        {
            Uri uri;
            bool isTheCorrectHost = false;

            // Evaluate the URL
            encounteredError = true;
            if (Uri.TryCreate(Application.absoluteURL, UriKind.Absolute, out uri) == true)
            {
                // Indicate there were no errors
                encounteredError = false;

                // Check if the scheme isn't file (i.e. local file run on computer)
                if (uri.Scheme != "file")
                {
                    // Make sure host matches any one of the domains
                    for (int index = 0; index < domainList.Count; ++index)
                    {
                        if (uri.Host == domainList[index])
                        {
                            isTheCorrectHost = true;
                            break;
                        }
                    }
                }
                else
                {
                    // If this is a file run by a local computer, indicate domain matched
                    isTheCorrectHost = true;
                }
            }
            return isTheCorrectHost;
        }

        void ForceRedirect(StringBuilder buf)
        {
            if (string.IsNullOrEmpty(redirectURL) == false)
            {
                // Evaluate the javascript
                Application.ExternalEval(GenerateRedirect(buf));
            }
        }

        string GenerateRedirect(StringBuilder buf)
        {
            buf.Length = 0;
            buf.Append("window.top.location='");
            buf.Append(redirectURL);
            buf.Append("';");
            return buf.ToString();
        }
#endif
        #endregion
    }
}

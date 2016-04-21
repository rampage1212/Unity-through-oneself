using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

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
        ///<summary>
        /// If it is a webplayer, then the domain must contain any
        /// one or more of these strings, or it will be redirected.
        /// This array is ignored if empty (i.e. no redirect will occur).
        ///</summary>
        [SerializeField]
        List<string> domainMustContain;
        ///<summary>
        /// This is where to redirect the webplayer page if none of
        /// the strings in domainMustContain are found.
        ///</summary>
        [SerializeField]
        string redirectURL;

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

        public override void SingletonAwake(Singleton instance)
        {
            // Shoot a coroutine
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
            if (domainMustContain.Count > 0)
            {
                StartCoroutine(CheckDomainList());
            }
#endif
        }

        public override void SceneAwake(Singleton instance)
        {
            // Do nothing
        }

#if (UNITY_WEBPLAYER || UNITY_WEBGL)
        IEnumerator CheckDomainList()
        {
                // Deactivate any objects
                int index = 0;
                for (index = 0; index < waitObjects.Length; ++index)
                {
                    waitObjects[index].SetActive(false);
                }

                // Grab the domains List from 
                StringBuilder buf = new StringBuilder();
                if (string.IsNullOrEmpty(remoteDomainListUrl) == false)
                {
                    // Grab remote domain list
                    buf.Length = 0;
                    AppendRemoteDomainList(buf);

                    WWW www = new WWW(buf.ToString());
                    yield return www;

                    // Check if there were any errors
                    if (string.IsNullOrEmpty(www.error) == true)
                    {
                        // If none, split the text file we've downloaded, and add it to the list
                        domainMustContain.AddRange(www.text.Split(splitRemoteDomainListUrlBy));
                    }
                }

                // Generate javascript code
                buf.Length = 0;
                AppendDomainCheckingConditionals(buf);
                AppendRedirect(buf);

                // Evaluate the javascript
#if !UNITY_EDITOR
            Application.ExternalEval(buf.ToString());
#else
                buf.Insert(0, "Output javascript in WebLocationCheck.cs:\n");
                Debug.Log(buf.ToString());
#endif

                // Reactivate any objects
                for (index = 0; index < waitObjects.Length; ++index)
                {
                    waitObjects[index].SetActive(true);
                }
        }

        void AppendRemoteDomainList(StringBuilder buf)
        {
            buf.Append(remoteDomainListUrl);
            buf.Append("?r=");
            buf.Append(UnityEngine.Random.value);
        }

        void AppendDomainCheckingConditionals(StringBuilder buf)
        {
            buf.AppendLine("if((document.location.protocol != 'file:') &&");
            for (int index = 0; index < domainMustContain.Count; index++)
            {
                if (index > 0)
                {
                    buf.AppendLine(" &&");
                }

                buf.Append("  (document.location.host.indexOf('");
                buf.Append(domainMustContain[index]);
                buf.Append("') == -1)");
            }
            buf.AppendLine(")");
        }

        void AppendRedirect(StringBuilder buf)
        {
            buf.Append("{ window.top.location='");
            buf.Append(redirectURL);
            buf.Append("'; }");
        }
#endif
    }
}

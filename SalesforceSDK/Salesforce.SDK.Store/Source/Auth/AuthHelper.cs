﻿/*
 * Copyright (c) 2013, salesforce.com, inc.
 * All rights reserved.
 * Redistribution and use of this software in source and binary forms, with or
 * without modification, are permitted provided that the following conditions
 * are met:
 * - Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * - Neither the name of salesforce.com, inc. nor the names of its contributors
 * may be used to endorse or promote products derived from this software without
 * specific prior written permission of salesforce.com, inc.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */
using Salesforce.SDK.Adaptation;
using Salesforce.SDK.Auth;
using System;
using Windows.Security.Authentication.Web;

namespace Salesforce.SDK.Auth
{
    /// <summary>
    /// Store specific implementation if IAuthHelper
    /// </summary>
    public class AuthHelper : IAuthHelper
    {
        /// <summary>
        /// Bring up the WebAuthenticationBroker
        /// </summary>
        /// <param name="loginOptions"></param>
        /// <param name="clientLoginPage"></param>
        public void StartLoginFlow(LoginOptions loginOptions)
        {
            DoAuthFlow(loginOptions);
        }

        private async void DoAuthFlow(LoginOptions loginOptions)
        {
            Uri loginUri = new Uri(OAuth2.ComputeAuthorizationUrl(loginOptions));
            Uri callbackUri = new Uri(loginOptions.CallbackUrl);

            WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, loginUri, callbackUri)   ;
            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                Uri responseUri = new Uri(webAuthenticationResult.ResponseData.ToString());
                AuthResponse authResponse = OAuth2.ParseFragment(responseUri.Fragment.Substring(1));
                PlatformAdapter.Resolve<IAuthHelper>().EndLoginFlow(loginOptions, authResponse);
            }
        }

        /// <summary>
        /// Persist oauth credentials via the AccountManager
        /// </summary>
        /// <param name="loginOptions"></param>
        /// <param name="authResponse"></param>
        public void EndLoginFlow(LoginOptions loginOptions, AuthResponse authResponse)
        {
            AccountManager.CreateNewAccount(loginOptions, authResponse);
        }
    }
}

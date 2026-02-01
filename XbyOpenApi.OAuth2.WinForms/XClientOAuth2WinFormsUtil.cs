using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XbyOpenApi.OAuth2.WinForms
{
  /// <summary>
  /// Utils for creating a OAuth2 X client using WinForms
  /// </summary>
  public static class XClientOAuth2WinFormsUtil
  {
    /// <summary>
    /// Fetches access token and optionally a refresh token for an app labeled as "public client" using the Authorization Code flow with Proof Key for Code Exchange (PKCE): 
    /// an integrated WebView browser window is opened where the user logins in to X.
    /// The browser window is redirected to the <paramref name="redirectUrl"/> where an authorization code is parsed from the response .
    /// After this is done, the browser window closes, and the OAuth2 access token (and optionally a refresh token) is fetched.
    /// 
    /// In the next step, you can create the OpenApi client and initialize it in such a way that a Authorization header is added to every request.
    /// </summary>
    /// <param name="formParent">Parent window for showing a modal dialog</param>
    /// <param name="redirectUrl">Redirect url must be send to the server.</param>
    /// <param name="clientId">ClientID of the app</param>
    /// <param name="fetchRefreshToken">Should a refresh token also be fetched?</param>
    /// <param name="scopes">List of required scopes, must not be empty.</param>
    /// <param name="codeChallengeSHA256">Use SHA256 method for code challenge (default). 
    /// Set to "false" to use PLAIN method (not recommended, just for testing purposes)</param>
    /// <returns>Access token and optionally a refresh token, or NULL if the user canceled the browser dialog.</returns>
    public static GetTokenResponse? GetAccessToken_PublicClient(Form formParent, string clientId, string redirectUrl, bool fetchRefreshToken, List<string> scopes,
      bool codeChallengeSHA256 = true)
    {
      return GetAccessToken(formParent, confidentialClient: false, clientId: clientId, clientSecret: null, redirectUrl: redirectUrl, 
        fetchRefreshToken: fetchRefreshToken, scopes: scopes, codeChallengeSHA256: codeChallengeSHA256);
    }


    /// <summary>
    /// Fetches access token and optionally a refresh token for an app labeld as "confidential client" 
    /// using the Authorization Code flow with Proof Key for Code Exchange (PKCE): 
    /// an integrated WebView browser window is opened where the user logins in to X.
    /// The browser window is redirected to the <paramref name="redirectUrl"/> where an authorization code is parsed from the response .
    /// After this is done, the browser window closes, and the OAuth2 access token (and optionally a refresh token) is fetched.
    /// 
    /// In the next step, you can create the OpenApi client and initialize it in such a way that a Authorization header is added to every request.
    /// </summary>
    /// <param name="formParent">Parent window for showing a modal dialog</param>
    /// <param name="redirectUrl">Redirect url must be send to the server.</param>
    /// <param name="clientId">ClientID of the app</param>
    /// <param name="clientSecret">Client secret of the app, required</param>
    /// <param name="fetchRefreshToken">Should a refresh token also be fetched?</param>
    /// <param name="scopes">List of required scopes, must not be empty.</param>
    /// <param name="codeChallengeSHA256">Use SHA256 method for code challenge (default). 
    /// Set to "false" to use PLAIN method (not recommended, just for testing purposes)</param>
    /// <returns>Access token and optionally a refresh token, or NULL if the user canceled the browser dialog.</returns>
    public static GetTokenResponse? GetAccessToken_ConfidentialClient(Form formParent, string clientId, string clientSecret,
      string redirectUrl, bool fetchRefreshToken, List<string> scopes,
      bool codeChallengeSHA256 = true)
    {
      return GetAccessToken(formParent, confidentialClient: true, clientId: clientId, clientSecret: clientSecret, redirectUrl: redirectUrl, 
        fetchRefreshToken: fetchRefreshToken, scopes: scopes, codeChallengeSHA256: codeChallengeSHA256);
    }

    /// <summary>
    /// Fetches access token and optionally a refresh token using the Authorization Code flow with Proof Key for Code Exchange (PKCE): 
    /// an integrated WebView browser window is opened where the user logins in to X.
    /// The browser window is redirected to the <paramref name="redirectUrl"/> where an authorization code is parsed from the response .
    /// After this is done, the browser window closes, and the OAuth2 access token (and optionally a refresh token) is fetched.
    /// 
    /// In the next step, you can create the OpenApi client and initialize it in such a way that a Authorization header is added to every request.
    /// </summary>
    /// <param name="formParent">Parent window for showing a modal dialog</param>
    /// <param name="redirectUrl">Redirect url must be send to the server.</param>
    /// <param name="confidentialClient">true: the X app is configured as confidential client.
    /// FALSE: the app is configured as public client.</param>
    /// <param name="clientId">Required if <paramref name="confidentialClient"/> = true: clientID</param>
    /// <param name="clientSecret">Required if <paramref name="confidentialClient"/> = true: client secret</param>
    /// <param name="fetchRefreshToken">Should a refresh token also be fetched?</param>
    /// <param name="scopes">List of required scopes, must not be empty.</param>
    /// <param name="codeChallengeSHA256">Use SHA256 method for code challenge (default). 
    /// Set to "false" to use PLAIN method (not recommended, just for testing purposes)</param>
    /// <returns>Access token and optionally a refresh token, or NULL if the user canceled the browser dialog.</returns>
    private static GetTokenResponse? GetAccessToken(Form formParent, bool confidentialClient, string clientId, string? clientSecret, string redirectUrl, bool fetchRefreshToken, List<string> scopes,
      bool codeChallengeSHA256)
    {
      //Create a random code challenge with method "SHA256" or "PLAIN":
      OAuth2CodeChallenge codeChallenge;
      if (codeChallengeSHA256)
      {
        codeChallenge = OAuth2CodeChallenge.CreateSHA256(XClientOAuth2Util.CreateRandomString());
      }
      else
      {
        codeChallenge = OAuth2CodeChallenge.CreatePlain(XClientOAuth2Util.CreateRandomString());
      }

      using DialogOAuth2LoginWebView dialogLogin = new DialogOAuth2LoginWebView(clientId, redirectUrl, fetchRefreshToken, scopes, codeChallenge);

      if (dialogLogin.ShowDialog(formParent) == DialogResult.OK)
      {
        //Zum Code ein AccessToken holen:
        //Ignore nullable warning - the token is always not null if the form was closed successfully.
        string code = dialogLogin.AuthorizationCode!;

        Task<GetTokenResponse> taskTokenResponse = Task.Run(Task<GetTokenResponse>? () =>
        {
          if (confidentialClient)
          {
            return XClientOAuth2Util.GetAccessTokenByAuthorizationCodeCodeForConfidentialClient(code, redirectUrl, clientId, clientSecret!, codeChallenge);
          }
          else
          {
            return XClientOAuth2Util.GetAccessTokenByAuthorizationCodeCodeForPublicClient(code, redirectUrl, clientId, codeChallenge);
          }
        });
        taskTokenResponse.Wait();
        GetTokenResponse tokenResponse = taskTokenResponse.Result;

        return tokenResponse;
      }
      else
      {
        return null;
      }
    }
  }
}
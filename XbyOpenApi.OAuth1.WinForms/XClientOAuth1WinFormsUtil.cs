using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TinyOAuth1;
using XbyOpenApi.Core.Client;

namespace XbyOpenApi.OAuth1.WinForms
{
  /// <summary>
  /// Utils for creating a OAuth1 X client using WinForms
  /// </summary>
  public static class XClientOAuth1WinFormsUtil
  {
    /// <summary>
    /// Fetches access token and acces token secret using the pin based OAuth flow: an external browser window is opened where the user logins in to X.
    /// Afterwards, the browser presents a PIN.
    /// Meanwhile, a modal dialog is shown where the user has to enter the PIN.
    /// After this is done, the OAuth1 access token is fetched.
    /// In the next step, you can create the OpenApi client and initialize it in such a way that every request is signed with the OAuth 1 header,
    /// using consumer api key, consumer api key secret, access token and access token secret.
    /// 
    /// </summary>
    /// <param name="formParent">Parent window for showing a modal dialog</param>
    /// <param name="consumerApiKey">consumer api key, must not be empty</param>
    /// <param name="consumerApiKeySecret">consumre api key secret, must not be empty</param>
    /// <returns>Access token and acces token secret, or NULL if the user canceled the dialog for entering the pin.</returns>
    public static AccessTokenInfo? GetAccessToken_PinBasedOAuthFlowSync(Form formParent,
      string consumerApiKey, string consumerApiKeySecret)
    {
      var tinyOAuth = XClientOAuth1Util.InitTinyOAuth(consumerApiKey, consumerApiKeySecret);

      // Get the request token and request token secret
      // Here, only pin based authentication is possible due to the fix "oauth_callback=oob" argument in TinyOAuth.
      // If we could specify a different callback url, we might use an embedded web browser.
      Task<RequestTokenInfo> taskRequestTokenInfo = Task.Run(async () => await tinyOAuth.GetRequestTokenAsync());
      taskRequestTokenInfo.Wait();
      RequestTokenInfo requestTokenInfo = taskRequestTokenInfo.Result;

      // Construct the authorization url
      var authorizationUrl = tinyOAuth.GetAuthorizationUrl(requestTokenInfo.RequestToken);

      // Go to the URL so that X authenticates the user and gives him a PIN code.
      Process.Start(new ProcessStartInfo(authorizationUrl)
      {
        UseShellExecute = true
      });

      // Ask the user to enter the pin code given by X
      using DialogEnterPIN dialogPIN = new DialogEnterPIN();
      
      if (dialogPIN.ShowDialog(formParent) != DialogResult.OK)
      {
        return null;
      }
      string pinCode = dialogPIN.PIN;

      Task<AccessTokenInfo> taskAccessTokenInfo = Task.Run(async () => await tinyOAuth.GetAccessTokenAsync(requestTokenInfo.RequestToken, requestTokenInfo.RequestTokenSecret, pinCode));
      taskAccessTokenInfo.Wait();
      AccessTokenInfo accessTokenInfo = taskAccessTokenInfo.Result;

      return accessTokenInfo;
    }
  }
}

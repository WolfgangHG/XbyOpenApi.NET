using Microsoft.Kiota.Http.HttpClientLibrary;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using TinyOAuth1;
using XbyOpenApi.Core.Client;

namespace XbyOpenApi.OAuth1
{
  /// <summary>
  /// Contains the functions to initialize an OAuth1 client and fetch an access token with several steps.
  /// 
  /// Currently, only the pin based version of "Obtaining Access Tokens using 3-legged OAuth flow" is supported,
  /// as TinyOAuth does not support defining a custom callback url.
  /// </summary>
  public class XClientOAuth1Util
  {
    /// <summary>
    /// Creates a X API client based on an OAuth1 access token.
    /// </summary>
    /// <param name="consumerApiKey">Consumer api key of the X application, required.</param>
    /// <param name="consumerApiKeySecret">Consumer api key secret of the X application, required.</param>
    /// <param name="oAuth1AccessToken">Access token, required</param>
    /// <param name="oAuth1AccessTokenSecret">Access token secret, required</param>
    /// <returns>X Client ready for X communication</returns>
    public static XClient InitXClient(string consumerApiKey, string consumerApiKeySecret,
      string oAuth1AccessToken, string oAuth1AccessTokenSecret)
    {
      TinyOAuth tinyOAuth = InitTinyOAuth(consumerApiKey, consumerApiKeySecret);

      var authProvider = new XOAuth1AuthenticationProvider(tinyOAuth, oAuth1AccessToken, oAuth1AccessTokenSecret);
      // Create request adapter using the HttpClient-based implementation
      var adapter = new HttpClientRequestAdapter(authProvider);

      XClient xClient = new XClient(adapter);
      return xClient;
    }

    /// <summary>
    /// Creates a TinyOAuth instance that is initialized for usage with the X api: defines the OAuth1 urls in the TinyOAuth config
    /// and set consumer api key and consumer api key secret.
    /// 
    /// This TinyOAuth instance can be used to start the PIN-based OAuth flow, or it can be used to
    /// set the authorization header when making service calls.
    /// </summary>
    /// <param name="consumerApiKey">Consumer api key of the X application, required.</param>
    /// <param name="consumerApiKeySecret">Consumer api key secret of the X application, required.</param>
    /// <returns>A TinyOAuth instance</returns>
    public static TinyOAuth InitTinyOAuth(string consumerApiKey, string consumerApiKeySecret)
    {
      if (string.IsNullOrWhiteSpace(consumerApiKey))
      {
        throw new ArgumentNullException(nameof(consumerApiKey));
      }
      if (string.IsNullOrWhiteSpace(consumerApiKeySecret))
      {
        throw new ArgumentNullException(nameof(consumerApiKeySecret));
      }

      TinyOAuthConfig config = new TinyOAuthConfig
      {
        AccessTokenUrl = "https://api.x.com/oauth/access_token",
        AuthorizeTokenUrl = "https://api.x.com/oauth/authorize",
        RequestTokenUrl = "https://api.x.com/oauth/request_token",
        ConsumerKey = consumerApiKey,
        ConsumerSecret = consumerApiKeySecret
      };

      TinyOAuth tinyOAuth = new TinyOAuth(config);

      return tinyOAuth;
    }

    /// <summary>
    /// This method shoud be invoked in the callback to the redirect url (e.g. in an embedded web browser), and it parses the OAuth 
    /// verifier from the query argument.
    /// </summary>
    /// <remarks>
    /// A sample url could be this:
    /// 
    /// <![CDATA[
    /// http://localhost/?oauth_token=vcwzUAAAAAABoVccAAABnKk-7ec&oauth_verifier=H2IZ2hifZs0W56C65ZDnpjqnrJltLvo2
    /// ]]>
    /// </remarks>
    /// <param name="strQuery">The query string that was called. Should start with the "redirect url" that was used in
    /// <see cref="TinyOAuth.GetAuthorizationUrl"/>. The authorization verifier is contained in a parameter "oauth_verifier".</param>
    /// <param name="requestTokenInfo">This is the request token that was sent to the server as part of the authorization url.
    /// The server provides it to the redirect url, and this method checks that it matches</param>
    /// <returns>Authorization code or NULL of the request contains "error"</returns>
    /// <exception cref="Exception">If request does not contain required arguments or does not contain a verifier.</exception>
    public static GetAuthorizationResponse? ParseAutorizationCode(string strQuery, RequestTokenInfo requestTokenInfo)
    {
      NameValueCollection queryArgs = HttpUtility.ParseQueryString(strQuery);


      //It seems there is no error redirect if the "cancel" button is clicked during authorization.

      //Success: get code.
      string token = queryArgs["oauth_token"];
      if (token == null)
      {
        throw new Exception("Error - response did not contain 'oauth_token': " + strQuery);
      }

      //This token must be identical to the request token used for creating the authorization url.
      if (token != requestTokenInfo.RequestToken)
      {
        throw new Exception($"Error - request token from response ({token}) does not match token used for creating the authorization url ({requestTokenInfo.RequestToken}");
      }

      string verifier = queryArgs["oauth_verifier"];
      if (verifier == null)
      {
        throw new Exception("Error - response did not contain 'oauth_verifier': " + strQuery);
      }

      return new GetAuthorizationResponse() { AuthorizationToken = token, AuthorizationVerifier = verifier };

    }
  }
}

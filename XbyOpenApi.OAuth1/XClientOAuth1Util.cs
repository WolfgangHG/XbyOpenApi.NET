using Microsoft.Kiota.Http.HttpClientLibrary;
using System;
using System.Threading.Tasks;
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
    /// Performs step 1 of the OAuth flow for fetching a user access token: 
    /// calls the "request_token" endpoint of the X api and returns the request token.
    /// 
    /// </summary>
    /// <param name="tinyOAuth">A TinyOAuth instance created by <see cref="InitTinyOAuth"/> </param>
    /// <returns>Request token and request token secret</returns>
    public static async Task<RequestTokenInfo> GetRequestToken(TinyOAuth tinyOAuth)
    {
      if (tinyOAuth == null)
      {
        throw new ArgumentNullException(nameof(tinyOAuth));
      }

      // Get the request token and request token secret
      // Here, only pin based authentication is possible due to the fix "oauth_callback=oob" argument in TinyOAuth
      return await tinyOAuth.GetRequestTokenAsync();

    }

    /// <summary>
    /// Prepares step 2 of the OAuth flow for fetching a user access token: 
    /// builds the authorization url and returns it.
    /// 
    /// The caller should open a browser window containing and wait for the user login to be finished.
    /// </summary>
    /// <param name="tinyOAuth">A TinyOAuth instance created by <see cref="InitTinyOAuth"/>, not null </param>
    /// <param name="requestTokenInfo">The Request token and secret fetched in <see cref="GetRequestToken"/>, not null</param>
    /// <returns>Authorization Url</returns>
    public static string GetAuthorizationUrl(TinyOAuth tinyOAuth, RequestTokenInfo requestTokenInfo)
    {
      if (tinyOAuth == null)
      {
        throw new ArgumentNullException(nameof(tinyOAuth));
      }
      if (requestTokenInfo == null)
      {
        throw new ArgumentNullException(nameof(requestTokenInfo));
      }

      // Construct the authorization url
      var authorizationUrl = tinyOAuth.GetAuthorizationUrl(requestTokenInfo.RequestToken);

      return authorizationUrl;

    }

    /// <summary>
    /// Performs step 3 of the OAuth flow for fetching a user access token:
    /// Fetches Access Token and Access Token secret based on a request token
    /// </summary>
    /// <param name="tinyOAuth">A TinyOAuth instance created by <see cref="InitTinyOAuth"/> </param>
    /// <param name="requestTokenInfo">The Request token and secret fetched in <see cref="GetRequestToken"/>, not null</param>
    /// <param name="pinCode">A pin code that was shown to the user after opening the authorization url returned from <see cref="GetAuthorizationUrl"/></param>
    /// <returns></returns>
    public static async Task<AccessTokenInfo> GetAccessToken(TinyOAuth tinyOAuth, RequestTokenInfo requestTokenInfo,
      string pinCode)
    {
      if (tinyOAuth == null)
      {
        throw new ArgumentNullException(nameof(tinyOAuth));
      }
      if (requestTokenInfo == null)
      {
        throw new ArgumentNullException(nameof(requestTokenInfo));
      }

      AccessTokenInfo accessTokenInfo = await tinyOAuth.GetAccessTokenAsync(requestTokenInfo.RequestToken, requestTokenInfo.RequestTokenSecret, pinCode);

      return accessTokenInfo;
    }
  }
}

using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using XbyOpenApi.Core.Client;

namespace XbyOpenApi.OAuth2
{
  /// <summary>
  /// Contains the functions to initialize an OAuth2 client and fetch an access token with several steps.
  /// 
  /// </summary>
  public class XClientOAuth2Util
  {
    #region Constants
    //List of scopes from https://docs.x.com/fundamentals/authentication/oauth-2-0/authorization-code#scopes

    /// <summary>
    /// All the Tweets you can view, including Tweets from protected accounts.
    /// </summary>
    public const string SCOPE_TWEET_READ = "tweet.read";
    /// <summary>
    /// Tweet and Retweet for you.
    /// </summary>
    public const string SCOPE_TWEET_WRITE = "tweet.write";
    /// <summary>
    /// Hide and unhide replies to your Tweets.
    /// </summary>
    public const string SCOPE_TWEET_MODERATE_WRITE = "tweet.moderate.write";
    /// <summary>
    /// Email from an authenticated user.
    /// </summary>
    public const string SCOPE_USERS_EMAIL = "users.email";
    /// <summary>
    /// Any account you can view, including protected accounts.
    /// </summary>
    public const string SCOPE_USERS_READ = "users.read";
    /// <summary>
    /// People who follow you and people who you follow.
    /// </summary>
    public const string SCOPE_FOLLOWS_READ = "follows.read";
    /// <summary>
    /// Follow and unfollow people for you.
    /// </summary>
    public const string SCOPE_FOLLOWS_WRITE = "follows.write";
    /// <summary>
    /// Stay connected to your account until you revoke access.
    /// 
    /// Used to fetch a refresh token, not meant for public usage of this library.
    /// </summary>
    private const string SCOPE_OFFLINE_ACCESS = "offline.access";
    /// <summary>
    /// All the Spaces you can view.
    /// </summary>
    public const string SCOPE_SPACE_READ = "space.read";
    /// <summary>
    /// Accounts you’ve muted.
    /// </summary>
    public const string SCOPE_MUTE_READ = "mute.read";
    /// <summary>
    /// Mute and unmute accounts for you.
    /// </summary>
    public const string SCOPE_MUTE_WRITE = "mute.write";
    /// <summary>
    /// Tweets you’ve liked and likes you can view.
    /// </summary>
    public const string SCOPE_LIKE_READ = "like.read";
    /// <summary>
    /// Like and un-like Tweets for you.
    /// </summary>
    public const string SCOPE_LIKE_WRITE = "like.write";
    /// <summary>
    /// Lists, list members, and list followers of lists you’ve created or are a member of, including private lists.
    /// </summary>
    public const string SCOPE_LIST_READ = "list.read";
    /// <summary>
    /// Create and manage Lists for you.
    /// </summary>
    public const string SCOPE_LIST_WRITE = "list.write";
    /// <summary>
    /// Accounts you’ve blocked.
    /// </summary>
    public const string SCOPE_BLOCK_READ = "block.read";
    /// <summary>
    /// Block and unblock accounts for you.
    /// </summary>
    public const string SCOPE_BLOCK_WRITE = "block.write";
    /// <summary>
    /// Get Bookmarked Tweets from an authenticated user.
    /// </summary>
    public const string SCOPE_BOOKMARK_READ = "bookmark.read";
    /// <summary>
    /// Bookmark and remove Bookmarks from Tweets.
    /// </summary>
    public const string SCOPE_BOOKMARK_WRITE = "bookmark.write";
    /// <summary>
    /// Upload media.
    /// </summary>
    public const string SCOPE_MEDIA_WRITE = "media.write";

    #endregion

    /// <summary>
    /// Creates a X API client based on an OAuth2 access token.
    /// </summary>
    /// <param name="oAuth2AccessToken">OAuth2 access token</param>
    /// <returns>X Client ready for X communication</returns>
    public static XClient InitXClient(string oAuth2AccessToken)
    {
      var accessTokenProvider = new XOAuth2AccessTokenProvider(oAuth2AccessToken);
      var authProvider = new BaseBearerTokenAuthenticationProvider(accessTokenProvider);
      // Create request adapter using the HttpClient-based implementation
      var adapter = new HttpClientRequestAdapter(authProvider);

      XClient xClient = new XClient(adapter);
      return xClient;
    }


    /// <summary>
    /// Builds the URL that is used for step 1: a client opens the authorize url with a browser, the user has to log in,
    /// and finally the redirect url is called which receives an authentication code that can be used for fetching an access token.
    /// </summary>
    /// <param name="clientId">ClientId, required</param>
    /// <param name="redirectUrl">Redirect url, e.g. "http://localhost" (not url encoded)</param>
    /// <param name="scopes">List of scopes, should contain at least one entry.</param>
    /// <param name="fetchRefreshToken">Do we also want to fetch a refresh token? This means that an additional scope "offline.access" is added to the scope list.</param>
    /// <param name="state">A (random) string that is sent back to the redirect url and that you provide to verify against CSRF attacks. 
    /// The length of this string can be up to 500 characters.
    /// You could create a random string by calling <see cref="CreateRandomString"/></param>
    /// <returns>URL to open in a browser.</returns>
    public static string GetAuthorizeUrl(string clientId, string redirectUrl, List<string> scopes, bool fetchRefreshToken,
      string state)
    {
      if (string.IsNullOrWhiteSpace(clientId) == true)
      {
        throw new ArgumentNullException(nameof(clientId));
      }
      if (string.IsNullOrWhiteSpace(redirectUrl) == true)
      {
        throw new ArgumentNullException(nameof(redirectUrl));
      }
      if (string.IsNullOrEmpty(state) == true)
      {
        throw new ArgumentNullException(nameof(state));
      }
      //State must not be longer than 500 chars:
      if (state.Length > 500)
      {
        throw new ArgumentException("State must not be longer than 500 chars.", nameof(state));
      }
      if(scopes == null)
      {
        throw new ArgumentNullException(nameof(scopes));
      }

      //Scopes: an empty list does not make sense..
      string scopesArg = string.Empty;
      //Concatenate scopes with a "space" char (%20)
      foreach (string scope in scopes)
      {
        if (string.IsNullOrEmpty(scopesArg) == false)
        {
          scopesArg += "%20";
        }
        scopesArg += scope;
      }

      //Append "offline.access" if a refresh token is requested.
      if (fetchRefreshToken == true)
      {
        if (string.IsNullOrEmpty(scopesArg) == false)
        {
          scopesArg += "%20";
        }
        scopesArg += SCOPE_OFFLINE_ACCESS;
      }

      string encodedRedirectURL = WebUtility.UrlEncode(redirectUrl);
      //Also encode state:
      string encodedState = WebUtility.UrlEncode(state);

      bool todo_CreateCodeChallenge; //And return to sender.
      string authorizeUrl = $"https://x.com/i/oauth2/authorize?response_type=code&client_id={clientId}&redirect_uri={encodedRedirectURL}&scope={scopesArg}&state={encodedState}&code_challenge=challenge&code_challenge_method=plain";

      return authorizeUrl;
    }

    /// <summary>
    /// This method shoud be invoked in the callback to the redirect url (e.g. in an embedded web browser), and it parses the authorization
    /// code from the query argument.
    /// </summary>
    /// <param name="strQuery">The query string that was called. Should start with the "redirect url" that was used in
    /// <see cref="GetAuthorizeUrl"/>. The authorization code is contained in a parameter "code".
    /// It might also contain a parameter "error", which means that the user has clicked "Cancel"</param>
    /// <param name="expectedState">This is the "state" parameter that was sent to the server as part of the authorization url 
    /// (<see cref="GetAuthorizeUrl"/>). The server provides it to the redirect url, and this method checks that it matches.</param>
    /// <returns>Authorization code or NULL of the request contains "error"</returns>
    /// <exception cref="OAuth2Exception">If state does not match or request does not contain code.</exception>
    public static string? ParseAutorizationCode(string strQuery, string expectedState)
    {
      NameValueCollection queryArgs = HttpUtility.ParseQueryString(strQuery);

      //Check "state":
      string receivedState = queryArgs["state"];
      if (receivedState != expectedState)
      {
        throw new OAuth2Exception($"Redirect url contains parameter 'state' with value \"{receivedState}\", but this does not match expected value \"{expectedState}\"");
      }

      //If the query contains an argument "error"...
      if (queryArgs["error"] != null)
      {
        //This happens if the "Cancel"-Button is clicked after login in the "Authorize" step.
        return null;
      }
      else
      {
        //Success: get code.
        string code = queryArgs["code"];
        if (code == null)
        {
          throw new OAuth2Exception("Error - response did not contain 'code': " + strQuery);
        }

        return code;

      }
    }

    /// <summary>
    /// Gets an access token from a authorization code.
    /// The response might also contain a refresh token, dependending on the requested value.
    /// 
    /// This method is used if the app is configured as "public client".
    /// </summary>
    /// <param name="authorizationCode">Authoriziation code sent to the redirect url</param>
    /// <param name="redirectUrl">Redirect url must be send to the server.</param>
    /// <param name="clientId">Required: clientID</param>
    /// <exception cref="OAuth2Exception">An error in fetching the token occured</exception>
    /// <returns>Token response, might also contain a refresh token.</returns>
    public static async Task<GetTokenResponse> GetAccessTokenByAuthorizationCodeCodeForPublicClient(string authorizationCode, string redirectUrl,
      string clientId)
    {
      return await GetAccessTokenByAuthorizationCodeCode(authorizationCode, redirectUrl, false, clientId: clientId, clientSecret: null);
    }

    /// <summary>
    /// Gets an access token from a authorization code.
    /// The response might also contain a refresh token, dependending on the requested value.
    /// 
    /// This method is used if the app is configured as "confidential client".
    /// </summary>
    /// <param name="authorizationCode">Authoriziation code sent to the redirect url</param>
    /// <param name="redirectUrl">Redirect url must be send to the server.</param>
    /// <param name="clientId">Required: clientID</param>
    /// <param name="clientSecret">Required: client secret</param>
    /// <exception cref="OAuth2Exception">An error in fetching the token occured</exception>
    /// <returns>Token response, might also contain a refresh token.</returns>
    public static async Task<GetTokenResponse> GetAccessTokenByAuthorizationCodeCodeForConfidentialClient(string authorizationCode, string redirectUrl,
      string clientId, string clientSecret)
    {
      return await GetAccessTokenByAuthorizationCodeCode(authorizationCode, redirectUrl, true, clientId, clientSecret);
    }


    /// <summary>
    /// Gets an access token from an authorization code.
    /// The response might also contain a refresh token, dependending on the requested value.
    /// </summary>
    /// <param name="authorizationCode">Authoriziation code sent to the redirect url</param>
    /// <param name="redirectUrl">Redirect url must be send to the server.</param>
    /// <param name="confidentialClient">true: the X app is configured as confidential client.
    /// FALSE: the app is configured as public client.</param>
    /// <param name="clientId">Required if <paramref name="confidentialClient"/> = true: clientID</param>
    /// <param name="clientSecret">Required if <paramref name="confidentialClient"/> = true: client secret</param>
    /// <exception cref="OAuth2Exception">An error in fetching the token occured</exception>
    /// <returns>Token response, might also contain a refresh token.</returns>
    private static async Task<GetTokenResponse> GetAccessTokenByAuthorizationCodeCode(string authorizationCode, string redirectUrl, bool confidentialClient,
      string? clientId, string? clientSecret)
    {
      Dictionary<string, string> formData = new Dictionary<string, string>();
      formData.Add("code", authorizationCode);
      formData.Add("grant_type", "authorization_code");
      if (confidentialClient == false)
      {
        if (clientId == null)
        {
          throw new ArgumentNullException(nameof(clientId));
        }
        formData.Add("client_id", clientId);
      }
      //The url will be automaticall encoded.
      formData.Add("redirect_uri", redirectUrl);
      formData.Add("code_verifier", "challenge");

      string url = $"https://api.x.com/2/oauth2/token";


      HttpController.HttpResponse response = await HttpController.ExecuteRequest(url, HttpMethod.Post, requestMessage =>
      {
        //ClientID and Secret must be specified if the type is "ConfidentialClient".
        //With type "Public Client", it also works, but I think the client secret does not matter here. Only the client Id is used.
        if (confidentialClient)
        {
          if (clientId == null)
          {
            throw new ArgumentNullException(nameof(clientId));
          }
          if (clientSecret == null)
          {
            throw new ArgumentNullException(nameof(clientSecret));
          }

          string userid = clientId;
          string password = clientSecret;
          //Concat with colon and encode base64:
          string header = userid + ":" + password;
          header = Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
          requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", header);
        }
        //requestMessage.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        requestMessage.Content = new FormUrlEncodedContent(formData);
      });

      //In case of an error rethrow the exception:
      if (response.IsOK == false)
      {
        throw new OAuth2Exception($"Error fetching the access token from URL {url}" + Environment.NewLine + response.ToString());
      }

      //"Data" must not be null.
      if(string.IsNullOrEmpty(response.Data))
      {
        throw new NullReferenceException("Reponse data is null - we cannot deserialize a token response object");
      }

      //Here, "data" is not null/empty - use the "null forgiving operator".
      GetTokenResponse? tokenresponse = JsonSerializer.Deserialize<GetTokenResponse>(response.Data!);

      if (tokenresponse == null)
      {
        //Must not happen. But as "Deserialize" might return null, we have to check this due the "nullable" project. 
        throw new InvalidOperationException("Deserialized TokenResponse was null (Data: " + response.Data + ")");
      }

      return tokenresponse;
    }

    /// <summary>
    /// Fetches an access token from a refresh token.
    /// 
    /// Note: a refresh token can only be used once.
    /// </summary>
    /// <param name="clientId">ClientId, must be provided</param>
    /// <param name="refreshToken">Refresh Token, must not be empty.</param>
    /// <returns>Access token, contains also a new refresh token</returns>
    /// <exception cref="ArgumentException">If any of the paramters is null/empty</exception>
    /// <exception cref="OAuth2Exception">Errors from the X call.</exception>
    public static async Task<GetTokenResponse> GetAccessTokenFromRefreshToken(string clientId, string refreshToken)
    {
      StringBuilder sbFehler = new StringBuilder();
      if (string.IsNullOrWhiteSpace(clientId) == true)
      {
        throw new ArgumentException("ClientID is required.");
      }

      if (string.IsNullOrEmpty(refreshToken) == true)
      {
        throw new ArgumentException("RefreshToken is required.");
      }

      Dictionary<string, string> formData = new Dictionary<string, string>();
      formData.Add("refresh_token", refreshToken);
      formData.Add("grant_type", "refresh_token");
      formData.Add("client_id", clientId);

      string strUrl = $"https://api.x.com/2/oauth2/token";

      HttpController.HttpResponse response = await HttpController.ExecuteRequest(strUrl, HttpMethod.Post, requestMessage =>
      {
        //Automatically done by setting "FormUrlEncodedContent".
        //requestMessage.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        requestMessage.Content = new FormUrlEncodedContent(formData);
      });

      if (response.IsOK == false)
      {
        throw new OAuth2Exception ("Fetching refresh token failed: "  + response.ToString());
      }

      //"Data" must not be null. Check anyway to avoid a compiler warning.
      if (response.Data == null)
      {
        throw new NullReferenceException("Reponse data is null - we cannot deserialize a token response object");
      }

      GetTokenResponse? tokenresponse = JsonSerializer.Deserialize<GetTokenResponse>(response.Data);

      if (tokenresponse == null)
      {
        //Must not happen. But as "Deserialize" might return null, we have to check this due the "nullable" project. 
        throw new InvalidOperationException("Deserialized TokenResponse was null (Data: " + response.Data + ")");
      }

      return tokenresponse;
    }


    private static readonly char[] charset = {
      'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','q','x','y','z',
      'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
      '0','1','2','3','4','5','6','7','8','9'
    };

    /// <summary>
    /// Create a random string containing letters a-z, A-Z and digits 0-9
    /// </summary>
    /// <param name="length">Length of the requested string</param>
    /// <returns>Random string</returns>
    public static string CreateRandomString(int length = 32)
    {
      Random random = new Random();
      char[] result = new char[length];
      for (int i = 0; i < length; i++)
        result[i] = charset[random.Next(charset.Length)];
      return new string(result);
    }
  }
}
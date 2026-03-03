using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TinyOAuth1;

namespace XbyOpenApi.OAuth1
{
  /// <summary>
  /// Extensions for <see cref="TinyOAuth"/>
  /// </summary>
  public static class TinyOAuth1Extensions
  {
    /// <summary>
    /// Fetches a request token which is used for building an authorization url in step 2 of the OAuth1 flow.
    /// 
    /// This is mainly a copy of <see cref="TinyOAuth.GetRequestTokenAsync"/>, with the only change that the redirect url can be configured.
    /// </summary>
    /// <param name="tinyOAuth">TinyOAuth instance</param>
    /// <param name="redirectUrl">Redirect url configured in the X application</param>
    /// <returns>Response of the token request</returns>
    public static async Task<RequestTokenInfo> GetRequestTokenAsync(this TinyOAuth tinyOAuth, string redirectUrl)
    {
      var nonce = GetNonce(tinyOAuth);
      var timeStamp = GetTimeStamp(tinyOAuth);

      TinyOAuthConfig config = GetConfig(tinyOAuth);

      // See 6.1.1
      var requestParameters = new List<string>
      {
        "oauth_consumer_key=" + config.ConsumerKey,
        "oauth_signature_method=HMAC-SHA1",
        "oauth_timestamp=" + timeStamp,
        "oauth_nonce=" + nonce,
        "oauth_version=1.0",
        //The only change in this method compared to TinyOAuth
        //"oauth_callback=oob" //TODO: Add parameter so it can be used :)
        "oauth_callback=" + redirectUrl
			};

      // Appendix A.5.1. Generating Signature Base String
      var singatureBaseString = GetSignatureBaseString(tinyOAuth, "POST", config.RequestTokenUrl, requestParameters);

      // Appendix A.5.2. Calculating Signature Value
      var signature = GetSignature(tinyOAuth, singatureBaseString, config.ConsumerSecret);

      // 6.1.2.Service Provider Issues an Unauthorized Request Token
      var responseText = await PostData(tinyOAuth, config.RequestTokenUrl,
        ConcatList(tinyOAuth, requestParameters, "&") + "&oauth_signature=" + Uri.EscapeDataString(signature));

      if (!string.IsNullOrEmpty(responseText))
      {
        //Returns an error like this if the callback url is invalid:
        //Probably, the "PostData" method of TinyOAuth ignores the return code.
        //<?xml version='1.0' encoding='UTF-8'?><errors><error code="415">Callback URL not approved for this client application. Approved callback URLs can be adjusted in your application settings</error></errors>
        if (responseText.Contains("<errors>"))
        {
          throw new Exception("Query for RequestToken failed: " + Environment.NewLine + responseText);
        }

        //oauth_token:
        //The Request Token.
        //	oauth_token_secret:
        //The Token Secret.

        //The url contains also parameter "oauth_callback_confirmed=true". But no need to check it, as it fails if the redirect url is invalid.

        string? oauthToken = null;
        string? oauthTokenSecret = null;
        //string oauthAuthorizeUrl = null;

        var keyValPairs = responseText.Split('&');

        for (var i = 0; i < keyValPairs.Length; i++)
        {
          var splits = keyValPairs[i].Split('=');
          switch (splits[0])
          {
            case "oauth_token":
              oauthToken = splits[1];
              break;
            case "oauth_token_secret":
              oauthTokenSecret = splits[1];
              break;
              // TODO: Handle this one?
              //case "xoauth_request_auth_url":
              //	oauthAuthorizeUrl = splits[1];
              //	break;
          }
        }

        return new RequestTokenInfo
        {
          RequestToken = oauthToken,
          RequestTokenSecret = oauthTokenSecret
        };
      }
      throw new Exception("Empty response text when getting the request token");
    }

    #region Helpers
    //Invoke methods of TinyOAuth using reflection.

    private static string GetNonce (TinyOAuth tinyOAuth)
    {
      MethodInfo methodInfo = tinyOAuth.GetType().GetMethod("GetNonce", BindingFlags.Instance | BindingFlags.NonPublic);
      return (string)methodInfo.Invoke(tinyOAuth, null);
    }

    private static string GetTimeStamp(TinyOAuth tinyOAuth)
    {
      MethodInfo methodInfo = tinyOAuth.GetType().GetMethod("GetTimeStamp", BindingFlags.Instance | BindingFlags.NonPublic);
      return (string)methodInfo.Invoke(tinyOAuth, null);
    }


    private static TinyOAuthConfig GetConfig(TinyOAuth tinyOAuth)
    {
      FieldInfo fieldInfo = tinyOAuth.GetType().GetField("_config", BindingFlags.Instance | BindingFlags.NonPublic);
      return (TinyOAuthConfig)fieldInfo.GetValue(tinyOAuth);
    }

    private static string GetSignatureBaseString(TinyOAuth tinyOAuth, string method, string url, List<string> requestParameters)
    {
      MethodInfo methodInfo = tinyOAuth.GetType().GetMethod("GetSignatureBaseString", BindingFlags.Instance | BindingFlags.NonPublic);
      return (string)methodInfo.Invoke(tinyOAuth, new object[] { method, url, requestParameters });
    }

    private static string GetSignature(TinyOAuth tinyOAuth, string signatureBaseString, string consumerSecret, string? tokenSecret = null)
    {
      MethodInfo methodInfo = tinyOAuth.GetType().GetMethod("GetSignature", BindingFlags.Instance | BindingFlags.NonPublic);
      return (string)methodInfo.Invoke(tinyOAuth, new object[] { signatureBaseString, consumerSecret, tokenSecret! });
    }


    private static Task<string> PostData(TinyOAuth tinyOAuth, string url, string postData)
    {
      MethodInfo methodInfo = tinyOAuth.GetType().GetMethod("PostData", BindingFlags.Instance | BindingFlags.NonPublic);
      return (Task<string>)methodInfo.Invoke(tinyOAuth, new object[] { url, postData });
    }


    private static string ConcatList(TinyOAuth tinyOAuth, IEnumerable<string> source, string separator)
    {
      MethodInfo methodInfo = tinyOAuth.GetType().GetMethod("ConcatList", BindingFlags.Static | BindingFlags.NonPublic);
      return (string)methodInfo.Invoke(tinyOAuth, new object[] { source, separator });
    }
    #endregion

  }
}

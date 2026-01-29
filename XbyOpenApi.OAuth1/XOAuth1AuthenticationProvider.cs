using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyOAuth1;

namespace XbyOpenApi.OAuth1
{
  /// <summary>
  /// AuthenticationProvider used for OAuth1 access to the X API.
  /// </summary>
  internal class XOAuth1AuthenticationProvider : IAuthenticationProvider
  {
    private TinyOAuth tinyOAuth;
    private string strAccessToken;
    private string strAccessTokenSecret;

    /// <summary>
    /// Creates the provider using a TinyOAuth instance and a access token.
    /// </summary>
    /// <param name="_tinyOAuth"></param>
    /// <param name="_strAccessToken"></param>
    /// <param name="_strAccessTokenSecret"></param>
    public XOAuth1AuthenticationProvider(TinyOAuth _tinyOAuth, string _strAccessToken, string _strAccessTokenSecret)
    {
      this.tinyOAuth = _tinyOAuth;
      this.strAccessToken = _strAccessToken;
      this.strAccessTokenSecret = _strAccessTokenSecret;
    }

    /// <summary>
    /// Add authentication header to a request. Here the OAuth1 signature is added. 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="additionalAuthenticationContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
    {
      //Convert URI and HttpMethod 
      string strUrl = request.URI.AbsoluteUri;
      HttpMethod httpMethod = request.HttpMethod switch
      {
        Method.GET => HttpMethod.Get,
        Method.POST => HttpMethod.Post,
        Method.DELETE => HttpMethod.Delete,
        _ => throw new NotImplementedException("Not supported for " + request.HttpMethod)
      };
      AuthenticationHeaderValue authHeader = this.tinyOAuth.GetAuthorizationHeader(this.strAccessToken, this.strAccessTokenSecret, strUrl, httpMethod);

      //"ToString" appends Scheme and actual authorization value, here "OAuth xyz":
      request.Headers.Add("Authorization", authHeader.ToString());

      return Task.CompletedTask;
    }
  }
}

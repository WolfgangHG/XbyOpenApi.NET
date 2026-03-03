using System;
using System.Collections.Generic;
using System.Text;
using TinyOAuth1;

namespace XbyOpenApi.OAuth1
{
  /// <summary>
  /// A response from the call to the <see cref="TinyOAuth.GetAuthorizationUrl"/>: the url invokes the redirect url,
  /// and the properties of this class are the parameters that are sent to the redirect url.
  /// </summary>
  public class GetAuthorizationResponse
  {
    /// <summary>
    /// The authorization token should match the <see cref="RequestTokenInfo.RequestToken"/>
    /// </summary>
    public string AuthorizationToken
    {
      get;
      set;
    } = string.Empty;

    /// <summary>
    /// This is the verifier required in the next step to fetch the access token.
    /// </summary>
    public string AuthorizationVerifier
    {
      get;
      set;
    } = string.Empty;
  }
}

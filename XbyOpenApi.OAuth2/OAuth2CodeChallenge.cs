using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace XbyOpenApi.OAuth2
{
  /// <summary>
  /// OAuth2 code challenge: the authorization url must contain a code challenge, and when fetching the actual access token
  /// you must send the matching code challenge.
  /// 
  /// Use the static methods <see cref="CreatePlain"/> or <see cref="CreateSHA256"/> to create an instance.
  /// </summary>
  public class OAuth2CodeChallenge
  {
    /// <summary>
    /// Plain text: Challenge and verifier are the same.
    /// This method name is sent to the server
    /// </summary>
    public const string METHOD_PLAIN = "plain";
    /// <summary>
    /// The challenge is a SHA256 hash of the verifier.
    /// This method name is sent to the server
    /// </summary>
    public const string METHOD_SHA256 = "S256";

    /// <summary>
    /// Create an instance.
    /// </summary>
    /// <param name="method">Method</param>
    /// <param name="challenge">Challenge</param>
    /// <param name="verifier">Verifier</param>
    private OAuth2CodeChallenge (string method, string challenge, string verifier)
    {
      this.Method = method;
      this.Challenge = challenge;
      this.Verifier = verifier;
    }


    /// <summary>
    /// Code challenge method (either <see cref="METHOD_SHA256"/> or <see cref="METHOD_PLAIN"/>)
    /// </summary>
    public string Method
    {
      get;
      private set;
    }

    /// <summary>
    /// For method <see cref="METHOD_SHA256"/>: hash of the <see cref="Verifier"/>.
    /// For method <see cref="METHOD_PLAIN"/>: same value as <see cref="Verifier"/>.
    /// </summary>
    public string Challenge
    {
      get;
      private set;
    }

    /// <summary>
    /// Original challenge string
    /// </summary>
    public string Verifier
    {
      get;
      private set;
    }

    /// <summary>
    /// Create a challenge of type "Plain": challenge and verifier are identical
    /// </summary>
    /// <param name="plainText">Original challenge string, written to <see cref="Verifier"/> </param>
    /// <returns>"Plain" challenge</returns>
    public static OAuth2CodeChallenge CreatePlain(string plainText)
    {
      OAuth2CodeChallenge challengePlain = new OAuth2CodeChallenge(METHOD_PLAIN, plainText, plainText);

      return challengePlain;
    }

    /// <summary>
    /// Creates a challenge for SHA256 method: the <see cref="Challenge"/> contains the SHA256 hash of the string.
    /// </summary>
    /// <param name="plainText">Original challenge string, written to <see cref="Verifier"/> </param>
    /// <returns>SHA256 challenge</returns>
    public static OAuth2CodeChallenge CreateSHA256(string plainText)
    {
      //Calculate the hash:
      using var hash = SHA256.Create();
      byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

      //It must be a Base64 encoded string:
      //Also replace specific chars:
      //https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow-with-pkce/add-login-using-the-authorization-code-flow-with-pkce#create-code-challenge
      string challenge = Convert.ToBase64String(result);
      challenge = challenge.Replace("+", "-");
      challenge = challenge.Replace("=", "");
      challenge = challenge.Replace("/", "_");

      
      OAuth2CodeChallenge challengeSHA256 = new OAuth2CodeChallenge(METHOD_SHA256, challenge, plainText);

      return challengeSHA256;
    }
  }
}

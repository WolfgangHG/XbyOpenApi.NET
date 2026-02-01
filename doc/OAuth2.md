# OpenAPI client with OAuth2 authorization

OAuth2 is all about fetching an Access token. Authorization for all requests is done with an `Authorization` header
with scheme `Bearer`, followed by a plain text Access Token.


# Preparation: adding Authorization header to a Kiota request

This chapter assumes that we already have an Access Token. Below is described to how fetch it.


We need an implementation of `Microsoft.Kiota.Abstractions.Authentication.IAccessTokenProvider` that is used to create
a `Microsoft.Kiota.Abstractions.Authentication.BaseBearerTokenAuthenticationProvider` which is used to create
a `Microsoft.Kiota.Http.HttpClientLibrary.HttpClientRequestAdapter`, and this one is used as an argument to create 
the Kiota generated REST client, in my sample it is the class `XClient`.

The `IAccessTokenProvider` provider in my sample looks like this:

```c#
public class XOAuth2AccessTokenProvider : IAccessTokenProvider
{
  private string accessToken;
  public XOAuth2AccessTokenProvider(string accessToken)
  {
    this.accessToken = accessToken;
  }
  public Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
  {
    return Task.FromResult(this.accessToken);
  }

  public AllowedHostsValidator AllowedHostsValidator => throw new NotImplementedException();
}
```
You just provide it an access token.

This token provider is used to create a API client:

```c#
XOAuth2AccessTokenProvider accessTokenProvider = new XOAuth2AccessTokenProvider(accessToken);
BaseBearerTokenAuthenticationProvider authProvider = new BaseBearerTokenAuthenticationProvider(accessTokenProvider);
HttpClientRequestAdapter adapter = new HttpClientRequestAdapter(authProvider);

XClient client = new XClient(adapter);
```

Now you can make API requests:

```c#
Get2UsersMeResponse response = await client.Two.Users.Me.GetAsync();
```


Note that this handling of the access token is rather simplified. As it expires after two hours, you should refresh it regularly.

# Fetching an access token

We use the [OAuth 2.0 Authorization Code Flow with PKCE](https://docs.x.com/fundamentals/authentication/oauth-2-0/authorization-code)
to fetch an access token which allows us to make requests on behalf of a user.

OAuth2 also provides [app-only bearer tokens](https://docs.x.com/fundamentals/authentication/oauth-2-0/bearer-tokens), 
but they can only be used for a few X features, so I just ignore them.

## Step 1: authorization code
We browse to an url that looks like this:

```
https://x.com/i/oauth2/authorize?response_type=code&client_id={clientId}&redirect_uri={encodedRedirectURL}&scope={scopes}&state=state&code_challenge=challenge&code_challenge_method=plain
```


The url contains several parameters:
* `client_id`: this is the client id of your application
* `redirect_uri`: this is the url encoded redirect url that is configured in your application. Mostly, it is is `http://localhost`,
  which is encoded as `http%3A%2F%2Flocalhost`
* `scope` is a list of requested privileges (separated by a space char), e.g. `users.read tweet.read tweet.write`. As the must be URL encoded,
  they should be `users.read%20tweet.read%20tweet.write`.
* `state` contains a string value that is provided to the call of the redirect url. It is used to validate in the redirect url that 
  it the call is the expected one and it is meant to prevent CSRF attacks.
* `code_challenge` and `code_challenge_method`: this is the PKCE (Proof Key for Code Exchange) part: the `code_challenge_method` can be `plain`
or `S256`. The sample url above uses `plain`, which is actually discouraged (but even the X doc only explain this). More details see below.


This authorization url is opened in an embedded browser control or (if the embedded browser control is not available) in a web browser.

### Code challenge
The `code_challenge` and the `code_challenge_method` are sent to the server as part of the authorization url. On successful authorization, you will receive an 
authentication code, which is used to fetch the access token. And in this step, you will provide
the `code_verifier`.

There are two methods `plain` and `S256`.

**Method `plain`**: `code_challenge` and `code_verifier` contain the same string.

**Method `S256`**: the X doc does not describe it, but look e.g. here: 
[Add Login Using the Authorization Code Flow with PKCE](https://auth0.com/docs/get-started/authentication-and-authorization-flow/authorization-code-flow-with-pkce/add-login-using-the-authorization-code-flow-with-pkce).

The `code_verifier` is again the original string (in my sample it is a random string).

The `code_challenge` contains a SHA256 hash of this string. It must be Base64 encoded, and three characters must be replaced:
* "+" becomes "-"
* "/" becomes "/"
* "=" is removed.

The C# code might look like this (snippet is taken from my implementation which uses `netstandard2.0`, so more elegant API might exist in 
newer .NET versions):

```c#

using var hash = SHA256.Create();
byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

//Build "challenge": it must be Base64 encoded:
string challenge = Convert.ToBase64String(result);

challenge = challenge.Replace("+", "-");
challenge = challenge.Replace("=", "");
challenge = challenge.Replace("/", "_");
```

### Handling the redirect url
It is configured as part of the X app settings, and for desktop applications it could be `http://localhost`:
![Redirect url](images/oauth2_redirecturl.png)

After completing the authorization process in the browser, you are redirected to this url, and the request url contains the authentication code
parameter. There are two ways to handle it.
* start a small webserver listening to this url. This is probably difficult to handle
* better: use a embedded web browser control and intercept the redirection to this url. We will use this approach.

In my sample I use WinForms UI, so I can use the `Microsoft.Web.WebView2.WinForms.WebView2` control from nuget package `Microsoft.Web.WebView2`:

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.Web.WebView2" Version="1.0.3719.77" />
  </ItemGroup>
```

I placed it in a form that is shown as a modal dialog. In `OnLoad` of the form, I navigate to the authorization url:

```c#
this.webBrowser.Source = new Uri(authorizeUrl);
```

Now the user performs a login and confirms the requested scopes. After completing this step (or when clicking "cancel"), the browser
is redirected to the redirect url. And here we can kick in and intercept this navigation, parse the authentication code from the url
and close the dialog. We do so be handling the event `WebBrowserControl.NavigationCompleted`:

```c#
private void webBrowser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
{
  if (this.webBrowser.Source.AbsoluteUri.StartsWith(redirectURL))
  {
    NameValueCollection queryArgs = HttpUtility.ParseQueryString(this.webBrowser.Source.Query);

    if (queryArgs["error"] != null)
    {
      //"Cancel" was clicked. Cancel this form.
      this.DialogResult = DialogResult.Cancel;
      this.Close();
      return;
    }
    else
    {
      //Success:
      string? code = queryArgs["code"];
      //Store the code:
      this.AuthorizationCode = code;

      this.DialogResult = DialogResult.OK;
      this.Close();

    }
  }
}
```

On successful login, the url looks like this:

```
http://localhost/?state=state&code=UkZJQ0d6RVZQbGVwdE...6MTowOmFjOjE
```

The "code" parameter is the authentication code required in the next step.

If the user clicks "cancel" in the last step, the url looks like this:
```
http://localhost/?error=access_denied&state=state
```
It contains an "error" parameter.

For both versions, the client code should check that the `state` parameter contains the same value that was set when creating the authorize url.
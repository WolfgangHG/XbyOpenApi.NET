# OpenAPI client with OAuth1 authorization

OAuth1 is rather complex to handle, as each request must be signed with an `Authorization` header
with scheme `OAuth`.
To do this request signing, you need the Consumer Api Key, the Consumer Api Key Secret, an Access Token and an Access Token Secret.

Getting them all might be really simple - as far as I remember all four values were displayed when I created the app first. But you cannot view
them later - if you did not note them, you have to recreate them.

The major part of the work (request signing and fetching an access token) might be handled by existing libraries. 
In this project, I use [TinyOAuth1](https://github.com/johot/TinyOAuth1), which does not seem to be maintained any longer,
but it works and OAuth1 will probably not evolve any more.


# Table of Contents

- [Initializing TinyOAuth1](#initializing-tinyoauth1)
- [Preparation: signing a Kiota request](#preparation-signing-a-kiota-request)
- [Fetching an access token on behalf of a user](#fetching-an-access-token-on-behalf-of-a-user)
  - [PIN based authorization](#pin-based-authorization)
  - [Authorization with a redirect url](#authorization-with-a-redirect-url)


# Initializing TinyOAuth1

First, add this Nuget package to the project:

```xml
  <ItemGroup>
    <PackageReference Include="TinyOAuth1" Version="1.1.0" />
  </ItemGroup>
```

Then create an instance of the class `TinyOAuth` like this:

```c#
TinyOAuthConfig config = new TinyOAuthConfig
{
  AccessTokenUrl = "https://api.x.com/oauth/access_token",
  AuthorizeTokenUrl = "https://api.x.com/oauth/authorize",
  RequestTokenUrl = "https://api.x.com/oauth/request_token",
  ConsumerKey = consumerApiKey,
  ConsumerSecret = consumerApiKeySecret
};

TinyOAuth tinyOAuth = new TinyOAuth(config);
```
You need the Consumer Api Key and the Consumer Api Key Secret and define the URLs which are used to fetch the access token.

# Preparation: signing a Kiota request

This chapter assumes that we already have an Access Token and Access Token Secret (either the ones that were shown when creating the app,
or fetched with the PIN based flow, see below).

We need an implementation of `Microsoft.Kiota.Abstractions.Authentication.IAuthenticationProvider` that is used to create
a `Microsoft.Kiota.Http.HttpClientLibrary.HttpClientRequestAdapter`, and this one is used as an argument to create 
the Kiota generated REST client, in my sample it is the class `XClient`.


So, this is the authentication provider from my sample:

```c#
public class XOAuth1AuthenticationProvider : IAuthenticationProvider
{
  private TinyOAuth tinyOAuth;
  private string accessToken;
  private string accessTokenSecret;

  public XOAuth1AuthenticationProvider(TinyOAuth tinyOAuth, string accessToken, string accessTokenSecret)
  {
    this.tinyOAuth = tinyOAuth;
    this.accessToken = accessToken;
    this.accessTokenSecret = accessTokenSecret;
  }

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
    AuthenticationHeaderValue authHeader = this.tinyOAuth.GetAuthorizationHeader(this.accessToken, this.accessTokenSecret, strUrl, httpMethod);

    //"ToString" appends Scheme and actual authorization value, here "OAuth xyz":
    request.Headers.Add("Authorization", authHeader.ToString());

    return Task.CompletedTask;
  }
}
```

It needs the `TinyOAuth` instance which contains Consumer Api Key and the Consumer Api Key Secret. We also need Access Token and Access Token secret.

Whenever a request is made, Kiota invokes `AuthenticateRequestAsync` and we can use `TinyOAuth.GetAuthorizationHeader` to create the
authorization header.

This `XOAuth1AuthenticationProvider` is used to create the Kiota generated REST client:

```c#
TinyOAuth tinyOAuth = InitTinyOAuth(consumerApiKey, consumerApiKeySecret);

var authProvider = new XOAuth1AuthenticationProvider(tinyOAuth, accessToken, accessTokenSecret);
var adapter = new HttpClientRequestAdapter(authProvider);

XClient client = new XClient(adapter);
```

Now you can make API requests:

```c#
Get2UsersMeResponse response = await client.Two.Users.Me.GetAsync();
```


# Fetching an access token on behalf of a user

We concentrate on the so called "3-legged OAuth flow" to perform actions on behalf of a user, see
https://developer.x.com/en/docs/tutorials/authenticating-with-twitter-api-for-enterprise/oauth1-0a-and-user-access-tokens
and https://docs.x.com/fundamentals/authentication/oauth-1-0a/obtaining-user-access-tokens.

Note that this flow always resulted in the same access token - it does not seem to expire in comparison to the OAuth2 flow.
As long as you don't revoke it in the X developer page, you can use it for offline access.

## PIN based authorization

The TinyOAuth1 package is limited to always using the pseudo redirect url `oob` (out-of-band OAuth). 
The user still visits X to login or authorize the app,  but they will not be automatically redirected to the application 
upon approving access. Instead, they will see a numerical PIN code and have to return to the application and enter this value.

* Step 1: create the `TinyOAuth` instance and invoke `GetRequestTokenAsync` to fetch a request token:

  ```c#
  TinyOAuth tinyOAuth = ....;

  RequestTokenInfo requestTokenInfo = await tinyOAuth.GetRequestTokenAsync();
  ```
  This sends a POST request to `https://api.x.com/oauth/request_token` containing among others a parameter `oauth_callback=oob` (this is the redirect url).
  This request contains an authorization header with OAuth signature.


* Step 2: Build an authorization url using this request token:
  ```c#
  string authorizationUrl = tinyOAuth.GetAuthorizationUrl(requestTokenInfo.RequestToken);
  ```

  This URL looks like this:
  ```
  https://api.x.com/oauth/authorize?oauth_token=hoRAzAAAAAABoVccAAABnBCczxk
  ```

  Start an external browser and meanwhile show a input control/form in your app that let the user type the PIN.
  Instead of the external browser, you might also use an embedded browser, but the user still has to copy the PIN from the browser window to your input field.

  ```c#
  Process.Start(new ProcessStartInfo(authorizationUrl)
  {
    UseShellExecute = true
  });
  ```

* Step 3: fetch  the access token:

  ```c#
  string pinCode = ...user enters pin...;

  AccessTokenInfo accessTokenInfo = await tinyOAuth.GetAccessTokenAsync(requestTokenInfo.RequestToken, requestTokenInfo.RequestTokenSecret, pinCode));
  ```

  This sends a POST request to `https://api.x.com/oauth/access_token` with the parameter `oauth_verifier` set to the PIN. 
  Again, this request contains an authorization header with OAuth signature.

  The result is finally the access token and the access token secret.
  
  
  
## Authorization with a redirect url

By copying the method `TinyOAuth.GetRequestTokenAsync` and adding a parameter `redirectUrl` (and doing some ugly reflection to call a lot of private methods in TinyOAuth1), 
I could implement authorization with a redirect url.

The redirect url is configured as part of the X app settings, and for desktop applications it could be `http://localhost`:

![Redirect url](images/oauth2_redirecturl.png)




* Step 1: create the `TinyOAuth` instance. In my sample, you invoke the extension method `TinyOAuth1Extensions.GetRequestTokenAsync(string redirectUrl)` to fetch a request token:

  ```c#
  TinyOAuth tinyOAuth = ....;

  RequestTokenInfo requestTokenInfo = await TinyOAuth1Extensions.GetRequestTokenAsync(tinyOAuth, redirectUrl);
  ```
  This sends a POST request to `https://api.x.com/oauth/request_token` containing among others a parameter `oauth_callback=...` (contains the redirect url).
  This request contains an authorization header with OAuth signature.

  The response looks like this
  ```
  oauth_token=zvMauAAAAAABoVccAAABnLUCxCw&oauth_token_secret=9fKAYDFrpahqrrOZLN9QW1YOjfHN0GQP&oauth_callback_confirmed=true
  ```

* Step 2: Build an authorization url using this request token:
  ```c#
  string authorizationUrl = tinyOAuth.GetAuthorizationUrl(requestTokenInfo.RequestToken);
  ```

  This URL looks like this:
  ```
  https://api.x.com/oauth/authorize?oauth_token=zvMauAAAAAABoVccAAABnLUCxCw
  ```

  Now start either an embedded browser and browse to this url or open the url in a browser and launch a small webserver that listens to the redirect url.  
  
  After the user has authorized, the redirect url will be called. You have to parse the arguments.
  
  The URLs looks like this:
  ```
  http://localhost/?oauth_token=zvMauAAAAAABoVccAAABnLUCxCw&oauth_verifier=H2IZ2hifZs0W56C65ZDnpjqnrJltLvo2
  ```
  
  Note: you should check the parameter `oauth_token` - it must match the Request Token from the previous step.
  
  Here is sample code, where I used a `WebView2` browser:
  
  ```c#
  private void webBrowser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
  {
    if (this.webBrowser.Source.AbsoluteUri.StartsWith(redirectURL))
    {
      NameValueCollection queryArgs = HttpUtility.ParseQueryString(strQuery);

      //This token must be identical to the request token used for creating the authorization url. You should check it.
      string token = queryArgs["oauth_token"];
      //... check the token ...
      
      string verifier = queryArgs["oauth_verifier"];
      
      this.AuthorizationVerifier = verifier;
      
      //Close the form...

    }
  }
  ```
  
  Note: in the OAuth2 flow, there was also an argument that was set when the "Cancel" button was clicked during Authorization.
  In the OAuth1 flow, this did not happen. Apparently, there is no easy way to detect cancellation.

* Step 3: fetch  the access token using the `verifier` from the previous step:

  ```c#
  AccessTokenInfo accessTokenInfo = await tinyOAuth.GetAccessTokenAsync(requestTokenInfo.RequestToken, requestTokenInfo.RequestTokenSecret, verifier));
  ```

  This sends a POST request to `https://api.x.com/oauth/access_token` with the parameter `oauth_verifier` set to the PIN. 
  Again, this request contains an authorization header with OAuth signature.

  The result is finally the access token and the access token secret.
  

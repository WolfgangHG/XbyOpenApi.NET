using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using TinyOAuth1;

namespace XbyOpenApi.OAuth1.WinForms
{
  /// <summary>
  /// Login form for X in an OAuth1 flow using a WebView2 control. 
  /// After login, the browser is redirected to the "redirect url" configured in the X application. This dialog handles this redirect
  /// and parses the authentication code.
  /// After successful (or canceled) login, the form auto closes itself.
  /// If login succeeded (DialogResult = OK), the <see cref="AuthorizationResponse"/> contains the code.
  /// </summary>
  public partial class DialogOAuth1LoginWebView : Form
  {
    #region Private Vars
    /// <summary>
    /// The authorization url is built in the constructor
    /// </summary>
    private string authorizationUrl;
    /// <summary>
    /// This is the plain redirect url, as specified in the X app.
    /// </summary>
    private string redirectURL;

    /// <summary>
    /// The request token is part of the authorization url and also used to verify the login result.
    /// </summary>
    private RequestTokenInfo requestTokenInfo;
    #endregion

    #region Public properties
    /// <summary>
    /// This is the AuthorizationCode which is returned by X after a successful authorization.
    /// NULL if authorization was canceled by clicking the "cancel" button on the login page or by closing this form.
    /// </summary>
    public GetAuthorizationResponse? AuthorizationResponse
    {
      get;
      private set;
    }

    #endregion

    #region Constructor
    /// <summary>
    /// The constructor just copies the arguments to member variables. 
    /// </summary>
    /// <param name="tinyOAuth">TinyOAuth instance</param>
    /// <param name="requestTokenInfo">This is the request token used to build the authorization url.</param>
    /// <param name="redirectUrl">This is the plain redirect url, as specified in the X app.</param>
    public DialogOAuth1LoginWebView(TinyOAuth tinyOAuth, RequestTokenInfo requestTokenInfo, string redirectUrl)
    {
      if (string.IsNullOrWhiteSpace(redirectUrl))
      {
        throw new ArgumentException(nameof(redirectUrl) + " must not be empty");
      }

      // Construct the authorization url
      string authorizationUrl = tinyOAuth.GetAuthorizationUrl(requestTokenInfo.RequestToken);

      //this.tinyOAuth = tinyOAuth;
      this.redirectURL = redirectUrl;
      this.authorizationUrl = authorizationUrl;
      this.requestTokenInfo = requestTokenInfo;


      InitializeComponent();
    }
    #endregion

    #region Overrides
    /// <summary>
    /// Form is loaded: browse to the authorization url
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.webBrowser.Source = new Uri(this.authorizationUrl);
    }
    #endregion

    #region Event handler
    /// <summary>
    /// The browser control has navigated to a page: if this is the redirect url, the parameters contains the authorization verifier
    /// Intercept this request and close the dialog.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void webBrowser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
    {
      if (this.webBrowser.Source.AbsoluteUri.StartsWith(this.redirectURL))
      {
        //The redirect url contains the authorization verifier.
        //Also check the "oauth_token" parameter that was set when building the authorization url.
        //Here, "Cancel" clicks don't seem to be detectable.

        
        //We could catch exceptions (e.g. invalid "oauth_token")
        GetAuthorizationResponse? authorizationResponse = XClientOAuth1Util.ParseAutorizationCode(this.webBrowser.Source.Query, this.requestTokenInfo);

        if (authorizationResponse != null)
        {
          this.AuthorizationResponse = authorizationResponse;

          //At this point, you can use the authorization code to create an access token and refresh token.

          this.DialogResult = DialogResult.OK;
          this.Close();
        }
        else
        {
          //Not detectable here:
          this.DialogResult = DialogResult.Cancel;
          this.Close();
          return;
        }
      }
    }

    /// <summary>
    /// After the browser control is initialized:
    /// a) switch off navigation options.
    /// b) reset the browser cache: delete all cookies, so that every time a new login is done.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void webBrowser_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
    {
      //Reset navigation options:
      this.webBrowser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
      this.webBrowser.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;

      
      this.webBrowser.CoreWebView2.CookieManager.DeleteAllCookies();

    }
    #endregion
  }
}
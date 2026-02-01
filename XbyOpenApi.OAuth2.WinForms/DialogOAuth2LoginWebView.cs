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

namespace XbyOpenApi.OAuth2.WinForms
{
  /// <summary>
  /// Login form for X in an OAuth2 flow using a WebView2 control. 
  /// After login, the browser is redirected to the "redirect url" configured in the X application. This dialog handles this redirect
  /// and parses the authentication code.
  /// After successful (or canceled) login, the form auto closes itself.
  /// If login succeeded (DialogResult = OK), the <see cref="AuthorizationCode"/> contains the code.
  /// </summary>
  public partial class DialogOAuth2LoginWebView : Form
  {
    #region Private Vars
    /// <summary>
    /// ClientID
    /// </summary>
    private string clientID;

    /// <summary>
    /// This is the plain redirect url, as specified in the X app.
    /// </summary>
    private string realRedirectURL;

    /// <summary>
    /// True: fetch also a refresh token.
    /// </summary>
    private bool fetchRefreshToken;

    /// <summary>
    /// Requested scopes.
    /// </summary>
    private List<string> scopes;

    /// <summary>
    /// The authorize url contains a "state" parameter (random string created here). The redirect url must also contain this parameter.
    /// </summary>
    private string state;
    #endregion

    #region Public properties
    /// <summary>
    /// This is the AuthorizationCode which is returned by X after a successful authorization.
    /// NULL if authorization was canceled by clicking the "cancel" button on the login page or by closing this form.
    /// </summary>
    public string? AuthorizationCode
    {
      get;
      private set;
    }

    #endregion

    #region Constructor
    /// <summary>
    /// The constructor just copies the arguments to member variables. 
    /// </summary>
    /// <param name="clientID">ClientID</param>
    /// <param name="redirectUrl">This is the plain redirect url, as specified in the X app.</param>
    /// <param name="fetchRefreshToken">Du we also want to fetch a RefreshToken (which means adding an additional 
    /// <paramref name="scopes"/>) </param>
    /// <param name="scopes">Requested scopes, must contain at least one item.</param>
    public DialogOAuth2LoginWebView(string clientID, string redirectUrl, bool fetchRefreshToken,
       List<string> scopes)
    {
      if (scopes == null)
      {
        throw new ArgumentNullException(nameof(scopes));
      }
      if (scopes.Count  == 0)
      {
        throw new ArgumentException("Scopes must not be empty");
      }

      this.clientID = clientID;
      this.realRedirectURL = redirectUrl;
      
      this.scopes = scopes;
      this.fetchRefreshToken = fetchRefreshToken;

      //Create random "state":
      this.state = XClientOAuth2Util.CreateRandomString();

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

      string authorizeUrl = XClientOAuth2Util.GetAuthorizeUrl(clientID, this.realRedirectURL, this.scopes, this.fetchRefreshToken, this.state);

      this.webBrowser.Source = new Uri(authorizeUrl);
    }
    #endregion

    #region Event handler
    /// <summary>
    /// The browser control has navigated to a page: if this is the redirect url, the parameters contains either an authorization code
    /// or an error state.
    /// Intercept this request and close the dialog.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void webBrowser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
    {
      if (this.webBrowser.Source.AbsoluteUri.StartsWith(realRedirectURL))
      {
        //The redirect url contains the authorization code or an error message (if cancel was clicked).
        //Also check the state parameter that was set when building the authorization url.

        string? code = XClientOAuth2Util.ParseAutorizationCode(this.webBrowser.Source.Query, this.state);

        if (code != null)
        {
          this.AuthorizationCode = code;

          //At this point, you can use the authorization code to create an access token and refresh token.

          this.DialogResult = DialogResult.OK;
          this.Close();
        }
        else
        {
          //"Cancel" was clicked. Cancel this form.
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TinyOAuth1;
using XbyOpenApi.Core.Client;
using XbyOpenApi.Core.Client.Models;
using XbyOpenApi.OAuth1;
using XbyOpenApi.OAuth1.WinForms;
using XbyOpenApi.OAuth2;
using XbyOpenApi.OAuth2.WinForms;

namespace XByOpenApi.Sample.WinForms
{
  /// <summary>
  /// X API tests
  /// </summary>
  public partial class FormXByKiotaTest : Form
  {
    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public FormXByKiotaTest()
    {
#if NET48
      //Set the default font to "SegoeUI 9", so that in .NET 48, the AutoScaleDimension of the .NET8 designer generated code
      //matches the actual of the form.
      //In .NET8, the font is automatically "SegoeUI 9".
      this.Font = SystemFonts.MessageBoxFont;
#endif
      InitializeComponent();


      //Write target framework to window title:
      object[] targetFrameworkAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TargetFrameworkAttribute), false);
      //There should be exactly one attribute.
      TargetFrameworkAttribute? ta = (TargetFrameworkAttribute?)targetFrameworkAttributes.FirstOrDefault();
      //Don't know whether a NULL check is required.
      string targetFramework = ta?.FrameworkDisplayName ?? "--unknown--";
      this.Text += $" ({targetFramework})";

      this.textBoxOAuth2RedirectUrl.Text = "http://localhost";
    }
    #endregion

    #region Eventhandler
    /// <summary>
    /// Button "Fetch OAuth1 access token with the PIN based flow"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonOAuth1PIN_Click(object sender, EventArgs e)
    {
      AccessTokenInfo? accessTokenInfo = XClientOAuth1WinFormsUtil.GetAccessToken_PinBasedOAuthFlowSync(this, this.OAuth1ConsumerApiKey, this.OAuth1ConsumerApiKeySecret);

      //"Null" means: user canceled the dialog.
      if (accessTokenInfo == null)
      {
        return;
      }

      this.textBoxOAuth1AccessToken.Text = accessTokenInfo.AccessToken;
      this.textBoxOAuth1AccessTokenSecret.Text = accessTokenInfo.AccessTokenSecret;
    }

    /// <summary>
    /// Button "Fetch OAuth2 2 access token by interactive login"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonOAuth2AccessToken_Click(object sender, EventArgs e)
    {
      //It seems we need those three scopes to view my own profile and to write Tweets.
      //"users.read" is not enough to view my own profile.
      List<string> scopes = new List<string>() { "users.read", "tweet.read", "tweet.write" };

      try
      {
        GetTokenResponse? tokenResponse;
        if (this.radioButtonOAuth2ConfidentialClient.Checked == true)
        {
          tokenResponse = XClientOAuth2WinFormsUtil.GetAccessToken_ConfidentialClient(this, this.OAuth2ClientID, this.OAuth2ClientSecret,
            this.OAuth2RedirectUrl, this.OAuth2FetchRefreshToken, scopes);
        }
        else
        {
          tokenResponse = XClientOAuth2WinFormsUtil.GetAccessToken_PublicClient(this, this.OAuth2ClientID,
           this.OAuth2RedirectUrl, this.OAuth2FetchRefreshToken, scopes);
        }
        if (tokenResponse == null)
        {
          return;
        }

        this.OAuth2AccessToken = tokenResponse.AccessToken;
        //Refresh token will only be filled if it was requested.
        this.OAuth2RefreshToken = tokenResponse.RefreshToken;
        MessageBox.Show(this, "Access Token was fetched");
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, ex.ToString());
      }
    }

    /// <summary>
    /// Radiobutton "OAuth2" changes check state: enable/disable dependant controls
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void radioButtonOAuth2_CheckedChanged(object sender, EventArgs e)
    {
      //Simply enable/disable the entire GroupBox.
      this.groupBoxOAuth1.Enabled = this.radioButtonOAuth1.Checked;
      this.groupBoxOAuth2.Enabled = this.radioButtonOAuth2.Checked;
    }

    /// <summary>
    /// Radiobutton "Public Client / Confidential Client" ändert sich: abhängige Controls enablen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void radioButtonOAuth2ConfidentialClient_CheckedChanged(object sender, EventArgs e)
    {
      this.textBoxOAuth2ClientSecret.Enabled = this.radioButtonOAuth2ConfidentialClient.Checked;
    }

    /// <summary>
    /// Click on button "fetch Access Token by Refresh Token"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonOAuth2AccessTokenByRefreshToken_Click(object sender, EventArgs e)
    {
      //A refresh token can only be used once.
      string strClientID = this.OAuth2ClientID;

      StringBuilder sbFehler = new StringBuilder();
      if (string.IsNullOrWhiteSpace(strClientID) == true)
      {
        sbFehler.AppendLine("Please providate ClientID.");
      }

      string strRefreshToken = this.OAuth2RefreshToken;
      if (string.IsNullOrEmpty(strRefreshToken) == true)
      {
        sbFehler.AppendLine("Please enter a RefreshToken.");
      }
      if (sbFehler.Length > 0)
      {
        MessageBox.Show(this, sbFehler.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      try
      {
        //Blocks...
        //GetTokenResponse tokenResponse = XClientOAuth2Util.GetAccessTokenFromRefreshToken(strClientID, strRefreshToken).GetAwaiter().GetResult();

        //Hack for blocking access...
        Task<GetTokenResponse> taskGetToken = Task.Run(Task<GetTokenResponse>? () =>
        {
          return XClientOAuth2Util.GetAccessTokenFromRefreshToken(strClientID, strRefreshToken);
        });
        taskGetToken.Wait();
        GetTokenResponse tokenResponse = taskGetToken.Result;

        this.OAuth2AccessToken = tokenResponse.AccessToken;
        //This returns another refresh token:
        this.OAuth2RefreshToken = tokenResponse.RefreshToken;
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, "An error occured while fetching the refresh token: " + Environment.NewLine + ex);
      }
    }

    /// <summary>
    /// Click on "Send simple tweet using the Kiota generated API client".
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonKiotaSimpleTweet_Click(object sender, EventArgs e)
    {
      if (this.CheckKeysProvided() == false)
      {
        return;
      }

      XClient xClient = this.InitXClient();

      try
      {
        TweetCreateRequest body = new TweetCreateRequest();
        body.Text = "Sample post created by Kiota";
        TweetCreateResponse response = xClient.Two.Tweets.PostAsync(body).GetAwaiter().GetResult();

        if (response.Data != null)
        {
          MessageBox.Show(this, "Success: Id = " + response.Data.Id);

          this.textBoxDeleteTweetId.Text = response.Data.Id.ToString();
        }
        else if (response.Errors != null)
        {
          string strErrors = LogProblems(response.Errors);
          MessageBox.Show("Error: " + strErrors);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, "An error occured while creating the tweet: " + ex.ToString());
      }
    }

    /// <summary>
    /// Click on button "Post tweet with image using the Kiota generated API client".
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonKiotaImage_Click(object sender, EventArgs e)
    {
      if (this.CheckKeysProvided() == false)
      {
        return;
      }


      XClient xClient = this.InitXClient();

      try
      {
        byte[] arrData = File.ReadAllBytes("..\\..\\..\\beispielbild.jpeg");

        MediaUploadRequestOneShot mediaUpload = new MediaUploadRequestOneShot();
        mediaUpload.Media = new MediaUploadRequestOneShot.MediaUploadRequestOneShot_media();
        mediaUpload.Media.MediaPayloadByte = new MediaPayloadByte();
        bool todo_GehtNicht;

        //mediaUpload.MediaType = MediaUploadRequestOneShot_media_type.ImageJpeg;

        MediaUploadResponse response = xClient.Two.Media.Upload.PostAsync(mediaUpload).GetAwaiter().GetResult();

        bool todo_TweetErzeugenUpload; //Fehlt noch
        if (response.Data != null)
        {
          MessageBox.Show(this, "Success: Id = " + response.Data.Id);

          //this.txtDeleteTweetId.Text = response.Data.Id.ToString();
        }
        else if (response.Errors != null)
        {
          string strErrors = LogProblems(response.Errors);
          MessageBox.Show("Error: " + strErrors);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, "An error occured while creating the tweet: " + ex.ToString());
      }
    }


    /// <summary>
    /// Delete a tweet using the Kiota generated api client.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonDeleteTweet_Click(object sender, EventArgs e)
    {
      if (this.CheckKeysProvided() == false)
      {
        return;
      }

      XClient xClient = this.InitXClient();

      try
      {
        string tweetId = this.textBoxDeleteTweetId.Text;
        //There is no error message, event a invalid ID results in "success"
        //The Kiota flag "--exclude-backward-compatible" generated a different API.
        TweetDeleteResponse response = xClient.Two.Tweets[tweetId].DeleteAsync().GetAwaiter().GetResult();

        if (response.Data != null)
        {
          MessageBox.Show(this, "Success: deleted = " + response.Data.Deleted);
        }
        else if (response.Errors != null)
        {
          string strErrors = LogProblems(response.Errors);
          MessageBox.Show("Error: " + strErrors);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, "Error on deleting a tweet: " + ex.ToString());
      }
    }

    /// <summary>
    /// Click on "fetch my own user data using the Kiota generated API client" (since Nov 2023 the free access can only fetch its own data)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonKiotaLookupUser_Click(object sender, EventArgs e)
    {
      if (this.CheckKeysProvided() == false)
      {
        return;
      }

      XClient client = this.InitXClient();

      try
      {
        Get2UsersMeResponse response = client.Two.Users.Me.GetAsync().GetAwaiter().GetResult();

        if (response.Data != null)
        {
          MessageBox.Show(this, "Success: Name = " + response.Data.Name + ", Id = " + response.Data.Id);
        }
        else if (response.Errors != null)
        {
          string strErrors = LogProblems(response.Errors);
          MessageBox.Show("Erorr: " + strErrors);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(this, "An error occured while fetching the user data: " + ex.ToString());
      }
    }

    /// <summary>
    /// Click on "fetch my own user data (Plain Http)"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonPlainHttpLookupUser_Click(object sender, EventArgs e)
    {
      if (this.CheckKeysProvided() == false)
      {
        return;
      }

      if (this.radioButtonOAuth1.Checked == true)
      {
        TinyOAuth tinyOAuth = this.InitTinyOAuth();


        //HttpResponse<string> result = await HttpController.ExecuteRequest("https://api.x.com/2/users/me", HttpMethod.Get, request =>
        //{
        //  request.Headers.Authorization = tinyOAuth.GetAuthorizationHeader(this.OAuth1AccessToken, this.OAuth1AccessTokenSecret, "https://api.x.com/2/users/me", HttpMethod.Get);
        //});

        //"GetAwaiter().GetResult" blocks.
        //But it works to call "Task.Run().Wait()", then fetch the result.
        Task<HttpController.HttpResponse> taskGetMe = Task.Run(Task<HttpController.HttpResponse>? () =>
        {
          return HttpController.ExecuteRequest("https://api.x.com/2/users/me", HttpMethod.Get, request =>
          {
            request.Headers.Authorization = tinyOAuth.GetAuthorizationHeader(this.OAuth1AccessToken, this.OAuth1AccessTokenSecret, "https://api.x.com/2/users/me", HttpMethod.Get);
          });
        });
        taskGetMe.Wait();
        HttpController.HttpResponse result = taskGetMe.Result;

        MessageBox.Show(this, result.ToString());
      }
      else
      {
        //HttpResponse<string> result = await HttpController.ExecuteRequest("https://api.x.com/2/users/me", HttpMethod.Get, request =>
        //{
        //  request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.OAuth2AccessToken);
        //});

        Task<HttpController.HttpResponse> taskGetMe = Task.Run(Task<HttpController.HttpResponse>? () =>
        {
          return HttpController.ExecuteRequest("https://api.x.com/2/users/me", HttpMethod.Get, request =>
          {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.OAuth2AccessToken);
          });
        });
        taskGetMe.Wait();
        HttpController.HttpResponse result = taskGetMe.Result;

        MessageBox.Show(this, result.ToString());
      }

    }

    #endregion

    #region Properties
    /// <summary>
    /// ConsumerApiKey for OAuth1 from the textbox
    /// </summary>
    private string OAuth1ConsumerApiKey
    {
      get
      {
        return this.textBoxOAuth1ConsumerAPIKey.Text;
      }
    }

    /// <summary>
    /// ConsumerApiKeySecret for OAuth1 from the textbox
    /// </summary>
    private string OAuth1ConsumerApiKeySecret
    {
      get
      {
        return this.textBoxOAuth1ConsumerAPIKeySecret.Text;
      }
    }

    /// <summary>
    /// AccessToken for OAuth1 from the textbox
    /// </summary>
    private string OAuth1AccessToken
    {
      get
      {
        return this.textBoxOAuth1AccessToken.Text;
      }
    }


    /// <summary>
    /// AccessTokenSecret for OAuth1 from the textbox
    /// </summary>
    private string OAuth1AccessTokenSecret
    {
      get
      {
        return this.textBoxOAuth1AccessTokenSecret.Text;
      }
    }

    /// <summary>
    /// ClientID for OAuth2 from the textbox
    /// </summary>
    private string OAuth2ClientID
    {
      get
      {
        return this.textBoxOAuth2ClientID.Text;
      }
    }

    /// <summary>
    /// RedirectUrl for OAuth2 from the textbox
    /// </summary>
    private string OAuth2RedirectUrl
    {
      get
      {
        return this.textBoxOAuth2RedirectUrl.Text;
      }
    }
    /// <summary>
    /// ClientSecret for OAuth2 (Confidential Client) from the textbox
    /// </summary>
    private string OAuth2ClientSecret
    {
      get
      {
        return this.textBoxOAuth2ClientSecret.Text;
      }
    }

    /// <summary>
    /// AccessToken for OAuth2 from the textbox
    /// </summary>
    private string OAuth2AccessToken
    {
      get
      {
        return this.textBoxOAuth2AccessToken.Text;
      }
      set
      {
        this.textBoxOAuth2AccessToken.Text = value;
      }
    }

    /// <summary>
    /// Should OAuth2 login also fetch a RefreshToken?
    /// This is the current state of the checkbox.
    /// </summary>
    private bool OAuth2FetchRefreshToken
    {
      get
      {
        return this.checkBoxOAuth2FetchRefreshToken.Checked;
      }
    }

    /// <summary>
    /// RefreshToken for OAuth2 from the textbox
    /// </summary>
    private string OAuth2RefreshToken
    {
      get
      {
        return this.textBoxOAuth2RefreshToken.Text;
      }
      set
      {
        this.textBoxOAuth2RefreshToken.Text = value;
      }
    }
    #endregion

    #region Private Helpers
    /// <summary>
    /// Checks that all necessary keys are provided, depending on checkboxes/radiobuttons.
    /// This method is an overall check for nearly any button click on the form.
    /// </summary>
    /// <returns></returns>
    private bool CheckKeysProvided()
    {
      if (this.radioButtonOAuth1.Checked == true)
      {
        if (string.IsNullOrWhiteSpace(this.OAuth1ConsumerApiKey) == true || string.IsNullOrWhiteSpace(this.OAuth1ConsumerApiKeySecret) == true ||
          string.IsNullOrWhiteSpace(this.OAuth1AccessToken) == true || string.IsNullOrWhiteSpace(this.OAuth1AccessTokenSecret) == true)
        {
          MessageBox.Show(this, "Please provide all four keys for OAuth1", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return false;

        }
        else
        {
          return true;
        }
      }
      else if (this.radioButtonOAuth2.Checked == true)
      {
        StringBuilder sbFehler = new StringBuilder();
        if (string.IsNullOrWhiteSpace(this.OAuth2ClientID) == true)
        {
          sbFehler.AppendLine("Please enter a ClientID.");
        }

        if (string.IsNullOrWhiteSpace(this.OAuth2RedirectUrl) == true)
        {
          sbFehler.AppendLine("Please provide a RedirectUrl.");
        }
        if (string.IsNullOrWhiteSpace(this.OAuth2AccessToken) == true)
        {
          sbFehler.AppendLine("Please fetch AccessToken or enter it.");
        }

        if (this.radioButtonOAuth2ConfidentialClient.Checked == true)
        {
          if (string.IsNullOrWhiteSpace(this.OAuth2ClientSecret) == true)
          {
            sbFehler.AppendLine("Please enter a ClientSecret.");
          }
        }

        if (sbFehler.Length > 0)
        {
          MessageBox.Show(this, sbFehler.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return false;
        }
        return true;
      }
      else
      {
        MessageBox.Show(this, "Unknown radio button", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
    }

    /// <summary>
    /// Creates a TinyOAuth from the current input.
    /// </summary>
    /// <returns></returns>
    private TinyOAuth InitTinyOAuth()
    {
      TinyOAuth tinyOAuth = XClientOAuth1Util.InitTinyOAuth(this.OAuth1ConsumerApiKey, this.OAuth1ConsumerApiKeySecret);
      
      return tinyOAuth;
    }

    /// <summary>
    /// Creates an XClient for OAuth1 or OAuth2. This requires that an AccessToken was fetched.
    /// </summary>
    /// <returns></returns>
    private XClient InitXClient()
    {
      if (this.radioButtonOAuth1.Checked == true)
      {
        return this.InitXClientOAuth1();
      }
      else
      {
        return this.InitXClientOAuth2Bearer();
      }
    }

    /// <summary>
    /// Creates an XClient for OAuth1. This requires that all four fields are filled.
    /// </summary>
    /// <returns></returns>
    private XClient InitXClientOAuth1()
    {
      XClient client = XClientOAuth1Util.InitXClient(this.OAuth1ConsumerApiKey, this.OAuth1ConsumerApiKeySecret, this.OAuth1AccessToken, this.OAuth1AccessTokenSecret);
      return client;
    }


    /// <summary>
    /// Creates an XClient for OAuth2. This requires that an AccessToken was fetched..
    /// </summary>
    /// <returns></returns>
    private XClient InitXClientOAuth2Bearer()
    {
      XClient client = XClientOAuth2Util.InitXClient(this.OAuth2AccessToken);
      return client;
    }

    /// <summary>
    /// Creates a logging output for a Kiota "Problems" Collection.
    /// </summary>
    /// <param name="_problems"></param>
    /// <returns></returns>
    private static string LogProblems(IEnumerable<Problem> _problems)
    {
      string strErrors = "";
      foreach (Problem problem in _problems)
      {
        strErrors += problem.Status + " " + problem.Title + " " + problem.Detail + Environment.NewLine;
      }

      return strErrors;
    }
    #endregion


  }


}


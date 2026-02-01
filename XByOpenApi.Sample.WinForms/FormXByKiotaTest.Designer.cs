namespace XByOpenApi.Sample.WinForms
{
  partial class FormXByKiotaTest
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.buttonKiotaSimpleTweet = new System.Windows.Forms.Button();
      this.groupBoxKiota = new System.Windows.Forms.GroupBox();
      this.labelDeleteTweetId = new System.Windows.Forms.Label();
      this.buttonDeleteTweet = new System.Windows.Forms.Button();
      this.textBoxDeleteTweetId = new System.Windows.Forms.TextBox();
      this.buttonKiotaLookupUser = new System.Windows.Forms.Button();
      this.buttonKiotaImage = new System.Windows.Forms.Button();
      this.groupBoxConfig = new System.Windows.Forms.GroupBox();
      this.radioButtonOAuth2 = new System.Windows.Forms.RadioButton();
      this.groupBoxOAuth2 = new System.Windows.Forms.GroupBox();
      this.buttonOAuth2AccessTokenByRefreshToken = new System.Windows.Forms.Button();
      this.textBoxOAuth2RefreshToken = new System.Windows.Forms.TextBox();
      this.labelOAuth2RefreshToken = new System.Windows.Forms.Label();
      this.checkBoxOAuth2FetchRefreshToken = new System.Windows.Forms.CheckBox();
      this.labelOAuth2ClientSecret = new System.Windows.Forms.Label();
      this.textBoxOAuth2ClientSecret = new System.Windows.Forms.TextBox();
      this.radioButtonOAuth2ConfidentialClient = new System.Windows.Forms.RadioButton();
      this.radioButtonOAuth2PublicClient = new System.Windows.Forms.RadioButton();
      this.textBoxOAuth2RedirectUrl = new System.Windows.Forms.TextBox();
      this.labelOAuth2RedirectUrl = new System.Windows.Forms.Label();
      this.textBoxOAuth2AccessToken = new System.Windows.Forms.TextBox();
      this.labelOAuth2AccessToken = new System.Windows.Forms.Label();
      this.textBoxOAuth2ClientID = new System.Windows.Forms.TextBox();
      this.labelOAuth2ClientID = new System.Windows.Forms.Label();
      this.buttonOAuth2AccessToken = new System.Windows.Forms.Button();
      this.groupBoxOAuth1 = new System.Windows.Forms.GroupBox();
      this.textBoxOAuth1AccessToken = new System.Windows.Forms.TextBox();
      this.labelOAuth1AccessToken = new System.Windows.Forms.Label();
      this.textBoxOAuth1AccessTokenSecret = new System.Windows.Forms.TextBox();
      this.buttonOAuth1PIN = new System.Windows.Forms.Button();
      this.labelOAuth1ConsumerAPIKeySecret = new System.Windows.Forms.Label();
      this.labelOAuth1AccessTokenSecret = new System.Windows.Forms.Label();
      this.textBoxOAuth1ConsumerAPIKeySecret = new System.Windows.Forms.TextBox();
      this.textBoxOAuth1ConsumerAPIKey = new System.Windows.Forms.TextBox();
      this.labelOAuth1ConsumerAPIKey = new System.Windows.Forms.Label();
      this.radioButtonOAuth1 = new System.Windows.Forms.RadioButton();
      this.groupBoxPlainHttp = new System.Windows.Forms.GroupBox();
      this.buttonPlainHttpLookupUser = new System.Windows.Forms.Button();
      this.panelOAuth2ClientType = new System.Windows.Forms.Panel();
      this.panelOAuth2CodeChallengeMethod = new System.Windows.Forms.Panel();
      this.radioButtonOAuth2CodeChallengeMethodSHA256 = new System.Windows.Forms.RadioButton();
      this.radioButtonOAuth2CodeChallengeMethodPlain = new System.Windows.Forms.RadioButton();
      this.groupBoxKiota.SuspendLayout();
      this.groupBoxConfig.SuspendLayout();
      this.groupBoxOAuth2.SuspendLayout();
      this.groupBoxOAuth1.SuspendLayout();
      this.groupBoxPlainHttp.SuspendLayout();
      this.panelOAuth2ClientType.SuspendLayout();
      this.panelOAuth2CodeChallengeMethod.SuspendLayout();
      this.SuspendLayout();
      // 
      // buttonKiotaSimpleTweet
      // 
      this.buttonKiotaSimpleTweet.Location = new System.Drawing.Point(7, 22);
      this.buttonKiotaSimpleTweet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.buttonKiotaSimpleTweet.Name = "buttonKiotaSimpleTweet";
      this.buttonKiotaSimpleTweet.Size = new System.Drawing.Size(147, 23);
      this.buttonKiotaSimpleTweet.TabIndex = 1;
      this.buttonKiotaSimpleTweet.Text = "Simple Tweet";
      this.buttonKiotaSimpleTweet.UseVisualStyleBackColor = true;
      this.buttonKiotaSimpleTweet.Click += this.buttonKiotaSimpleTweet_Click;
      // 
      // groupBoxKiota
      // 
      this.groupBoxKiota.Controls.Add(this.labelDeleteTweetId);
      this.groupBoxKiota.Controls.Add(this.buttonDeleteTweet);
      this.groupBoxKiota.Controls.Add(this.textBoxDeleteTweetId);
      this.groupBoxKiota.Controls.Add(this.buttonKiotaLookupUser);
      this.groupBoxKiota.Controls.Add(this.buttonKiotaImage);
      this.groupBoxKiota.Controls.Add(this.buttonKiotaSimpleTweet);
      this.groupBoxKiota.Location = new System.Drawing.Point(27, 520);
      this.groupBoxKiota.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.groupBoxKiota.Name = "groupBoxKiota";
      this.groupBoxKiota.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.groupBoxKiota.Size = new System.Drawing.Size(368, 180);
      this.groupBoxKiota.TabIndex = 2;
      this.groupBoxKiota.TabStop = false;
      this.groupBoxKiota.Text = "Kiota/OpenAPI";
      // 
      // labelDeleteTweetId
      // 
      this.labelDeleteTweetId.AutoSize = true;
      this.labelDeleteTweetId.Location = new System.Drawing.Point(7, 96);
      this.labelDeleteTweetId.Name = "labelDeleteTweetId";
      this.labelDeleteTweetId.Size = new System.Drawing.Size(56, 15);
      this.labelDeleteTweetId.TabIndex = 8;
      this.labelDeleteTweetId.Text = "Tweet-Id:";
      // 
      // buttonDeleteTweet
      // 
      this.buttonDeleteTweet.Location = new System.Drawing.Point(268, 92);
      this.buttonDeleteTweet.Name = "buttonDeleteTweet";
      this.buttonDeleteTweet.Size = new System.Drawing.Size(75, 23);
      this.buttonDeleteTweet.TabIndex = 7;
      this.buttonDeleteTweet.Text = "Delete";
      this.buttonDeleteTweet.UseVisualStyleBackColor = true;
      this.buttonDeleteTweet.Click += this.buttonDeleteTweet_Click;
      // 
      // textBoxDeleteTweetId
      // 
      this.textBoxDeleteTweetId.Location = new System.Drawing.Point(68, 93);
      this.textBoxDeleteTweetId.Name = "textBoxDeleteTweetId";
      this.textBoxDeleteTweetId.Size = new System.Drawing.Size(194, 23);
      this.textBoxDeleteTweetId.TabIndex = 6;
      // 
      // buttonKiotaLookupUser
      // 
      this.buttonKiotaLookupUser.Location = new System.Drawing.Point(7, 137);
      this.buttonKiotaLookupUser.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.buttonKiotaLookupUser.Name = "buttonKiotaLookupUser";
      this.buttonKiotaLookupUser.Size = new System.Drawing.Size(147, 23);
      this.buttonKiotaLookupUser.TabIndex = 3;
      this.buttonKiotaLookupUser.Text = "My user data";
      this.buttonKiotaLookupUser.UseVisualStyleBackColor = true;
      this.buttonKiotaLookupUser.Click += this.buttonKiotaLookupUser_Click;
      // 
      // buttonKiotaImage
      // 
      this.buttonKiotaImage.Location = new System.Drawing.Point(7, 55);
      this.buttonKiotaImage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.buttonKiotaImage.Name = "buttonKiotaImage";
      this.buttonKiotaImage.Size = new System.Drawing.Size(147, 23);
      this.buttonKiotaImage.TabIndex = 2;
      this.buttonKiotaImage.Text = "Tweet with image";
      this.buttonKiotaImage.UseVisualStyleBackColor = true;
      this.buttonKiotaImage.Click += this.buttonKiotaImage_Click;
      // 
      // groupBoxConfig
      // 
      this.groupBoxConfig.Controls.Add(this.radioButtonOAuth2);
      this.groupBoxConfig.Controls.Add(this.groupBoxOAuth2);
      this.groupBoxConfig.Controls.Add(this.groupBoxOAuth1);
      this.groupBoxConfig.Controls.Add(this.radioButtonOAuth1);
      this.groupBoxConfig.Location = new System.Drawing.Point(27, 16);
      this.groupBoxConfig.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.groupBoxConfig.Name = "groupBoxConfig";
      this.groupBoxConfig.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.groupBoxConfig.Size = new System.Drawing.Size(755, 498);
      this.groupBoxConfig.TabIndex = 0;
      this.groupBoxConfig.TabStop = false;
      this.groupBoxConfig.Text = "Authorization config";
      // 
      // radioButtonOAuth2
      // 
      this.radioButtonOAuth2.AutoSize = true;
      this.radioButtonOAuth2.Location = new System.Drawing.Point(8, 200);
      this.radioButtonOAuth2.Name = "radioButtonOAuth2";
      this.radioButtonOAuth2.Size = new System.Drawing.Size(134, 19);
      this.radioButtonOAuth2.TabIndex = 2;
      this.radioButtonOAuth2.TabStop = true;
      this.radioButtonOAuth2.Text = "OAuth2-Anmeldung";
      this.radioButtonOAuth2.UseVisualStyleBackColor = true;
      this.radioButtonOAuth2.CheckedChanged += this.radioButtonOAuth2_CheckedChanged;
      // 
      // groupBoxOAuth2
      // 
      this.groupBoxOAuth2.Controls.Add(this.panelOAuth2CodeChallengeMethod);
      this.groupBoxOAuth2.Controls.Add(this.panelOAuth2ClientType);
      this.groupBoxOAuth2.Controls.Add(this.buttonOAuth2AccessTokenByRefreshToken);
      this.groupBoxOAuth2.Controls.Add(this.textBoxOAuth2RefreshToken);
      this.groupBoxOAuth2.Controls.Add(this.labelOAuth2RefreshToken);
      this.groupBoxOAuth2.Controls.Add(this.checkBoxOAuth2FetchRefreshToken);
      this.groupBoxOAuth2.Controls.Add(this.textBoxOAuth2RedirectUrl);
      this.groupBoxOAuth2.Controls.Add(this.labelOAuth2RedirectUrl);
      this.groupBoxOAuth2.Controls.Add(this.textBoxOAuth2AccessToken);
      this.groupBoxOAuth2.Controls.Add(this.labelOAuth2AccessToken);
      this.groupBoxOAuth2.Controls.Add(this.textBoxOAuth2ClientID);
      this.groupBoxOAuth2.Controls.Add(this.labelOAuth2ClientID);
      this.groupBoxOAuth2.Controls.Add(this.buttonOAuth2AccessToken);
      this.groupBoxOAuth2.Enabled = false;
      this.groupBoxOAuth2.Location = new System.Drawing.Point(20, 236);
      this.groupBoxOAuth2.Name = "groupBoxOAuth2";
      this.groupBoxOAuth2.Size = new System.Drawing.Size(713, 256);
      this.groupBoxOAuth2.TabIndex = 3;
      this.groupBoxOAuth2.TabStop = false;
      this.groupBoxOAuth2.Text = "OAuth2";
      // 
      // buttonOAuth2AccessTokenByRefreshToken
      // 
      this.buttonOAuth2AccessTokenByRefreshToken.Location = new System.Drawing.Point(575, 183);
      this.buttonOAuth2AccessTokenByRefreshToken.Name = "buttonOAuth2AccessTokenByRefreshToken";
      this.buttonOAuth2AccessTokenByRefreshToken.Size = new System.Drawing.Size(132, 23);
      this.buttonOAuth2AccessTokenByRefreshToken.TabIndex = 10;
      this.buttonOAuth2AccessTokenByRefreshToken.Text = "by Refresh Token";
      this.buttonOAuth2AccessTokenByRefreshToken.UseVisualStyleBackColor = true;
      this.buttonOAuth2AccessTokenByRefreshToken.Click += this.buttonOAuth2AccessTokenByRefreshToken_Click;
      // 
      // textBoxOAuth2RefreshToken
      // 
      this.textBoxOAuth2RefreshToken.Location = new System.Drawing.Point(180, 184);
      this.textBoxOAuth2RefreshToken.Name = "textBoxOAuth2RefreshToken";
      this.textBoxOAuth2RefreshToken.Size = new System.Drawing.Size(389, 23);
      this.textBoxOAuth2RefreshToken.TabIndex = 9;
      // 
      // labelOAuth2RefreshToken
      // 
      this.labelOAuth2RefreshToken.AutoSize = true;
      this.labelOAuth2RefreshToken.Location = new System.Drawing.Point(23, 187);
      this.labelOAuth2RefreshToken.Name = "labelOAuth2RefreshToken";
      this.labelOAuth2RefreshToken.Size = new System.Drawing.Size(79, 15);
      this.labelOAuth2RefreshToken.TabIndex = 8;
      this.labelOAuth2RefreshToken.Text = "Refresh token";
      // 
      // checkBoxOAuth2FetchRefreshToken
      // 
      this.checkBoxOAuth2FetchRefreshToken.AutoSize = true;
      this.checkBoxOAuth2FetchRefreshToken.Location = new System.Drawing.Point(180, 159);
      this.checkBoxOAuth2FetchRefreshToken.Name = "checkBoxOAuth2FetchRefreshToken";
      this.checkBoxOAuth2FetchRefreshToken.Size = new System.Drawing.Size(127, 19);
      this.checkBoxOAuth2FetchRefreshToken.TabIndex = 6;
      this.checkBoxOAuth2FetchRefreshToken.Text = "Fetch refresh token";
      this.checkBoxOAuth2FetchRefreshToken.UseVisualStyleBackColor = true;
      // 
      // labelOAuth2ClientSecret
      // 
      this.labelOAuth2ClientSecret.AutoSize = true;
      this.labelOAuth2ClientSecret.Location = new System.Drawing.Point(138, 22);
      this.labelOAuth2ClientSecret.Name = "labelOAuth2ClientSecret";
      this.labelOAuth2ClientSecret.Size = new System.Drawing.Size(73, 15);
      this.labelOAuth2ClientSecret.TabIndex = 2;
      this.labelOAuth2ClientSecret.Text = "Client Secret";
      // 
      // textBoxOAuth2ClientSecret
      // 
      this.textBoxOAuth2ClientSecret.Enabled = false;
      this.textBoxOAuth2ClientSecret.Location = new System.Drawing.Point(217, 19);
      this.textBoxOAuth2ClientSecret.Name = "textBoxOAuth2ClientSecret";
      this.textBoxOAuth2ClientSecret.Size = new System.Drawing.Size(172, 23);
      this.textBoxOAuth2ClientSecret.TabIndex = 3;
      // 
      // radioButtonOAuth2ConfidentialClient
      // 
      this.radioButtonOAuth2ConfidentialClient.AutoSize = true;
      this.radioButtonOAuth2ConfidentialClient.Location = new System.Drawing.Point(0, 20);
      this.radioButtonOAuth2ConfidentialClient.Name = "radioButtonOAuth2ConfidentialClient";
      this.radioButtonOAuth2ConfidentialClient.Size = new System.Drawing.Size(122, 19);
      this.radioButtonOAuth2ConfidentialClient.TabIndex = 1;
      this.radioButtonOAuth2ConfidentialClient.TabStop = true;
      this.radioButtonOAuth2ConfidentialClient.Text = "Confidential client";
      this.radioButtonOAuth2ConfidentialClient.UseVisualStyleBackColor = true;
      this.radioButtonOAuth2ConfidentialClient.CheckedChanged += this.radioButtonOAuth2ConfidentialClient_CheckedChanged;
      // 
      // radioButtonOAuth2PublicClient
      // 
      this.radioButtonOAuth2PublicClient.AutoSize = true;
      this.radioButtonOAuth2PublicClient.Checked = true;
      this.radioButtonOAuth2PublicClient.Location = new System.Drawing.Point(0, 0);
      this.radioButtonOAuth2PublicClient.Name = "radioButtonOAuth2PublicClient";
      this.radioButtonOAuth2PublicClient.Size = new System.Drawing.Size(90, 19);
      this.radioButtonOAuth2PublicClient.TabIndex = 0;
      this.radioButtonOAuth2PublicClient.TabStop = true;
      this.radioButtonOAuth2PublicClient.Text = "Public client";
      this.radioButtonOAuth2PublicClient.UseVisualStyleBackColor = true;
      // 
      // textBoxOAuth2RedirectUrl
      // 
      this.textBoxOAuth2RedirectUrl.Location = new System.Drawing.Point(180, 51);
      this.textBoxOAuth2RedirectUrl.Name = "textBoxOAuth2RedirectUrl";
      this.textBoxOAuth2RedirectUrl.Size = new System.Drawing.Size(389, 23);
      this.textBoxOAuth2RedirectUrl.TabIndex = 3;
      // 
      // labelOAuth2RedirectUrl
      // 
      this.labelOAuth2RedirectUrl.AutoSize = true;
      this.labelOAuth2RedirectUrl.Location = new System.Drawing.Point(22, 54);
      this.labelOAuth2RedirectUrl.Name = "labelOAuth2RedirectUrl";
      this.labelOAuth2RedirectUrl.Size = new System.Drawing.Size(71, 15);
      this.labelOAuth2RedirectUrl.TabIndex = 2;
      this.labelOAuth2RedirectUrl.Text = "Redirect Url:";
      // 
      // textBoxOAuth2AccessToken
      // 
      this.textBoxOAuth2AccessToken.Location = new System.Drawing.Point(180, 221);
      this.textBoxOAuth2AccessToken.Name = "textBoxOAuth2AccessToken";
      this.textBoxOAuth2AccessToken.Size = new System.Drawing.Size(389, 23);
      this.textBoxOAuth2AccessToken.TabIndex = 12;
      // 
      // labelOAuth2AccessToken
      // 
      this.labelOAuth2AccessToken.AutoSize = true;
      this.labelOAuth2AccessToken.Location = new System.Drawing.Point(23, 224);
      this.labelOAuth2AccessToken.Name = "labelOAuth2AccessToken";
      this.labelOAuth2AccessToken.Size = new System.Drawing.Size(79, 15);
      this.labelOAuth2AccessToken.TabIndex = 11;
      this.labelOAuth2AccessToken.Text = "Access token:";
      // 
      // textBoxOAuth2ClientID
      // 
      this.textBoxOAuth2ClientID.Location = new System.Drawing.Point(180, 22);
      this.textBoxOAuth2ClientID.Name = "textBoxOAuth2ClientID";
      this.textBoxOAuth2ClientID.Size = new System.Drawing.Size(389, 23);
      this.textBoxOAuth2ClientID.TabIndex = 1;
      // 
      // labelOAuth2ClientID
      // 
      this.labelOAuth2ClientID.AutoSize = true;
      this.labelOAuth2ClientID.Location = new System.Drawing.Point(22, 25);
      this.labelOAuth2ClientID.Name = "labelOAuth2ClientID";
      this.labelOAuth2ClientID.Size = new System.Drawing.Size(55, 15);
      this.labelOAuth2ClientID.TabIndex = 0;
      this.labelOAuth2ClientID.Text = "Client ID:";
      // 
      // buttonOAuth2AccessToken
      // 
      this.buttonOAuth2AccessToken.Location = new System.Drawing.Point(575, 156);
      this.buttonOAuth2AccessToken.Name = "buttonOAuth2AccessToken";
      this.buttonOAuth2AccessToken.Size = new System.Drawing.Size(132, 23);
      this.buttonOAuth2AccessToken.TabIndex = 7;
      this.buttonOAuth2AccessToken.Text = "Access Token";
      this.buttonOAuth2AccessToken.UseVisualStyleBackColor = true;
      this.buttonOAuth2AccessToken.Click += this.buttonOAuth2AccessToken_Click;
      // 
      // groupBoxOAuth1
      // 
      this.groupBoxOAuth1.Controls.Add(this.textBoxOAuth1AccessToken);
      this.groupBoxOAuth1.Controls.Add(this.labelOAuth1AccessToken);
      this.groupBoxOAuth1.Controls.Add(this.textBoxOAuth1AccessTokenSecret);
      this.groupBoxOAuth1.Controls.Add(this.buttonOAuth1PIN);
      this.groupBoxOAuth1.Controls.Add(this.labelOAuth1ConsumerAPIKeySecret);
      this.groupBoxOAuth1.Controls.Add(this.labelOAuth1AccessTokenSecret);
      this.groupBoxOAuth1.Controls.Add(this.textBoxOAuth1ConsumerAPIKeySecret);
      this.groupBoxOAuth1.Controls.Add(this.textBoxOAuth1ConsumerAPIKey);
      this.groupBoxOAuth1.Controls.Add(this.labelOAuth1ConsumerAPIKey);
      this.groupBoxOAuth1.Location = new System.Drawing.Point(27, 47);
      this.groupBoxOAuth1.Name = "groupBoxOAuth1";
      this.groupBoxOAuth1.Size = new System.Drawing.Size(706, 147);
      this.groupBoxOAuth1.TabIndex = 1;
      this.groupBoxOAuth1.TabStop = false;
      this.groupBoxOAuth1.Text = "OAuth1 configuration";
      // 
      // textBoxOAuth1AccessToken
      // 
      this.textBoxOAuth1AccessToken.Location = new System.Drawing.Point(173, 84);
      this.textBoxOAuth1AccessToken.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.textBoxOAuth1AccessToken.Name = "textBoxOAuth1AccessToken";
      this.textBoxOAuth1AccessToken.Size = new System.Drawing.Size(389, 23);
      this.textBoxOAuth1AccessToken.TabIndex = 5;
      // 
      // labelOAuth1AccessToken
      // 
      this.labelOAuth1AccessToken.AutoSize = true;
      this.labelOAuth1AccessToken.Location = new System.Drawing.Point(15, 87);
      this.labelOAuth1AccessToken.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.labelOAuth1AccessToken.Name = "labelOAuth1AccessToken";
      this.labelOAuth1AccessToken.Size = new System.Drawing.Size(79, 15);
      this.labelOAuth1AccessToken.TabIndex = 4;
      this.labelOAuth1AccessToken.Text = "Access token:";
      // 
      // textBoxOAuth1AccessTokenSecret
      // 
      this.textBoxOAuth1AccessTokenSecret.Location = new System.Drawing.Point(173, 114);
      this.textBoxOAuth1AccessTokenSecret.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.textBoxOAuth1AccessTokenSecret.Name = "textBoxOAuth1AccessTokenSecret";
      this.textBoxOAuth1AccessTokenSecret.Size = new System.Drawing.Size(389, 23);
      this.textBoxOAuth1AccessTokenSecret.TabIndex = 8;
      // 
      // buttonOAuth1PIN
      // 
      this.buttonOAuth1PIN.Location = new System.Drawing.Point(569, 83);
      this.buttonOAuth1PIN.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.buttonOAuth1PIN.Name = "buttonOAuth1PIN";
      this.buttonOAuth1PIN.Size = new System.Drawing.Size(130, 23);
      this.buttonOAuth1PIN.TabIndex = 6;
      this.buttonOAuth1PIN.Text = "With PIN";
      this.buttonOAuth1PIN.UseVisualStyleBackColor = true;
      this.buttonOAuth1PIN.Click += this.buttonOAuth1PIN_Click;
      // 
      // labelOAuth1ConsumerAPIKeySecret
      // 
      this.labelOAuth1ConsumerAPIKeySecret.AutoSize = true;
      this.labelOAuth1ConsumerAPIKeySecret.Location = new System.Drawing.Point(15, 55);
      this.labelOAuth1ConsumerAPIKeySecret.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.labelOAuth1ConsumerAPIKeySecret.Name = "labelOAuth1ConsumerAPIKeySecret";
      this.labelOAuth1ConsumerAPIKeySecret.Size = new System.Drawing.Size(141, 15);
      this.labelOAuth1ConsumerAPIKeySecret.TabIndex = 2;
      this.labelOAuth1ConsumerAPIKeySecret.Text = "Consumer API key secret:";
      // 
      // labelOAuth1AccessTokenSecret
      // 
      this.labelOAuth1AccessTokenSecret.AutoSize = true;
      this.labelOAuth1AccessTokenSecret.Location = new System.Drawing.Point(15, 117);
      this.labelOAuth1AccessTokenSecret.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.labelOAuth1AccessTokenSecret.Name = "labelOAuth1AccessTokenSecret";
      this.labelOAuth1AccessTokenSecret.Size = new System.Drawing.Size(113, 15);
      this.labelOAuth1AccessTokenSecret.TabIndex = 7;
      this.labelOAuth1AccessTokenSecret.Text = "Access token secret:";
      // 
      // textBoxOAuth1ConsumerAPIKeySecret
      // 
      this.textBoxOAuth1ConsumerAPIKeySecret.Location = new System.Drawing.Point(173, 52);
      this.textBoxOAuth1ConsumerAPIKeySecret.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.textBoxOAuth1ConsumerAPIKeySecret.Name = "textBoxOAuth1ConsumerAPIKeySecret";
      this.textBoxOAuth1ConsumerAPIKeySecret.Size = new System.Drawing.Size(389, 23);
      this.textBoxOAuth1ConsumerAPIKeySecret.TabIndex = 3;
      // 
      // textBoxOAuth1ConsumerAPIKey
      // 
      this.textBoxOAuth1ConsumerAPIKey.Location = new System.Drawing.Point(173, 22);
      this.textBoxOAuth1ConsumerAPIKey.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.textBoxOAuth1ConsumerAPIKey.Name = "textBoxOAuth1ConsumerAPIKey";
      this.textBoxOAuth1ConsumerAPIKey.Size = new System.Drawing.Size(389, 23);
      this.textBoxOAuth1ConsumerAPIKey.TabIndex = 1;
      // 
      // labelOAuth1ConsumerAPIKey
      // 
      this.labelOAuth1ConsumerAPIKey.AutoSize = true;
      this.labelOAuth1ConsumerAPIKey.Location = new System.Drawing.Point(15, 25);
      this.labelOAuth1ConsumerAPIKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.labelOAuth1ConsumerAPIKey.Name = "labelOAuth1ConsumerAPIKey";
      this.labelOAuth1ConsumerAPIKey.Size = new System.Drawing.Size(107, 15);
      this.labelOAuth1ConsumerAPIKey.TabIndex = 0;
      this.labelOAuth1ConsumerAPIKey.Text = "Consumer API key:";
      // 
      // radioButtonOAuth1
      // 
      this.radioButtonOAuth1.AutoSize = true;
      this.radioButtonOAuth1.Checked = true;
      this.radioButtonOAuth1.Location = new System.Drawing.Point(8, 22);
      this.radioButtonOAuth1.Name = "radioButtonOAuth1";
      this.radioButtonOAuth1.Size = new System.Drawing.Size(99, 19);
      this.radioButtonOAuth1.TabIndex = 0;
      this.radioButtonOAuth1.TabStop = true;
      this.radioButtonOAuth1.Text = "OAuth1 login:";
      this.radioButtonOAuth1.UseVisualStyleBackColor = true;
      // 
      // groupBoxPlainHttp
      // 
      this.groupBoxPlainHttp.Controls.Add(this.buttonPlainHttpLookupUser);
      this.groupBoxPlainHttp.Location = new System.Drawing.Point(402, 520);
      this.groupBoxPlainHttp.Name = "groupBoxPlainHttp";
      this.groupBoxPlainHttp.Size = new System.Drawing.Size(378, 178);
      this.groupBoxPlainHttp.TabIndex = 3;
      this.groupBoxPlainHttp.TabStop = false;
      this.groupBoxPlainHttp.Text = "Plain Http";
      // 
      // buttonPlainHttpLookupUser
      // 
      this.buttonPlainHttpLookupUser.Location = new System.Drawing.Point(7, 137);
      this.buttonPlainHttpLookupUser.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.buttonPlainHttpLookupUser.Name = "buttonPlainHttpLookupUser";
      this.buttonPlainHttpLookupUser.Size = new System.Drawing.Size(147, 23);
      this.buttonPlainHttpLookupUser.TabIndex = 4;
      this.buttonPlainHttpLookupUser.Text = "My user data";
      this.buttonPlainHttpLookupUser.UseVisualStyleBackColor = true;
      this.buttonPlainHttpLookupUser.Click += this.buttonPlainHttpLookupUser_Click;
      // 
      // panelOAuth2ClientType
      // 
      this.panelOAuth2ClientType.Controls.Add(this.radioButtonOAuth2PublicClient);
      this.panelOAuth2ClientType.Controls.Add(this.radioButtonOAuth2ConfidentialClient);
      this.panelOAuth2ClientType.Controls.Add(this.labelOAuth2ClientSecret);
      this.panelOAuth2ClientType.Controls.Add(this.textBoxOAuth2ClientSecret);
      this.panelOAuth2ClientType.Location = new System.Drawing.Point(180, 81);
      this.panelOAuth2ClientType.Name = "panelOAuth2ClientType";
      this.panelOAuth2ClientType.Size = new System.Drawing.Size(389, 44);
      this.panelOAuth2ClientType.TabIndex = 4;
      // 
      // panelOAuth2CodeChallengeMethod
      // 
      this.panelOAuth2CodeChallengeMethod.Controls.Add(this.radioButtonOAuth2CodeChallengeMethodSHA256);
      this.panelOAuth2CodeChallengeMethod.Controls.Add(this.radioButtonOAuth2CodeChallengeMethodPlain);
      this.panelOAuth2CodeChallengeMethod.Location = new System.Drawing.Point(180, 131);
      this.panelOAuth2CodeChallengeMethod.Name = "panelOAuth2CodeChallengeMethod";
      this.panelOAuth2CodeChallengeMethod.Size = new System.Drawing.Size(389, 19);
      this.panelOAuth2CodeChallengeMethod.TabIndex = 5;
      // 
      // radioButtonOAuth2CodeChallengeMethodSHA256
      // 
      this.radioButtonOAuth2CodeChallengeMethodSHA256.AutoSize = true;
      this.radioButtonOAuth2CodeChallengeMethodSHA256.Checked = true;
      this.radioButtonOAuth2CodeChallengeMethodSHA256.Location = new System.Drawing.Point(0, 0);
      this.radioButtonOAuth2CodeChallengeMethodSHA256.Name = "radioButtonOAuth2CodeChallengeMethodSHA256";
      this.radioButtonOAuth2CodeChallengeMethodSHA256.Size = new System.Drawing.Size(154, 19);
      this.radioButtonOAuth2CodeChallengeMethodSHA256.TabIndex = 0;
      this.radioButtonOAuth2CodeChallengeMethodSHA256.TabStop = true;
      this.radioButtonOAuth2CodeChallengeMethodSHA256.Text = "Code challenge: SHA256";
      this.radioButtonOAuth2CodeChallengeMethodSHA256.UseVisualStyleBackColor = true;
      // 
      // radioButtonOAuth2CodeChallengeMethodPlain
      // 
      this.radioButtonOAuth2CodeChallengeMethodPlain.AutoSize = true;
      this.radioButtonOAuth2CodeChallengeMethodPlain.Location = new System.Drawing.Point(217, 0);
      this.radioButtonOAuth2CodeChallengeMethodPlain.Name = "radioButtonOAuth2CodeChallengeMethodPlain";
      this.radioButtonOAuth2CodeChallengeMethodPlain.Size = new System.Drawing.Size(51, 19);
      this.radioButtonOAuth2CodeChallengeMethodPlain.TabIndex = 1;
      this.radioButtonOAuth2CodeChallengeMethodPlain.TabStop = true;
      this.radioButtonOAuth2CodeChallengeMethodPlain.Text = "Plain";
      this.radioButtonOAuth2CodeChallengeMethodPlain.UseVisualStyleBackColor = true;
      // 
      // FormXByKiotaTest
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(792, 712);
      this.Controls.Add(this.groupBoxPlainHttp);
      this.Controls.Add(this.groupBoxConfig);
      this.Controls.Add(this.groupBoxKiota);
      this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.Name = "FormXByKiotaTest";
      this.Text = "X test";
      this.groupBoxKiota.ResumeLayout(false);
      this.groupBoxKiota.PerformLayout();
      this.groupBoxConfig.ResumeLayout(false);
      this.groupBoxConfig.PerformLayout();
      this.groupBoxOAuth2.ResumeLayout(false);
      this.groupBoxOAuth2.PerformLayout();
      this.groupBoxOAuth1.ResumeLayout(false);
      this.groupBoxOAuth1.PerformLayout();
      this.groupBoxPlainHttp.ResumeLayout(false);
      this.panelOAuth2ClientType.ResumeLayout(false);
      this.panelOAuth2ClientType.PerformLayout();
      this.panelOAuth2CodeChallengeMethod.ResumeLayout(false);
      this.panelOAuth2CodeChallengeMethod.PerformLayout();
      this.ResumeLayout(false);
    }

    #endregion
    private System.Windows.Forms.Button buttonKiotaSimpleTweet;
    private System.Windows.Forms.GroupBox groupBoxKiota;
    private System.Windows.Forms.Button buttonKiotaImage;
    private System.Windows.Forms.GroupBox groupBoxConfig;
    private System.Windows.Forms.Label labelOAuth1ConsumerAPIKey;
    private System.Windows.Forms.TextBox textBoxOAuth1ConsumerAPIKey;
    private System.Windows.Forms.Label labelOAuth1AccessTokenSecret;
    private System.Windows.Forms.TextBox textBoxOAuth1AccessTokenSecret;
    private System.Windows.Forms.Label labelOAuth1AccessToken;
    private System.Windows.Forms.TextBox textBoxOAuth1AccessToken;
    private System.Windows.Forms.Label labelOAuth1ConsumerAPIKeySecret;
    private System.Windows.Forms.TextBox textBoxOAuth1ConsumerAPIKeySecret;
    private System.Windows.Forms.Button buttonKiotaLookupUser;
    private System.Windows.Forms.Button buttonOAuth1PIN;
    private System.Windows.Forms.Button buttonDeleteTweet;
    private System.Windows.Forms.TextBox textBoxDeleteTweetId;
    private System.Windows.Forms.Label labelDeleteTweetId;
    private System.Windows.Forms.GroupBox groupBoxPlainHttp;
    private System.Windows.Forms.Button buttonPlainHttpLookupUser;
    private System.Windows.Forms.Button buttonOAuth2AccessToken;
    private System.Windows.Forms.GroupBox groupBoxOAuth1;
    private System.Windows.Forms.RadioButton radioButtonOAuth1;
    private System.Windows.Forms.RadioButton radioButtonOAuth2;
    private System.Windows.Forms.GroupBox groupBoxOAuth2;
    private System.Windows.Forms.TextBox textBoxOAuth2ClientID;
    private System.Windows.Forms.Label labelOAuth2ClientID;
    private System.Windows.Forms.TextBox textBoxOAuth2AccessToken;
    private System.Windows.Forms.Label labelOAuth2AccessToken;
    private System.Windows.Forms.TextBox textBoxOAuth2RedirectUrl;
    private System.Windows.Forms.Label labelOAuth2RedirectUrl;
    private System.Windows.Forms.RadioButton radioButtonOAuth2ConfidentialClient;
    private System.Windows.Forms.RadioButton radioButtonOAuth2PublicClient;
    private System.Windows.Forms.Label labelOAuth2ClientSecret;
    private System.Windows.Forms.TextBox textBoxOAuth2ClientSecret;
    private System.Windows.Forms.CheckBox checkBoxOAuth2FetchRefreshToken;
    private System.Windows.Forms.TextBox textBoxOAuth2RefreshToken;
    private System.Windows.Forms.Label labelOAuth2RefreshToken;
    private System.Windows.Forms.Button buttonOAuth2AccessTokenByRefreshToken;
    private System.Windows.Forms.Panel panelOAuth2ClientType;
    private System.Windows.Forms.Panel panelOAuth2CodeChallengeMethod;
    private System.Windows.Forms.RadioButton radioButtonOAuth2CodeChallengeMethodSHA256;
    private System.Windows.Forms.RadioButton radioButtonOAuth2CodeChallengeMethodPlain;
  }
}
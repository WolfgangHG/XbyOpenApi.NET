namespace XbyOpenApi.OAuth1.WinForms
{
  partial class DialogOAuth1LoginWebView
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogOAuth1LoginWebView));
      this.webBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();
      ((System.ComponentModel.ISupportInitialize)this.webBrowser).BeginInit();
      this.SuspendLayout();
      // 
      // webBrowser
      // 
      this.webBrowser.AllowExternalDrop = true;
      resources.ApplyResources(this.webBrowser, "webBrowser");
      this.webBrowser.CreationProperties = null;
      this.webBrowser.DefaultBackgroundColor = System.Drawing.Color.White;
      this.webBrowser.Name = "webBrowser";
      this.webBrowser.ZoomFactor = 1D;
      this.webBrowser.CoreWebView2InitializationCompleted += this.webBrowser_CoreWebView2InitializationCompleted;
      this.webBrowser.NavigationCompleted += this.webBrowser_NavigationCompleted;
      // 
      // DialogOAuth2LoginWebView
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.webBrowser);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "DialogOAuth2LoginWebView";
      this.ShowInTaskbar = false;
      ((System.ComponentModel.ISupportInitialize)this.webBrowser).EndInit();
      this.ResumeLayout(false);
    }

    #endregion

    private Microsoft.Web.WebView2.WinForms.WebView2 webBrowser;
  }
}
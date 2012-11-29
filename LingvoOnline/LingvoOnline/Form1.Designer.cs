namespace LingvoOnline
{
  partial class Form1
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
      System.Windows.Forms.Label label1;
      this.mySearch = new System.Windows.Forms.TextBox();
      this.myWeb = new System.Windows.Forms.WebBrowser();
      label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // mySearch
      // 
      this.mySearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.mySearch.Location = new System.Drawing.Point(102, 13);
      this.mySearch.Name = "mySearch";
      this.mySearch.Size = new System.Drawing.Size(563, 20);
      this.mySearch.TabIndex = 0;
      this.mySearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.mySearch_KeyUp);
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new System.Drawing.Point(61, 13);
      label1.Name = "label1";
      label1.Size = new System.Drawing.Size(33, 13);
      label1.TabIndex = 1;
      label1.Text = "Wort:";
      // 
      // myWeb
      // 
      this.myWeb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.myWeb.Location = new System.Drawing.Point(12, 39);
      this.myWeb.MinimumSize = new System.Drawing.Size(20, 20);
      this.myWeb.Name = "myWeb";
      this.myWeb.Size = new System.Drawing.Size(656, 399);
      this.myWeb.TabIndex = 2;
      this.myWeb.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.myWeb_DocumentCompleted);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(680, 450);
      this.Controls.Add(this.myWeb);
      this.Controls.Add(label1);
      this.Controls.Add(this.mySearch);
      this.MinimumSize = new System.Drawing.Size(500, 300);
      this.Name = "Form1";
      this.Text = "Lingvo.Online translate";
      this.Activated += new System.EventHandler(this.Form1_Activated);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox mySearch;
    private System.Windows.Forms.WebBrowser myWeb;
  }
}


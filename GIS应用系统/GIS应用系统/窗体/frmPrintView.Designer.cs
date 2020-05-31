namespace GIS应用系统
{
    partial class frmPrintView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrintView));
            this.PrintPageLayoutControl = new ESRI.ArcGIS.Controls.AxPageLayoutControl();
            this.pageSetup = new System.Windows.Forms.Button();
            this.printPreview = new System.Windows.Forms.Button();
            this.print = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PrintPageLayoutControl)).BeginInit();
            this.SuspendLayout();
            // 
            // PrintPageLayoutControl
            // 
            this.PrintPageLayoutControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PrintPageLayoutControl.Location = new System.Drawing.Point(3, 1);
            this.PrintPageLayoutControl.Name = "PrintPageLayoutControl";
            this.PrintPageLayoutControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("PrintPageLayoutControl.OcxState")));
            this.PrintPageLayoutControl.Size = new System.Drawing.Size(360, 455);
            this.PrintPageLayoutControl.TabIndex = 0;
            // 
            // pageSetup
            // 
            this.pageSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pageSetup.Location = new System.Drawing.Point(382, 40);
            this.pageSetup.Name = "pageSetup";
            this.pageSetup.Size = new System.Drawing.Size(116, 51);
            this.pageSetup.TabIndex = 1;
            this.pageSetup.Text = "页面设置";
            this.pageSetup.UseVisualStyleBackColor = true;
            this.pageSetup.Click += new System.EventHandler(this.pageSetup_Click);
            // 
            // printPreview
            // 
            this.printPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.printPreview.Location = new System.Drawing.Point(382, 186);
            this.printPreview.Name = "printPreview";
            this.printPreview.Size = new System.Drawing.Size(116, 51);
            this.printPreview.TabIndex = 2;
            this.printPreview.Text = "打印预览";
            this.printPreview.UseVisualStyleBackColor = true;
            this.printPreview.Click += new System.EventHandler(this.printPreview_Click);
            // 
            // print
            // 
            this.print.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.print.Location = new System.Drawing.Point(382, 346);
            this.print.Name = "print";
            this.print.Size = new System.Drawing.Size(116, 49);
            this.print.TabIndex = 3;
            this.print.Text = "打印";
            this.print.UseVisualStyleBackColor = true;
            this.print.Click += new System.EventHandler(this.print_Click);
            // 
            // frmPrintView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(526, 468);
            this.Controls.Add(this.print);
            this.Controls.Add(this.printPreview);
            this.Controls.Add(this.pageSetup);
            this.Controls.Add(this.PrintPageLayoutControl);
            this.Name = "frmPrintView";
            this.Text = "打印";
            this.Load += new System.EventHandler(this.frmPrintView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PrintPageLayoutControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ESRI.ArcGIS.Controls.AxPageLayoutControl PrintPageLayoutControl;
        private System.Windows.Forms.Button pageSetup;
        private System.Windows.Forms.Button printPreview;
        private System.Windows.Forms.Button print;
    }
}
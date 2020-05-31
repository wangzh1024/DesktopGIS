namespace GIS应用系统
{
    partial class DotDensitySymbols
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DotDensitySymbols));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSelectSymbol = new System.Windows.Forms.Button();
            this.txtDotValue = new System.Windows.Forms.TextBox();
            this.btnSelectBackColor = new System.Windows.Forms.Button();
            this.nudDotSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lvRendererFields = new System.Windows.Forms.ListView();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAllOut = new System.Windows.Forms.Button();
            this.btnOut = new System.Windows.Forms.Button();
            this.btnIn = new System.Windows.Forms.Button();
            this.lstSourceFields = new System.Windows.Forms.ListBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSymbolize = new System.Windows.Forms.Button();
            this.cbxLayers2Symbolize = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Largeimage = new System.Windows.Forms.ImageList(this.components);
            this.Smallimage = new System.Windows.Forms.ImageList(this.components);
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDotSize)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSelectSymbol);
            this.groupBox2.Controls.Add(this.txtDotValue);
            this.groupBox2.Controls.Add(this.btnSelectBackColor);
            this.groupBox2.Controls.Add(this.nudDotSize);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(16, 234);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(448, 101);
            this.groupBox2.TabIndex = 28;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "符号设置";
            // 
            // btnSelectSymbol
            // 
            this.btnSelectSymbol.BackColor = System.Drawing.Color.Bisque;
            this.btnSelectSymbol.Location = new System.Drawing.Point(276, 21);
            this.btnSelectSymbol.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSelectSymbol.Name = "btnSelectSymbol";
            this.btnSelectSymbol.Size = new System.Drawing.Size(139, 29);
            this.btnSelectSymbol.TabIndex = 22;
            this.btnSelectSymbol.Text = "选择点符号";
            this.btnSelectSymbol.UseVisualStyleBackColor = false;
            this.btnSelectSymbol.Click += new System.EventHandler(this.btnSelectSymbol_Click);
            // 
            // txtDotValue
            // 
            this.txtDotValue.Location = new System.Drawing.Point(361, 60);
            this.txtDotValue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDotValue.Name = "txtDotValue";
            this.txtDotValue.Size = new System.Drawing.Size(52, 25);
            this.txtDotValue.TabIndex = 21;
            this.txtDotValue.Text = "2000";
            this.txtDotValue.TextChanged += new System.EventHandler(this.txtDotValue_TextChanged);
            // 
            // btnSelectBackColor
            // 
            this.btnSelectBackColor.BackColor = System.Drawing.Color.Bisque;
            this.btnSelectBackColor.Location = new System.Drawing.Point(35, 21);
            this.btnSelectBackColor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSelectBackColor.Name = "btnSelectBackColor";
            this.btnSelectBackColor.Size = new System.Drawing.Size(211, 29);
            this.btnSelectBackColor.TabIndex = 14;
            this.btnSelectBackColor.Text = "选择背景色...";
            this.btnSelectBackColor.UseVisualStyleBackColor = false;
            this.btnSelectBackColor.Click += new System.EventHandler(this.btnSelectBackColor_Click);
            // 
            // nudDotSize
            // 
            this.nudDotSize.DecimalPlaces = 2;
            this.nudDotSize.Location = new System.Drawing.Point(133, 60);
            this.nudDotSize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudDotSize.Maximum = new decimal(new int[] {
            18,
            0,
            0,
            0});
            this.nudDotSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nudDotSize.Name = "nudDotSize";
            this.nudDotSize.Size = new System.Drawing.Size(72, 25);
            this.nudDotSize.TabIndex = 20;
            this.nudDotSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudDotSize.ValueChanged += new System.EventHandler(this.nudDotSize_ValueChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(276, 68);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 19);
            this.label4.TabIndex = 5;
            this.label4.Text = "点值大小：";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(32, 68);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "点符号大小：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvRendererFields);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnAllOut);
            this.groupBox1.Controls.Add(this.btnOut);
            this.groupBox1.Controls.Add(this.btnIn);
            this.groupBox1.Controls.Add(this.lstSourceFields);
            this.groupBox1.Location = new System.Drawing.Point(16, 45);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(448, 181);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "字段";
            // 
            // lvRendererFields
            // 
            this.lvRendererFields.Location = new System.Drawing.Point(264, 40);
            this.lvRendererFields.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lvRendererFields.Name = "lvRendererFields";
            this.lvRendererFields.Size = new System.Drawing.Size(169, 124);
            this.lvRendererFields.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvRendererFields.TabIndex = 28;
            this.lvRendererFields.UseCompatibleStateImageBehavior = false;
            this.lvRendererFields.DoubleClick += new System.EventHandler(this.lvRendererFields_DoubleClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(273, 20);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 15);
            this.label5.TabIndex = 27;
            this.label5.Text = "用于符号化的字段：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 26;
            this.label1.Text = "所有字段：";
            // 
            // btnAllOut
            // 
            this.btnAllOut.Image = ((System.Drawing.Image)(resources.GetObject("btnAllOut.Image")));
            this.btnAllOut.Location = new System.Drawing.Point(199, 124);
            this.btnAllOut.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAllOut.Name = "btnAllOut";
            this.btnAllOut.Size = new System.Drawing.Size(47, 29);
            this.btnAllOut.TabIndex = 3;
            this.btnAllOut.UseVisualStyleBackColor = true;
            this.btnAllOut.Click += new System.EventHandler(this.btnAllOut_Click);
            // 
            // btnOut
            // 
            this.btnOut.Image = ((System.Drawing.Image)(resources.GetObject("btnOut.Image")));
            this.btnOut.Location = new System.Drawing.Point(199, 88);
            this.btnOut.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOut.Name = "btnOut";
            this.btnOut.Size = new System.Drawing.Size(47, 29);
            this.btnOut.TabIndex = 2;
            this.btnOut.UseVisualStyleBackColor = true;
            this.btnOut.Click += new System.EventHandler(this.btnOut_Click);
            // 
            // btnIn
            // 
            this.btnIn.Image = ((System.Drawing.Image)(resources.GetObject("btnIn.Image")));
            this.btnIn.Location = new System.Drawing.Point(199, 51);
            this.btnIn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnIn.Name = "btnIn";
            this.btnIn.Size = new System.Drawing.Size(47, 29);
            this.btnIn.TabIndex = 1;
            this.btnIn.UseVisualStyleBackColor = true;
            this.btnIn.Click += new System.EventHandler(this.btnIn_Click);
            // 
            // lstSourceFields
            // 
            this.lstSourceFields.FormattingEnabled = true;
            this.lstSourceFields.ItemHeight = 15;
            this.lstSourceFields.Location = new System.Drawing.Point(13, 40);
            this.lstSourceFields.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lstSourceFields.Name = "lstSourceFields";
            this.lstSourceFields.Size = new System.Drawing.Size(169, 124);
            this.lstSourceFields.Sorted = true;
            this.lstSourceFields.TabIndex = 0;
            this.lstSourceFields.DoubleClick += new System.EventHandler(this.lstSourceFields_DoubleClick);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(315, 436);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(88, 29);
            this.btnClose.TabIndex = 25;
            this.btnClose.Text = "关 闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSymbolize
            // 
            this.btnSymbolize.Location = new System.Drawing.Point(315, 382);
            this.btnSymbolize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSymbolize.Name = "btnSymbolize";
            this.btnSymbolize.Size = new System.Drawing.Size(88, 29);
            this.btnSymbolize.TabIndex = 24;
            this.btnSymbolize.Text = "符号化";
            this.btnSymbolize.UseVisualStyleBackColor = true;
            this.btnSymbolize.Click += new System.EventHandler(this.btnSymbolize_Click);
            // 
            // cbxLayers2Symbolize
            // 
            this.cbxLayers2Symbolize.FormattingEnabled = true;
            this.cbxLayers2Symbolize.Location = new System.Drawing.Point(156, 12);
            this.cbxLayers2Symbolize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbxLayers2Symbolize.Name = "cbxLayers2Symbolize";
            this.cbxLayers2Symbolize.Size = new System.Drawing.Size(307, 23);
            this.cbxLayers2Symbolize.TabIndex = 23;
            this.cbxLayers2Symbolize.SelectedIndexChanged += new System.EventHandler(this.cbxLayers2Symbolize_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 18);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 15);
            this.label2.TabIndex = 22;
            this.label2.Text = "选择符号化图层：";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(16, 345);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(239, 149);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            // 
            // Largeimage
            // 
            this.Largeimage.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.Largeimage.ImageSize = new System.Drawing.Size(16, 16);
            this.Largeimage.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // Smallimage
            // 
            this.Smallimage.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.Smallimage.ImageSize = new System.Drawing.Size(16, 16);
            this.Smallimage.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // DotDensitySymbols
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MistyRose;
            this.ClientSize = new System.Drawing.Size(496, 512);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSymbolize);
            this.Controls.Add(this.cbxLayers2Symbolize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "DotDensitySymbols";
            this.Text = "点值符号化";
            this.Load += new System.EventHandler(this.DotDensitySymbols_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDotSize)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtDotValue;
        private System.Windows.Forms.Button btnSelectBackColor;
        private System.Windows.Forms.NumericUpDown nudDotSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lvRendererFields;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAllOut;
        private System.Windows.Forms.Button btnOut;
        private System.Windows.Forms.Button btnIn;
        private System.Windows.Forms.ListBox lstSourceFields;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSymbolize;
        private System.Windows.Forms.ComboBox cbxLayers2Symbolize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnSelectSymbol;
        private System.Windows.Forms.ImageList Largeimage;
        private System.Windows.Forms.ImageList Smallimage;
    }
}
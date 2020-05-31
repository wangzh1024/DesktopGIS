namespace GIS应用系统
{
    partial class frmAddXYData
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
            this.labelOpenExcelData = new System.Windows.Forms.Label();
            this.labelSelectDataTable = new System.Windows.Forms.Label();
            this.labelXField = new System.Windows.Forms.Label();
            this.labelYField = new System.Windows.Forms.Label();
            this.textBoxExcelPath = new System.Windows.Forms.TextBox();
            this.comboBoxExcelSheets = new System.Windows.Forms.ComboBox();
            this.comboBoxXField = new System.Windows.Forms.ComboBox();
            this.comboBoxYield = new System.Windows.Forms.ComboBox();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelOpenExcelData
            // 
            this.labelOpenExcelData.AutoSize = true;
            this.labelOpenExcelData.Location = new System.Drawing.Point(46, 52);
            this.labelOpenExcelData.Name = "labelOpenExcelData";
            this.labelOpenExcelData.Size = new System.Drawing.Size(122, 15);
            this.labelOpenExcelData.TabIndex = 0;
            this.labelOpenExcelData.Text = "打开Excel文件：";
            // 
            // labelSelectDataTable
            // 
            this.labelSelectDataTable.AutoSize = true;
            this.labelSelectDataTable.Location = new System.Drawing.Point(46, 122);
            this.labelSelectDataTable.Name = "labelSelectDataTable";
            this.labelSelectDataTable.Size = new System.Drawing.Size(97, 15);
            this.labelSelectDataTable.TabIndex = 1;
            this.labelSelectDataTable.Text = "选择数据表：";
            // 
            // labelXField
            // 
            this.labelXField.AutoSize = true;
            this.labelXField.Location = new System.Drawing.Point(49, 184);
            this.labelXField.Name = "labelXField";
            this.labelXField.Size = new System.Drawing.Size(60, 15);
            this.labelXField.TabIndex = 2;
            this.labelXField.Text = "X字段：";
            // 
            // labelYField
            // 
            this.labelYField.AutoSize = true;
            this.labelYField.Location = new System.Drawing.Point(353, 187);
            this.labelYField.Name = "labelYField";
            this.labelYField.Size = new System.Drawing.Size(60, 15);
            this.labelYField.TabIndex = 3;
            this.labelYField.Text = "Y字段：";
            // 
            // textBoxExcelPath
            // 
            this.textBoxExcelPath.Location = new System.Drawing.Point(174, 49);
            this.textBoxExcelPath.Name = "textBoxExcelPath";
            this.textBoxExcelPath.Size = new System.Drawing.Size(275, 25);
            this.textBoxExcelPath.TabIndex = 4;
            // 
            // comboBoxExcelSheets
            // 
            this.comboBoxExcelSheets.FormattingEnabled = true;
            this.comboBoxExcelSheets.Location = new System.Drawing.Point(174, 122);
            this.comboBoxExcelSheets.Name = "comboBoxExcelSheets";
            this.comboBoxExcelSheets.Size = new System.Drawing.Size(121, 23);
            this.comboBoxExcelSheets.TabIndex = 5;
            this.comboBoxExcelSheets.SelectedIndexChanged += new System.EventHandler(this.comboBoxExcelSheets_SelectedIndexChanged);
            // 
            // comboBoxXField
            // 
            this.comboBoxXField.FormattingEnabled = true;
            this.comboBoxXField.Location = new System.Drawing.Point(174, 184);
            this.comboBoxXField.Name = "comboBoxXField";
            this.comboBoxXField.Size = new System.Drawing.Size(121, 23);
            this.comboBoxXField.TabIndex = 6;
            // 
            // comboBoxYield
            // 
            this.comboBoxYield.FormattingEnabled = true;
            this.comboBoxYield.Location = new System.Drawing.Point(441, 181);
            this.comboBoxYield.Name = "comboBoxYield";
            this.comboBoxYield.Size = new System.Drawing.Size(121, 23);
            this.comboBoxYield.TabIndex = 7;
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(474, 49);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(100, 29);
            this.buttonOpen.TabIndex = 8;
            this.buttonOpen.Text = "打开";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.btnOpenExcel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(174, 269);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 29);
            this.buttonOK.TabIndex = 9;
            this.buttonOK.Text = "确定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancle
            // 
            this.buttonCancle.Location = new System.Drawing.Point(329, 269);
            this.buttonCancle.Name = "buttonCancle";
            this.buttonCancle.Size = new System.Drawing.Size(100, 29);
            this.buttonCancle.TabIndex = 10;
            this.buttonCancle.Text = "取消";
            this.buttonCancle.UseVisualStyleBackColor = true;
            this.buttonCancle.Click += new System.EventHandler(this.buttonCancle_Click);
            // 
            // frmAddXYData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 335);
            this.Controls.Add(this.buttonCancle);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.comboBoxYield);
            this.Controls.Add(this.comboBoxXField);
            this.Controls.Add(this.comboBoxExcelSheets);
            this.Controls.Add(this.textBoxExcelPath);
            this.Controls.Add(this.labelYField);
            this.Controls.Add(this.labelXField);
            this.Controls.Add(this.labelSelectDataTable);
            this.Controls.Add(this.labelOpenExcelData);
            this.Name = "frmAddXYData";
            this.Text = "添加XY数据";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelOpenExcelData;
        private System.Windows.Forms.Label labelSelectDataTable;
        private System.Windows.Forms.Label labelXField;
        private System.Windows.Forms.Label labelYField;
        private System.Windows.Forms.TextBox textBoxExcelPath;
        private System.Windows.Forms.ComboBox comboBoxExcelSheets;
        private System.Windows.Forms.ComboBox comboBoxXField;
        private System.Windows.Forms.ComboBox comboBoxYield;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancle;
    }
}
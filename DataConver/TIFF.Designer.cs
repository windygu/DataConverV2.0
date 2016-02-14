namespace DataConver
{
    partial class TIFF
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TIFF));
            this.circularProgress1 = new DevComponents.DotNetBar.Controls.CircularProgress();
            this.but_tif = new DevComponents.DotNetBar.ButtonX();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textEdit2 = new DevExpress.XtraEditors.TextEdit();
            this.but_output = new DevComponents.DotNetBar.ButtonX();
            this.but_input = new DevComponents.DotNetBar.ButtonX();
            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).BeginInit();
            this.panelEx1.SuspendLayout();
            this.SuspendLayout();
            // 
            // circularProgress1
            // 
            // 
            // 
            // 
            this.circularProgress1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.circularProgress1.Location = new System.Drawing.Point(186, 117);
            this.circularProgress1.Name = "circularProgress1";
            this.circularProgress1.Size = new System.Drawing.Size(97, 41);
            this.circularProgress1.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeXP;
            this.circularProgress1.TabIndex = 0;
            // 
            // but_tif
            // 
            this.but_tif.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.but_tif.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.but_tif.Location = new System.Drawing.Point(179, 179);
            this.but_tif.Name = "but_tif";
            this.but_tif.Size = new System.Drawing.Size(108, 23);
            this.but_tif.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.but_tif.TabIndex = 1;
            this.but_tif.Text = "提取淹没过程";
            // 
            // textEdit1
            // 
            this.textEdit1.Location = new System.Drawing.Point(129, 89);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Size = new System.Drawing.Size(280, 20);
            this.textEdit1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "设置数据输出路径：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "设置数据读入路径：";
            // 
            // textEdit2
            // 
            this.textEdit2.Location = new System.Drawing.Point(129, 26);
            this.textEdit2.Name = "textEdit2";
            this.textEdit2.Size = new System.Drawing.Size(280, 20);
            this.textEdit2.TabIndex = 5;
            // 
            // but_output
            // 
            this.but_output.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.but_output.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.but_output.Image = ((System.Drawing.Image)(resources.GetObject("but_output.Image")));
            this.but_output.Location = new System.Drawing.Point(425, 82);
            this.but_output.Name = "but_output";
            this.but_output.Size = new System.Drawing.Size(37, 27);
            this.but_output.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.but_output.Symbol = "";
            this.but_output.TabIndex = 7;
            // 
            // but_input
            // 
            this.but_input.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.but_input.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.but_input.Image = ((System.Drawing.Image)(resources.GetObject("but_input.Image")));
            this.but_input.Location = new System.Drawing.Point(425, 23);
            this.but_input.Name = "but_input";
            this.but_input.Size = new System.Drawing.Size(37, 27);
            this.but_input.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.but_input.Symbol = "";
            this.but_input.TabIndex = 7;
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.panelEx1.Controls.Add(this.circularProgress1);
            this.panelEx1.Controls.Add(this.label2);
            this.panelEx1.Controls.Add(this.but_tif);
            this.panelEx1.Controls.Add(this.but_input);
            this.panelEx1.Controls.Add(this.textEdit1);
            this.panelEx1.Controls.Add(this.but_output);
            this.panelEx1.Controls.Add(this.label1);
            this.panelEx1.Controls.Add(this.textEdit2);
            this.panelEx1.DisabledBackColor = System.Drawing.Color.Empty;
            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEx1.Location = new System.Drawing.Point(0, 0);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(482, 214);
            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.TabIndex = 8;
            // 
            // TIFF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 214);
            this.Controls.Add(this.panelEx1);
            this.Name = "TIFF";
            this.Text = "生成淹没过程";
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).EndInit();
            this.panelEx1.ResumeLayout(false);
            this.panelEx1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.CircularProgress circularProgress1;
        private DevComponents.DotNetBar.ButtonX but_tif;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.TextEdit textEdit2;
        private DevComponents.DotNetBar.ButtonX but_output;
        private DevComponents.DotNetBar.ButtonX but_input;
        private DevComponents.DotNetBar.PanelEx panelEx1;
    }
}
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
            this.but_tif = new DevComponents.DotNetBar.ButtonX();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.but_output = new DevComponents.DotNetBar.ButtonX();
            this.but_input = new DevComponents.DotNetBar.ButtonX();
            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
            this.buttonX1 = new DevComponents.DotNetBar.ButtonX();
            this.label3 = new System.Windows.Forms.Label();
            this.ExtractMesg = new DevComponents.DotNetBar.LabelX();
            this.ReferShp = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.ReferTxt = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.ReferCz = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.panelEx1.SuspendLayout();
            this.SuspendLayout();
            // 
            // but_tif
            // 
            this.but_tif.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.but_tif.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.but_tif.Location = new System.Drawing.Point(175, 152);
            this.but_tif.Name = "but_tif";
            this.but_tif.Size = new System.Drawing.Size(108, 23);
            this.but_tif.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.but_tif.TabIndex = 1;
            this.but_tif.Text = "提取淹没过程";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "设置叠加村庄数据：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "设置参考数据：";
            // 
            // but_output
            // 
            this.but_output.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.but_output.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.but_output.Image = ((System.Drawing.Image)(resources.GetObject("but_output.Image")));
            this.but_output.Location = new System.Drawing.Point(425, 86);
            this.but_output.Name = "but_output";
            this.but_output.Size = new System.Drawing.Size(37, 22);
            this.but_output.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.but_output.Symbol = "";
            this.but_output.TabIndex = 7;
            this.but_output.Click += new System.EventHandler(this.but_output_Click);
            // 
            // but_input
            // 
            this.but_input.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.but_input.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.but_input.Image = ((System.Drawing.Image)(resources.GetObject("but_input.Image")));
            this.but_input.Location = new System.Drawing.Point(425, 23);
            this.but_input.Name = "but_input";
            this.but_input.Size = new System.Drawing.Size(37, 22);
            this.but_input.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.but_input.Symbol = "";
            this.but_input.TabIndex = 7;
            this.but_input.Click += new System.EventHandler(this.but_input_Click);
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.panelEx1.Controls.Add(this.ReferCz);
            this.panelEx1.Controls.Add(this.ReferTxt);
            this.panelEx1.Controls.Add(this.ReferShp);
            this.panelEx1.Controls.Add(this.ExtractMesg);
            this.panelEx1.Controls.Add(this.label3);
            this.panelEx1.Controls.Add(this.label2);
            this.panelEx1.Controls.Add(this.but_tif);
            this.panelEx1.Controls.Add(this.buttonX1);
            this.panelEx1.Controls.Add(this.but_input);
            this.panelEx1.Controls.Add(this.but_output);
            this.panelEx1.Controls.Add(this.label1);
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
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1.Image = ((System.Drawing.Image)(resources.GetObject("buttonX1.Image")));
            this.buttonX1.Location = new System.Drawing.Point(425, 54);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.Size = new System.Drawing.Size(37, 20);
            this.buttonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1.Symbol = "";
            this.buttonX1.TabIndex = 7;
            this.buttonX1.Click += new System.EventHandler(this.buttonX1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "设置文本数据：";
            // 
            // ExtractMesg
            // 
            // 
            // 
            // 
            this.ExtractMesg.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ExtractMesg.Location = new System.Drawing.Point(175, 123);
            this.ExtractMesg.Name = "ExtractMesg";
            this.ExtractMesg.Size = new System.Drawing.Size(215, 23);
            this.ExtractMesg.TabIndex = 8;
            this.ExtractMesg.Text = "提取信息输出";
            // 
            // ReferShp
            // 
            // 
            // 
            // 
            this.ReferShp.Border.Class = "TextBoxBorder";
            this.ReferShp.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ReferShp.ButtonCustom.Tooltip = "";
            this.ReferShp.ButtonCustom2.Tooltip = "";
            this.ReferShp.Location = new System.Drawing.Point(108, 24);
            this.ReferShp.Name = "ReferShp";
            this.ReferShp.PreventEnterBeep = true;
            this.ReferShp.Size = new System.Drawing.Size(311, 21);
            this.ReferShp.TabIndex = 9;
            // 
            // ReferTxt
            // 
            // 
            // 
            // 
            this.ReferTxt.Border.Class = "TextBoxBorder";
            this.ReferTxt.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ReferTxt.ButtonCustom.Tooltip = "";
            this.ReferTxt.ButtonCustom2.Tooltip = "";
            this.ReferTxt.Location = new System.Drawing.Point(108, 54);
            this.ReferTxt.Name = "ReferTxt";
            this.ReferTxt.PreventEnterBeep = true;
            this.ReferTxt.Size = new System.Drawing.Size(311, 21);
            this.ReferTxt.TabIndex = 9;
            // 
            // ReferCz
            // 
            // 
            // 
            // 
            this.ReferCz.Border.Class = "TextBoxBorder";
            this.ReferCz.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.ReferCz.ButtonCustom.Tooltip = "";
            this.ReferCz.ButtonCustom2.Tooltip = "";
            this.ReferCz.Location = new System.Drawing.Point(124, 87);
            this.ReferCz.Name = "ReferCz";
            this.ReferCz.PreventEnterBeep = true;
            this.ReferCz.Size = new System.Drawing.Size(295, 21);
            this.ReferCz.TabIndex = 9;
            // 
            // TIFF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 214);
            this.Controls.Add(this.panelEx1);
            this.Name = "TIFF";
            this.Text = "生成淹没过程";
            this.panelEx1.ResumeLayout(false);
            this.panelEx1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX but_tif;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevComponents.DotNetBar.ButtonX but_output;
        private DevComponents.DotNetBar.ButtonX but_input;
        private DevComponents.DotNetBar.PanelEx panelEx1;
        private System.Windows.Forms.Label label3;
        private DevComponents.DotNetBar.ButtonX buttonX1;
        private DevComponents.DotNetBar.LabelX ExtractMesg;
        private DevComponents.DotNetBar.Controls.TextBoxX ReferShp;
        private DevComponents.DotNetBar.Controls.TextBoxX ReferTxt;
        private DevComponents.DotNetBar.Controls.TextBoxX ReferCz;
    }
}
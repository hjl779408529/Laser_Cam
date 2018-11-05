namespace Laser_Version2._0.UI
{
    partial class Laser_Watt
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
            this.Re_connect = new System.Windows.Forms.Button();
            this.Com_List = new System.Windows.Forms.ComboBox();
            this.Com_Status = new System.Windows.Forms.PictureBox();
            this.Current_Laser_Watt = new System.Windows.Forms.TextBox();
            this.Laser_Percent = new System.Windows.Forms.TextBox();
            this.Laser_Percent_Label = new System.Windows.Forms.Label();
            this.Laser_Current_Watt_Label = new System.Windows.Forms.Label();
            this.Cal_Data_Acquisition = new System.Windows.Forms.Button();
            this.Acquisition_Once = new System.Windows.Forms.Button();
            this.Save_Data = new System.Windows.Forms.Button();
            this.Refresh_List = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Com_Status)).BeginInit();
            this.SuspendLayout();
            // 
            // Re_connect
            // 
            this.Re_connect.Location = new System.Drawing.Point(194, 68);
            this.Re_connect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Re_connect.Name = "Re_connect";
            this.Re_connect.Size = new System.Drawing.Size(122, 35);
            this.Re_connect.TabIndex = 18;
            this.Re_connect.Text = "打开串口";
            this.Re_connect.UseVisualStyleBackColor = true;
            this.Re_connect.Click += new System.EventHandler(this.Re_connect_Click);
            // 
            // Com_List
            // 
            this.Com_List.FormattingEnabled = true;
            this.Com_List.Location = new System.Drawing.Point(194, 28);
            this.Com_List.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Com_List.Name = "Com_List";
            this.Com_List.Size = new System.Drawing.Size(121, 26);
            this.Com_List.TabIndex = 19;
            this.Com_List.SelectedIndexChanged += new System.EventHandler(this.Com_List_SelectedIndexChanged);
            // 
            // Com_Status
            // 
            this.Com_Status.Location = new System.Drawing.Point(395, 19);
            this.Com_Status.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Com_Status.Name = "Com_Status";
            this.Com_Status.Size = new System.Drawing.Size(32, 32);
            this.Com_Status.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.Com_Status.TabIndex = 21;
            this.Com_Status.TabStop = false;
            // 
            // Current_Laser_Watt
            // 
            this.Current_Laser_Watt.Location = new System.Drawing.Point(291, 175);
            this.Current_Laser_Watt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Current_Laser_Watt.Name = "Current_Laser_Watt";
            this.Current_Laser_Watt.Size = new System.Drawing.Size(121, 28);
            this.Current_Laser_Watt.TabIndex = 22;
            // 
            // Laser_Percent
            // 
            this.Laser_Percent.Location = new System.Drawing.Point(102, 175);
            this.Laser_Percent.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Laser_Percent.Name = "Laser_Percent";
            this.Laser_Percent.Size = new System.Drawing.Size(121, 28);
            this.Laser_Percent.TabIndex = 23;
            // 
            // Laser_Percent_Label
            // 
            this.Laser_Percent_Label.AutoSize = true;
            this.Laser_Percent_Label.Location = new System.Drawing.Point(82, 144);
            this.Laser_Percent_Label.Name = "Laser_Percent_Label";
            this.Laser_Percent_Label.Size = new System.Drawing.Size(161, 18);
            this.Laser_Percent_Label.TabIndex = 24;
            this.Laser_Percent_Label.Text = "激光输出百分比(%)";
            // 
            // Laser_Current_Watt_Label
            // 
            this.Laser_Current_Watt_Label.AutoSize = true;
            this.Laser_Current_Watt_Label.Location = new System.Drawing.Point(275, 144);
            this.Laser_Current_Watt_Label.Name = "Laser_Current_Watt_Label";
            this.Laser_Current_Watt_Label.Size = new System.Drawing.Size(152, 18);
            this.Laser_Current_Watt_Label.TabIndex = 25;
            this.Laser_Current_Watt_Label.Text = "激光实时功率(mw)";
            // 
            // Cal_Data_Acquisition
            // 
            this.Cal_Data_Acquisition.Enabled = false;
            this.Cal_Data_Acquisition.Location = new System.Drawing.Point(575, 9);
            this.Cal_Data_Acquisition.Name = "Cal_Data_Acquisition";
            this.Cal_Data_Acquisition.Size = new System.Drawing.Size(141, 42);
            this.Cal_Data_Acquisition.TabIndex = 26;
            this.Cal_Data_Acquisition.Text = "矫正数据采集";
            this.Cal_Data_Acquisition.UseVisualStyleBackColor = true;
            this.Cal_Data_Acquisition.Visible = false;
            this.Cal_Data_Acquisition.Click += new System.EventHandler(this.Cal_Data_Acquisition_Click);
            // 
            // Acquisition_Once
            // 
            this.Acquisition_Once.Location = new System.Drawing.Point(94, 228);
            this.Acquisition_Once.Name = "Acquisition_Once";
            this.Acquisition_Once.Size = new System.Drawing.Size(136, 42);
            this.Acquisition_Once.TabIndex = 27;
            this.Acquisition_Once.Text = "采集一次";
            this.Acquisition_Once.UseVisualStyleBackColor = true;
            this.Acquisition_Once.Click += new System.EventHandler(this.Acquisition_Once_Click);
            // 
            // Save_Data
            // 
            this.Save_Data.Location = new System.Drawing.Point(283, 228);
            this.Save_Data.Name = "Save_Data";
            this.Save_Data.Size = new System.Drawing.Size(136, 42);
            this.Save_Data.TabIndex = 28;
            this.Save_Data.Text = "保存采集数据";
            this.Save_Data.UseVisualStyleBackColor = true;
            this.Save_Data.Click += new System.EventHandler(this.Save_Data_Click);
            // 
            // Refresh_List
            // 
            this.Refresh_List.Location = new System.Drawing.Point(46, 23);
            this.Refresh_List.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Refresh_List.Name = "Refresh_List";
            this.Refresh_List.Size = new System.Drawing.Size(122, 35);
            this.Refresh_List.TabIndex = 29;
            this.Refresh_List.Text = "更新列表";
            this.Refresh_List.UseVisualStyleBackColor = true;
            this.Refresh_List.Click += new System.EventHandler(this.Refresh_List_Click);
            // 
            // Laser_Watt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 287);
            this.Controls.Add(this.Refresh_List);
            this.Controls.Add(this.Save_Data);
            this.Controls.Add(this.Acquisition_Once);
            this.Controls.Add(this.Cal_Data_Acquisition);
            this.Controls.Add(this.Laser_Current_Watt_Label);
            this.Controls.Add(this.Laser_Percent_Label);
            this.Controls.Add(this.Laser_Percent);
            this.Controls.Add(this.Current_Laser_Watt);
            this.Controls.Add(this.Com_Status);
            this.Controls.Add(this.Re_connect);
            this.Controls.Add(this.Com_List);
            this.Name = "Laser_Watt";
            this.Text = "Laser_Watt";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Laser_Watt_FormClosed);
            this.Load += new System.EventHandler(this.Laser_Watt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Com_Status)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Com_Status;
        private System.Windows.Forms.Button Re_connect;
        private System.Windows.Forms.ComboBox Com_List;
        private System.Windows.Forms.TextBox Current_Laser_Watt;
        private System.Windows.Forms.TextBox Laser_Percent;
        private System.Windows.Forms.Label Laser_Percent_Label;
        private System.Windows.Forms.Label Laser_Current_Watt_Label;
        private System.Windows.Forms.Button Cal_Data_Acquisition;
        private System.Windows.Forms.Button Acquisition_Once;
        private System.Windows.Forms.Button Save_Data;
        private System.Windows.Forms.Button Refresh_List;
    }
}
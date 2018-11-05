namespace Laser_Version2._0
{
    partial class Laser_Control_Panel
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
            this.Power_On = new System.Windows.Forms.Button();
            this.Power_OFF = new System.Windows.Forms.Button();
            this.Refresh_Status = new System.Windows.Forms.Button();
            this.Reset_Laser = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Seed1_Current = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Amp2_Current = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Amp1_Current = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Com_List = new System.Windows.Forms.ComboBox();
            this.Re_connect = new System.Windows.Forms.Button();
            this.Laser_Frequence_Set_Value = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Laser_Watt_Set_Value = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.Watt_Confirm = new System.Windows.Forms.Button();
            this.Frequence_Confirm = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Amp2_Set_Current = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.Amp1_Set_Current = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.Seed_Set_Current = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.Com_Status = new System.Windows.Forms.PictureBox();
            this.Refresh_List = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Com_Status)).BeginInit();
            this.SuspendLayout();
            // 
            // Power_On
            // 
            this.Power_On.Location = new System.Drawing.Point(30, 109);
            this.Power_On.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Power_On.Name = "Power_On";
            this.Power_On.Size = new System.Drawing.Size(165, 58);
            this.Power_On.TabIndex = 0;
            this.Power_On.Text = "一键开机";
            this.Power_On.UseVisualStyleBackColor = true;
            this.Power_On.Click += new System.EventHandler(this.Power_On_Click);
            // 
            // Power_OFF
            // 
            this.Power_OFF.Location = new System.Drawing.Point(30, 602);
            this.Power_OFF.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Power_OFF.Name = "Power_OFF";
            this.Power_OFF.Size = new System.Drawing.Size(165, 58);
            this.Power_OFF.TabIndex = 1;
            this.Power_OFF.Text = "一键关机";
            this.Power_OFF.UseVisualStyleBackColor = true;
            this.Power_OFF.Click += new System.EventHandler(this.Power_OFF_Click);
            // 
            // Refresh_Status
            // 
            this.Refresh_Status.Location = new System.Drawing.Point(45, 54);
            this.Refresh_Status.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Refresh_Status.Name = "Refresh_Status";
            this.Refresh_Status.Size = new System.Drawing.Size(183, 58);
            this.Refresh_Status.TabIndex = 2;
            this.Refresh_Status.Text = "状态更新";
            this.Refresh_Status.UseVisualStyleBackColor = true;
            this.Refresh_Status.Click += new System.EventHandler(this.Refresh_Status_Click);
            // 
            // Reset_Laser
            // 
            this.Reset_Laser.Location = new System.Drawing.Point(45, 132);
            this.Reset_Laser.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Reset_Laser.Name = "Reset_Laser";
            this.Reset_Laser.Size = new System.Drawing.Size(183, 58);
            this.Reset_Laser.TabIndex = 3;
            this.Reset_Laser.Text = "复    位";
            this.Reset_Laser.UseVisualStyleBackColor = true;
            this.Reset_Laser.Click += new System.EventHandler(this.Reset_Laser_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "手动操作";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "实时电流信息";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "Seed 电流(A):";
            // 
            // Seed1_Current
            // 
            this.Seed1_Current.Location = new System.Drawing.Point(148, 50);
            this.Seed1_Current.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Seed1_Current.Name = "Seed1_Current";
            this.Seed1_Current.Size = new System.Drawing.Size(100, 28);
            this.Seed1_Current.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 18);
            this.label4.TabIndex = 8;
            this.label4.Text = "Amp1 电流(A):";
            // 
            // Amp2_Current
            // 
            this.Amp2_Current.Location = new System.Drawing.Point(148, 170);
            this.Amp2_Current.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Amp2_Current.Name = "Amp2_Current";
            this.Amp2_Current.Size = new System.Drawing.Size(100, 28);
            this.Amp2_Current.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 18);
            this.label5.TabIndex = 10;
            this.label5.Text = "Amp2 电流(A):";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Amp2_Current);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.Amp1_Current);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.Seed1_Current);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(539, 152);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(273, 217);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // Amp1_Current
            // 
            this.Amp1_Current.Location = new System.Drawing.Point(148, 110);
            this.Amp1_Current.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Amp1_Current.Name = "Amp1_Current";
            this.Amp1_Current.Size = new System.Drawing.Size(100, 28);
            this.Amp1_Current.TabIndex = 9;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.Reset_Laser);
            this.groupBox2.Controls.Add(this.Refresh_Status);
            this.groupBox2.Location = new System.Drawing.Point(243, 151);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(273, 217);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Menu;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(30, 187);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(165, 395);
            this.richTextBox1.TabIndex = 14;
            this.richTextBox1.Text = "";
            // 
            // Com_List
            // 
            this.Com_List.FormattingEnabled = true;
            this.Com_List.Location = new System.Drawing.Point(413, 26);
            this.Com_List.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Com_List.Name = "Com_List";
            this.Com_List.Size = new System.Drawing.Size(121, 26);
            this.Com_List.TabIndex = 15;
            this.Com_List.SelectedIndexChanged += new System.EventHandler(this.Com_List_SelectedIndexChanged);
            // 
            // Re_connect
            // 
            this.Re_connect.Location = new System.Drawing.Point(413, 66);
            this.Re_connect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Re_connect.Name = "Re_connect";
            this.Re_connect.Size = new System.Drawing.Size(122, 35);
            this.Re_connect.TabIndex = 5;
            this.Re_connect.Text = "打开串口";
            this.Re_connect.UseVisualStyleBackColor = true;
            this.Re_connect.Click += new System.EventHandler(this.Re_connect_Click);
            // 
            // Laser_Frequence_Set_Value
            // 
            this.Laser_Frequence_Set_Value.Location = new System.Drawing.Point(228, 96);
            this.Laser_Frequence_Set_Value.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Laser_Frequence_Set_Value.Name = "Laser_Frequence_Set_Value";
            this.Laser_Frequence_Set_Value.Size = new System.Drawing.Size(100, 28);
            this.Laser_Frequence_Set_Value.TabIndex = 15;
            this.Laser_Frequence_Set_Value.TextChanged += new System.EventHandler(this.Laser_Frequence_Set_Value_TextChanged);
            this.Laser_Frequence_Set_Value.MouseEnter += new System.EventHandler(this.Laser_Frequence_Set_Value_MouseEnter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(143, 18);
            this.label7.TabIndex = 14;
            this.label7.Text = "激光频率 (KHz):";
            // 
            // Laser_Watt_Set_Value
            // 
            this.Laser_Watt_Set_Value.Location = new System.Drawing.Point(228, 41);
            this.Laser_Watt_Set_Value.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Laser_Watt_Set_Value.Name = "Laser_Watt_Set_Value";
            this.Laser_Watt_Set_Value.Size = new System.Drawing.Size(100, 28);
            this.Laser_Watt_Set_Value.TabIndex = 13;
            this.Laser_Watt_Set_Value.TextChanged += new System.EventHandler(this.Laser_Watt_Set_Value_TextChanged);
            this.Laser_Watt_Set_Value.MouseEnter += new System.EventHandler(this.Laser_Watt_Set_Value_MouseEnter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 18);
            this.label8.TabIndex = 12;
            this.label8.Text = "激光功率 (%):";
            // 
            // Watt_Confirm
            // 
            this.Watt_Confirm.Location = new System.Drawing.Point(364, 38);
            this.Watt_Confirm.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Watt_Confirm.Name = "Watt_Confirm";
            this.Watt_Confirm.Size = new System.Drawing.Size(122, 35);
            this.Watt_Confirm.TabIndex = 18;
            this.Watt_Confirm.Text = "功率写入";
            this.Watt_Confirm.UseVisualStyleBackColor = true;
            this.Watt_Confirm.Click += new System.EventHandler(this.Watt_Confirm_Click);
            // 
            // Frequence_Confirm
            // 
            this.Frequence_Confirm.Location = new System.Drawing.Point(364, 92);
            this.Frequence_Confirm.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Frequence_Confirm.Name = "Frequence_Confirm";
            this.Frequence_Confirm.Size = new System.Drawing.Size(122, 35);
            this.Frequence_Confirm.TabIndex = 19;
            this.Frequence_Confirm.Text = "频率写入";
            this.Frequence_Confirm.UseVisualStyleBackColor = true;
            this.Frequence_Confirm.Click += new System.EventHandler(this.Frequence_Confirm_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Amp2_Set_Current);
            this.groupBox3.Controls.Add(this.Frequence_Confirm);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.Watt_Confirm);
            this.groupBox3.Controls.Add(this.Amp1_Set_Current);
            this.groupBox3.Controls.Add(this.Laser_Frequence_Set_Value);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.Seed_Set_Current);
            this.groupBox3.Controls.Add(this.Laser_Watt_Set_Value);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(243, 395);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Size = new System.Drawing.Size(526, 305);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "激光参数";
            // 
            // Amp2_Set_Current
            // 
            this.Amp2_Set_Current.Location = new System.Drawing.Point(228, 260);
            this.Amp2_Set_Current.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Amp2_Set_Current.Name = "Amp2_Set_Current";
            this.Amp2_Set_Current.Size = new System.Drawing.Size(100, 28);
            this.Amp2_Set_Current.TabIndex = 17;
            this.Amp2_Set_Current.TextChanged += new System.EventHandler(this.Amp2_Set_Current_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 266);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(188, 18);
            this.label9.TabIndex = 16;
            this.label9.Text = "Amp2 电流 设置值(A):";
            // 
            // Amp1_Set_Current
            // 
            this.Amp1_Set_Current.Location = new System.Drawing.Point(228, 206);
            this.Amp1_Set_Current.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Amp1_Set_Current.Name = "Amp1_Set_Current";
            this.Amp1_Set_Current.Size = new System.Drawing.Size(100, 28);
            this.Amp1_Set_Current.TabIndex = 15;
            this.Amp1_Set_Current.TextChanged += new System.EventHandler(this.Amp1_Set_Current_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(24, 211);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(188, 18);
            this.label10.TabIndex = 14;
            this.label10.Text = "Amp1 电流 设置值(A):";
            // 
            // Seed_Set_Current
            // 
            this.Seed_Set_Current.Location = new System.Drawing.Point(228, 151);
            this.Seed_Set_Current.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Seed_Set_Current.Name = "Seed_Set_Current";
            this.Seed_Set_Current.Size = new System.Drawing.Size(100, 28);
            this.Seed_Set_Current.TabIndex = 13;
            this.Seed_Set_Current.TextChanged += new System.EventHandler(this.Seed_Set_Current_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(24, 156);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(188, 18);
            this.label11.TabIndex = 12;
            this.label11.Text = "Seed 电流 设置值(A):";
            // 
            // Com_Status
            // 
            this.Com_Status.Location = new System.Drawing.Point(614, 17);
            this.Com_Status.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Com_Status.Name = "Com_Status";
            this.Com_Status.Size = new System.Drawing.Size(32, 32);
            this.Com_Status.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.Com_Status.TabIndex = 17;
            this.Com_Status.TabStop = false;
            // 
            // Refresh_List
            // 
            this.Refresh_List.Location = new System.Drawing.Point(261, 22);
            this.Refresh_List.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Refresh_List.Name = "Refresh_List";
            this.Refresh_List.Size = new System.Drawing.Size(122, 35);
            this.Refresh_List.TabIndex = 30;
            this.Refresh_List.Text = "更新列表";
            this.Refresh_List.UseVisualStyleBackColor = true;
            this.Refresh_List.Click += new System.EventHandler(this.Refresh_List_Click);
            // 
            // Laser_Control_Panel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 719);
            this.Controls.Add(this.Refresh_List);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.Com_Status);
            this.Controls.Add(this.Re_connect);
            this.Controls.Add(this.Com_List);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Power_OFF);
            this.Controls.Add(this.Power_On);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Laser_Control_Panel";
            this.Text = "Laser_Control_Panel";
            this.Load += new System.EventHandler(this.Laser_Control_Panel_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Com_Status)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Power_On;
        private System.Windows.Forms.Button Power_OFF;
        private System.Windows.Forms.Button Refresh_Status;
        private System.Windows.Forms.Button Reset_Laser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Seed1_Current;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Amp2_Current;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ComboBox Com_List;
        private System.Windows.Forms.Button Re_connect;
        private System.Windows.Forms.PictureBox Com_Status;
        private System.Windows.Forms.TextBox Amp1_Current;
        private System.Windows.Forms.TextBox Laser_Frequence_Set_Value;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox Laser_Watt_Set_Value;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button Watt_Confirm;
        private System.Windows.Forms.Button Frequence_Confirm;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox Amp2_Set_Current;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox Amp1_Set_Current;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox Seed_Set_Current;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button Refresh_List;
    }
}
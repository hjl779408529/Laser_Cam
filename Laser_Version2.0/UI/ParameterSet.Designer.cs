namespace Laser_Version2._0
{
    partial class ParameterSet
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
            this.Correct_Cam_Cor = new System.Windows.Forms.Button();
            this.Disconnect_Tcp = new System.Windows.Forms.Button();
            this.Re_Connect = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button4 = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox18 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox19 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Mark_Group = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Mark_Type_List = new System.Windows.Forms.ComboBox();
            this.Re_Cali_Mark = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.Set_txt_markY4 = new System.Windows.Forms.TextBox();
            this.Set_txt_markX4 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.Set_Btn_Mark1 = new System.Windows.Forms.Button();
            this.Set_txt_markX1 = new System.Windows.Forms.TextBox();
            this.Set_txt_markY3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Set_txt_markX3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Set_txt_markY2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Set_txt_markX2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Set_txt_markY1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Set_txt_valueK = new System.Windows.Forms.TextBox();
            this.Correct_Rtc_Cor = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Acquisation_Rtc_Correct = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.Mark_Group.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // Correct_Cam_Cor
            // 
            this.Correct_Cam_Cor.Location = new System.Drawing.Point(238, 38);
            this.Correct_Cam_Cor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Correct_Cam_Cor.Name = "Correct_Cam_Cor";
            this.Correct_Cam_Cor.Size = new System.Drawing.Size(205, 52);
            this.Correct_Cam_Cor.TabIndex = 101;
            this.Correct_Cam_Cor.Text = "相机坐标系标定";
            this.Correct_Cam_Cor.UseVisualStyleBackColor = true;
            this.Correct_Cam_Cor.Click += new System.EventHandler(this.Correct_Cam_Cor_Click);
            // 
            // Disconnect_Tcp
            // 
            this.Disconnect_Tcp.Location = new System.Drawing.Point(153, 45);
            this.Disconnect_Tcp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Disconnect_Tcp.Name = "Disconnect_Tcp";
            this.Disconnect_Tcp.Size = new System.Drawing.Size(109, 52);
            this.Disconnect_Tcp.TabIndex = 100;
            this.Disconnect_Tcp.Text = "断开相机";
            this.Disconnect_Tcp.UseVisualStyleBackColor = true;
            this.Disconnect_Tcp.Click += new System.EventHandler(this.Disconnect_Tcp_Click);
            // 
            // Re_Connect
            // 
            this.Re_Connect.Location = new System.Drawing.Point(20, 45);
            this.Re_Connect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Re_Connect.Name = "Re_Connect";
            this.Re_Connect.Size = new System.Drawing.Size(109, 52);
            this.Re_Connect.TabIndex = 99;
            this.Re_Connect.Text = "重连相机";
            this.Re_Connect.UseVisualStyleBackColor = true;
            this.Re_Connect.Click += new System.EventHandler(this.Re_Connect_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(503, 45);
            this.button5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(109, 52);
            this.button5.TabIndex = 8;
            this.button5.Text = "触发拍照";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDown1.Location = new System.Drawing.Point(425, 54);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(48, 35);
            this.numericUpDown1.TabIndex = 98;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(238, 182);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(205, 52);
            this.button4.TabIndex = 8;
            this.button4.Text = "振镜与ORG的距离矫正";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(12, 178);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(206, 18);
            this.label20.TabIndex = 97;
            this.label20.Text = "振镜与ORG 中心差值Y/mm";
            // 
            // textBox18
            // 
            this.textBox18.Location = new System.Drawing.Point(40, 208);
            this.textBox18.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new System.Drawing.Size(150, 28);
            this.textBox18.TabIndex = 96;
            this.textBox18.TextChanged += new System.EventHandler(this.textBox18_TextChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(12, 101);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(206, 18);
            this.label21.TabIndex = 95;
            this.label21.Text = "振镜与ORG 中心差值X/mm";
            // 
            // textBox19
            // 
            this.textBox19.Location = new System.Drawing.Point(40, 132);
            this.textBox19.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new System.Drawing.Size(150, 28);
            this.textBox19.TabIndex = 94;
            this.textBox19.TextChanged += new System.EventHandler(this.textBox19_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(39, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(152, 18);
            this.label7.TabIndex = 5;
            this.label7.Text = "像素K值 mm/pixel";
            // 
            // Mark_Group
            // 
            this.Mark_Group.Controls.Add(this.label1);
            this.Mark_Group.Controls.Add(this.Mark_Type_List);
            this.Mark_Group.Controls.Add(this.Re_Cali_Mark);
            this.Mark_Group.Controls.Add(this.button6);
            this.Mark_Group.Controls.Add(this.Set_txt_markY4);
            this.Mark_Group.Controls.Add(this.Set_txt_markX4);
            this.Mark_Group.Controls.Add(this.label8);
            this.Mark_Group.Controls.Add(this.button3);
            this.Mark_Group.Controls.Add(this.button2);
            this.Mark_Group.Controls.Add(this.button1);
            this.Mark_Group.Controls.Add(this.Set_Btn_Mark1);
            this.Mark_Group.Controls.Add(this.Set_txt_markX1);
            this.Mark_Group.Controls.Add(this.Set_txt_markY3);
            this.Mark_Group.Controls.Add(this.label2);
            this.Mark_Group.Controls.Add(this.Set_txt_markX3);
            this.Mark_Group.Controls.Add(this.label5);
            this.Mark_Group.Controls.Add(this.Set_txt_markY2);
            this.Mark_Group.Controls.Add(this.label6);
            this.Mark_Group.Controls.Add(this.Set_txt_markX2);
            this.Mark_Group.Controls.Add(this.label3);
            this.Mark_Group.Controls.Add(this.Set_txt_markY1);
            this.Mark_Group.Controls.Add(this.label4);
            this.Mark_Group.Location = new System.Drawing.Point(36, 157);
            this.Mark_Group.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Mark_Group.Name = "Mark_Group";
            this.Mark_Group.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Mark_Group.Size = new System.Drawing.Size(650, 325);
            this.Mark_Group.TabIndex = 4;
            this.Mark_Group.TabStop = false;
            this.Mark_Group.Text = "Mark参数";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(52, 252);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 105;
            this.label1.Text = "Mark类型";
            // 
            // Mark_Type_List
            // 
            this.Mark_Type_List.FormattingEnabled = true;
            this.Mark_Type_List.Items.AddRange(new object[] {
            "黑色圆白背景",
            "白色圆黑背景",
            "黑色矩形白背景",
            "黑色十字白背景"});
            this.Mark_Type_List.Location = new System.Drawing.Point(15, 284);
            this.Mark_Type_List.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Mark_Type_List.Name = "Mark_Type_List";
            this.Mark_Type_List.Size = new System.Drawing.Size(162, 26);
            this.Mark_Type_List.TabIndex = 20;
            this.Mark_Type_List.SelectedIndexChanged += new System.EventHandler(this.Mark_Type_List_SelectedIndexChanged);
            // 
            // Re_Cali_Mark
            // 
            this.Re_Cali_Mark.Location = new System.Drawing.Point(433, 258);
            this.Re_Cali_Mark.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Re_Cali_Mark.Name = "Re_Cali_Mark";
            this.Re_Cali_Mark.Size = new System.Drawing.Size(195, 52);
            this.Re_Cali_Mark.TabIndex = 12;
            this.Re_Cali_Mark.Text = "二次矫正Mark坐标";
            this.Re_Cali_Mark.UseVisualStyleBackColor = true;
            this.Re_Cali_Mark.Click += new System.EventHandler(this.Re_Cali_Mark_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(545, 205);
            this.button6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(91, 28);
            this.button6.TabIndex = 11;
            this.button6.Text = "定位";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // Set_txt_markY4
            // 
            this.Set_txt_markY4.Location = new System.Drawing.Point(367, 205);
            this.Set_txt_markY4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markY4.Name = "Set_txt_markY4";
            this.Set_txt_markY4.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markY4.TabIndex = 9;
            this.Set_txt_markY4.TextChanged += new System.EventHandler(this.Set_txt_markY4_TextChanged);
            // 
            // Set_txt_markX4
            // 
            this.Set_txt_markX4.Location = new System.Drawing.Point(169, 205);
            this.Set_txt_markX4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markX4.Name = "Set_txt_markX4";
            this.Set_txt_markX4.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markX4.TabIndex = 10;
            this.Set_txt_markX4.TextChanged += new System.EventHandler(this.Set_txt_markX4_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 211);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(152, 18);
            this.label8.TabIndex = 8;
            this.label8.Text = "Mark 点4（右下）";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(544, 156);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(91, 28);
            this.button3.TabIndex = 7;
            this.button3.Text = "定位";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(544, 109);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(91, 28);
            this.button2.TabIndex = 6;
            this.button2.Text = "定位";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(544, 61);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 28);
            this.button1.TabIndex = 5;
            this.button1.Text = "定位";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Set_Btn_Mark1
            // 
            this.Set_Btn_Mark1.Location = new System.Drawing.Point(202, 258);
            this.Set_Btn_Mark1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_Btn_Mark1.Name = "Set_Btn_Mark1";
            this.Set_Btn_Mark1.Size = new System.Drawing.Size(195, 52);
            this.Set_Btn_Mark1.TabIndex = 4;
            this.Set_Btn_Mark1.Text = "矫正Mark坐标";
            this.Set_Btn_Mark1.UseVisualStyleBackColor = true;
            this.Set_Btn_Mark1.Click += new System.EventHandler(this.Set_Btn_Mark1_Click);
            // 
            // Set_txt_markX1
            // 
            this.Set_txt_markX1.Location = new System.Drawing.Point(168, 61);
            this.Set_txt_markX1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markX1.Name = "Set_txt_markX1";
            this.Set_txt_markX1.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markX1.TabIndex = 3;
            this.Set_txt_markX1.TextChanged += new System.EventHandler(this.Set_txt_markX1_TextChanged);
            // 
            // Set_txt_markY3
            // 
            this.Set_txt_markY3.Location = new System.Drawing.Point(366, 156);
            this.Set_txt_markY3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markY3.Name = "Set_txt_markY3";
            this.Set_txt_markY3.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markY3.TabIndex = 3;
            this.Set_txt_markY3.TextChanged += new System.EventHandler(this.Set_txt_markY3_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "Mark 点1（左下）";
            // 
            // Set_txt_markX3
            // 
            this.Set_txt_markX3.Location = new System.Drawing.Point(168, 156);
            this.Set_txt_markX3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markX3.Name = "Set_txt_markX3";
            this.Set_txt_markX3.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markX3.TabIndex = 3;
            this.Set_txt_markX3.TextChanged += new System.EventHandler(this.Set_txt_markX3_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(231, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 18);
            this.label5.TabIndex = 2;
            this.label5.Text = "X ";
            // 
            // Set_txt_markY2
            // 
            this.Set_txt_markY2.Location = new System.Drawing.Point(366, 109);
            this.Set_txt_markY2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markY2.Name = "Set_txt_markY2";
            this.Set_txt_markY2.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markY2.TabIndex = 3;
            this.Set_txt_markY2.TextChanged += new System.EventHandler(this.Set_txt_markY2_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(433, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 18);
            this.label6.TabIndex = 2;
            this.label6.Text = "Y";
            // 
            // Set_txt_markX2
            // 
            this.Set_txt_markX2.Location = new System.Drawing.Point(168, 109);
            this.Set_txt_markX2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markX2.Name = "Set_txt_markX2";
            this.Set_txt_markX2.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markX2.TabIndex = 3;
            this.Set_txt_markX2.TextChanged += new System.EventHandler(this.Set_txt_markX2_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "Mark 点2（左上）";
            // 
            // Set_txt_markY1
            // 
            this.Set_txt_markY1.Location = new System.Drawing.Point(366, 61);
            this.Set_txt_markY1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markY1.Name = "Set_txt_markY1";
            this.Set_txt_markY1.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markY1.TabIndex = 3;
            this.Set_txt_markY1.TextChanged += new System.EventHandler(this.Set_txt_markY1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 162);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 18);
            this.label4.TabIndex = 2;
            this.label4.Text = "Mark 点3（右上）";
            // 
            // Set_txt_valueK
            // 
            this.Set_txt_valueK.Location = new System.Drawing.Point(40, 58);
            this.Set_txt_valueK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_valueK.Name = "Set_txt_valueK";
            this.Set_txt_valueK.Size = new System.Drawing.Size(150, 28);
            this.Set_txt_valueK.TabIndex = 1;
            this.Set_txt_valueK.TextChanged += new System.EventHandler(this.Set_txt_valueK_TextChanged);
            // 
            // Correct_Rtc_Cor
            // 
            this.Correct_Rtc_Cor.Location = new System.Drawing.Point(238, 110);
            this.Correct_Rtc_Cor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Correct_Rtc_Cor.Name = "Correct_Rtc_Cor";
            this.Correct_Rtc_Cor.Size = new System.Drawing.Size(205, 52);
            this.Correct_Rtc_Cor.TabIndex = 102;
            this.Correct_Rtc_Cor.Text = "矫正振镜坐标系偏转角";
            this.Correct_Rtc_Cor.UseVisualStyleBackColor = true;
            this.Correct_Rtc_Cor.Click += new System.EventHandler(this.Correct_Rtc_Cor_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Acquisation_Rtc_Correct);
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Controls.Add(this.Correct_Rtc_Cor);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.Correct_Cam_Cor);
            this.groupBox3.Controls.Add(this.textBox18);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.Set_txt_valueK);
            this.groupBox3.Controls.Add(this.textBox19);
            this.groupBox3.Location = new System.Drawing.Point(36, 506);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(650, 252);
            this.groupBox3.TabIndex = 103;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "偏差矫正";
            // 
            // Acquisation_Rtc_Correct
            // 
            this.Acquisation_Rtc_Correct.Location = new System.Drawing.Point(457, 38);
            this.Acquisation_Rtc_Correct.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Acquisation_Rtc_Correct.Name = "Acquisation_Rtc_Correct";
            this.Acquisation_Rtc_Correct.Size = new System.Drawing.Size(178, 52);
            this.Acquisation_Rtc_Correct.TabIndex = 103;
            this.Acquisation_Rtc_Correct.Text = "采集振镜校准数据";
            this.Acquisation_Rtc_Correct.UseVisualStyleBackColor = true;
            this.Acquisation_Rtc_Correct.Click += new System.EventHandler(this.Acquisation_Rtc_Correct_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(300, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(118, 24);
            this.label9.TabIndex = 104;
            this.label9.Text = "触发代码:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.numericUpDown1);
            this.groupBox4.Controls.Add(this.Disconnect_Tcp);
            this.groupBox4.Controls.Add(this.button5);
            this.groupBox4.Controls.Add(this.Re_Connect);
            this.groupBox4.Location = new System.Drawing.Point(36, 19);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(650, 121);
            this.groupBox4.TabIndex = 105;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "相机操作";
            // 
            // ParameterSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 772);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.Mark_Group);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ParameterSet";
            this.Text = "ParameterSet";
            this.Load += new System.EventHandler(this.ParameterSet_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.Mark_Group.ResumeLayout(false);
            this.Mark_Group.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox Set_txt_valueK;
        private System.Windows.Forms.GroupBox Mark_Group;
        private System.Windows.Forms.TextBox Set_txt_markY3;
        private System.Windows.Forms.TextBox Set_txt_markX3;
        private System.Windows.Forms.TextBox Set_txt_markY2;
        private System.Windows.Forms.TextBox Set_txt_markX2;
        private System.Windows.Forms.TextBox Set_txt_markY1;
        private System.Windows.Forms.TextBox Set_txt_markX1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Set_Btn_Mark1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox textBox18;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textBox19;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button Re_Connect;
        private System.Windows.Forms.Button Disconnect_Tcp;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TextBox Set_txt_markY4;
        private System.Windows.Forms.TextBox Set_txt_markX4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button Re_Cali_Mark;
        private System.Windows.Forms.Button Correct_Cam_Cor;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button Correct_Rtc_Cor;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button Acquisation_Rtc_Correct;
        private System.Windows.Forms.ComboBox Mark_Type_List;
        private System.Windows.Forms.Label label1;
    }
}
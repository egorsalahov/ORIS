namespace GameAndDot.GUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            listboxPlayers = new ListBox();
            textBox1 = new TextBox();
            button1 = new Button();
            label2 = new Label();
            usernameLbl = new Label();
            label4 = new Label();
            colorLbl = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(332, 179);
            label1.Name = "label1";
            label1.Size = new Size(127, 32);
            label1.TabIndex = 0;
            label1.Text = "Ваше имя:";
            label1.Click += label1_Click;
            // 
            // listboxPlayers
            // 
            listboxPlayers.Font = new Font("Segoe UI", 12F);
            listboxPlayers.FormattingEnabled = true;
            listboxPlayers.Location = new Point(1207, 239);
            listboxPlayers.Margin = new Padding(5);
            listboxPlayers.Name = "listboxPlayers";
            listboxPlayers.Size = new Size(370, 364);
            listboxPlayers.TabIndex = 1;
            listboxPlayers.Visible = false;
            listboxPlayers.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(465, 179);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(109, 39);
            textBox1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(618, 179);
            button1.Name = "button1";
            button1.Size = new Size(150, 46);
            button1.TabIndex = 7;
            button1.Text = "войти";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(1130, 33);
            label2.Name = "label2";
            label2.Size = new Size(179, 45);
            label2.TabIndex = 8;
            label2.Text = "Username: ";
            label2.Visible = false;
            label2.Click += label2_Click;
            // 
            // usernameLbl
            // 
            usernameLbl.AutoSize = true;
            usernameLbl.Font = new Font("Segoe UI", 12F);
            usernameLbl.Location = new Point(1351, 33);
            usernameLbl.Name = "usernameLbl";
            usernameLbl.Size = new Size(105, 45);
            usernameLbl.TabIndex = 9;
            usernameLbl.Text = "label3";
            usernameLbl.Visible = false;
            usernameLbl.Click += usernameLbl_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(1130, 105);
            label4.Name = "label4";
            label4.Size = new Size(113, 45);
            label4.TabIndex = 10;
            label4.Text = "Color: ";
            label4.Visible = false;
            label4.Click += label4_Click_1;
            // 
            // colorLbl
            // 
            colorLbl.AutoSize = true;
            colorLbl.Font = new Font("Segoe UI", 12F);
            colorLbl.Location = new Point(1351, 105);
            colorLbl.Name = "colorLbl";
            colorLbl.Size = new Size(81, 45);
            colorLbl.TabIndex = 11;
            colorLbl.Text = "#......";
            colorLbl.Visible = false;
            colorLbl.Click += label5_Click_1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1911, 822);
            Controls.Add(colorLbl);
            Controls.Add(label4);
            Controls.Add(usernameLbl);
            Controls.Add(label2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(listboxPlayers);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            MouseDown += Form1_MouseDown_1;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ListBox listboxPlayers;
        private TextBox textBox1;
        private Button button1;
        private Label label2;
        private Label usernameLbl;
        private Label label4;
        private Label colorLbl;
    }
}

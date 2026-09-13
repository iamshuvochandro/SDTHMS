
namespace SDTHMS
{
    partial class Dashboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dashboard));
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.picProfile = new System.Windows.Forms.PictureBox();
            this.btnToday = new System.Windows.Forms.Button();
            this.btnHabits = new System.Windows.Forms.Button();
            this.btnGoals = new System.Windows.Forms.Button();
            this.btnHealth = new System.Windows.Forms.Button();
            this.lblSummary1 = new System.Windows.Forms.Panel();
            this.lblSummary2 = new System.Windows.Forms.Panel();
            this.lblSummary3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvTodaySchedule = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lstSuggestions = new System.Windows.Forms.CheckedListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picProfile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTodaySchedule)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.SpringGreen;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLogout.Font = new System.Drawing.Font("Gill Sans MT", 10.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.Location = new System.Drawing.Point(800, 451);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(2);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(104, 32);
            this.btnLogout.TabIndex = 4;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Algerian", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.Location = new System.Drawing.Point(11, 9);
            this.lblWelcome.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(217, 30);
            this.lblWelcome.TabIndex = 3;
            this.lblWelcome.Text = "Welcome! User";
            // 
            // picProfile
            // 
            this.picProfile.Location = new System.Drawing.Point(16, 42);
            this.picProfile.Name = "picProfile";
            this.picProfile.Size = new System.Drawing.Size(142, 128);
            this.picProfile.TabIndex = 5;
            this.picProfile.TabStop = false;
            // 
            // btnToday
            // 
            this.btnToday.BackColor = System.Drawing.Color.SpringGreen;
            this.btnToday.Location = new System.Drawing.Point(54, 234);
            this.btnToday.Name = "btnToday";
            this.btnToday.Size = new System.Drawing.Size(104, 36);
            this.btnToday.TabIndex = 6;
            this.btnToday.Text = "Today";
            this.btnToday.UseVisualStyleBackColor = false;
            this.btnToday.Click += new System.EventHandler(this.btnToday_Click);
            // 
            // btnHabits
            // 
            this.btnHabits.BackColor = System.Drawing.Color.SpringGreen;
            this.btnHabits.Location = new System.Drawing.Point(54, 272);
            this.btnHabits.Name = "btnHabits";
            this.btnHabits.Size = new System.Drawing.Size(104, 36);
            this.btnHabits.TabIndex = 7;
            this.btnHabits.Text = "Habits";
            this.btnHabits.UseVisualStyleBackColor = false;
            this.btnHabits.Click += new System.EventHandler(this.btnHabits_Click);
            // 
            // btnGoals
            // 
            this.btnGoals.BackColor = System.Drawing.Color.SpringGreen;
            this.btnGoals.Location = new System.Drawing.Point(54, 310);
            this.btnGoals.Name = "btnGoals";
            this.btnGoals.Size = new System.Drawing.Size(104, 36);
            this.btnGoals.TabIndex = 8;
            this.btnGoals.Text = "Goals";
            this.btnGoals.UseVisualStyleBackColor = false;
            this.btnGoals.Click += new System.EventHandler(this.btnGoals_Click);
            // 
            // btnHealth
            // 
            this.btnHealth.BackColor = System.Drawing.Color.SpringGreen;
            this.btnHealth.Location = new System.Drawing.Point(54, 348);
            this.btnHealth.Name = "btnHealth";
            this.btnHealth.Size = new System.Drawing.Size(104, 36);
            this.btnHealth.TabIndex = 9;
            this.btnHealth.Text = "Health";
            this.btnHealth.UseVisualStyleBackColor = false;
            this.btnHealth.Click += new System.EventHandler(this.btnHealth_Click);
            // 
            // lblSummary1
            // 
            this.lblSummary1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
            this.lblSummary1.Location = new System.Drawing.Point(212, 85);
            this.lblSummary1.Name = "lblSummary1";
            this.lblSummary1.Size = new System.Drawing.Size(200, 100);
            this.lblSummary1.TabIndex = 10;
            this.lblSummary1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // lblSummary2
            // 
            this.lblSummary2.Location = new System.Drawing.Point(446, 85);
            this.lblSummary2.Name = "lblSummary2";
            this.lblSummary2.Size = new System.Drawing.Size(200, 100);
            this.lblSummary2.TabIndex = 11;
            this.lblSummary2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // lblSummary3
            // 
            this.lblSummary3.Location = new System.Drawing.Point(679, 85);
            this.lblSummary3.Name = "lblSummary3";
            this.lblSummary3.Size = new System.Drawing.Size(200, 100);
            this.lblSummary3.TabIndex = 11;
            this.lblSummary3.Paint += new System.Windows.Forms.PaintEventHandler(this.panel3_Paint);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 17);
            this.label2.TabIndex = 12;
            this.label2.Text = "Nav Menu";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(209, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "Productivity Summary";
            // 
            // dgvTodaySchedule
            // 
            this.dgvTodaySchedule.AllowUserToAddRows = false;
            this.dgvTodaySchedule.AllowUserToOrderColumns = true;
            this.dgvTodaySchedule.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvTodaySchedule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTodaySchedule.Location = new System.Drawing.Point(212, 228);
            this.dgvTodaySchedule.Name = "dgvTodaySchedule";
            this.dgvTodaySchedule.RowHeadersWidth = 51;
            this.dgvTodaySchedule.RowTemplate.Height = 24;
            this.dgvTodaySchedule.Size = new System.Drawing.Size(434, 150);
            this.dgvTodaySchedule.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(209, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 17);
            this.label4.TabIndex = 15;
            this.label4.Text = "Schedule & Alarms";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(676, 224);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(171, 17);
            this.label5.TabIndex = 17;
            this.label5.Text = "Improvement Suggestions";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(212, 390);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(105, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Add Task";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnAddTask_Click);
            // 
            // lstSuggestions
            // 
            this.lstSuggestions.FormattingEnabled = true;
            this.lstSuggestions.Location = new System.Drawing.Point(679, 244);
            this.lstSuggestions.Name = "lstSuggestions";
            this.lstSuggestions.Size = new System.Drawing.Size(200, 140);
            this.lstSuggestions.TabIndex = 19;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(545, 390);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 23);
            this.button2.TabIndex = 20;
            this.button2.Text = "Add Health";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnAddHealth_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(434, 390);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(105, 23);
            this.button3.TabIndex = 21;
            this.button3.Text = "Add Goal";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnAddGoal_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(323, 390);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(105, 23);
            this.button4.TabIndex = 22;
            this.button4.Text = "Add Habit";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btnAddHabit_Click);
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PeachPuff;
            this.ClientSize = new System.Drawing.Size(915, 494);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lstSuggestions);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dgvTodaySchedule);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblSummary2);
            this.Controls.Add(this.lblSummary3);
            this.Controls.Add(this.lblSummary1);
            this.Controls.Add(this.btnHealth);
            this.Controls.Add(this.btnGoals);
            this.Controls.Add(this.btnHabits);
            this.Controls.Add(this.btnToday);
            this.Controls.Add(this.picProfile);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.lblWelcome);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Dashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dashboard";
            this.Load += new System.EventHandler(this.Dashboard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picProfile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTodaySchedule)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.PictureBox picProfile;
        private System.Windows.Forms.Button btnToday;
        private System.Windows.Forms.Button btnHabits;
        private System.Windows.Forms.Button btnGoals;
        private System.Windows.Forms.Button btnHealth;
        private System.Windows.Forms.Panel lblSummary1;
        private System.Windows.Forms.Panel lblSummary2;
        private System.Windows.Forms.Panel lblSummary3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvTodaySchedule;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckedListBox lstSuggestions;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}
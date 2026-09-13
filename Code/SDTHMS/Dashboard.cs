using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SDTHMS
{
    public partial class Dashboard : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SDTHMS"].ConnectionString;
        private int currentUserId;

        private string completedTasksText = "0 / 0";
        private string activeHabitsText = "0 Streaks";
        private string productivityScoreText = "0%";

        public Dashboard(int userId)
        {
            InitializeComponent();
            this.currentUserId = userId;
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            dgvTodaySchedule.CellContentClick += dgvTodaySchedule_CellContentClick;

            LoadUserProfile();
            LoadProductivitySummary();
            LoadTodayTasks();
            LoadManagerSuggestions();
        }

        private void LoadUserProfile()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT Username, UserImage FROM dbo.Users WHERE Id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : "USER";
                                lblWelcome.Text = "WELCOME! " + username.ToUpper();

                                if (reader["UserImage"] != DBNull.Value)
                                {
                                    byte[] imgData = (byte[])reader["UserImage"];
                                    using (MemoryStream ms = new MemoryStream(imgData))
                                    {
                                        if (picProfile.Image != null)
                                        {
                                            picProfile.Image.Dispose();
                                        }
                                        picProfile.Image = Image.FromStream(ms);
                                        picProfile.SizeMode = PictureBoxSizeMode.StretchImage;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading user profile: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductivitySummary()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    double taskScore = 0;
                    double habitScore = 0;
                    double goalScore = 0;
                    double healthScore = 0;

                    int taskCompleted = 0;
                    int taskTotal = 0;

                    string taskQuery = @"SELECT 
                                            ISNULL(SUM(CASE WHEN IsCompleted = 1 THEN 1 ELSE 0 END), 0) AS Completed,
                                            COUNT(*) AS Total 
                                         FROM dbo.Tasks 
                                         WHERE UserId = @UserId 
                                           AND CAST(ScheduleTime AS DATE) = CAST(GETDATE() AS DATE)";

                    using (SqlCommand cmd = new SqlCommand(taskQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                taskCompleted = Convert.ToInt32(reader["Completed"]);
                                taskTotal = Convert.ToInt32(reader["Total"]);
                                if (taskTotal > 0)
                                {
                                    taskScore = ((double)taskCompleted / taskTotal) * 100;
                                }
                            }
                        }
                    }
                    completedTasksText = $"{taskCompleted} / {taskTotal}";
                    string habitQuery = @"SELECT 
                                            COUNT(*) AS TotalHabits,
                                            ISNULL(SUM(CASE WHEN CAST(LastCompletedDate AS DATE) = CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END), 0) AS HabitsDoneToday,
                                            ISNULL(MAX(StreakCount), 0) AS MaxStreak
                                          FROM dbo.Habits 
                                          WHERE UserId = @UserId";

                    int maxStreak = 0;
                    using (SqlCommand cmd = new SqlCommand(habitQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int totalHabits = Convert.ToInt32(reader["TotalHabits"]);
                                int habitsDoneToday = Convert.ToInt32(reader["HabitsDoneToday"]);
                                maxStreak = Convert.ToInt32(reader["MaxStreak"]);

                                if (totalHabits > 0)
                                {
                                    habitScore = ((double)habitsDoneToday / totalHabits) * 100;
                                }
                            }
                        }
                    }
                    activeHabitsText = $"{maxStreak} Streaks";
                    string goalQuery = @"SELECT 
                                            COUNT(*) AS TotalGoals,
                                            ISNULL(SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END), 0) AS CompletedGoals
                                         FROM dbo.Goals 
                                         WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(goalQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int totalGoals = Convert.ToInt32(reader["TotalGoals"]);
                                int completedGoals = Convert.ToInt32(reader["CompletedGoals"]);

                                if (totalGoals > 0)
                                {
                                    goalScore = ((double)completedGoals / totalGoals) * 100;
                                }
                            }
                        }
                    }
                    string healthQuery = @"SELECT TOP 1 SleepHours 
                                           FROM dbo.HealthMetrics 
                                           WHERE UserId = @UserId 
                                             AND CAST(MetricDate AS DATE) = CAST(GETDATE() AS DATE)
                                           ORDER BY MetricDate DESC";

                    using (SqlCommand cmd = new SqlCommand(healthQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            int sleepHours = Convert.ToInt32(result);
                            healthScore = Math.Min(100, (sleepHours / 8.0) * 100);
                        }
                    }
                    double overallScore = (taskScore * 0.25) + (habitScore * 0.25) + (goalScore * 0.25) + (healthScore * 0.25);
                    productivityScoreText = $"{Math.Round(overallScore)}%";

                    this.Invalidate(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading summary stats: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel p)
            {
                DrawPanelMetric(e.Graphics, p, "Completed Tasks", completedTasksText);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel p)
            {
                DrawPanelMetric(e.Graphics, p, "Active Habits", activeHabitsText);
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel p)
            {
                DrawPanelMetric(e.Graphics, p, "Productivity Score", productivityScoreText);
            }
        }

        private void DrawPanelMetric(Graphics g, Panel p, string title, string value)
        {
            g.Clear(p.BackColor);
            using (Font titleFont = new Font("Segoe UI", 9f, FontStyle.Regular))
            using (Font valueFont = new Font("Segoe UI", 15f, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(title, titleFont, brush, new RectangleF(0, 5, p.Width, 20), sf);
                g.DrawString(value, valueFont, brush, new RectangleF(0, 25, p.Width, 35), sf);
            }
        }

        private void LoadManagerSuggestions()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT SuggestionText, CreatedAt 
                                     FROM dbo.ManagerSuggestions 
                                     WHERE UserId = @UserId 
                                     ORDER BY CreatedAt DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);
                        con.Open();

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        lstSuggestions.Items.Clear();

                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                string dateStr = Convert.ToDateTime(row["CreatedAt"]).ToString("yyyy-MM-dd");
                                string suggestion = row["SuggestionText"].ToString();
                                lstSuggestions.Items.Add($"[{dateStr}] {suggestion}");
                            }
                        }
                        else
                        {
                            lstSuggestions.Items.Add("No suggestions from manager yet.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suggestions: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            LoadTodayTasks();
        }

        private void btnHabits_Click(object sender, EventArgs e)
        {
            LoadUserHabits();
        }

        private void btnGoals_Click(object sender, EventArgs e)
        {
            LoadUserGoals();
        }

        private void btnHealth_Click(object sender, EventArgs e)
        {
            LoadUserHealth();
        }

        private void btnAddTask_Click(object sender, EventArgs e)
        {
            using (TasksForm addTaskForm = new TasksForm(currentUserId))
            {
                if (addTaskForm.ShowDialog() == DialogResult.OK)
                {
                    LoadTodayTasks();
                    LoadProductivitySummary();
                }
            }
        }

        private void btnAddHabit_Click(object sender, EventArgs e)
        {
            using (HabitsForm addHabitForm = new HabitsForm(currentUserId))
            {
                if (addHabitForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUserHabits();
                    LoadProductivitySummary();
                }
            }
        }

        private void btnAddGoal_Click(object sender, EventArgs e)
        {
            using (GoalsForm addGoalForm = new GoalsForm(currentUserId))
            {
                if (addGoalForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUserGoals();
                    LoadProductivitySummary();
                }
            }
        }

        private void btnAddHealth_Click(object sender, EventArgs e)
        {
            using (HealthForm addHealthForm = new HealthForm(currentUserId))
            {
                if (addHealthForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUserHealth();
                    LoadProductivitySummary();
                }
            }
        }

        private void LoadTodayTasks()
        {
            RemoveActionColumns();
            string query = @"SELECT TaskId, TaskName AS [Task Name], Category, ScheduleTime AS [Time], 
                                    HasAlarm AS [Alarm Enabled], IsCompleted AS [Completed] 
                             FROM dbo.Tasks 
                             WHERE UserId = @UserId 
                               AND CAST(ScheduleTime AS DATE) = CAST(GETDATE() AS DATE)";
            PopulateGrid(query);

            if (dgvTodaySchedule.Columns.Contains("TaskId"))
            {
                dgvTodaySchedule.Columns["TaskId"].Visible = false;
            }
        }

        private void LoadUserHabits()
        {
            RemoveActionColumns();
            string query = @"SELECT HabitId, HabitName AS [Habit Name], Frequency, StreakCount AS [Streak] 
                             FROM dbo.Habits 
                             WHERE UserId = @UserId";
            PopulateGrid(query);

            if (dgvTodaySchedule.Columns.Contains("HabitId"))
            {
                dgvTodaySchedule.Columns["HabitId"].Visible = false;
            }

            if (!dgvTodaySchedule.Columns.Contains("btnStreak"))
            {
                DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn();
                btnCol.Name = "btnStreak";
                btnCol.HeaderText = "Action";
                btnCol.Text = "+ Streak";
                btnCol.UseColumnTextForButtonValue = true;
                dgvTodaySchedule.Columns.Add(btnCol);
            }
        }

        private void LoadUserGoals()
        {
            RemoveActionColumns();
            string query = @"SELECT GoalId, GoalTitle AS [Goal Title], TargetDate AS [Target Date], Status 
                             FROM dbo.Goals 
                             WHERE UserId = @UserId";
            PopulateGrid(query);

            if (dgvTodaySchedule.Columns.Contains("GoalId"))
            {
                dgvTodaySchedule.Columns["GoalId"].Visible = false;
            }
        }

        private void LoadUserHealth()
        {
            RemoveActionColumns();
            string query = @"SELECT MetricId, MetricDate AS [Date], WaterIntake AS [Water (L)], SleepHours AS [Sleep (Hrs)] 
                             FROM dbo.HealthMetrics 
                             WHERE UserId = @UserId";
            PopulateGrid(query);

            if (dgvTodaySchedule.Columns.Contains("MetricId"))
            {
                dgvTodaySchedule.Columns["MetricId"].Visible = false;
            }
        }

        private void RemoveActionColumns()
        {
            if (dgvTodaySchedule.Columns.Contains("btnStreak"))
            {
                dgvTodaySchedule.Columns.Remove("btnStreak");
            }
        }

        private void PopulateGrid(string query)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            dgvTodaySchedule.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                            dgvTodaySchedule.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading grid data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvTodaySchedule_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvTodaySchedule.Rows[e.RowIndex];

            if (dgvTodaySchedule.Columns.Contains("TaskId") && dgvTodaySchedule.Columns.Contains("Completed") && e.ColumnIndex == dgvTodaySchedule.Columns["Completed"].Index)
            {
                int taskId = Convert.ToInt32(row.Cells["TaskId"].Value);
                bool currentStatus = Convert.ToBoolean(row.Cells["Completed"].Value);
                bool newStatus = !currentStatus;

                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE dbo.Tasks SET IsCompleted = @IsCompleted WHERE TaskId = @TaskId AND UserId = @UserId";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@IsCompleted", newStatus);
                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    LoadTodayTasks();
                    LoadProductivitySummary();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating task status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (dgvTodaySchedule.Columns.Contains("HabitId") && dgvTodaySchedule.Columns.Contains("btnStreak") && e.ColumnIndex == dgvTodaySchedule.Columns["btnStreak"].Index)
            {
                int habitId = Convert.ToInt32(row.Cells["HabitId"].Value);

                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE dbo.Habits SET StreakCount = StreakCount + 1, LastCompletedDate = GETDATE() WHERE HabitId = @HabitId AND UserId = @UserId";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@HabitId", habitId);
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    LoadUserHabits();
                    LoadProductivitySummary();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating habit streak: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (dgvTodaySchedule.Columns.Contains("GoalId") && dgvTodaySchedule.Columns.Contains("Status") && e.ColumnIndex == dgvTodaySchedule.Columns["Status"].Index)
            {
                int goalId = Convert.ToInt32(row.Cells["GoalId"].Value);
                string currentStatus = row.Cells["Status"].Value != null ? row.Cells["Status"].Value.ToString() : "";
                string newStatus = currentStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) ? "In Progress" : "Completed";

                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE dbo.Goals SET Status = @Status WHERE GoalId = @GoalId AND UserId = @UserId";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@GoalId", goalId);
                        cmd.Parameters.AddWithValue("@UserId", currentUserId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    LoadUserGoals();
                    LoadProductivitySummary();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating goal status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            this.Hide();
            login.FormClosed += (s, args) => this.Close();
            login.Show();
        }
    }
}
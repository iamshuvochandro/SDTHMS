using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SDTHMS.Models;

namespace SDTHMS
{
    public partial class Manager_Dashboard : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SDTHMS"].ConnectionString;
        private string loggedInUsername;
        private int currentManagerId = 0;

        private string totalUsersText = "0 Users";
        private string totalHabitsText = "0 Habits";
        private string systemCompletionText = "0%";

        public Manager_Dashboard()
        {
            InitializeComponent();
        }

        public Manager_Dashboard(string username)
        {
            InitializeComponent();
            this.loggedInUsername = username;
        }

        private void Manager_Dashboard_Load(object sender, EventArgs e)
        {
            LoadUserProfile();

            if (currentManagerId <= 0)
            {
                MessageBox.Show("Manager profile could not be verified.", "Access Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadSystemOverview();
            LoadUserData();
        }

        private void LoadUserProfile()
        {
            if (string.IsNullOrWhiteSpace(loggedInUsername))
                return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT Id, Username, UserImage FROM dbo.Users WHERE Username = @Username AND LTRIM(RTRIM(Role)) = 'Manager'";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", loggedInUsername);
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentManagerId = Convert.ToInt32(reader["Id"]);

                                string username = reader["Username"] != DBNull.Value ? reader["Username"].ToString() : "MANAGER";

                                lblWelcome.Text = "WELCOME! " + username.ToUpper();

                                if (reader["UserImage"] != DBNull.Value)
                                {
                                    byte[] imgData = (byte[])reader["UserImage"];

                                    using (MemoryStream ms = new MemoryStream(imgData))
                                    {
                                        using (Image tempImg = Image.FromStream(ms))
                                        {
                                            Image oldImg = picProfile.Image;
                                            picProfile.Image = new Bitmap(tempImg);
                                            oldImg?.Dispose();
                                        }
                                    }

                                    picProfile.SizeMode = PictureBoxSizeMode.StretchImage;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSystemOverview()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string usersQuery = "SELECT COUNT(*) FROM dbo.Users WHERE LTRIM(RTRIM(Role)) = 'User'";

                    using (SqlCommand cmdUsers = new SqlCommand(usersQuery, con))
                    {
                        int users = Convert.ToInt32(cmdUsers.ExecuteScalar());
                        totalUsersText = $"{users} Active Users";
                    }

                    string habitsQuery = "SELECT COUNT(*) FROM dbo.Habits h INNER JOIN dbo.Users u ON h.UserId = u.Id WHERE LTRIM(RTRIM(u.Role)) = 'User'";

                    using (SqlCommand cmdHabits = new SqlCommand(habitsQuery, con))
                    {
                        int habits = Convert.ToInt32(cmdHabits.ExecuteScalar());
                        totalHabitsText = $"{habits} Habits Tracked";
                    }

                    string completionQuery = "SELECT ISNULL(SUM(CASE WHEN t.IsCompleted = 1 THEN 1 ELSE 0 END), 0) AS Completed, COUNT(*) AS Total FROM dbo.Tasks t INNER JOIN dbo.Users u ON t.UserId = u.Id WHERE LTRIM(RTRIM(u.Role)) = 'User'";

                    using (SqlCommand cmd = new SqlCommand(completionQuery, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int completed = Convert.ToInt32(reader["Completed"]);
                            int total = Convert.ToInt32(reader["Total"]);

                            int rate = total > 0 ? (int)Math.Round((double)completed / total * 100) : 0;

                            systemCompletionText = $"{rate}% System Rate";
                        }
                    }

                    this.Invalidate(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading system overview: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void managerPanel1_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel p)
            {
                DrawPanelMetric(e.Graphics, p, "Total Users", totalUsersText);
            }
        }

        private void managerPanel2_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel p)
            {
                DrawPanelMetric(e.Graphics, p, "Active Habits", totalHabitsText);
            }
        }

        private void managerPanel3_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel p)
            {
                DrawPanelMetric(e.Graphics, p, "Completion Rate", systemCompletionText);
            }
        }

        private void DrawPanelMetric(Graphics g, Panel p, string title, string value)
        {
            g.Clear(p.BackColor);

            using (Font titleFont = new Font("Segoe UI", 9f, FontStyle.Regular))
            using (Font valueFont = new Font("Segoe UI", 15f, FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Black))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(title, titleFont, brush, new RectangleF(0, 5, p.Width, 20), sf);
                g.DrawString(value, valueFont, brush, new RectangleF(0, 25, p.Width, 35), sf);
            }
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            LoadUserData();
        }

        private void btnHabits_Click(object sender, EventArgs e)
        {
            LoadAllHabits();
        }

        private void btnStats_Click(object sender, EventArgs e)
        {
            LoadStats();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            string query = "SELECT Id, Username, Role, Mobile FROM dbo.Users WHERE LTRIM(RTRIM(Role)) = 'User' AND (Username LIKE @Search OR Role LIKE @Search)";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Search", "%" + searchText + "%");

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Search Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns.Contains("Id"))
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                if (row.Cells["Id"].Value != null && int.TryParse(row.Cells["Id"].Value.ToString(), out int selectedUserId))
                {
                    txtTargetUserId.Text = selectedUserId.ToString();
                }
            }
        }

        private void btnSendSuggestion_Click(object sender, EventArgs e)
        {
            if (currentManagerId <= 0)
            {
                MessageBox.Show("Manager account could not be verified.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtTargetUserId.Text.Trim(), out int targetUserId))
            {
                MessageBox.Show("Please select a user from the grid or enter a valid numeric User ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (targetUserId <= 0)
            {
                MessageBox.Show("Please select a valid user.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ManagerSuggestion suggestion = new ManagerSuggestion(targetUserId, currentManagerId, txtSuggestionInput.Text.Trim());

            if (!suggestion.IsValid())
            {
                MessageBox.Show("Please enter a valid suggestion before sending.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string managerCheckQuery = "SELECT COUNT(*) FROM dbo.Users WHERE Id = @ManagerId AND LTRIM(RTRIM(Role)) = 'Manager'";

                    using (SqlCommand managerCheckCmd = new SqlCommand(managerCheckQuery, con))
                    {
                        managerCheckCmd.Parameters.Add("@ManagerId", SqlDbType.Int).Value = currentManagerId;

                        int managerExists = Convert.ToInt32(managerCheckCmd.ExecuteScalar());

                        if (managerExists == 0)
                        {
                            MessageBox.Show("The current account is not a valid Manager account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string userCheckQuery = "SELECT COUNT(*) FROM dbo.Users WHERE Id = @UserId AND LTRIM(RTRIM(Role)) = 'User'";

                    using (SqlCommand userCheckCmd = new SqlCommand(userCheckQuery, con))
                    {
                        userCheckCmd.Parameters.Add("@UserId", SqlDbType.Int).Value = targetUserId;

                        int userExists = Convert.ToInt32(userCheckCmd.ExecuteScalar());

                        if (userExists == 0)
                        {
                            MessageBox.Show("The selected User ID does not belong to a valid User account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string query = "INSERT INTO dbo.ManagerSuggestions (UserId, ManagerId, SuggestionText, CreatedAt) VALUES (@UserId, @ManagerId, @SuggestionText, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserId", suggestion.UserId);
                        cmd.Parameters.AddWithValue("@ManagerId", suggestion.ManagerId);
                        cmd.Parameters.AddWithValue("@SuggestionText", suggestion.SuggestionText);
                        cmd.Parameters.AddWithValue("@CreatedAt", suggestion.CreatedAt);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show($"Suggestion successfully sent to User ID {suggestion.UserId}!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        txtSuggestionInput.Clear();
                        txtTargetUserId.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending suggestion: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserData()
        {
            ExecuteQuery("SELECT Id, Username, Role, Mobile FROM dbo.Users WHERE LTRIM(RTRIM(Role)) = 'User'");
        }

        private void LoadAllHabits()
        {
            ExecuteQuery("SELECT h.HabitId, h.UserId, h.HabitName, h.Frequency, h.StreakCount FROM dbo.Habits h INNER JOIN dbo.Users u ON h.UserId = u.Id WHERE LTRIM(RTRIM(u.Role)) = 'User'");
        }

        private void LoadStats()
        {
            ExecuteQuery("SELECT t.UserId, COUNT(t.TaskId) AS TotalTasks FROM dbo.Tasks t INNER JOIN dbo.Users u ON t.UserId = u.Id WHERE LTRIM(RTRIM(u.Role)) = 'User' GROUP BY t.UserId");
        }

        private void ExecuteQuery(string query)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, con))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Load Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
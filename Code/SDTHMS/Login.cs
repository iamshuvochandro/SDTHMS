using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using SDTHMS.Models;

namespace SDTHMS
{
    public partial class Login : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SDTHMS"].ConnectionString;

        public Login()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Registration register = new Registration();
            this.Hide();
            register.FormClosed += (s, args) => this.Close();
            register.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string adminUsername = ConfigurationManager.AppSettings["AdminUsername"];
            string adminPassword = ConfigurationManager.AppSettings["AdminPassword"];

            if (!string.IsNullOrEmpty(adminUsername) && string.Equals(username, adminUsername, StringComparison.OrdinalIgnoreCase) && string.Equals(password, adminPassword, StringComparison.Ordinal))
            {
                MessageBox.Show("Admin login successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Admin_Dashboard adminDashboard = new Admin_Dashboard(username);
                this.Hide();
                adminDashboard.FormClosed += (s, args) => this.Close();
                adminDashboard.Show();

                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT Id, Username, Password, Mobile, Address, Role, UserImage, CreatedAtUtc FROM Users WHERE Username = @Username";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            User loggedInUser = new User(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.IsDBNull(3) ? null : reader.GetString(3),
                                reader.IsDBNull(4) ? null : reader.GetString(4),
                                reader.GetString(5),
                                reader.IsDBNull(6) ? null : (byte[])reader[6],
                                reader.GetDateTime(7)
                            );

                            bool isValidPassword = false;

                            try
                            {
                                isValidPassword = BCrypt.Net.BCrypt.Verify(password, loggedInUser.Password);
                            }
                            catch
                            {
                                isValidPassword = password == loggedInUser.Password;
                            }

                            if (!isValidPassword)
                            {
                                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            string role = loggedInUser.Role == null ? "" : loggedInUser.Role.Trim();

                            if (string.Equals(role, "Manager", StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show("Manager login successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                Manager_Dashboard managerDashboard = new Manager_Dashboard(loggedInUser.Username);
                                this.Hide();
                                managerDashboard.FormClosed += (s, args) => this.Close();
                                managerDashboard.Show();

                                return;
                            }

                            if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show("Login successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                Dashboard dashboard = new Dashboard(loggedInUser.Id);
                                this.Hide();
                                dashboard.FormClosed += (s, args) => this.Close();
                                dashboard.Show();

                                return;
                            }

                            MessageBox.Show("This account has an invalid role.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login failed.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !textBox2.UseSystemPasswordChar;

            if (textBox2.UseSystemPasswordChar)
            {
                button3.Text = "👁";
            }
            else
            {
                button3.Text = "🙈";
            }
        }
    }
}
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BCrypt.Net;

namespace SDTHMS
{
    public partial class Admin_Dashboard : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["SDTHMS"].ConnectionString;
        byte[] userImage;
        private string loggedInAdmin;

        public Admin_Dashboard()
        {
            InitializeComponent();
            textBox4.MaxLength = 11;
            LoadUsers();
        }

        public Admin_Dashboard(string username) : this()
        {
            loggedInAdmin = username;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            Hide();
            login.ShowDialog();
            Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void LoadUsers()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT Id, Username, Mobile, Address, Role, CreatedAtUtc, UserImage 
                                     FROM Users 
                                     ORDER BY Id DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dt.Columns.Add("Image", typeof(Image));

                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["UserImage"] != DBNull.Value)
                        {
                            row["Image"] = ByteToImage((byte[])row["UserImage"]);
                        }
                    }

                    dataGridView1.DataSource = dt;

                    if (dataGridView1.Columns.Contains("Password"))
                    {
                        dataGridView1.Columns["Password"].Visible = false;
                    }

                    if (dataGridView1.Columns.Contains("UserImage"))
                    {
                        dataGridView1.Columns["UserImage"].Visible = false;
                    }

                    dataGridView1.RowTemplate.Height = 60;

                    DataGridViewImageColumn imageColumn = dataGridView1.Columns["Image"] as DataGridViewImageColumn;
                    if (imageColumn != null)
                    {
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load failed.\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Image ByteToImage(byte[] bytes)
        {
            if (bytes == null) return null;

            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                using (Image image = Image.FromStream(ms))
                {
                    return new Bitmap(image);
                }
            }
            catch
            {
                return null;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            textBox1.Text = row.Cells["Id"].Value.ToString();
            textBox2.Text = row.Cells["Username"].Value.ToString();
            textBox4.Text = row.Cells["Mobile"].Value.ToString();
            textBox5.Text = row.Cells["Address"].Value.ToString();

            txtPassword.Clear();

            if (Controls.ContainsKey("comboBoxRole") && row.Cells["Role"].Value != DBNull.Value)
            {
                ComboBox cbRole = Controls["comboBoxRole"] as ComboBox;
                if (cbRole != null)
                {
                    cbRole.SelectedItem = row.Cells["Role"].Value.ToString();
                }
            }

            if (row.Cells["UserImage"].Value != DBNull.Value)
            {
                userImage = (byte[])row.Cells["UserImage"].Value;
                pictureBox1.Image = ByteToImage(userImage);
            }
            else
            {
                userImage = null;
                pictureBox1.Image = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (open.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    userImage = File.ReadAllBytes(open.FileName);
                    pictureBox1.Image = ByteToImage(userImage);
                }
                catch
                {
                    MessageBox.Show("Could not load image.", "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string username = textBox2.Text.Trim();
            string password = txtPassword.Text.Trim();
            string mobile = textBox4.Text.Trim();
            string address = textBox5.Text.Trim();
            string role = "User";

            if (Controls.ContainsKey("comboBoxRole"))
            {
                ComboBox cbRole = Controls["comboBoxRole"] as ComboBox;
                if (cbRole != null && cbRole.SelectedItem != null)
                {
                    role = cbRole.SelectedItem.ToString().Trim();
                }
            }

            if (!CheckInputs(username, mobile, address)) return;

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Password is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (UserExists(username, mobile))
            {
                MessageBox.Show("Username or mobile number already exists.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Users (Username, Password, Mobile, Address, Role, UserImage)
                                     VALUES (@Username, @Password, @Mobile, @Address, @Role, @UserImage)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = username;

                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = hashedPassword;

                        cmd.Parameters.Add("@Mobile", SqlDbType.NVarChar, 20).Value = mobile;
                        cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 255).Value = address;
                        cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 50).Value = role;
                        cmd.Parameters.Add("@UserImage", SqlDbType.VarBinary, -1).Value = (object)userImage ?? DBNull.Value;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("User added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add failed.\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Please select a valid user to update.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = textBox2.Text.Trim();
            string mobile = textBox4.Text.Trim();
            string address = textBox5.Text.Trim();
            string role = "User";

            if (Controls.ContainsKey("comboBoxRole"))
            {
                ComboBox cbRole = Controls["comboBoxRole"] as ComboBox;
                if (cbRole != null && cbRole.SelectedItem != null)
                {
                    role = cbRole.SelectedItem.ToString();
                }
            }

            if (!CheckInputs(username, mobile, address)) return;

            if (UserExists(username, mobile, id))
            {
                MessageBox.Show("Another user already has this username or mobile number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Users
                                     SET Username = @Username,
                                         Mobile = @Mobile,
                                         Address = @Address,
                                         Role = @Role,
                                         UserImage = @UserImage
                                     WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = username;
                        cmd.Parameters.Add("@Mobile", SqlDbType.NVarChar, 20).Value = mobile;
                        cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 255).Value = address;
                        cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 50).Value = role;
                        cmd.Parameters.Add("@UserImage", SqlDbType.VarBinary, -1).Value = (object)userImage ?? DBNull.Value;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("User updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed.\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int id) || id <= 0)
            {
                MessageBox.Show("Please select a valid user to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this user?",
                "Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"DELETE FROM Tasks WHERE UserId = @Id;
                                     DELETE FROM Habits WHERE UserId = @Id;
                                     DELETE FROM Goals WHERE UserId = @Id;
                                     DELETE FROM HealthMetrics WHERE UserId = @Id;
                                     DELETE FROM ManagerSuggestions WHERE UserId = @Id OR ManagerId = @Id;
                                     DELETE FROM Users WHERE Id = @Id;";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed.\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckInputs(string username, string mobile, string address)
        {
            username = username.Trim();
            mobile = mobile.Trim();
            address = address.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(mobile))
            {
                MessageBox.Show("Mobile number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return false;
            }

            if (mobile.Length != 11)
            {
                MessageBox.Show("Mobile number should be exactly 11 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return false;
            }

            foreach (char c in mobile)
            {
                if (!char.IsDigit(c))
                {
                    MessageBox.Show("Mobile number must contain digits only.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox4.Focus();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Address is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox5.Focus();
                return false;
            }

            return true;
        }

        private bool UserExists(string username, string mobile, int excludeUserId = 0)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT COUNT(*) 
                                     FROM Users 
                                     WHERE (Username = @Username OR Mobile = @Mobile) 
                                       AND Id <> @ExcludeUserId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = username.Trim();
                        cmd.Parameters.Add("@Mobile", SqlDbType.NVarChar, 20).Value = mobile.Trim();
                        cmd.Parameters.Add("@ExcludeUserId", SqlDbType.Int).Value = excludeUserId;

                        con.Open();
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not check existing user.\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void ClearInputs()
        {
            textBox1.Clear();
            textBox2.Clear();
            txtPassword.Clear();
            textBox4.Clear();
            textBox5.Clear();

            if (comboBoxRole.Items.Count > 0)
            {
                comboBoxRole.SelectedIndex = 0;
            }

            pictureBox1.Image = null;
            userImage = null;
            textBox2.Focus();
        }

        private void Admin_Dashboard_Load(object sender, EventArgs e)
        {
            if (comboBoxRole.Items.Count > 0)
            {
                comboBoxRole.SelectedIndex = 0;
            }
        }
    }
}
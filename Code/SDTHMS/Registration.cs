using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BCrypt.Net;
using SDTHMS.Models; 

namespace SDTHMS
{
    public partial class Registration : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SDTHMS"].ConnectionString;
        private byte[] userImage;

        public Registration()
        {
            InitializeComponent();
            textBox3.MaxLength = 11;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (open.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var img = Image.FromFile(open.FileName))
                        {
                            pictureBox1.Image = new Bitmap(img);
                        }
                        userImage = File.ReadAllBytes(open.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Could not load image: " + ex.Message, "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string rawPassword = textBox2.Text.Trim();
            string mobile = textBox3.Text.Trim();
            string address = textBox4.Text.Trim();

            if (!ValidateRegistrationInputs(username, rawPassword, mobile, address))
            {
                return;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

            User user = new User(username,hashedPassword, mobile, address);

            user.UserImage = userImage;

            if (!user.IsValid())
            {
                MessageBox.Show("Invalid user information.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username OR Mobile = @Mobile";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = user.Username;
                        checkCmd.Parameters.Add("@Mobile", SqlDbType.NVarChar, 20).Value = user.Mobile;

                        int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (existingCount > 0)
                        {
                            MessageBox.Show("Username or mobile number already exists.", "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string query = "INSERT INTO Users (Username, Password, Mobile, Address, Role, UserImage, CreatedAtUtc) VALUES (@Username, @Password, @Mobile, @Address, @Role, @UserImage, @CreatedAtUtc)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = user.Username;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar, -1).Value = user.Password;
                        cmd.Parameters.Add("@Mobile", SqlDbType.NVarChar, 20).Value = user.Mobile;
                        cmd.Parameters.Add("@Address", SqlDbType.NVarChar, -1).Value = user.Address;
                        cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 20).Value = user.Role;
                        cmd.Parameters.Add("@UserImage", SqlDbType.VarBinary, -1).Value = (object)user.UserImage ?? DBNull.Value;
                        cmd.Parameters.Add("@CreatedAtUtc", SqlDbType.DateTime).Value = user.CreatedAtUtc;

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Registration successful. Please log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Login login = new Login();

                this.Hide();

                login.FormClosed += (s, args) => this.Close();
                login.Show();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Registration failed.\n" + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration failed.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ValidateRegistrationInputs(
             string username,
             string password,
             string mobile,
             string address)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username is required.","Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                textBox1.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Password is required.","Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                textBox2.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(mobile))
            {
                MessageBox.Show( "Mobile number is required.", "Validation Error", MessageBoxButtons.OK,MessageBoxIcon.Warning);

                textBox3.Focus();
                return false;
            }

            if (mobile.Length != 11)
            {
                MessageBox.Show( "Mobile number must be exactly 11 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                textBox3.Focus();
                return false;
            }

            foreach (char c in mobile)
            {
                if (!char.IsDigit(c))
                {
                    MessageBox.Show( "Mobile number must contain digits only.", "Validation Error", MessageBoxButtons.OK,  MessageBoxIcon.Warning);

                    textBox3.Focus();
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show( "Address is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                textBox4.Focus();
                return false;
            }

            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
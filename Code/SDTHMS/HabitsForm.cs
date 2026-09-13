using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SDTHMS.Models;

namespace SDTHMS
{
    public partial class HabitsForm : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["SDTHMS"].ConnectionString;
        private int currentUserId;

        public HabitsForm(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
        }

        private void HabitsForm_Load(object sender, EventArgs e)
        {
            cmbFrequency.Items.Clear();
            cmbFrequency.Items.Add("Daily");
            cmbFrequency.Items.Add("Weekly");
            cmbFrequency.Items.Add("Monthly");
            cmbFrequency.SelectedIndex = 0;

            LoadHabits();
        }

        private void LoadHabits()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT HabitId, HabitName, Frequency, StreakCount, LastCompletedDate 
                                     FROM Habits 
                                     WHERE UserId = @UserId 
                                     ORDER BY HabitId DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvHabits.DataSource = dt;

                    if (dgvHabits.Columns.Contains("HabitId"))
                    {
                        dgvHabits.Columns["HabitId"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load habits.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string selectedFreq = cmbFrequency.SelectedItem != null ? cmbFrequency.SelectedItem.ToString() : "Daily";

            Habit habit = new Habit(
                currentUserId,
                txtHabitName.Text.Trim(),
                selectedFreq
            );

            if (!habit.IsValid())
            {
                MessageBox.Show("Please enter a valid habit name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHabitName.Focus();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Habits (UserId, HabitName, Frequency, StreakCount, LastCompletedDate)
                                     VALUES (@UserId, @HabitName, @Frequency, @StreakCount, @LastCompletedDate)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserId", habit.UserId);
                    cmd.Parameters.AddWithValue("@HabitName", habit.HabitName);
                    cmd.Parameters.AddWithValue("@Frequency", habit.Frequency);
                    cmd.Parameters.AddWithValue("@StreakCount", habit.StreakCount);
                    cmd.Parameters.AddWithValue("@LastCompletedDate", (object)habit.LastCompletedDate ?? DBNull.Value);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Habit saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                LoadHabits();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save habit.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvHabits.CurrentRow == null || dgvHabits.CurrentRow.Index < 0)
            {
                MessageBox.Show("Please select a habit from the list to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int habitId = Convert.ToInt32(dgvHabits.CurrentRow.Cells["HabitId"].Value);

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this habit?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Habits WHERE HabitId = @HabitId AND UserId = @UserId";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@HabitId", habitId);
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Habit deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadHabits();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete habit.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearInputs()
        {
            txtHabitName.Clear();
            if (cmbFrequency.Items.Count > 0) cmbFrequency.SelectedIndex = 0;
            txtHabitName.Focus();
        }
    }
}
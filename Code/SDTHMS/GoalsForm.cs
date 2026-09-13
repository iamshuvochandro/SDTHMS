using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SDTHMS.Models;

namespace SDTHMS
{
    public partial class GoalsForm : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["SDTHMS"].ConnectionString;
        private int currentUserId;

        public GoalsForm(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
        }

        private void GoalsForm_Load(object sender, EventArgs e)
        {
            dtpTargetDate.Format = DateTimePickerFormat.Short;
            dtpTargetDate.Value = DateTime.Now;

            LoadGoals();
        }

        private void LoadGoals()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT GoalId, GoalTitle, TargetDate, Status 
                                     FROM Goals 
                                     WHERE UserId = @UserId 
                                     ORDER BY TargetDate ASC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvGoals.DataSource = dt;

                    if (dgvGoals.Columns.Contains("GoalId"))
                    {
                        dgvGoals.Columns["GoalId"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load goals.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Goal goal = new Goal(
                currentUserId,
                txtGoalTitle.Text.Trim(),
                dtpTargetDate.Value.Date
            );

            if (!goal.IsValid())
            {
                MessageBox.Show("Please enter a valid goal title.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGoalTitle.Focus();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Goals (UserId, GoalTitle, TargetDate, Status)
                                     VALUES (@UserId, @GoalTitle, @TargetDate, @Status)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserId", goal.UserId);
                    cmd.Parameters.AddWithValue("@GoalTitle", goal.GoalTitle);
                    cmd.Parameters.AddWithValue("@TargetDate", goal.TargetDate);
                    cmd.Parameters.AddWithValue("@Status", goal.Status);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Goal saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                LoadGoals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save goal.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvGoals.CurrentRow == null || dgvGoals.CurrentRow.Index < 0)
            {
                MessageBox.Show("Please select a goal from the list to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int goalId = Convert.ToInt32(dgvGoals.CurrentRow.Cells["GoalId"].Value);

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this goal?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Goals WHERE GoalId = @GoalId AND UserId = @UserId";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@GoalId", goalId);
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Goal deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadGoals();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete goal.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearInputs()
        {
            txtGoalTitle.Clear();
            dtpTargetDate.Value = DateTime.Now;
            txtGoalTitle.Focus();
        }
    }
}
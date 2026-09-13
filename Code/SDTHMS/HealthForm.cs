using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using SDTHMS.Models;

namespace SDTHMS
{
    public partial class HealthForm : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["SDTHMS"].ConnectionString;
        private int currentUserId;

        public HealthForm(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
        }

        private void HealthForm_Load(object sender, EventArgs e)
        {
            numSleepHours.Minimum = 1;
            numSleepHours.Maximum = 8;
            numSleepHours.Value = 1;

            numTimeBasis.Minimum = 1;
            numTimeBasis.Maximum = 30;
            numTimeBasis.Value = 1;

            LoadHealthMetrics();
        }

        private void LoadHealthMetrics()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT MetricId, WaterIntake, SleepHours, TimeBasis, MetricDate 
                                     FROM HealthMetrics 
                                     WHERE UserId = @UserId 
                                     ORDER BY MetricDate DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvHealth.DataSource = dt;

                    if (dgvHealth.Columns.Contains("MetricId"))
                    {
                        dgvHealth.Columns["MetricId"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load health metrics.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string waterIntake = txtWaterIntake.Text.Trim();

            if (string.IsNullOrWhiteSpace(waterIntake))
            {
                MessageBox.Show("Please enter water intake.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtWaterIntake.Focus();
                return;
            }

            decimal waterAmount;

            if (!decimal.TryParse(waterIntake, NumberStyles.Number, CultureInfo.CurrentCulture, out waterAmount) || waterAmount < 0)
            {
                MessageBox.Show("Water intake must be a valid non-negative number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtWaterIntake.Focus();
                return;
            }

            HealthMetric metric = new HealthMetric(currentUserId, waterIntake, Convert.ToInt32(numSleepHours.Value), Convert.ToInt32(numTimeBasis.Value));

            if (!metric.IsValid())
            {
                MessageBox.Show("Please enter valid health metrics.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtWaterIntake.Focus();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO HealthMetrics (UserId, MetricDate, WaterIntake, SleepHours, TimeBasis)
                                     VALUES (@UserId, @MetricDate, @WaterIntake, @SleepHours, @TimeBasis)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserId", metric.UserId);
                    cmd.Parameters.AddWithValue("@MetricDate", metric.MetricDate);
                    cmd.Parameters.AddWithValue("@WaterIntake", metric.WaterIntake);
                    cmd.Parameters.AddWithValue("@SleepHours", metric.SleepHours);
                    cmd.Parameters.AddWithValue("@TimeBasis", metric.TimeBasis);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Health data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                LoadHealthMetrics();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save health record.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvHealth.CurrentRow == null || dgvHealth.CurrentRow.Index < 0)
            {
                MessageBox.Show("Please select a record from the list to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int metricId = Convert.ToInt32(dgvHealth.CurrentRow.Cells["MetricId"].Value);

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this record?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM HealthMetrics WHERE MetricId = @MetricId AND UserId = @UserId";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MetricId", metricId);
                    cmd.Parameters.AddWithValue("@UserId", currentUserId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadHealthMetrics();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete record.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearInputs()
        {
            txtWaterIntake.Clear();
            numSleepHours.Value = 1;
            numTimeBasis.Value = 1;
            txtWaterIntake.Focus();
        }
    }
}
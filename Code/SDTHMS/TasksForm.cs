using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SDTHMS.Models;

namespace SDTHMS
{
    public partial class TasksForm : Form
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["SDTHMS"].ConnectionString;
        private readonly int currentUserId;

        public TasksForm(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
        }

        private void TasksForm_Load(object sender, EventArgs e)
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("Work");
            cmbCategory.Items.Add("Personal");
            cmbCategory.Items.Add("Study");
            cmbCategory.Items.Add("General");
            cmbCategory.SelectedIndex = 0;

            dtpScheduleTime.Format = DateTimePickerFormat.Time;
            dtpScheduleTime.ShowUpDown = true;

            LoadTasks();
        }

        private void LoadTasks()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT TaskId, TaskName, Category, ScheduleTime, HasAlarm, IsCompleted, CreatedAt 
                                     FROM Tasks 
                                     WHERE UserId = @UserId 
                                     ORDER BY ScheduleTime ASC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = currentUserId;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dgvTasks.DataSource = dt;
                        }
                    }

                    if (dgvTasks.Columns.Contains("TaskId"))
                    {
                        dgvTasks.Columns["TaskId"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load tasks.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            TaskItem task = new TaskItem(
                currentUserId,
                txtTaskName.Text.Trim(),
                cmbCategory.SelectedItem != null ? cmbCategory.SelectedItem.ToString() : "General",
                dtpScheduleTime.Value,
                chkEnableAlarm.Checked
            );

            if (!task.IsValid())
            {
                MessageBox.Show("Please enter valid task information.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTaskName.Focus();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Tasks (UserId, TaskName, Category, ScheduleTime, HasAlarm, IsCompleted, CreatedAt)
                                     VALUES (@UserId, @TaskName, @Category, @ScheduleTime, @HasAlarm, @IsCompleted, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = task.UserId;
                        cmd.Parameters.Add("@TaskName", SqlDbType.NVarChar, 100).Value = task.TaskName;
                        cmd.Parameters.Add("@Category", SqlDbType.NVarChar, 50).Value = task.Category;
                        cmd.Parameters.Add("@ScheduleTime", SqlDbType.DateTime).Value = task.ScheduleTime;
                        cmd.Parameters.Add("@HasAlarm", SqlDbType.Bit).Value = task.HasAlarm;
                        cmd.Parameters.Add("@IsCompleted", SqlDbType.Bit).Value = task.IsCompleted;
                        cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = task.CreatedAt;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Task saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearInputs();
                LoadTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save task.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTasks.CurrentRow == null || dgvTasks.CurrentRow.Index < 0)
            {
                MessageBox.Show("Please select a task from the list to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int taskId = Convert.ToInt32(dgvTasks.CurrentRow.Cells["TaskId"].Value);

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this task?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes) return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Tasks WHERE TaskId = @TaskId AND UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add("@TaskId", SqlDbType.Int).Value = taskId;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = currentUserId;

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Task deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete task.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearInputs()
        {
            txtTaskName.Clear();
            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;
            dtpScheduleTime.Value = DateTime.Now;
            chkEnableAlarm.Checked = false;
            txtTaskName.Focus();
        }
    }
}
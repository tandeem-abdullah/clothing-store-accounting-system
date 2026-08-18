using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace clothes_store
{
    public partial class FRM_Users : Form
    {
        SqlConnection cn = new SqlConnection("Server=DESKTOP-2902PO6;DataBase=binmahfoz;Integrated Security=true");
        private int selectedUserID = -1;
        private bool isEditingPassword = false;
        public FRM_Users()
        {
            InitializeComponent();
            LoadUsers();
            LoadRoles();
        }
        private void LoadRoles()
        {
            cmbRole.Items.AddRange(new string[] { "Admin", "Manager", "Cashier", "Employee" });
            cmbRole.SelectedIndex = 0;
        }
        private void LoadUsers()
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SP_Users_Select", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchText", txtSearch.Text.Trim());

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridViewUsers.DataSource = dt;
                    dataGridViewUsers.Columns["UserID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ في تحميل البيانات: " + ex.Message);
            }
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال اسم المستخدم وكلمة المرور");
                return;
            }

            try
            {
                string hashedPassword = HashPassword(txtPassword.Text);

                using (SqlCommand cmd = new SqlCommand("SP_Users_Insert", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    cmd.Parameters.AddWithValue("@Role", cmbRole.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());

                    cn.Open();
                    int newID = Convert.ToInt32(cmd.ExecuteScalar());
                    cn.Close();

                    MessageBox.Show($"✅ تم إضافة الموظف بنجاح - الرقم: {newID}");
                    ClearFields();
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ في الإضافة: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedUserID == -1)
            {
                MessageBox.Show("⚠️ الرجاء اختيار موظف للتعديل");
                return;
            }

            try
            {
                using (SqlCommand cmd = new SqlCommand("SP_Users_Update", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", selectedUserID);
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@Role", cmbRole.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());

                    // إذا كان المستخدم يريد تغيير كلمة السر
                    if (isEditingPassword && !string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        string hashedPassword = HashPassword(txtPassword.Text);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PasswordHash", DBNull.Value);
                    }

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();

                    MessageBox.Show("✅ تم تعديل الموظف بنجاح");
                    ClearFields();
                    LoadUsers();
                    selectedUserID = -1;
                    isEditingPassword = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ في التعديل: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedUserID == -1)
            {
                MessageBox.Show("⚠️ الرجاء اختيار موظف للحذف");
                return;
            }

            if (MessageBox.Show("⚠️ هل أنت متأكد من حذف هذا الموظف؟", "تأكيد الحذف", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Users_Delete", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", selectedUserID);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();

                        MessageBox.Show("✅ تم حذف الموظف بنجاح");
                        ClearFields();
                        LoadUsers();
                        selectedUserID = -1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ خطأ في الحذف: " + ex.Message);
                }
            }
        }

        private void dataGridViewUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewUsers.Rows[e.RowIndex];
                selectedUserID = Convert.ToInt32(row.Cells["UserID"].Value);
                txtUsername.Text = row.Cells["Username"].Value.ToString();
                cmbRole.SelectedItem = row.Cells["Role"].Value.ToString();
                txtFullName.Text = row.Cells["FullName"]?.Value?.ToString() ?? "";
                txtPhone.Text = row.Cells["Phone"]?.Value?.ToString() ?? "";

                // إخفاء كلمة السر عند التعديل
                txtPassword.Text = "";
                isEditingPassword = false;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            selectedUserID = -1;
            isEditingPassword = false;
        }
        private void ClearFields()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtFullName.Text = "";
            txtPhone.Text = "";
            cmbRole.SelectedIndex = 0;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            isEditingPassword = true;
            MessageBox.Show("⚠️ الرجاء إدخال كلمة المرور الجديدة ثم الضغط على زر التعديل");
        }
    }
}





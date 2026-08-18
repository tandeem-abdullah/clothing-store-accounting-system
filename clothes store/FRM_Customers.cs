using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace clothes_store
{
    public partial class FRM_Customers : Form
    {
        SqlConnection cn = new SqlConnection("Server=DESKTOP-2902PO6;DataBase=binmahfoz;Integrated Security=true");
        private int selectedCustomerID = -1;
        public FRM_Customers()
        {
            InitializeComponent();
            LoadCustomers();
        }
        private void LoadCustomers()
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SP_Customers_Select", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchText", txtSearch.Text.Trim());

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridViewCustomers.DataSource = dt;
                    dataGridViewCustomers.Columns["CustomerID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ في تحميل البيانات: " + ex.Message);
            }
        }
        private void ClearFields()
        {
            txtName.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال اسم العميل");
                return;
            }

            try
            {
                using (SqlCommand cmd = new SqlCommand("SP_Customers_Insert", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerName", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());

                    cn.Open();
                    int newID = Convert.ToInt32(cmd.ExecuteScalar());
                    cn.Close();

                    MessageBox.Show($"✅ تم إضافة العميل بنجاح - الرقم: {newID}");
                    ClearFields();
                    LoadCustomers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ في الإضافة: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedCustomerID == -1)
            {
                MessageBox.Show("⚠️ الرجاء اختيار عميل للتعديل");
                return;
            }

            try
            {
                using (SqlCommand cmd = new SqlCommand("SP_Customers_Update", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerID", selectedCustomerID);
                    cmd.Parameters.AddWithValue("@CustomerName", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();

                    MessageBox.Show("✅ تم تعديل العميل بنجاح");
                    ClearFields();
                    LoadCustomers();
                    selectedCustomerID = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ في التعديل: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCustomerID == -1)
            {
                MessageBox.Show("⚠️ الرجاء اختيار عميل للحذف");
                return;
            }

            if (MessageBox.Show("⚠️ هل أنت متأكد من حذف هذا العميل؟", "تأكيد الحذف", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SP_Customers_Delete", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CustomerID", selectedCustomerID);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();

                        MessageBox.Show("✅ تم حذف العميل بنجاح");
                        ClearFields();
                        LoadCustomers();
                        selectedCustomerID = -1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ خطأ في الحذف: " + ex.Message);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            selectedCustomerID = -1;
        }

        private void dataGridViewCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewCustomers.Rows[e.RowIndex];
                selectedCustomerID = Convert.ToInt32(row.Cells["CustomerID"].Value);
                txtName.Text = row.Cells["CustomerName"].Value.ToString();
                txtPhone.Text = row.Cells["Phone"]?.Value?.ToString() ?? "";
                txtAddress.Text = row.Cells["Address"]?.Value?.ToString() ?? "";
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadCustomers();
        }
    }

}


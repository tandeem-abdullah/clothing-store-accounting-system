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

namespace clothes_store
{
    public partial class FRM_Manage_Product : Form
    {

        SqlConnection cn = new SqlConnection("Server=DESKTOP-2902PO6;DataBase=binmahfoz;Integrated Security=true");
        public FRM_Manage_Product()
        {
            InitializeComponent();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void الرئيسيةToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dashboard Dash = new Dashboard();
            Dash.Show();
            this.Hide();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("AddProduct", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Barcode", txt_QR.Text);
            cmd.Parameters.AddWithValue("@ProductName", txt_Prod_Name.Text);
            cmd.Parameters.AddWithValue("@CategoryID", Convert.ToInt32(comboCategory.SelectedValue));
            cmd.Parameters.AddWithValue("@Size", txt_Prod_Size.Text);
            cmd.Parameters.AddWithValue("@Color", txt_Prod_Color.Text);
            cmd.Parameters.AddWithValue("@PurchasePrice", Convert.ToDecimal(txt_Price_Pay.Text));
            cmd.Parameters.AddWithValue("@SalePrice", Convert.ToDecimal(txt_Price_Sale.Text));
            cmd.Parameters.AddWithValue("@QuantityInStock", Convert.ToInt32(txt_Qte.Text));

            cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();

            MessageBox.Show(" تم إضافة المنتج بنجاح");
            LoadProducts();

        }

        private void btn_Alter_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("UpdateProduct", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ProductID", Convert.ToInt32(txtProductID.Text));
            cmd.Parameters.AddWithValue("@Barcode", txt_QR.Text);
            cmd.Parameters.AddWithValue("@ProductName", txt_Prod_Name.Text);
            cmd.Parameters.AddWithValue("@CategoryID", Convert.ToInt32(comboCategory.SelectedValue));
            cmd.Parameters.AddWithValue("@Size", txt_Prod_Size.Text);
            cmd.Parameters.AddWithValue("@Color", txt_Prod_Color.Text);
            cmd.Parameters.AddWithValue("@PurchasePrice", Convert.ToDecimal(txt_Price_Pay.Text));
            cmd.Parameters.AddWithValue("@SalePrice", Convert.ToDecimal(txt_Price_Sale.Text));
            cmd.Parameters.AddWithValue("@QuantityInStock", Convert.ToInt32(txt_Qte.Text));

            cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();

            MessageBox.Show(" تم تعديل المنتج بنجاح","تعديل",MessageBoxButtons.OK,MessageBoxIcon.Information);
            LoadProducts();
        }

        private void FRM_Manage_Product_Load(object sender, EventArgs e)
        {
           LoadCategories();
            LoadProducts();
        }
        private void LoadCategories()
        {
           
            
                SqlDataAdapter da = new SqlDataAdapter("SELECT CategoryID, CategoryName FROM Categories",cn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                comboCategory.DataSource = dt;
                comboCategory.DisplayMember = "CategoryName"; 
                comboCategory.ValueMember = "CategoryID";     
            
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("DeleteProduct", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@ProductID", Convert.ToInt32(txtProductID.Text));

            cn.Open();
            cmd.ExecuteNonQuery();
            cn.Close();

            MessageBox.Show("️ تم حذف المنتج بنجاح");
            LoadProducts();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("SearchProducts", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SearchTerm", txt_Search.Text);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dataGridView1.DataSource = dt; 
        }

        private void LoadProducts()
        {
            
            
                SqlCommand cmd = new SqlCommand("GetAllProducts", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;

                
                if (dataGridView1.Columns["CategoryID"] != null)
                {
                    dataGridView1.Columns["CategoryID"].Visible = false;
                }
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtProductID.Text = row.Cells["ProductID"].Value.ToString();
                txt_QR.Text = row.Cells["Barcode"].Value.ToString();
                txt_Prod_Name.Text = row.Cells["ProductName"].Value.ToString();
                txt_Prod_Size.Text = row.Cells["Size"].Value.ToString();
                txt_Prod_Color.Text = row.Cells["Color"].Value.ToString();
                txt_Price_Pay.Text = row.Cells["PurchasePrice"].Value.ToString();
                txt_Price_Sale.Text = row.Cells["SalePrice"].Value.ToString();
                txt_Qte.Text = row.Cells["QuantityInStock"].Value.ToString();

                
                comboCategory.SelectedValue = row.Cells["CategoryID"].Value;
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            txtProductID.Clear();
            txt_QR.Clear();
            txt_Prod_Name.Clear();
            txt_Prod_Size.Clear();
            txt_Prod_Color.Clear();
            txt_Price_Pay.Clear();
            txt_Price_Sale.Clear();
            txt_Qte.Clear();

        }

        private void خروجToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dashboard frm = new Dashboard();
            frm.Show();
            this.Hide();
        }
    }

}

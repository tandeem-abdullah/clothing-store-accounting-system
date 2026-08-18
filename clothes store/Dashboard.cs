using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace clothes_store
{
    public partial class Dashboard : Form
    {
        SqlConnection cn =new  SqlConnection("Server=DESKTOP-2902PO6;DataBase=binmahfoz;Integrated Security=true"); 
        public Dashboard()
        {
            InitializeComponent();
           

        }

        private void Dashboard_Load(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        
        private void ادارةالمنتجاتToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FRM_Manage_Product product = new FRM_Manage_Product();
            product.Show();
            this.Hide();
        }

        private void ادارةالموظفينToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FRM_Users form =new FRM_Users();    
            form.Show();
            this.Hide();
        }

        private void btn_Mang_Prod_Click(object sender, EventArgs e)
        {
            FRM_Manage_Product fRM = new FRM_Manage_Product();
            fRM.Show();
            this.Hide();
        }

        private void خروجToolStripMenuItem_Click(object sender, EventArgs e)
        {
          Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FRM_Pruches fRM = new FRM_Pruches();
            fRM.Show();
            this.Hide();
        }
    }
}

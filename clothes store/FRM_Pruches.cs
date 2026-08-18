using CrystalDecisions.CrystalReports.Engine;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace clothes_store
{
    public partial class FRM_Pruches : Form
    {
        SqlConnection cn = new SqlConnection("Server=DESKTOP-2902PO6;DataBase=binmahfoz;Integrated Security=true");
        private int currentUserID = 1;
        public FRM_Pruches()
        {
            InitializeComponent();
            LoadCustomers();
            LoadProducts();
        }
        private void LoadCustomers()
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SP_GetCustomersForSale", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbCustomers.DisplayMember = "CustomerName";
                    cmbCustomers.ValueMember = "CustomerID";
                    cmbCustomers.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ في تحميل العملاء: " + ex.Message);
            }
        }
        private void LoadProducts()
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SP_SearchProducts", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchText", "");
                    cmd.Parameters.AddWithValue("@SearchByBarcode", 0);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridViewProducts.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ في تحميل المنتجات: " + ex.Message);
            }
        }
        private void CalculateTotal()
        {
            if (!string.IsNullOrEmpty(txt_Qte.Text) && !string.IsNullOrEmpty(txtPrice_Sale.Text))
            {
                try
                {
                    int quantity = Convert.ToInt32(txt_Qte.Text);
                    decimal price = Convert.ToDecimal(txtPrice_Sale.Text);
                    decimal total = quantity * price;

                    if (!string.IsNullOrEmpty(txt_Discount.Text))
                    {
                        decimal discount = Convert.ToDecimal(txt_Discount.Text);
                        total -= discount;
                    }

                    if (!string.IsNullOrEmpty(txt_Tax.Text))
                    {
                        decimal tax = Convert.ToDecimal(txt_Tax.Text);
                        total += tax;
                    }

                    txtTotal.Text = total.ToString("N2");
                }
                catch
                {
                    txtTotal.Text = "0";
                }
            }
        }
        private bool ValidateSaleData()
        {
            if (string.IsNullOrEmpty(txt_Qte.Text) || Convert.ToInt32(txt_Qte.Text) <= 0)
            {
                MessageBox.Show("⚠️ الرجاء إدخال كمية صحيحة");
                return false;
            }

            if (string.IsNullOrEmpty(txtPrice_Sale.Text) || Convert.ToDecimal(txtPrice_Sale.Text) <= 0)
            {
                MessageBox.Show("⚠️ الرجاء إدخال سعر صحيح");
                return false;
            }

            return true;
        }
        private void ClearSaleFields()
        {
            txtProductID.Text = "";
            txtBarcode.Text = "";
            txtProductName.Text = "";
            txtColor.Text = "";
            txtSize.Text = "";
            txt_Qte.Text = "";
            txtPrice_Sale.Text = "";
            txtTotal.Text = "";
            txt_Discount.Text = "0";
            txt_Tax.Text = "0";
        }
        private void GenerateInvoicePDF(int saleId)
        {
            try
            {
                // 1. جلب بيانات الفاتورة من قاعدة البيانات
                DataTable invoiceData = GetInvoiceData(saleId);

                if (invoiceData == null || invoiceData.Rows.Count == 0)
                {
                    MessageBox.Show("⚠️ لا توجد بيانات للفاتورة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. قراءة قالب HTML
                string templatePath = Path.Combine(Application.StartupPath, "Templates", "invoice_template.html");

                if (!File.Exists(templatePath))
                {
                    MessageBox.Show("❌ ملف القالب غير موجود", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string htmlTemplate = File.ReadAllText(templatePath, Encoding.UTF8);

                // 3. تعبئة البيانات الأساسية
                DataRow firstRow = invoiceData.Rows[0];
                htmlTemplate = htmlTemplate.Replace("{{SaleID}}", firstRow["SaleID"].ToString());
                htmlTemplate = htmlTemplate.Replace("{{SaleDate}}", Convert.ToDateTime(firstRow["SaleDate"]).ToString("yyyy/MM/dd"));
                htmlTemplate = htmlTemplate.Replace("{{CustomerName}}", firstRow["CustomerName"].ToString());
                htmlTemplate = htmlTemplate.Replace("{{Phone}}", firstRow["Phone"].ToString());
                htmlTemplate = htmlTemplate.Replace("{{Address}}", firstRow["Address"].ToString());
                htmlTemplate = htmlTemplate.Replace("{{CashierName}}", firstRow["CashierName"].ToString());
                htmlTemplate = htmlTemplate.Replace("{{TotalAmount}}", Convert.ToDecimal(firstRow["TotalAmount"]).ToString("N2"));
                htmlTemplate = htmlTemplate.Replace("{{Discount}}", Convert.ToDecimal(firstRow["Discount"]).ToString("N2"));
                htmlTemplate = htmlTemplate.Replace("{{Tax}}", Convert.ToDecimal(firstRow["Tax"]).ToString("N2"));
                htmlTemplate = htmlTemplate.Replace("{{NetAmount}}", Convert.ToDecimal(firstRow["NetAmount"]).ToString("N2"));
                htmlTemplate = htmlTemplate.Replace("{{PrintDate}}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                // 4. توليد جدول المنتجات
                StringBuilder productsHtml = new StringBuilder();
                int counter = 1;

                foreach (DataRow row in invoiceData.Rows)
                {
                    productsHtml.AppendLine("<tr>");
                    productsHtml.AppendLine($"<td>{counter}</td>");
                    productsHtml.AppendLine($"<td>{row["ProductName"]}</td>");
                    productsHtml.AppendLine($"<td>{row["Barcode"]}</td>");
                    productsHtml.AppendLine($"<td>{row["Quantity"]}</td>");
                    productsHtml.AppendLine($"<td>{Convert.ToDecimal(row["UnitSalePrice"]).ToString("N2")} ر.س</td>");
                    productsHtml.AppendLine($"<td>{Convert.ToDecimal(row["Total"]).ToString("N2")} ر.س</td>");
                    productsHtml.AppendLine("</tr>");
                    counter++;
                }

                htmlTemplate = htmlTemplate.Replace("{{Products}}", productsHtml.ToString());

                // 5. تحويل HTML إلى PDF
                HtmlToPdf converter = new HtmlToPdf();

                // إعدادات PDF
                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                converter.Options.MarginTop = 20;
                converter.Options.MarginBottom = 20;
                converter.Options.MarginLeft = 20;
                converter.Options.MarginRight = 20;

                // التحويل
                PdfDocument pdf = converter.ConvertHtmlString(htmlTemplate);

                // 6. حفظ الملف
                string invoicesFolder = Path.Combine(Application.StartupPath, "Invoices");
                Directory.CreateDirectory(invoicesFolder);

                string fileName = $"Invoice_{saleId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(invoicesFolder, fileName);

                pdf.Save(filePath);
                pdf.Close();

                // 7. فتح الملف تلقائياً
                System.Diagnostics.Process.Start(filePath);

                MessageBox.Show($"✅ تم إنشاء الفاتورة بنجاح\n\nرقم الفاتورة: {saleId}\nالمسار: {filePath}",
                                "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في إنشاء الفاتورة:\n{ex.Message}",
                                "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private DataTable GetInvoiceData(int saleId)
        {
            DataTable dt = new DataTable();

            try
            {
                // استخدام كائن الاتصال الموجود في الفورم
                using (SqlCommand cmd = new SqlCommand("SP_GetInvoiceForCrystal", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SaleID", saleId);

                    // إدارة حالة الاتصال
                    bool shouldClose = false;
                    if (cn.State != ConnectionState.Open)
                    {
                        cn.Open();
                        shouldClose = true;
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);

                    // إغلاق الاتصال إذا كنا قد فتحناه
                    if (shouldClose && cn.State == ConnectionState.Open)
                        cn.Close();
                }
            }
            catch (Exception ex)
            {
                // التأكد من إغلاق الاتصال في حالة الخطأ
                if (cn.State == ConnectionState.Open)
                    cn.Close();

                MessageBox.Show($"❌ خطأ في جلب البيانات:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }
        private void PrintInvoiceDirectly(int saleId)
        {
            try
            {
                DataTable invoiceData = GetInvoiceData(saleId);

                if (invoiceData.Rows.Count == 0)
                    return;

                PrintDialog printDialog = new PrintDialog();
                PrintDocument printDoc = new PrintDocument();

                printDoc.PrintPage += (sender, e) =>
                {
                    Graphics g = e.Graphics;
                    Font titleFont = new Font("Arial", 16, FontStyle.Bold);
                    Font normalFont = new Font("Arial", 12);
                    Font smallFont = new Font("Arial", 10);

                    float y = 50;
                    float margin = 50;

                    // عنوان الفاتورة
                    g.DrawString("فاتورة بيع - متجر بن محفوظ", titleFont, Brushes.Black, margin, y);
                    y += 40;

                    // بيانات الفاتورة
                    DataRow row = invoiceData.Rows[0];
                    g.DrawString($"رقم الفاتورة: {row["SaleID"]}", normalFont, Brushes.Black, margin, y);
                    y += 25;
                    g.DrawString($"التاريخ: {Convert.ToDateTime(row["SaleDate"]):yyyy/MM/dd}", normalFont, Brushes.Black, margin, y);
                    y += 25;
                    g.DrawString($"العميل: {row["CustomerName"]}", normalFont, Brushes.Black, margin, y);
                    y += 30;

                    // تفاصيل المنتجات
                    g.DrawString("المنتجات:", normalFont, Brushes.Black, margin, y);
                    y += 25;

                    foreach (DataRow productRow in invoiceData.Rows)
                    {
                        string productText = $"{productRow["ProductName"]} - {productRow["Quantity"]} x {Convert.ToDecimal(productRow["UnitSalePrice"]):N2}";
                        g.DrawString(productText, smallFont, Brushes.Black, margin + 20, y);
                        y += 20;
                    }

                    y += 20;
                    g.DrawString($"الإجمالي: {Convert.ToDecimal(row["NetAmount"]):N2} ريال", normalFont, Brushes.Black, margin, y);
                };

                printDialog.Document = printDoc;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDoc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الطباعة: {ex.Message}");
            }
        }
        private void LoadInvoices()
        {
            string invoicesFolder = Path.Combine(Application.StartupPath, "Invoices");

            if (Directory.Exists(invoicesFolder))
            {
                var pdfFiles = Directory.GetFiles(invoicesFolder, "*.pdf")
                                      .OrderByDescending(f => File.GetCreationTime(f))
                                      .ToArray();

                listBoxInvoices.DataSource = pdfFiles.Select(Path.GetFileName).ToArray();
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewProducts.Rows[e.RowIndex];

                txtProductID.Text = row.Cells["ProductID"].Value.ToString();
                txtBarcode.Text = row.Cells["Barcode"].Value?.ToString() ?? "";
                txtProductName.Text = row.Cells["ProductName"].Value.ToString();
                txtColor.Text = row.Cells["Color"].Value?.ToString() ?? "";
                txtSize.Text = row.Cells["Size"].Value?.ToString() ?? "";
                txtPrice_Sale.Text = row.Cells["SalePrice"].Value.ToString();

                txt_Qte.Focus();
            }
        }

        private void txt_Qte_TextChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }

        private void txtPrice_Sale_TextChanged(object sender, EventArgs e)
        {
            CalculateTotal();
        }
       

        private void btn_Sale_Click(object sender, EventArgs e)
        {
            //if (cmbCustomers.SelectedValue == null || string.IsNullOrEmpty(txtProductID.Text))
            //{
            //    MessageBox.Show("⚠️ الرجاء اختيار عميل ومنتج");
            //    return;
            //}

            //if (!ValidateSaleData())
            //    return;

            //try
            //{
            //    using (SqlCommand cmd = new SqlCommand("SP_CompleteSale", cn))
            //    {
            //        cmd.CommandType = CommandType.StoredProcedure;

            //        cmd.Parameters.AddWithValue("@CustomerID", cmbCustomers.SelectedValue);
            //        cmd.Parameters.AddWithValue("@UserID", currentUserID);
            //        cmd.Parameters.AddWithValue("@ProductID", Convert.ToInt32(txtProductID.Text));
            //        cmd.Parameters.AddWithValue("@Quantity", Convert.ToInt32(txt_Qte.Text));
            //        cmd.Parameters.AddWithValue("@UnitSalePrice", Convert.ToDecimal(txtPrice_Sale.Text));
            //        cmd.Parameters.AddWithValue("@Discount", Convert.ToDecimal(txt_Discount.Text ?? "0"));
            //        cmd.Parameters.AddWithValue("@Tax", Convert.ToDecimal(txt_Tax.Text ?? "0"));

            //        SqlParameter outputParam = new SqlParameter("@NewSaleID", SqlDbType.Int);
            //        outputParam.Direction = ParameterDirection.Output;
            //        cmd.Parameters.Add(outputParam);

            //        cn.Open();
            //        cmd.ExecuteNonQuery();
            //        cn.Close();

            //        int newSaleID = (int)outputParam.Value;
            //        MessageBox.Show($"✅ تم البيع بنجاح - رقم الفاتورة: {newSaleID}");
            //        ClearSaleFields();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"❌ خطأ في البيع: {ex.Message}");
            //}


            if (cmbCustomers.SelectedValue == null || string.IsNullOrEmpty(txtProductID.Text))
            {
                MessageBox.Show("⚠️ الرجاء اختيار عميل ومنتج");
                return;
            }

            if (!ValidateSaleData())
                return;

            try
            {
                int newSaleID = 0;

                using (SqlCommand cmd = new SqlCommand("SP_CompleteSale", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CustomerID", cmbCustomers.SelectedValue);
                    cmd.Parameters.AddWithValue("@UserID", currentUserID);
                    cmd.Parameters.AddWithValue("@ProductID", Convert.ToInt32(txtProductID.Text));
                    cmd.Parameters.AddWithValue("@Quantity", Convert.ToInt32(txt_Qte.Text));
                    cmd.Parameters.AddWithValue("@UnitSalePrice", Convert.ToDecimal(txtPrice_Sale.Text));
                    cmd.Parameters.AddWithValue("@Discount", Convert.ToDecimal(txt_Discount.Text ?? "0"));
                    cmd.Parameters.AddWithValue("@Tax", Convert.ToDecimal(txt_Tax.Text ?? "0"));

                    SqlParameter outputParam = new SqlParameter("@NewSaleID", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(outputParam);

                    // إدارة حالة الاتصال
                    bool shouldClose = false;
                    if (cn.State != ConnectionState.Open)
                    {
                        cn.Open();
                        shouldClose = true;
                    }

                    cmd.ExecuteNonQuery();
                    newSaleID = (int)outputParam.Value;

                    // إغلاق الاتصال إذا كنا قد فتحناه
                    if (shouldClose && cn.State == ConnectionState.Open)
                        cn.Close();
                }

                MessageBox.Show($"✅ تم البيع بنجاح - رقم الفاتورة: {newSaleID}");

                // توليد الفاتورة PDF
                GenerateInvoicePDF(newSaleID);

                ClearSaleFields();
            }
            catch (Exception ex)
            {
                // التأكد من إغلاق الاتصال في حالة الخطأ
                if (cn.State == ConnectionState.Open)
                    cn.Close();

                MessageBox.Show($"❌ خطأ في البيع: {ex.Message}");
            }


        }

        private void txtSearchProduct_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SP_SearchProducts", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SearchText", txtSearchProduct.Text.Trim());
                    cmd.Parameters.AddWithValue("@SearchByBarcode", 0);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridViewProducts.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ في البحث: " + ex.Message);
            }
        }

        private void txtBarcodeSe_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBarcodeSe.Text))
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetProductDetails", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Barcode", txtBarcodeSe.Text);
                        cmd.Parameters.AddWithValue("@ProductID", DBNull.Value);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            txtProductID.Text = row["ProductID"].ToString();
                            txtBarcode.Text = row["Barcode"].ToString();
                            txtProductName.Text = row["ProductName"].ToString();
                            txtColor.Text = row["Color"].ToString();
                            txtSize.Text = row["Size"].ToString();
                            txtPrice_Sale.Text = row["SalePrice"].ToString();
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("❌ خطأ في البحث بالباركود: " + ex.Message);
                }
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {

            try
            {
                string invoicesFolder = Path.Combine(Application.StartupPath, "Invoices");

                if (!Directory.Exists(invoicesFolder))
                {
                    Directory.CreateDirectory(invoicesFolder);
                    MessageBox.Show("تم إنشاء مجلد الفواتير لأول مرة");
                }

                System.Diagnostics.Process.Start(invoicesFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في فتح المجلد: {ex.Message}");
            }
        }

        private void listBoxInvoices_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBoxInvoices_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxInvoices.SelectedItem != null)
            {
                string fileName = listBoxInvoices.SelectedItem.ToString();
                string filePath = Path.Combine(Application.StartupPath, "Invoices", fileName);
                System.Diagnostics.Process.Start(filePath);
            }
        }

        // دالة مساعدة لإدارة حالة الاتصال
        private void ManageConnection(bool open)
        {
            try
            {
                if (open && cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }
                else if (!open && cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في إدارة الاتصال: {ex.Message}");
            }
        }

        // استخدام الدالة المساعدة
        private void ExampleUsage()
        {
            try
            {
                ManageConnection(true); // فتح الاتصال

                // تنفيذ الأوامر هنا

                ManageConnection(false); // إغلاق الاتصال
            }
            catch (Exception ex)
            {
                ManageConnection(false); // التأكد من الإغلاق في حالة الخطأ
                throw;
            }
        }

        private void خروجToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // في حدث إغلاق الفورم
           
                try
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                    cn.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"خطأ في إغلاق الاتصال: {ex.Message}");
                }
            
        }

        private void FRM_Pruches_Load(object sender, EventArgs e)
        {
            try
            {
                // اختبار الاتصال
                cn.Open();
                cn.Close();
                MessageBox.Show("✅ الاتصال بقاعدة البيانات ناجح");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ فشل الاتصال بقاعدة البيانات: {ex.Message}");
            }
        }
    }
}



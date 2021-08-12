using QuanLyQuanCaFe.DAO;
using QuanLyQuanCaFe.DTO;
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

namespace QuanLyQuanCaFe
{
    public partial class fLogin : Form
    {
        SqlConnection con;
        SqlCommand com;
        string conStr = @"Data Source=NHAC\TNHAC;Initial Catalog=QuanLyQuanCafe;Integrated Security=True";
        public fLogin()
        {
            InitializeComponent();
        }
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            //string userName = txtTaiKhoan.Text;
            //string passWord = txtMK.Text;
            //if (Login(userName, passWord))
            //{
            //    Account loginAccount = AccountDAO.Instance.GetAccountByUserName(userName);
            //    fTableManager f = new fTableManager(loginAccount);
            //    this.Hide();
            //    f.ShowDialog();
            //    this.Show();
            //}
            //else
            //{
            //    MessageBox.Show("Sai tên tài khoản hoặc mật khẩu!");
            //} 
            if (con.State == ConnectionState.Open)
                con.Close();
            con.Open();
            try
            {
                string cmd = "Select Count(*) From Account ";
                cmd += "where UserName='" + txtTaiKhoan.Text.Trim() + "'";
                cmd += " and Password='" + txtMK.Text.Trim() + "'";
                com.CommandText = cmd;
                int drTK = (int)com.ExecuteScalar();
                if (drTK > 0)
                {
                    Account loginaccount = AccountDAO.Instance.GetAccountByUserName(txtTaiKhoan.Text);
                    MessageBox.Show("Đăng nhập thành công.", "Kếtt quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //this.Close();
                    fTableManager f = new fTableManager(loginaccount);
                    this.Hide();
                    f.ShowDialog();
                    this.Show();

                }
                else
                {
                    MessageBox.Show("Đăng nhập thất bại!!!", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //txtTaiKhoan.ResetText();
                    //txtMK.ResetText();
                    txtTaiKhoan.Focus();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("ERROR: " + ex.Message);
            }
        }
        //bool Login(string userName, string passWord)
        //{
        //    return AccountDAO.Instance.Login(userName,passWord);
        //}

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void fLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có thực sự muốn thoát chương trình?", "Thông báo", MessageBoxButtons.OKCancel)
                 != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void fLogin_Load(object sender, EventArgs e)
        {
            try
            {
                con = new SqlConnection(conStr);
                if (con.State == ConnectionState.Open) con.Close();
                con.Open();
                com = new SqlCommand();
                com.Connection = con;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

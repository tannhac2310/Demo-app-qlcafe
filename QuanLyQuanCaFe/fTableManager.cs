using QuanLyQuanCaFe.DAO;
using QuanLyQuanCaFe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCaFe
{
    public partial class fTableManager : Form
    {
        private Account loginAccount;

        public Account LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount.Type); }
        }

        public fTableManager(Account acc)
        {
            InitializeComponent();

            this.LoginAccount = acc;

            LoadTable();
            LoadCategory();
            LoadComboBoxTable(cbSwitchTable);
        }

        #region Method

        void ChangeAccount(int type)
        {
            adminToolStripMenuItem.Enabled = type == 1;
            thôngTinTàiKhoảnToolStripMenuItem.Text += "( " + LoginAccount.DisplayName + ")";
        }

        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cbCategory.DataSource = listCategory;
            cbCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cbFood.DataSource = listFood;
            cbFood.DisplayMember = "Name";
        }

        void LoadTable()
        {
            flpTable.Controls.Clear();
            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
                Button btn = new Button() 
                { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight};
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;

                switch(item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.LightBlue;
                        break;
                    default:
                        btn.BackColor = Color.Green;
                        break;

                }

                flpTable.Controls.Add(btn);

            }
        }


        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float totalPrice = 0;

            foreach (DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;
                lsvBill.Items.Add(lsvItem);

            }
            CultureInfo culture = new CultureInfo("vi-VN");
            //Thread.CurrentThread.CurrentCulture = culture;
            txttotalPrice.Text = totalPrice.ToString("c", culture);

        }

        void LoadComboBoxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";
        }

        #endregion

        #region Events
        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAccountProfile f = new fAccountProfile(LoginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }

        private void f_UpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }

        private void adminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fAdmin f = new fAdmin();
            f.loginAccount = LoginAccount;
            f.InsertTable += f_InsertTable;
            f.DeleteTable += f_DeleteTable;
            f.UpdateTable += f_UpdateTable;
            f.InsertCategory += f_InsertCategory;
            f.DeleteCategory += f_DeleteCategory;
            f.UpdateCategory += f_UpdateCategory;
            f.InsertFood += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.UpdateFood += f_UpdateFood;
            f.ShowDialog();
        }

        private void f_DeleteTable(object sender, EventArgs e)
        {
            LoadTable();
            LoadComboBoxTable(cbSwitchTable);
        }

        private void f_UpdateTable(object sender, EventArgs e)
        {
            LoadTable();
            LoadComboBoxTable(cbSwitchTable);
        }

        private void f_InsertTable(object sender, EventArgs e)
        {
            LoadTable();
            LoadComboBoxTable(cbSwitchTable);
        }

        //Category
        private void f_UpdateCategory(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void f_DeleteCategory(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID); 
        }

        private void f_InsertCategory(object sender, EventArgs e)
        {
            LoadCategory();
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        //Food
        private void f_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void f_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
            LoadTable();
        }

        private void f_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);

        }

        private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem == null)
                return;

            Category selected = cb.SelectedItem as Category;
            id = selected.ID;
            LoadFoodListByCategoryID(id);
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn!!!");
                return;
            }    

            int idBill = BillDAO.Instance.GetUncheckBillByIDTableID(table.ID);
            int foodID = (cbFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;

            if (idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID , count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }
            ShowBill(table.ID);

            LoadTable();
        }

        private void btnCheckOut_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            if(table == null)
            {
                MessageBox.Show("Hãy chọn bàn muốn thanh toán!!!");
                    return;
            }    

            int idBill = BillDAO.Instance.GetUncheckBillByIDTableID(table.ID);
            int discount = (int)nmDisCount.Value;

            double totalPrice = double.Parse(txttotalPrice.Text, NumberStyles.Currency, new CultureInfo("vi-VN"));
            double finalTotalPrice = totalPrice - ((totalPrice / 100) * discount);

            DialogResult traloi;
            traloi = MessageBox.Show("Bạn có muốn in hoá đơn không?", "Thông báo",
            MessageBoxButtons.YesNoCancel);

            if (idBill != -1)
            {
                if (traloi == DialogResult.Yes)
                {
                    printPreviewBill.Document = printDocumentBill;
                    printPreviewBill.ShowDialog();
                    BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                    ShowBill(table.ID);
                    LoadTable();
                }
                else if (traloi == DialogResult.No)
                {
                    if (MessageBox.Show(string.Format("Bạn có chắc muốn thanh toán hoá đơn cho bàn {0}\nTổng tiền - (Tổng Tiền / 100) x Giảm giá\n => {1} - ({1} / 100) x {2} = {3}",
                    table.Name, totalPrice, discount, finalTotalPrice), "Thông báo",
                    MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                        BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                    ShowBill(table.ID);
                    LoadTable();
                }
            }
        }

        private void btnChuyenBan_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn muốn chuyển!!!");
                return;
            }
            string idn1 = (lsvBill.Tag as Table).Name;
            string idn2 = (cbSwitchTable.SelectedItem as Table).Name;
            int id1 = (lsvBill.Tag as Table).ID;
            int id2 = (cbSwitchTable.SelectedItem as Table).ID;
            if (MessageBox.Show(string.Format("Bạn có thực sự muốn chuyển bàn {0} sang bàn {1}", idn1, idn2), "Thông báo",
                MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);

                LoadTable();
            }
        }


        #endregion

        private void btnPrintBill_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            if (table == null)
            {
                MessageBox.Show("Hãy chọn đúng bàn muốn in hoá đơn!!!");
                return;
            }    
            printDocumentBill.Print();
        }

        private void printDocumentBill_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            List<DTO.Menu> listBillInfo = DAO.MenuDAO.Instance.GetListMenuByTable(table.ID);

            e.Graphics.DrawString("HOÁ ĐƠN", new Font("Times New Roman", 30, FontStyle.Bold),
                Brushes.Black, new Point(330, 50));
            e.Graphics.DrawString("Date: " + DateTime.Now, new Font("Times New Roman", 15, FontStyle.Regular),
                Brushes.Black, new Point(25, 160));
            e.Graphics.DrawString("" + table.Name, new Font("Times New Roman", 15, FontStyle.Regular),
                Brushes.Black, new Point(25, 190));
            e.Graphics.DrawString("-----------------------------------------------------------------------------------" +
                "---------------------------------------", new Font("Times New Roman", 15, FontStyle.Regular),
               Brushes.Black, new Point(25, 235));
            e.Graphics.DrawString("Tên món", new Font("Times New Roman", 15, FontStyle.Regular),
                Brushes.Black, new Point(30, 255));
            e.Graphics.DrawString("Số lượng", new Font("Times New Roman", 15, FontStyle.Regular),
                Brushes.Black, new Point(300, 255));
            e.Graphics.DrawString("Đơn giá", new Font("Times New Roman", 15, FontStyle.Regular),
                Brushes.Black, new Point(480, 255));
            e.Graphics.DrawString("Tổng tiền", new Font("Times New Roman", 15, FontStyle.Regular),
                Brushes.Black, new Point(660, 255));
            e.Graphics.DrawString("-----------------------------------------------------------------------------------" +
                "---------------------------------------", new Font("Times New Roman", 15, FontStyle.Regular),
                   Brushes.Black, new Point(25, 270));

            double totalPrice = 0;
            int pos = 295;
            foreach (DTO.Menu item in listBillInfo)
            {
                e.Graphics.DrawString(item.FoodName, new Font("Times New Roman", 15, FontStyle.Regular),
                Brushes.Black, new Point(30, pos));
                e.Graphics.DrawString(item.Count.ToString(), new Font("Times New Roman", 15, FontStyle.Regular),
               Brushes.Black, new Point(310, pos));
                e.Graphics.DrawString(item.Price.ToString(), new Font("Times New Roman", 15, FontStyle.Regular),
               Brushes.Black, new Point(480, pos));
                totalPrice += item.TotalPrice;
                e.Graphics.DrawString(item.TotalPrice.ToString(), new Font("Times New Roman", 15, FontStyle.Regular),
               Brushes.Black, new Point(660, pos));
                pos += 30;
            }
            e.Graphics.DrawString("-----------------------------------------------------------------------------------" +
                "---------------------------------------", new Font("Times New Roman", 15, FontStyle.Regular),
                   Brushes.Black, new Point(25, pos));
            //Tính tổng hoá đơn
            int discount = (int)nmDisCount.Value;
            double total = double.Parse(txttotalPrice.Text, NumberStyles.Currency, new CultureInfo("vi-VN"));
            double finalPrice = total - (total / 100) * discount;

            e.Graphics.DrawString("Tổng:          " + total + "đ", new Font("Times New Roman", 15, FontStyle.Regular),
              Brushes.Black, new Point(580, pos + 30));
            e.Graphics.DrawString("Giảm giá:   " + nmDisCount.Value + "%", new Font("Times New Roman", 15, FontStyle.Regular),
              Brushes.Black, new Point(580, pos + 60));
            e.Graphics.DrawString("Thành tiền: " + finalPrice + "đ", new Font("Times New Roman", 15, FontStyle.Regular),
               Brushes.Black, new Point(580, pos + 90));
        }
    }
}

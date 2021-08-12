using Microsoft.Reporting.WinForms;
using QuanLyQuanCaFe.DAO;
using QuanLyQuanCaFe.DTO;
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
using Account = QuanLyQuanCaFe.DTO.Account;
using Bill = QuanLyQuanCaFe.DTO.Bill;
using Food = QuanLyQuanCaFe.DTO.Food;

namespace QuanLyQuanCaFe
{
    public partial class fAdmin : Form
    {
        BindingSource foodList = new BindingSource();
        BindingSource accountList = new BindingSource();
        BindingSource categoryList = new BindingSource();
        BindingSource tableList = new BindingSource();

        //Không cho xoá tài khoản hiện tại
        public Account loginAccount;
        
        public fAdmin()
        {
            InitializeComponent();
            //this.Controls.Add(this.reportViewer1);
            Load();

        }

        private void Load()
        {
            dgvFood.DataSource = foodList;
            dgvAccount.DataSource = accountList;
            dgvCategory.DataSource = categoryList;
            dgvTable.DataSource = tableList;

            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);

            LoadListFood();
            AddFoodBinding();
            LoadCategoryIntoComboBox(cbFoodCategory);

            LoadAccount();
            AddAcountBinding();

            LoadCategory();
            AddCategoryBinding();

            LoadTable();
            AddTableBinding();

        }
        #region Methods

        //Table - Method
        void AddTableBinding()
        {
            txtIDTable.DataBindings.Add(new Binding("Text", dgvTable.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txtTableName.DataBindings.Add(new Binding("Text", dgvTable.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txtTableStatus.DataBindings.Add(new Binding("Text", dgvTable.DataSource, "status", true, DataSourceUpdateMode.Never));
        }


        void LoadTable()
        {
            tableList.DataSource = TableDAO.Instance.LoadTableList();
        }

        //Category Method

        void AddCategoryBinding()
        {
            txtCategoryID.DataBindings.Add(new Binding("Text", dgvCategory.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txtCategoryName.DataBindings.Add(new Binding("Text", dgvCategory.DataSource, "Name", true, DataSourceUpdateMode.Never));
        }

        void LoadCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetListCategory();
        }



        //Account Method

        void AddAcountBinding()
        {
            txtAccountName.DataBindings.Add(new Binding("Text", dgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txtHienThiAccount.DataBindings.Add(new Binding("Text", dgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            nmLoaiTK.DataBindings.Add(new Binding("Value", dgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));
        }

        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();

        }

        void AddAccount(string userName, string DisplayName, int type)
        {
            if (checkAccount())
            {
                if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn thêm tài khoản {0} không?", userName),
                  "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    if (AccountDAO.Instance.InsertAccount(userName, DisplayName, type))
                    {
                        MessageBox.Show("Thêm tài khoản thành công!!!");
                    }
                    else
                    {
                        MessageBox.Show("Thêm tài khoản thất bại!!!");
                    }
                    LoadAccount();
                }
            }
        }

        void EditAccount(string userName, string DisplayName, int type)
        {
            if (checkAccount())
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn sửa không?",
               "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    if (AccountDAO.Instance.UpdateInfoAccount(userName, DisplayName, type))
                    {
                        MessageBox.Show("Cập nhật tài khoản thành công!!!");
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật tài khoản thất bại!!!");
                    }
                    LoadAccount();
                }
            }
        }

        void DeleteAccount(string userName)
        {
            if (loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("Không thể xoá tài khoản đang sử dụng!");
                return;
            }
            if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn xoá {0} không?", userName),
               "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                if (AccountDAO.Instance.DeleteAccount(userName))
                {
                    MessageBox.Show("Xoá tài khoản thành công!!!");
                }
                else
                {
                    MessageBox.Show("Xoá tài khoản thất bại!!!");
                }
                LoadAccount();
            }
        }

        void ResetPass(string name)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn RESET lại PASSWORD không?",
              "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                if (AccountDAO.Instance.ResetPassword(name))
                {
                    MessageBox.Show("Đã đặt lại mật khẩu!!!");
                }
                else
                {
                    MessageBox.Show("Đặt lại thất bại!!!");
                }
            }
        }

        //Food Method

        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }

        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        void AddFoodBinding()
        {
            txtFoodName.DataBindings.Add(new Binding("Text", dgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
            txtFoodID.DataBindings.Add(new Binding("Text", dgvFood.DataSource, "ID",true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));

        }

        void LoadCategoryIntoComboBox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }

        //Thống kê Method
        void LoadDateTimePickerBill()
        {

            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }

        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);

        }

        #endregion


        #region Events

        //FOOD
        private void btnViewBill_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        private void btnWatchFood_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }

        private void txtFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvFood.SelectedCells.Count > 0)
                {
                    int id = (int)dgvFood.SelectedCells[0].OwningRow.Cells["CategoryID"].Value;

                    Category category = CategoryDAO.Instance.GetCategoryByID(id);

                    cbFoodCategory.SelectedItem = category;
                    int index = -1;
                    int i = 0;
                    foreach (Category item in cbFoodCategory.Items)
                    {
                        if (item.ID == category.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }
                    cbFoodCategory.SelectedIndex = index;
                }
            }
            catch {  }
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txtFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            if (checkfood())
            {
                if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn thêm {0} không?", name),
               "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    if (FoodDAO.Instance.InsertFood(name, categoryID, price))
                    {
                        MessageBox.Show("Thêm món thành công!");
                        LoadListFood();
                        if (insertFood != null)
                            insertFood(this, new EventArgs());
                    }
                    else { MessageBox.Show("Không thể thêm thức ăn!"); }
                }
            }
        }

        private void btnEditFood_Click(object sender, EventArgs e)
        {
            string name = txtFoodName.Text;
            int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            int idFood = Convert.ToInt32(txtFoodID.Text);

            if (checkfood())
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn sửa không?",
                   "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    if (FoodDAO.Instance.UpdateFood(idFood, name, categoryID, price))
                    {
                        MessageBox.Show("Sửa món thành công!");
                        LoadListFood();
                        if (updateFood != null)
                        {
                            updateFood(this, new EventArgs());
                        }
                    }
                    else { MessageBox.Show("Không thể sửa thức ăn!"); }
                }
            }
        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int idFood = Convert.ToInt32(txtFoodID.Text);
            string name = txtFoodName.Text;
            if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn xoá {0} không?", name),
                "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                if (FoodDAO.Instance.DeleteFood(idFood))
                {
                    MessageBox.Show("Xoá món thành công!");
                    LoadListFood();
                    if (deleteFood != null)
                    {
                        deleteFood(this, new EventArgs());
                    }
                }
                else { MessageBox.Show("Không thể xoá thức ăn!"); }
            }
        }

        private void btnFindFood_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(txtSearchFoodName.Text);
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }

        }
        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        //Account

        private void btnXemAccount_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }

        private void btnAddAcount_Click(object sender, EventArgs e)
        {
            string userName = txtAccountName.Text;
            string displayName = txtHienThiAccount.Text;
            int type = (int)nmLoaiTK.Value;

            AddAccount(userName, displayName, type);
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txtAccountName.Text;

            DeleteAccount(userName);
        }

        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            string userName = txtAccountName.Text;
            string displayName = txtHienThiAccount.Text;
            int type = (int)nmLoaiTK.Value;

            EditAccount(userName, displayName, type);
        }

        private void btnResetMK_Click(object sender, EventArgs e)
        {
            string username = txtAccountName.Text;

            ResetPass(username);
        }

        //Category

        private void btnXemCategory_Click(object sender, EventArgs e)
        {
            LoadCategory();
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string name = txtCategoryName.Text;
            if (checkCategory())
            {
                if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn thêm {0} không?", name),
                   "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    if (CategoryDAO.Instance.InsertCategory(name))
                    {
                        MessageBox.Show("Thêm loại thức uống thành công!!!");
                        LoadCategory();
                        LoadCategoryIntoComboBox(cbFoodCategory);
                        if (insertCategory != null)
                        {
                            insertCategory(this, new EventArgs());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi khi thêm loại thức uống!!!");
                    }
                }
            }

        }

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtCategoryID.Text);
            string name = txtCategoryName.Text;
            if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn xoá {0} không?", name),
               "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                if (CategoryDAO.Instance.DeleteCategory(id))
                {
                    MessageBox.Show("Xoá loại thức uống thành công!!!");
                    LoadCategory();
                    LoadListFood();
                    LoadCategoryIntoComboBox(cbFoodCategory);
                    if (deleteCategory != null)
                    {
                        deleteCategory(this, new EventArgs());
                    }
                }
                else
                {
                    MessageBox.Show("Có lỗi khi xoá loại thức uống!!!");
                }
            }
        }

        private void btnEditCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtCategoryID.Text);
            string name = txtCategoryName.Text;
            if (checkCategory())
            {
                if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn sửa thành {0} không?", name),
                  "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    if (CategoryDAO.Instance.EditCategory(id, name))
                    {
                        MessageBox.Show("Cập nhật loại thức uống thành công!!!");
                        LoadCategory();
                        LoadCategoryIntoComboBox(cbFoodCategory);
                        if (updateCategory != null)
                        {
                            updateCategory(this, new EventArgs());
                        }

                    }
                    else
                    {
                        MessageBox.Show("Có lỗi khi cập nhật loại thức uống!!!");
                    }
                }
            }
        }


        private event EventHandler insertCategory;
        public event EventHandler InsertCategory
        {
            add { insertCategory += value; }
            remove { insertCategory -= value; }
        }

        private event EventHandler deleteCategory;
        public event EventHandler DeleteCategory
        {
            add { deleteCategory += value; }
            remove { deleteCategory -= value; }

        }
        private event EventHandler updateCategory;
        public event EventHandler UpdateCategory
        {
            add { updateCategory += value; }
            remove { updateCategory -= value; }
        }

        private void btnWatchTable_Click(object sender, EventArgs e)
        {
            LoadTable();
        }

        private void btnAddTable_Click(object sender, EventArgs e)
        {
            string name = txtTableName.Text;
            if (checkTable())
            {
                if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn thêm {0} không?", name),
                  "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    if (TableDAO.Instance.InsertTable(name))
                    {
                        MessageBox.Show("Thêm bàn mới thành công!!!");
                        LoadTable();
                        if (insertTable != null)
                        {
                            insertTable(this, new EventArgs());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi khi thêm bàn mới!!!");
                    }
                }
            }
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtIDTable.Text);
            string name = txtTableName.Text;
            string status = txtTableStatus.Text;
            if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn xoá {0} không?", name),
            "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                switch (status)
                {
                    case "Trống":
                        if (TableDAO.Instance.DeleteTable(id))
                        {
                            MessageBox.Show("Xoá bàn thành công!!!");
                            LoadTable();
                            if (deleteTable != null)
                            {
                                deleteTable(this, new EventArgs());
                            }
                        }
                        else
                        {
                            MessageBox.Show("Có lỗi khi xoá bàn!!!");
                        }
                        break;
                    default:
                        MessageBox.Show("Không thể xoá bàn đang có khách!");
                        break;
                }
            }
        }

        private void btnEditTable_Click(object sender, EventArgs e)
        {
            string name = txtTableName.Text;
            int id = Convert.ToInt32(txtIDTable.Text);
            if (checkTable())
            {
                if (MessageBox.Show(string.Format("Bạn có chắc chắn muốn sửa thành {0} không?", name),
                "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    if (TableDAO.Instance.UpdateTable(id, name))
                    {
                        MessageBox.Show("Cập nhật bàn thành công!!!");
                        LoadTable();
                        if (updateTable != null)
                            updateTable(this, new EventArgs());
                    }
                    else
                    {
                        MessageBox.Show("Có lỗi khi cập nhật bàn!!!");
                    }
                }
            }
        }

        private void txtIDTable_TextChanged(object sender, EventArgs e)
        {
   
        }

        private event EventHandler insertTable;
        public event EventHandler InsertTable
        {
            add { insertTable += value; }
            remove { insertTable -= value; }
        }

        private event EventHandler deleteTable;
        public event EventHandler DeleteTable
        {
            add { deleteTable += value; }
            remove { deleteTable -= value; }

        }
        private event EventHandler updateTable;
        public event EventHandler UpdateTable
        {
            add { updateTable += value; }
            remove { updateTable -= value; }
        }
        private bool checkfood()
        {
            bool check = true;
            if (txtFoodName.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Bạn chưa nhập tên thức ăn!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtFoodName.Focus();
                check = false;
            }
            return check;
        }
        private bool checkCategory()
        {
            bool check = true;
            if (txtCategoryName.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Bạn chưa nhập tên loại thức ăn!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCategoryName.Focus();
                check = false;
            }
            return check;
        }
        private bool checkTable()
        {
            bool check = true;
            if (txtTableName.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Bạn chưa nhập tên bàn!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTableName.Focus();
                check = false;
            }
            return check;
        }
        private bool checkAccount()
        {
            bool check = true;
            if (txtAccountName.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Bạn chưa nhập tên tài khoản!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAccountName.Focus();
                check = false;
            }
            if (txtHienThiAccount.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Bạn chưa nhập tên hiển thị!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtHienThiAccount.Focus();
                check = false;
            }
            return check;
        }

        #endregion


        

        private void tcAdmin_Click(object sender, EventArgs e)
        {

        }
    }
}

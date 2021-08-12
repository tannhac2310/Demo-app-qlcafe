using QuanLyQuanCaFe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCaFe.DAO
{
    public class CategoryDAO
    {
        private static CategoryDAO instance;

        public static CategoryDAO Instance
        {
            get { if (instance == null) instance = new CategoryDAO(); return CategoryDAO.instance; }
            private set { CategoryDAO.instance = value; }
        }

        private CategoryDAO() { }

        public List<Category> GetListCategory()
        {
            List<Category> list = new List<Category>();
            string query = "select * from FoodCategory";

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Category category = new Category(item);
                list.Add(category);
            }
            return list;
        }

        public Category GetCategoryByID(int id)
        {
            Category category = null;
            string query = "select * from FoodCategory where id = " + id;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                category = new Category(item);
                return category;
            }

            return category;
        }

        public bool InsertCategory(string name)
        {
            //string query = string.Format("Insert into dbo.FoodCategory(name) Values (N'{0}')", name);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspInsertCategory @name ",
                new object[] { name });
            return result > 0;
        }
        public bool DeleteCategory(int id)
        {
            //string query = string.Format("Delete FoodCategory where id = {0}", id);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspDeleteCategory @id",
               new object[] { id });
            return result > 0;
        }

        public bool EditCategory(int id, string name)
        {
            //string query = string.Format("UPDATE FoodCategory set name = N'{1}' where id = {0}", id, name);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspUpdateCategory @id , @name ",
                new object[] { id, name });
            return result > 0;
        }
    }
}

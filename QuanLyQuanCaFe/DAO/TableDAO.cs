using QuanLyQuanCaFe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCaFe.DAO
{
    public class TableDAO
    {
        private static TableDAO instance;

        public static TableDAO Instance
        {
            get { if (instance == null) instance = new TableDAO(); return TableDAO.instance; }
            private set { TableDAO.instance = value; }

        }

        public static int TableWidth = 90;
        public static int TableHeight = 90;
        private TableDAO() { }

        public void SwitchTable(int id1, int id2)
        {
            DataProvider.Instance.ExecuteQuery("usp_SwitchTable @idTable1 , @idTable2 ", new object[] { id1, id2 });
        }

        public List<Table> LoadTableList()
        {
            List<Table> tableList = new List<Table>();
            DataTable data = DataProvider.Instance.ExecuteQuery("exec uspGetTableList");

            foreach (DataRow item in data.Rows)
            {
                Table table = new Table(item);
                tableList.Add(table);
            }

            return tableList;
        }

        public Table GetTableByID(int id)
        {
            Table table = null;
            string query = "select * from TableFood where id = " + id;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                table = new Table(item);
                return table;
            }

            return table;
        }

        public int GetMaxIDTable()
        {
            try
            {
                return (int)DataProvider.Instance.ExecuteScalar("Select MAX(id) FROM TableFood");
            }
            catch
            {
                return 1;
            }
        }

        public bool InsertTable(string name)
        {
            //int k = GetMaxIDTable() + 1;

            //string query = "Insert into dbo.TableFood(name) Values (N'Bàn "+ k +"')";
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspInsertTable @name ",
               new object[] { name });
            return result > 0;
        }

        public bool DeleteTable(int id)
        {
            //string query = string.Format("Delete dbo.TableFood where id = {0}", id);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspDeleteTable @id",
                new object[] { id });
            return result > 0;
        }
        public bool UpdateTable(int id, string name)
        {
            //string query = string.Format("UPDATE dbo.TableFood SET name = N'{1}' WHERE id = {0}", id, name);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspUpdateTable @id , @name ",
                new object[] { id, name });
            return result > 0;
        }
    }
}

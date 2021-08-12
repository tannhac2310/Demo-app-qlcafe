using QuanLyQuanCaFe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCaFe.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get { if (instance == null) instance = new AccountDAO(); return instance; }

            private set { instance = value; }
        }
        private AccountDAO() { }

        public bool Login(string userName, string passWord)
        {
            byte[] temp = ASCIIEncoding.ASCII.GetBytes(passWord);
            byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

            string hasPass = "";
            foreach (byte item in hasData)
            {
                hasPass += item;
            }

            var list = hasData.ToString();
            list.Reverse();

            string query = "USP_Login @username , @password";

            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] { userName, passWord /*List*/});
            return result.Rows.Count > 0;
        }

        public bool UpdateAccount(string userName, string displayName, string pass, string newPass)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("Exec USP_UpdateAccount @userName , @displayName , @password , @newPassword",
                new object[] { userName, displayName, pass, newPass });
            return result > 0;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("Select Username, displayName, type from Account");
        }

        public Account GetAccountByUserName(string userName)
        {
            DataTable data = DataProvider.Instance.ExecuteQuery("Select * From Account where username = '" + userName + "'");

            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }

            return null;
        }

        public bool InsertAccount(string name, string displayname, int type)
        {
            //string query = string.Format("Insert into dbo.Account(username, displayName, type) Values (N'{0}',N'{1}',{2})", name, displayname, type);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspInsertAccount @username , @displayname , @type ",
                new object[] { name, displayname, type });
            return result > 0;
        }

        public bool UpdateInfoAccount(string name, string displayname, int type)
        {
            //string query = string.Format("Update Account SET DisplayName = N'{1}', type = {2} where username = N'{0}'", name, displayname, type);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspUpdateInfoAccount @username , @displayname , @type",
               new object[] { name, displayname, type });
            return result > 0;
        }

        public bool DeleteAccount(string name)
        {

            //string query = string.Format("Delete Account where username = N'{0}'", name);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspDeleteAccount @username",
                new object[] { name });
            return result > 0;
        }

        public bool ResetPassword(string name)
        {
            //string query = string.Format("Update Account set password = N'0' where username = N'{0}'", name);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspResetPassAccount @username",
                new object[] { name });
            return result > 0;
        }
    }

}

﻿using QuanLyQuanCaFe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCaFe.DAO
{
    public class FoodDAO
    {
        private static FoodDAO instance;

        public static FoodDAO Instance 
        { 
            get { if (instance == null) instance = new FoodDAO(); return FoodDAO.instance; }
            set { FoodDAO.instance = value; }
        }

        private FoodDAO() { }

        public List<Food> GetFoodByCategoryID(int id)
        {
            List<Food> list = new List<Food>();
            string query = "select * from food where idCategory = " + id;

            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach(DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }    
            return list;

        }
        public List<Food> GetListFood()
        {
            List<Food> list = new List<Food>();
            string query = "Select * From Food";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }
            return list;
        }

        public List<Food> SearchFoodByName(string name)
        {
            List<Food> list = new List<Food>();
            string query = string.Format("SELECT * FROM dbo.Food WHERE dbo.fuConvertToUnsign1(name) LIKE N'%' + dbo.fuConvertToUnsign1(N'{0}') + '%'", name);
            DataTable data = DataProvider.Instance.ExecuteQuery(query);

            foreach (DataRow item in data.Rows)
            {
                Food food = new Food(item);
                list.Add(food);
            }
            return list;
        }
        
        public bool InsertFood(string name, int id, float price)
        {
            //string query = string.Format("Insert into dbo.Food(name, idCategory, price) Values (N'{0}',{1},{2})", name, id, price);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspInsertFood @name , @idCate , @price ",
                new object[] { name, id, price });
            return result > 0;
        }

        public bool UpdateFood(int idFood, string name,int id, float price)
        {
            //string query = string.Format("Update Food set name = N'{0}', idCategory = {1}, price = {2} where id = {3}", name, id, price, idFood);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspUpdateFood @name , @idCate , @price , @idFood",
              new object[] { name, id, price, idFood });
            return result > 0;
        }

        public bool DeleteFood(int idFood)
        {
            BillInfoDAO.Instance.DeleteBillInfoByFoodID(idFood);
            //string query = string.Format("Delete Food where id = {0}", idFood);
            //int result = DataProvider.Instance.ExecuteNonQuery(query);
            int result = DataProvider.Instance.ExecuteNonQuery(" exec uspDeleteFood @idFood",
               new object[] { idFood });
            return result > 0;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RxFair.Data.Utility
{
    public static class Common
    {
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            var data = new List<T>();
            for (var index = 0; index < dt.Rows.Count; index++)
            {
                var row = dt.Rows[index];
                var item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            var temp = typeof(T);
            var obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (var pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                }
            }
            return obj;
        }

        public static T GetItem<T>(DataTable dataTable)
        {
            var obj = Activator.CreateInstance<T>();
            return obj;
        }

        private static bool IsAnyNullOrEmpty(object myObject)
        {
            return myObject.GetType().GetProperties().All(pi => !string.IsNullOrEmpty((string)pi.GetValue(myObject)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TSSLogParser.EFCore
{
    public static class Extensions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> entityList) where T : class
        {
            var properties = typeof(T).GetProperties();
            var table = new DataTable();

            foreach (var property in properties)
            {
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                table.Columns.Add(property.Name, type);
            }
            foreach (var entity in entityList)
            {
                table.Rows.Add(properties.Select(p => p.GetValue(entity, null)).ToArray());
            }
            table.TableName = typeof(T).Name;
            return table;
        }
    }
}

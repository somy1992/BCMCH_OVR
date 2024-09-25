using System.Collections.Generic;
using System.Data.SqlClient;

namespace BCMCHOVR.Helpers
{
    public static partial class Extensions
    {
        public static IDataGrid ToDataGrid(this System.Data.SqlClient.SqlDataReader reader)
        {
            if (reader == null) { return null; }

            IDataGrid dataGrid = new DataGrid();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                dataGrid.AddHeader(reader.GetName(i));
            }

            while (reader.Read())
            {
                var row = dataGrid.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader[i].ToString());
                }
            }
            return dataGrid;
        }

        public static JsonList ToJsonList(this SqlDataReader reader)
        {
            if (reader.HasRows)
            {
                JsonList jsonList = new();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        jsonList.Add(reader.GetName(i), reader[i].ToString());
                    }
                    break;
                }
                return jsonList;
            }
            return null;
        }
        public static Dictionary<string, string> ToStringList
      (this SqlDataReader reader)
        {
            var dict = new Dictionary<string, string>();

            if (reader.HasRows)
            {
                reader.Read();

                for (int lp = 0; lp < reader.FieldCount; lp++)
                {
                    dict.Add(reader.GetName(lp), reader.GetValue(lp).ToString());
                }
            }
            return dict;
        }

        public static string ToHtml(this Dictionary<string, string> itemList, bool sepeartion)
        {
            string html = string.Format("<table style=\"{0}\">", "width:100%");
            string sep = sepeartion ? "<td> : </td>" : "";
            foreach (var item in itemList)
            {
                if (item.Value == "") { continue; }
                html += string.Format(
                    @"<tr><td style={0}>{1}</td>" + sep + "<td>{2}</td></tr>",
                    "=width:150px", item.Key, item.Value
                    );
            }
            return html + "</table>";
        }

    }
}

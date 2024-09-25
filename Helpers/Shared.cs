using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace BCMCHOVR.Helpers
{
    public class Shared
    {
        public static string ReadSqlFile(string filePath)
        {
            string value;
            using StreamReader sr = File.OpenText(filePath);
            value = sr.ReadToEnd();
            return value;
        }
        public static string GetEmailTemplate(string path)
        {
            return ReadSqlFile(System.Environment.CurrentDirectory + "//wwwroot//" + path);
        }
        public static string StripHTML(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", String.Empty);
        }

    }
    public interface IDataGrid
    {
        string Scope { set; }

        bool HasRow();

        void AddHeader(string value);

        IList<string> GetHeader();

        IList<string> NewRow();

        IList<IList<string>> GetRows();

        bool ConvertSqlDataReader(SqlDataReader reader);

        bool ConvertSqlDataTable(SqlDataReader reader);
    }

    public class DataGrid : IDataGrid
    {
        private IList<string> Headers { get; set; }
        private IList<IList<string>> Rows { get; set; }
        private string scope;

        public string Scope
        {
            set
            {
                scope = value;
            }
        }

        public int RowCount
        {
            get
            {
                return Rows.Count;
            }
        }

        public bool HasRow()
        {
            return RowCount > 0 ? true : false;
        }

        public DataGrid()
        {
            scope = "";
            Headers = new List<string>();
            Rows = new List<IList<string>>();
        }

        public void AddHeader(string value)
        {
            Headers.Add(value);
        }

        public IList<string> GetHeader()
        {
            return Headers;
        }

        public IList<IList<string>> GetRows()
        {
            return Rows;
        }

        public IList<string> NewRow()
        {
            var row = new List<string>();
            Rows.Add(row);
            return row;
        }

        public bool ConvertSqlDataTable(SqlDataReader reader)
        {
            return false;
        }

        public bool ConvertSqlDataReader(SqlDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                AddHeader(reader.GetName(i));
            }

            while (reader.Read())
            {
                var row = NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader[i].ToString());
                }
            }
            return true;
        }
    }

    public class JsonDataGrid
    {
        public IList<string> Header { get; set; }
        public IList<IList<string>> Rows { get; set; }

        public JsonDataGrid()
        {
            Header = new List<string>();
            Rows = new List<IList<string>>();
        }
    }


}

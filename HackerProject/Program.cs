using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Data;

namespace HackerProject
{
    public class Program
    {
        private string id;
        private string type;
        private string name;
        private double version;
        private long size;
        
        private string run;
        private string delete;
        private string upload;
        private string download;
        private string hide;
        private string unhide;
        private string spublic;
        private string sprivate;
        private string encrypt;
        private string decrypt;


        public Program()
        {

        }

        public string Id { get => id; set => id = value; }
        public string Type { get => type; set => type = value; }
        public string Name { get => name; set => name = value; }
        public double Version { get => version; set => version = value; }
        public long Size { get => size; set => size = value; }
        public string Run { get => run; set => run = value; }
        public string Delete { get => delete; set => delete = value; }
        public string Upload { get => upload; set => upload = value; }
        public string Download { get => download; set => download = value; }
        public string Hide { get => hide; set => hide = value; }
        public string Unhide { get => unhide; set => unhide = value; }
        public string Spublic { get => spublic; set => spublic = value; }
        public string Sprivate { get => sprivate; set => sprivate = value; }
        public string Encrypt { get => encrypt; set => encrypt = value; }
        public string Decrypt { get => decrypt; set => decrypt = value; }





        //private static void LoadBaseTable()
        //{
        //    string constr = @"Data Source=.\sqlexpress;Initial Catalog=hacker_project;Integrated Security=True";
        //    SqlConnection conn = new SqlConnection(constr);

        //    try
        //    {

        //        string q = "select * from software";
        //        SqlCommand cmd = new SqlCommand(q, conn);
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);

        //        adapter.Fill(baseTable);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DHCY_server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = "select * from goods order by level DESC;";
            DataTable dt= MysqlHelper.ExecuteDataTable(str);
            dataGridView1.DataSource = dt;
            List<box_in_pro> boxs = new List<box_in_pro>();
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                box_in_pro box = new box_in_pro();
                box.set_all(row[0].ToString(), System.Int32.Parse(row[1].ToString()), System.Int32.Parse(row[2].ToString()), System.Int32.Parse(row[4].ToString()), row[3].ToString());
                boxs.Add(box);
     
            }
            algorithm_lb.algorithm_lb_main(ref boxs, false);
            //MessageBox.Show(out_txt.file_path);
            //string str1 = "insert into goods (id,start,end,weight,level) VALUES('BJ',1,2,3,4);";
            //int k = MysqlHelper.ExecuteNonQuery(str1);
            //MessageBox.Show(k.ToString());
            //k = MysqlHelper.CloseConnection();
            //MessageBox.Show(k.ToString());
        }
    }
}

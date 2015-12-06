using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DHCY_server
{
    class out_txt
    {
        public static void select_f(ref string file_path)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath == string.Empty)
            {
                // System.Windows.Forms.MessageBox.Show("请选择输出文件要保存的文件夹");
            }
            else
            {
                // /*****************转到输出函数中去******************
                file_path = fbd.SelectedPath;
            }
        }
        public static void out_txt_main()
        {
            string file_path = null;

            select_f(ref file_path);

             //创建一个TXT文件，用于保存数据
            if (File.Exists(file_path + "\\result.txt"))
                File.Delete(file_path + "\\result.xlsx");
            //创建一个txt文件
            FileStream fout =
                   new FileStream(file_path + "\\result.txt", FileMode.Create, FileAccess.ReadWrite);
            var stream = new StreamWriter(fout);

            //输出记录数目为64×站数，每次都是按照站进行排列

            string box_number = null;
            string up_box_number = null;

            for (int i = 0; i < algorithm_lb.rail_waystation.Count; i++)
            {
                //64代表有64个集装箱卡槽数目
                for (int j = 0; j < 64; j++)
                {
                    box_number = algorithm_lb.all_list[i][j].box_number;
                    up_box_number = algorithm_lb.all_list[i][j].up_box_number;

                    if (box_number == null)
                    {
                        box_number = "NO";
                    }

                    if (up_box_number == null)
                    {
                        up_box_number = "NO";
                    }

                    stream.WriteLine((i+1)+","+(j+1)+","+
                        box_number+","+
                        algorithm_lb.all_list[i][j].has_box_down + "," +
                        algorithm_lb.all_list[i][j].is_EM+","+
                        algorithm_lb.all_list[i][j].has_box_up+","+
                        up_box_number
                        );
                }
            }

            stream.Close();
            fout.Close();
        }
    }
}

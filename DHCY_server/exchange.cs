using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DHCY_server
{
    //数据内部的存储格式单元，
    //将车的站数和下车的站数进行转换
    public class box_in_pro {
        public string box_number;   //编号
        public int box_from;        //出发站
        public int box_destation;   //目的站
        public int box_is_EM;       //是否是紧急件
        public string box_weight;   //集装箱的质量
        public int box_is_selected;
        public box_in_pro()
        {
            //设置没有被选择
            box_is_selected = 0;
        }

        public void set_all(string number, int form, int des, int EM, string wei)
        {
            box_number = number;
            box_from = form;
            box_destation = des;
            box_is_EM = EM;
            box_weight = wei;
        }
    }

    
}

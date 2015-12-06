using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DHCY_server
{
    //基本链表信息结构体
    public class train_unio {
       public string box_number;        //集装箱编号
       public int train_x;
       public int train_y;
       public bool has_a_box;           //此位置有集装箱
       public bool has_box_up;          //此战有集装箱上车
       public bool has_box_down;        //此战有集装箱下车
       public string up_box_number;     //此战上车的集装箱编号
       public bool is_EM;               //是否是紧急件
       public train_unio()
       {
           train_x = 0;
           train_y = 0;
           has_a_box = false;
           has_box_up = false;
           has_box_down = false;
           is_EM = false;
       }
    }
    //空白处查找
    public class trian_blank {
        public int train_no;            //列车的列号
        public int start;               //行起始
        public int end;                 //行终止
    }
    //单列替换类
    public class trian_take_place
    {
        public int value;               //价值
        public int tain_no;             //车的编号
        public int start;               //替换的起始
        public int end;                 //替换的结束
    }
    //双行替换类
    public class train_take_double
    {
        public trian_take_place insert_place;
        public trian_take_place clear_place;
        public int sum_value;
    }
    //创建20个向量
    class algorithm_lb
    {
 

       public static List<string> rail_waystation =new List<string>();
      
 
        public static List<train_unio>[] all_list;
        //初始化20个向量
        public static void init_vector()
        {  
            //先申明站数个向量
            for (int i = 0; i < 9; i++)
            {
                rail_waystation.Add("北京西");
                rail_waystation.Add("石家庄");
                rail_waystation.Add("邢台东");
            }
            //rail_waystation.Add("运城");
            all_list = new List<train_unio>[rail_waystation.Count];


            for (int i = 0; i < rail_waystation.Count; i++)
            {
                List<train_unio> temp = new List<train_unio>();
                all_list[i] = temp;
                for (int j = 0; j < 64; j++)
                {
                    train_unio temp1 = new train_unio();
                    all_list[i].Add(temp1);
                }
            }
            
        }
        //调度维护类
        public class algotithm_b
        {
            int i;
            int j;
            int train_no;
            int N;
            //设置全部数字代码
            public void set_ijtN(int a1, int a2, int a3, int a4)
            {
                i = a1;
                j = a2;
                train_no = a3;
                N = a4;
            }
            //i++操作
            public void iplus1()
            {
                i++;
            }
            //read i
            public int readi()
            {
                return i;
            }
            //j++操作
            public void jplus1()
            {
                j++;
            }
            //read j
            public int readj()
            {
                return j;
            }
            //train_no++操作
            public void trainplus1()
            {
                train_no++;
            }
            //read train_no操作
            public int read_t()
            {
                return train_no;
            }
            //设置train_no操作
            public void set_trian_no(int a)
            {
                train_no = a;
            }
            //N++操作
            public void Nplus1()
            {
                N++;
            }
            //read N
            public int readN()
            {
                return N;
            }
            //设置 N的值
            public void setN(int a)
            {
                N = a;
            }
        }

        //主调度代码
        public static void algorithm_lb_main(
            ref List<box_in_pro> pro_box_list,
            bool is_test)
        {
            /*, "新乡东",
                                          "郑州东","漯河西", "驻马店西", "武汉" ,"咸宁北",
                                          "岳阳东","长沙南","衡阳东","祁东","祁阳",
                                           "永州","东安东","全州南","兴安北","桂林"};*/
            //初始输出向量
            init_vector();
            //控制变量数据结构
            algotithm_b control_data = new algotithm_b();
            control_data.set_ijtN(0,rail_waystation.Count-1,1,1);
            //存储选择的集装箱
            List<box_in_pro> selected_list = new List<box_in_pro>();
            selected_box(ref pro_box_list,
                ref selected_list, ref control_data);   //选择集装箱

            while (control_data.readi() != rail_waystation.Count-1)          //大程序的出口
            {
                while (control_data.readN()!= 5)        //N的出口
                {
                    while (control_data.read_t() != 9)  //Train_no的出口
                    {
                        if (selected_list.Count != 0)
                        {
                            insert_Nbox(ref control_data, ref selected_list,
                                ref pro_box_list);          //N区插入集装箱
                            control_data.trainplus1();      //Train_no++
                        }
                        else
                        {
                            goto add_box;
                        }
                    }
                    control_data.set_trian_no(1);           //trian_no重新归1
                    control_data.Nplus1();                  //N++
                }
                control_data.setN(1);                       //N归1
                add_box: control_data.iplus1();             //i++
                int sum = selected_list.Count;
                for (int i = 0; i < sum; i++)               //清除未选择的集装箱
                {
                    selected_list.RemoveAt(selected_list.Count - 1);
                }
                selected_box(ref pro_box_list,
                       ref selected_list, ref control_data); //选择集装箱
            }
            //56区代码插入
            algorithm_door(ref pro_box_list);
            //紧急件处理
            algorithm_noliner(ref pro_box_list);
            //紧急件二次处理
            algorithm_last_try(ref pro_box_list);
            if(!is_test)
            {
               // app_output.app_out(ref all_list);

                //输出为txt的格式，一个格式就是一行
                out_txt.out_txt_main();
            }
        }

       
        //选择集装箱,每次选择出发站一样的集装箱
        public static void selected_box(
            ref List<box_in_pro> pro_box_list,
            ref List<box_in_pro> selected_list,
            ref algotithm_b control_data)
        {
            for (int i = 0; i < pro_box_list.Count(); i++)
            {
                //满足要求，放入选择链表中
                if (pro_box_list[i].box_from== control_data.readi() &&
                    pro_box_list[i].box_is_selected == 0)
                {
                    selected_list.Add(pro_box_list[i]);
                }
            }
        }

        //在trian_no,N区，j行是否被占用,站用返回-1，正常返回0，这个版本将不再使用
        public static int test_has_box(ref algotithm_b control_data)
        {
            int error = 0;
            int all_trian_no = 0;        //计算在车上的位置
            int vector_number = 0;       //计算要判断的列数
            int start1 = control_data.readi();   //判断起始行数
            int end1 = control_data.readj();     //判断终止行数                 
            //根据参数计算要判断的列数,计算要进行判断的编号
            all_trian_no = find_alltrain_no(ref control_data,ref vector_number);

            //进行判断
            for (int j = start1; j <= end1; j++)
            {
                if (vector_number == 2)
                {
                    //判断
                    if (all_list[j][all_trian_no].has_a_box)
                    {
                        error = -1;
                    }
                    if (all_list[j][all_trian_no + 1].has_a_box)
                    {
                        error = -1;
                    }
                }
                else if (vector_number == 1)
                {
                    if (all_list[j][all_trian_no].has_a_box)
                    {
                        error = -1;
                    }
                }
            }
            return error;
        }


        //返回列车所在的号码,并计算要判断的集装箱数目
        public static int find_alltrain_no(ref algotithm_b control_data, 
            ref int vector_number)
        {
            int all_trian_no = 0;
            switch (control_data.readN())
            {
                case 1: vector_number = 2; all_trian_no = 8 * control_data.read_t() - 8 + 2+1; break;
                case 2: vector_number = 1; all_trian_no = 8 * control_data.read_t() - 8 + 4; break;
                case 3: vector_number = 2; all_trian_no = 8 * control_data.read_t() - 8 + 6+1; break;
                case 4: vector_number = 1; all_trian_no = 8 * control_data.read_t() - 8 + 0; break;
                case 5: vector_number = 1; all_trian_no = 8 * control_data.read_t() - 8 + 1; break;
                case 6: vector_number = 1; all_trian_no = 8 * control_data.read_t() - 8 + 5; break;
            }
            return all_trian_no;
        }


        //装填集装箱,假如集装箱被装填了，那么需要在原始序列中装填它
        //现在已经不再使用这个函数，已经被改进版取代了
        public static void fill_vector(ref algotithm_b control_data,
             ref List<box_in_pro> selected_list,
             ref List<box_in_pro> pro_box_list)
        {
            int all_trainno = 0;
            int temp = 0;
            //获取集装箱位置
            all_trainno = find_alltrain_no(ref control_data, ref temp);
            //
        }


        //插入集装箱,某个区插入的计算
        public static int insert_Nbox(ref algotithm_b control_data,
            ref List<box_in_pro> selected_list,
            ref List<box_in_pro> pro_box_list)
        {
            int error_no = 0;
            int all_trainno = 0;          //所在列数
            int temp = 0;                 //判断某一区有几个位置
            all_trainno = find_alltrain_no(ref control_data, ref temp);

            int min_up_board = 0;         //计算集装箱的最小下届
            int max_down_board = 0;       //计算集装箱的最大上界

            int up_board = 0;             //上界
            int down_board = 0;           //下界
            //检测上下界
            find_up_down(all_trainno,ref min_up_board,ref max_down_board,
                ref control_data);
            for (int i = 0; i < temp; i++)
            {
                up_board = min_up_board;
                down_board = max_down_board;
                int count = -1;              //选择计数器
                while (up_board != down_board)
                {
                    //查找是否有满足条件的集装箱
                    for (int j = 0; j < selected_list.Count; j++)
                    {
                        if (up_board == selected_list[j].box_from &&
                            down_board == selected_list[j].box_destation)
                        {
                            count = j;//保存计数器
                            break;    //找到跳出循环
                        }
                    }
                    if (count != -1)
                    {
                        //找到了，跳出循环
                        break;
                    }
                    //没找到，那么下界减去1
                    down_board--;
                }
                if (up_board == down_board)
                {
                    return -1;          //下界等于上界，说明错误
                }
                if (count != -1)
                {
                    insert_one_box(all_trainno, up_board, down_board,
                        selected_list[count], ref pro_box_list);    //标记集装箱
                    selected_list.RemoveAt(count);                  //删除已经选择的集装箱
                }
                max_down_board = down_board;                        //下届计数器更新
                all_trainno--;  //计数器减1
            }
             return error_no;
        }


        //查找上下届的函数,总整体控制来说，为了做成线性的函数，只是起到了检测的作用
        public static void find_up_down(int trian_no,ref int up,ref int down
            , ref algotithm_b control_data)
        {
            //默认值设置
            up = control_data.readi();
            down = rail_waystation.Count - 1;

            //对于向下生长的函数，那么下届就是rail_waystation.Count-1
            for (int i =control_data.readi(); i <rail_waystation.Count-1;
                i++)
            {
                if (all_list[i][trian_no].has_a_box)
                {
                    //有集装箱，那么就是下一个
                    up = i + 1;
                }
            }
        }
        //直接插入，参数为，位置，上界，下届，选择的box_in_pro的单元
        //插入完成之后，再原始数据矩阵中标记
        public static void insert_one_box(int place,int up,int down,
            box_in_pro temp_box, ref List<box_in_pro> pro_box_list)
        {
            for (int i = up; i <= down; i++)
            {
                if (temp_box.box_is_EM==1)
                {
                    all_list[i][place].is_EM = true;
                }
                if (i == up)                                                //头站设置
                {
                    //约定为本站到下一站占用则为有集装箱
                    all_list[i][place].has_box_up = true;                   //有集装箱要上车
                    all_list[i][place].has_a_box = true;                    //有集装箱站用
                    all_list[i][place].up_box_number = temp_box.box_number;
                }
                else if (i == down)
                {
                    all_list[i][place].has_box_down = true;                 //有集装箱下车
                    all_list[i][place].box_number = temp_box.box_number;    //集装箱下车编号
                }
                else {
                    all_list[i][place].has_a_box = true;                    //有集装箱
                    all_list[i][place].box_number = temp_box.box_number;    //集装箱编号
                }
            }
            //标记集装箱
            for (int i = 0; i < pro_box_list.Count; i++)
            {
                if (pro_box_list[i].box_number == temp_box.box_number)
                {
                    pro_box_list[i].box_is_selected = 1;
                    break;
                }
            }
        }

        //门区域插入，有两个门，门区域5和门区域6
        public static void algorithm_door(
            ref List<box_in_pro> pro_box_list)
        {
            algotithm_b control_data = new algotithm_b();
            control_data.set_ijtN(0, rail_waystation.Count - 1, 1, 5);
            //存储选择的集装箱
            List<box_in_pro> selected_list = new List<box_in_pro>();
            selected_box(ref pro_box_list,
                ref selected_list, ref control_data);   //选择集装箱

            while (control_data.readi() != rail_waystation.Count - 1)  //大程序的出口
            {
                while (control_data.readN() != 7)        //N的出口
                {
                    while (control_data.read_t() != 9)  //Train_no的出口
                    {
                        if (selected_list.Count != 0)
                        {
                            //5，6区的集装箱插入
                            insert_N56(ref control_data,ref selected_list,
                                ref pro_box_list);
                            control_data.trainplus1();      //Train_no++
                        }
                        else
                        {
                            goto add_box;
                        }
                    }
                    control_data.set_trian_no(1);           //trian_no重新归1
                    control_data.Nplus1();                  //N++
                }
                control_data.setN(5);                       //N归1
            add_box: control_data.iplus1();                 //i++
                int sum = selected_list.Count;
                for (int i = 0; i < sum; i++)  //清除未选择的集装箱
                {
                    selected_list.RemoveAt(selected_list.Count - 1);
                }
                selected_box(ref pro_box_list,
                       ref selected_list, ref control_data); //选择集装箱
            }
        }
        //56区集装箱插入
        public static int insert_N56(ref algotithm_b control_data,
            ref List<box_in_pro> selected_list,
            ref List<box_in_pro> pro_box_list)
        {
            int error_no = 0;
            int all_trainno = 0;          //所在列数
            int temp = 0;                 //判断某一区有几个位置
            all_trainno = find_alltrain_no(ref control_data, ref temp);

            int min_up_board = 0;         //计算集装箱的最小下届
            int max_down_board = 0;       //计算集装箱的最大上界

            int up_board = 0;             //上界
            int down_board = 0;           //下界
            //检测上下界
            test(ref min_up_board,ref max_down_board,
                all_trainno,control_data);
            up_board = min_up_board;
            down_board = max_down_board;
            int count = -1;              //选择计数器
            while (up_board != down_board)
            {
               //查找是否有满足条件的集装箱
               for (int j = 0; j < selected_list.Count; j++)
               {
                   if (up_board == selected_list[j].box_from &&
                      down_board == selected_list[j].box_destation)
                     {
                            count = j;//保存计数器
                            break;    //找到跳出循环
                      }
               }
               if (count != -1)
               {
                  //找到了，跳出循环
                  break;
               }
               //没找到，那么下界减去1
               down_board--;
           }
           if (up_board == down_board)
           {
               return -1;          //下界等于上界，说明错误
           }
           if (count != -1)
           {
                insert_one_box(all_trainno, up_board, down_board,
                    selected_list[count], ref pro_box_list);    //标记集装箱
                    selected_list.RemoveAt(count);              //删除已经选择的集装箱
           }
           max_down_board = down_board;                         //下届计数器更新
           return error_no;
        }
        //检测上下界，中间的集装箱不能阻挡上车和下车
        public static void test(ref int up,
            ref int down, int train_no, 
            algotithm_b control_data)
        { 
            //先检测上界
            up = control_data.readi();
            int k = train_no;
            int left_down = rail_waystation.Count - 1;  //左边底界最大值
            int right_down = rail_waystation.Count - 1; //右边底界最大值

            //下面的是为了修复重大bug而加上的约束条件
            int right_right_dowm = rail_waystation.Count - 1; //右右边底界最大值

            //确定上界，可以证明这个算法是正确的,下面这个循环是为了检测这个集装箱是否适合
            //第一次使用为正确，下面的都是正确的
            for (int i = up; i < rail_waystation.Count - 1; i++)
            {
                if (all_list[i][k].has_a_box)
                {
                    //检测到有集装箱出错，直接返回
                    down = rail_waystation.Count - 1;
                    up = rail_waystation.Count - 1;
                    return;
                }
            }
            //确定左下界
            for (int i = up; i < rail_waystation.Count - 1; i++)
            {
                if (i == 0)//消除同站上车的问题
                {
                    if (all_list[i][k - 1].has_box_down)
                    {
                        left_down = i;
                        break;
                    }
                }
                else
                {
                    if (all_list[i][k - 1].has_box_down ||
                       all_list[i][k - 1].has_box_up)
                    {
                        left_down = i;
                        break;
                    }
                }
            }
            //确定右下界
            for (int i = up; i < rail_waystation.Count - 1; i++)
            {
                if (i == up)//消除同站上车的问题
                {
                    if (all_list[i][k + 1].has_box_down)
                    {
                        right_down = i;
                        break;
                    }
                }
                else
                {
                    if (all_list[i][k + 1].has_box_down||
                        all_list[i][k + 1].has_box_up)
                    {
                        right_down = i;
                        break;
                    }
                }
            }
            //确定右右下届
            for (int i = up; i < rail_waystation.Count - 1; i++)
            {
                if (i == up)//消除同站上车的问题
                {
                    if (all_list[i][k + 2].has_box_down)
                    {
                        right_right_dowm = i;
                        break;
                    }
                }
                else
                {
                    if (all_list[i][k + 2].has_box_down ||
                        all_list[i][k + 2].has_box_up)
                    {
                        right_right_dowm = i;
                        break;
                    }
                }
            }
            //选择比较小的那个
            if (left_down > right_down)
            {
                left_down = right_down;
                if (left_down > right_right_dowm)
                {
                    left_down = right_right_dowm;
                }
            }
            down = left_down;
        }

        //加急件插入,这里是加急件的入口
        public static void algorithm_noliner(
            ref List<box_in_pro> pro_box_list)
        {
            algotithm_b control_data = new algotithm_b();
            control_data.set_ijtN(0, rail_waystation.Count - 1, 1, 1);         //要采用全部遍历的方法
            //存储选择的集装箱
            List<box_in_pro> selected_list = new List<box_in_pro>();
            //选择集装箱
            select_em_box(ref pro_box_list,ref selected_list);
            /* 这段代码，从逻辑上来说，和前面的遍历法一样，所以将不再使用
            for (int i = 0; i < 64; i++)
            {
                List<trian_blank> blank_list=new List<trian_blank>();
                find_train_blank(ref blank_list,i);//寻找空白处
            }
             */
            while (selected_list.Count != 0)
            {
                int i = selected_list.Count - 1;

                List<trian_take_place> my_list = new List<trian_take_place>();
                //判断约束和计算代价
                find_colum_vlaue(ref my_list, selected_list[i]);
                int min = 1000;
                int k = 0;
                for (int j = 0; j < my_list.Count; j++)
                {
                    if (min > my_list[j].value)
                    {
                        min = my_list[j].value;
                        k = j;
                    }
                }
                //进行替代的工作,1000这个数是
                //计算价值的时候不可以替换的价值
                if (min != 1000)
                {
                    take_place_box(selected_list[i],
                        my_list[k], ref pro_box_list);
                }

                //不管是否成功，都要将这个删除
                selected_list.RemoveAt(i);
            }
        }

        //加急件的最后尝试
        public static void algorithm_last_try(
            ref List<box_in_pro> pro_box_list)
        { 
            //采用两个区计价方法
            //只计算1，4，5，8区
            //代价比较大
            algotithm_b control_data = new algotithm_b();
            control_data.set_ijtN(0, rail_waystation.Count - 1, 1, 1);         //只是遍历其中的一半
            //存储选择的集装箱
            List<box_in_pro> selected_list = new List<box_in_pro>();
            //选择加急件集装箱
            select_em_box(ref pro_box_list, ref selected_list);

            while (selected_list.Count != 0)
            {
                int i = selected_list.Count - 1;

                //生成一个链表
                List<train_take_double> my_list =
                    new List<train_take_double>();
                count_double_value(ref my_list, selected_list[i]);
                //找到最小代价的
                int min = 1000;
                int k = 0;
                for (int j = 0; j < my_list.Count; j++)
                {
                    if (min > my_list[j].sum_value)
                    {
                        min = my_list[j].sum_value;
                        k = j;
                    }
                }
                //插入和清除工作
                if (min < 1000)
                {
                    take_place_box(selected_list[i],
                        my_list[k].insert_place, ref pro_box_list);
                    clear_place_box(selected_list[i],
                        my_list[k].clear_place, ref pro_box_list);

                }
                //不管是否成功，都要将这个删除
                selected_list.RemoveAt(i);
            }
        }

        //计算所有的搜寻代价，太长了，拆分成两个同样的函数，等待优化，3，7未完成
        public static void count_double_value(
            ref List<train_take_double> my_list,
          box_in_pro temp_box)
        {
            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                {
                    //左边
                    count_right(ref my_list,temp_box,i);
                }
                else if (i % 8 == 3)
                {
                    //右边
                    // count_left(ref my_list, temp_box, i);
                }
                else if (i % 8 == 4)
                {
                    //左边
                    count_right(ref my_list, temp_box, i);
                }
                else if (i % 8 == 7)
                {
                    //右边
                    //count_left(ref my_list, temp_box, i);
                }
            }
        }

        //给左边定位
        public static void count_right(
          ref List<train_take_double> my_list,
          box_in_pro temp_box,
            int i)
        {
            //要加入的集装箱
            train_take_double add_temp =
                new train_take_double();

            int up = temp_box.box_from;
            int down = temp_box.box_destation;
            //要插入的集装箱处理
            trian_take_place temp1_box =
               new trian_take_place();
            temp1_box.value = cla_value(i, ref up, ref down);
            temp1_box.end = down;           //下界
            temp1_box.start = up;           //上界
            temp1_box.tain_no = i;          //车位号

            //第二列处理
            up = temp_box.box_from;
            down = temp_box.box_destation;

            trian_take_place temp2_box =
                new trian_take_place();
            temp2_box.value = cla_value(i + 1, ref up, ref down);
            temp2_box.end = down;           //下届
            temp2_box.start = up;           //上界
            temp2_box.tain_no = i+1;        //车位号

            add_temp.insert_place = temp1_box;
            add_temp.clear_place = temp2_box;
            add_temp.sum_value = temp1_box.value + temp2_box.value;
            // cla_value(); 
            my_list.Add(add_temp);
        }

        //给右边定位
        public static void count_left(
         ref List<train_take_double> my_list,
         box_in_pro temp_box,
           int i)
        {
            //要加入的集装箱
            train_take_double add_temp =
                new train_take_double();

            int up = temp_box.box_from;
            int down = temp_box.box_destation;
            //要插入的集装箱处理
            trian_take_place temp1_box =
               new trian_take_place();
            temp1_box.value = cla_value(i, ref up, ref down);
            temp1_box.end = down;
            temp1_box.start = up;
            temp1_box.tain_no = i;          //车位号
            //第二列处理
            up = temp_box.box_from;
            down = temp_box.box_destation;

            trian_take_place temp2_box =
                new trian_take_place();
            temp2_box.value = cla_value(i - 1, ref up, ref down);
            temp2_box.end = down;
            temp2_box.start = up;
            temp2_box.tain_no = i - 1;        //车位号

            add_temp.insert_place = temp1_box;
            add_temp.clear_place = temp2_box;
            add_temp.sum_value = temp1_box.value + temp2_box.value;
            // cla_value(); 
            my_list.Add(add_temp);
        }


        //加急件集装箱的选择,将之前为选择的加急件全部选择进入
        public static void select_em_box(
            ref List<box_in_pro> pro_box_list,
            ref List<box_in_pro> selected_list)
        {
            for (int i = 0; i < pro_box_list.Count(); i++)
            {
                //满足要求，放入选择链表中
                if (pro_box_list[i].box_is_EM==1&&
                    pro_box_list[i].box_is_selected == 0)
                {
                    selected_list.Add(pro_box_list[i]);
                }
            }
        }

        //寻找一列的空白区域,不在使用这种算法
        public static void find_train_blank(
            ref List<trian_blank> blank_list,
            int train_no)
        {
            int start = 0;  //起始
            //遍历，查找各段
            for (int i = 0; i < 20; i++)
            {
                //区位装填
                if (all_list[i][train_no].has_box_up&&i!=0
                    && !all_list[i][train_no].has_box_down||
                    !all_list[i][train_no].has_a_box&&i==19)
                {
                    trian_blank temp = new trian_blank();
                    //申请一个新的变量
                    temp.start = start;
                    temp.end = i;
                    temp.train_no = train_no;
                    blank_list.Add(temp);
                }
                if (all_list[i][train_no].has_box_down&&
                    !all_list[i][train_no].has_box_up)
                {
                    start = i;
                }
            }
        }

        //遍历列车，寻找一样的上下界的空位，
        //并且能够满足上下车的条件，如果找到就将位置返回
        //新算法不再使用这种方法
        public static int find_train_empty(int up,int down)
        {
            int error = -1;
            for (int train_no = 0; train_no < 64; train_no++)
            {
               //首先检查是否是空的
               if (check_empty(up, down, train_no))
               {
                   //检查上下车的条件,满足则装填
                   if (check_left_right(up,down,train_no))
                   {
                       //返回可以插入的位置
                       return train_no;
                   }
               }
            }
            return error;
        }
        //检测某一段区间内是否有集装箱,不再使用
        public static bool check_empty(int up, int down,
            int train_no)
        {
            bool result = true;
            for ( int i = up; i<=down;i++)
            {
                if (all_list[i][train_no].has_a_box)
                {
                    return false;
                }
            }
            return result;
        }

        //检测左边和右边能不能满足上下车的条件
        //不能使自己上不了车或者下不了车
        public static bool check_left_right(int up, int down,
            int train_no)
        {
            bool error = false;
            //由于车厢是非线性的，那么只能分段判断
            if (train_no % 8 == 0 || train_no % 8 == 4)
            {
                error = check_0_4(up,down,train_no);
            }
            else if (train_no % 8 == 1 || train_no % 8 == 5)
            {
                error = check_1_5(up, down, train_no);
            }
            else if (train_no % 8 == 2||train_no%8==6)
            {
                //3号位置和6号位置
                error = check_2_6(up, down, train_no);
            }
            else if (train_no % 8 == 3||train_no % 8==7)
            {
                error = check_3_7(up, down, train_no);
            }
            return error;
        }

        //原来那个check太长了，现在拆分成四个
        public static bool check_0_4(int up, int down,
            int train_no)
        {
            //1号位置和5号位置约束检查
            bool fail1 = false;
            if ((all_list[up][train_no + 1].has_box_up ||
               !all_list[up][train_no + 1].has_a_box) &&
                (!all_list[down][train_no + 1].has_a_box ||
                all_list[down][train_no + 1].has_box_down))
            {
                fail1 = true;
            }
            return fail1;
        }

        public static bool check_1_5(int up, int down,
            int train_no)
        {
            //2号位置与6号位置约束检查
            bool fail1 = false;
            bool fail2 = false;
            bool fail3 = false;
            //检查左边的约束条件（先上，同上），（后下，同下）
            if ((all_list[up][train_no - 1].has_box_up ||
               all_list[up][train_no - 1].has_a_box) &&
                (all_list[down][train_no - 1].has_a_box ||
                all_list[down][train_no - 1].has_box_down))
            {
                fail1 = true;
                //新插入的要满足左右两边都不能有上下车的情况
                for (int i = up; i <down; i++)
                {
                    if (all_list[i][train_no - 1].has_box_down)
                    {
                        fail1 = false;
                        break;
                    }
                }
            }
            else //检查为空的情况
            {
                bool temp_fail = true;
                for (int i = up; up <= down; i++)
                {
                    if (!all_list[i][train_no - 1].has_a_box)
                    {
                        temp_fail = false;
                        break;
                    }
                }
                fail1 = temp_fail;
            }
            //检查右边的约束条件（先上，同上），（后下，同下）
            if ((all_list[up][train_no + 1].has_box_up ||
               all_list[up][train_no + 1].has_a_box) &&
                (all_list[down][train_no + 1].has_a_box ||
                all_list[down][train_no + 1].has_box_down))
            {
                fail2 = true;
                //新插入的要满足左右两边都不能有上下车的情况
                for (int i = up; i < down; i++)
                {
                    if (all_list[i][train_no + 1].has_box_down)
                    {
                        fail2 = false;
                        break;
                    }
                }
            }
            else //检查为空的情况
            {
                bool temp_fail = true;
                for (int i = up; up <= down; i++)
                {
                    if (!all_list[i][train_no + 1].has_a_box)
                    {
                        temp_fail = false;
                        break;
                    }
                }
                fail2 = temp_fail;
            }

            //检查右边的约束条件（先上，同上），（后下，同下）
            if ((all_list[up][train_no + 2].has_box_up ||
               all_list[up][train_no + 2].has_a_box) &&
                (all_list[down][train_no + 2].has_a_box ||
                all_list[down][train_no + 2].has_box_down))
            {
                fail2 = true;
                //新插入的要满足左右两边都不能有上下车的情况
                for (int i = up; i < down; i++)
                {
                    if (all_list[i][train_no + 2].has_box_down)
                    {
                        fail2 = false;
                        break;
                    }
                }
            }
            else //检查为空的情况
            {
                bool temp_fail = true;
                for (int i = up; up <= down; i++)
                {
                    if (!all_list[i][train_no + 2].has_a_box)
                    {
                        temp_fail = false;
                        break;
                    }
                }
                fail3 = temp_fail;
            }
            return fail1 && fail2 && fail3;
        }

        
        public static bool check_2_6(int up, int down,
            int train_no)
        {
            bool fail0 = false;
            bool fail1 = false;
            bool fail2 = false;
            if ((all_list[up][train_no - 2].has_box_up ||
              !all_list[up][train_no - 2].has_a_box) &&
               (!all_list[down][train_no - 2].has_a_box ||
               all_list[down][train_no - 2].has_box_down))
            {
                fail0 = true;
            }
            //检查左边的约束条件
            if ((all_list[up][train_no - 1].has_box_up ||
               !all_list[up][train_no - 1].has_a_box) &&
                (!all_list[down][train_no - 1].has_a_box ||
                all_list[down][train_no - 1].has_box_down))
            {
                fail1 = true;
            }
            //检查右边的约束条件（先上，同上），（后下，同下）
            if ((all_list[up][train_no + 1].has_box_up ||
               all_list[up][train_no + 1].has_a_box) &&
                (all_list[down][train_no + 1].has_a_box ||
                all_list[down][train_no + 1].has_box_down))
            {
                fail2 = true;
                //新插入的要满足左右两边都不能有上下车的情况
                for (int i = up; i < down; i++)
                {
                    if (all_list[i][train_no + 1].has_box_down)
                    {
                        fail2 = false;
                        break;
                    }
                }
            }
            else //检查为空的情况
            {
                bool temp_fail = true;
                for (int i = up; i <= down; i++)
                {
                    if (!all_list[i][train_no + 1].has_a_box)
                    {
                        temp_fail = false;
                    }
                    fail2 = temp_fail;
                }
            }
            return fail1 && fail2 && fail0;
        }

        public static bool check_3_7(int up, int down,
            int train_no)
        {
            bool fail0 = false;
            bool fail1 = false;
            if ((all_list[up][train_no - 2].has_box_up ||
              !all_list[up][train_no - 2].has_a_box) &&
               (!all_list[down][train_no - 2].has_a_box ||
               all_list[down][train_no - 2].has_box_down))
            {
                fail0 = true;
            }
            if ((all_list[up][train_no - 1].has_box_up ||
               !all_list[up][train_no - 1].has_a_box) &&
                (!all_list[down][train_no - 1].has_a_box ||
                all_list[down][train_no - 1].has_box_down))
            {
                fail1 = true;
            }
            return fail1 && fail0;
        }

        //找合适的集装箱替换，能够确保上车
        public static int fin_train_box(int up, int down)
        {
            for (int i = 0; i < 64; i++)
            { 
                //转到约束条件检查的函数中
                check_trian_box(up,down,i);
            }
            return -1;
        }
        //检查约束条件
        public static bool check_trian_box(int up, int down,
            int train_no)
        {

            return false;
        }
        //代价搜寻函数，如果不可以替换，那么就返回1000
        public static void find_colum_vlaue(
            ref List<trian_take_place> my_list,
            box_in_pro temp_box)
        {
            for (int i = 0; i < 64; i++)
            {
                //检查是否满足约束条件
                if (check_left_right(temp_box.box_from,
                    temp_box.box_destation, i))
                {
                    //计算要撤除的集装箱范围
                    int up = temp_box.box_from;
                    int down = temp_box.box_destation;
                    //上下届，价值计划
                    trian_take_place temp = new trian_take_place();
                    temp.value = cla_value(i, ref up, ref down);
                    temp.start = up;            //上界
                    temp.end = down;            //下届
                    temp.tain_no = i;           //编号
                    my_list.Add(temp);
                }
                else
                {
                    trian_take_place temp = new trian_take_place();
                    temp.value = 1000;
                    my_list.Add(temp);
                }
            }
        }
        //撤除集装箱的计算范围,这个函数可以优化
        //原有的是紧急件没有解决,新版本解决了
        public static int cla_value(int trian_no,
            ref int up,ref int down)
        {
            bool loop_var = true;
            //处理上界
            int new_value = down - up;
            while (loop_var)
            {
                //没有集装箱，那么说明这个就是上界
                //直接跳出循环
                if (!all_list[up][trian_no].has_a_box)
                {
                    loop_var = false;
                    break;
                }
                //有集装箱上车，那么就是找到了上界
                //找到了直接跳出循环
                if (all_list[up][trian_no].has_box_up)
                {
                    loop_var = false;
                    break;
                }
                up--;
            }
            //处理下界
            loop_var = true;
            while(loop_var)
            {
                if (!all_list[down][trian_no].has_a_box)
                {
                    loop_var = false;
                    break;
                }
                //有集装箱上车，那么就是找到了上界
                //找到了直接跳出循环
                if (all_list[down][trian_no].has_box_down)
                {
                    loop_var = false;
                    break;
                }
                down++;
            }
            //检测上下届是否有加急件，是的话返回1000
            for (int i = up; i <= down; i++)
            {
                if (all_list[i][trian_no].is_EM)
                {
                    new_value = 1000;
                    return new_value;
                }
            }
                new_value = down - up;
            return new_value;
        }

        //重新标记某一个集装箱，供集装箱擦除函数使用
        public static void re_mark(string box_number, 
            ref List<box_in_pro> pro_box_list)
        {
            for (int i = 0; i < pro_box_list.Count; i++)
            {
                if (pro_box_list[i].box_number == box_number)
                {
                    pro_box_list[i].box_is_selected = 0;
                    break;
                }
            }
        }

        //取代集装箱
        public static void take_place_box(box_in_pro temp_box,
            trian_take_place temp, 
            ref List<box_in_pro> pro_box_list)
        {
            //先全部清理
            for (int i = temp.start; i <= temp.end; i++)
            {
                if (i == temp.start)
                {
                    //起始点不清理有集装箱下车标志
                    all_list[i][temp.tain_no].has_a_box = false;
                    //all_list[i][temp.tain_no].box_number = null;
                    all_list[i][temp.tain_no].is_EM = false;
                    //重新标记集装箱
                    re_mark(all_list[i][temp.tain_no].up_box_number,ref pro_box_list);
                    all_list[i][temp.tain_no].up_box_number = null;
                    all_list[i][temp.tain_no].has_box_up= false;
                }
                else if (i == temp.end)
                {
                    //终点不清理上车标志
                    all_list[i][temp.tain_no].has_a_box = false;
                    //重新标记集装箱
                    re_mark(all_list[i][temp.tain_no].box_number, ref pro_box_list);
                    all_list[i][temp.tain_no].box_number = null;
                    all_list[i][temp.tain_no].is_EM = false;
                    all_list[i][temp.tain_no].has_box_down = false;
                }
                else
                {

                    all_list[i][temp.tain_no].has_a_box = false;
                    //重新标记集装箱
                    re_mark(all_list[i][temp.tain_no].box_number, ref pro_box_list);
                    all_list[i][temp.tain_no].box_number = null;
                    all_list[i][temp.tain_no].is_EM = false;
                    //重新标记集装箱
                    re_mark(all_list[i][temp.tain_no].up_box_number, ref pro_box_list);
                    all_list[i][temp.tain_no].up_box_number = null;
                    all_list[i][temp.tain_no].has_box_down = false;
                    all_list[i][temp.tain_no].has_box_up = false;

                }
            }
            //直接使用前面的插入函数进行插入
            insert_one_box(temp.tain_no,temp_box.box_from,
                temp_box.box_destation,temp_box, ref pro_box_list);
        }
        //清除集装箱
        public static void clear_place_box(box_in_pro temp_box,
            trian_take_place temp,
            ref List<box_in_pro> pro_box_list)
        {
            //先全部清理
            for (int i = temp.start; i <= temp.end; i++)
            {
                if (i == temp.start)
                {
                    //起始点不清理有集装箱下车标志
                    all_list[i][temp.tain_no].has_a_box = false;
                    //重新标记集装箱
                    re_mark(all_list[i][temp.tain_no].box_number, ref pro_box_list);
                    //all_list[i][temp.tain_no].box_number = null;
                    all_list[i][temp.tain_no].is_EM = false;
                    //重新标记集装箱
                    re_mark(all_list[i][temp.tain_no].up_box_number, ref pro_box_list);
                    all_list[i][temp.tain_no].up_box_number = null;
                    all_list[i][temp.tain_no].has_box_up = false;
                }
                else if (i == temp.end)
                {
                    //终点不清理上车标志
                    //all_list[i][temp.tain_no].has_a_box = false;
                    //重新标记集装箱
                    re_mark(all_list[i][temp.tain_no].box_number, ref pro_box_list);
                    all_list[i][temp.tain_no].box_number = null;
                    all_list[i][temp.tain_no].is_EM = false;
                    //重新标记集装箱
                    re_mark(all_list[i][temp.tain_no].up_box_number, ref pro_box_list);
                    //all_list[i][temp.tain_no].up_box_number = null;
                    all_list[i][temp.tain_no].has_box_down = false;
                }
                else
                {
                    all_list[i][temp.tain_no].has_a_box = false;
                    //重新标记集装箱
                    re_mark(all_list[i][temp.tain_no].box_number, ref pro_box_list);
                    all_list[i][temp.tain_no].box_number = null;
                    all_list[i][temp.tain_no].is_EM = false;
                    //重新标记集装箱
                    re_mark(all_list[i][temp.tain_no].up_box_number, ref pro_box_list);
                    all_list[i][temp.tain_no].up_box_number = null;
                    all_list[i][temp.tain_no].has_box_down = false;
                    all_list[i][temp.tain_no].has_box_up = false;

                }
            }
        }
    }
}

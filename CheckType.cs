
/***思路:将玩家的牌按升序排序.然后将牌进行拆分,分存在4个数组中.拆分规则如下:

假设有牌:333\444\555\789

则拆分后数组中的数据如下

arr[0]:345789

arr[1]:345

arr[2]:345

arr[3]:null

可以看出拆分规则是:如果遇到相同数字的牌则存到下一个数组的末尾.

拆分完后可以根据各数组的存储情况判定玩家出牌的类型,上面例子arr[3]为空.可以排除掉4带1(2).炸弹.的情况根据arr[2]为顺子且个数大于1,且arr[2]中存放的牌的张数乘以3刚好等于arr[0]的张数+arr[1]的张数.则可以判定是三带一的飞机.其他类型的牌也有相似的规律.以下是该算法的核心源代码.本算法用C#编写.
*********/
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class CheckType : MonoBehaviour
{
    public static CheckType Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private int[][] DiffRow(int[] nums)
    {
        int[][] list = new int[4][];
        for (int i = 0; i < list.Length; i++)
        {
            list[i] = new int[20];
        }
        int[] rowIndex = new int[4];
        int columIndex = 0;

        for (int i = 0; i < nums.Length; i++)
        {
            if (i + 1 < nums.Length)
            {
                if (nums[i] != 0)
                {
                    list[columIndex][rowIndex[columIndex]] = nums[i];
                    rowIndex[columIndex]++;
                }

                if (nums[i] == nums[i + 1])
                {
                    columIndex++;
                }
                else
                {
                    columIndex = 0;
                }
            }
            else if (nums[i] != 0)
                list[columIndex][rowIndex[columIndex]] = nums[i];
        }
        Debug.Log("list[0]:" + string.Join(",", list[0]));
        Debug.Log("list[1]:" + string.Join(",", list[1]));
        Debug.Log("list[2]:" + string.Join(",", list[2]));
        Debug.Log("list[3]:" + string.Join(",", list[3]));
        return list;
    }


    private int checkListCount(int[][] list, int rowIndex, int compStart, int rowMaxCount)
    {

/* LIST 代表单顺.

*DOUB 代表双顺.

*FEI0 代表三顺.

*FEI1 代表三带一的飞机

*FEI2 代表三带二的飞机

*FOR1 代表四带1

*FOR2 代表四带2

*ROCK 代表大小王

*/             
        //判断出牌规则，返回符合出牌规则的长度
        int listCount = 1;
        for (int i = compStart; i < rowMaxCount - 1; i++)
        {
            if (list[rowIndex][i] + 1 == list[rowIndex][i + 1])
                listCount++;
            else
                listCount = 1;
        }
        return listCount;
    }

    private int[] CutZero(int[] list)
    {
        List<int> rtlist = new List<int>();
        for (int i = 0; i < list.Length; i++)
        {
            if(list[i]!=0)
            {
                rtlist.Add(list[i]);
            }
            else
            {
                break;
            }
        }
        return rtlist.ToArray();
    }
    private bool ListEqual(int[] list1,int[] list2)
    {

        if (list1.Length == 0 || list2.Length == 0) return false;
        if (list1.Length == list2.Length &&
                list1[0] == list2[0]
             && list1[list1.Length - 1] == list2[list2.Length - 1])
            return true;
        return false;
    }
    private string CheckStraight(int[] list)
    {
        string rtStr = "";
        if (list.Length == 1)
        {
            rtStr = "1:" + list[0].ToString();
            return rtStr;
        }
        if(list[list.Length-1]!=15)    //不带2
        {
            if (list[list.Length - 1] - list[0] == list.Length - 1)
                rtStr = list.Length.ToString() + ":" + list[list.Length - 1].ToString();
        }
        else //带2
        {
            if(list[list.Length-2]==14)  //带A
            {
                if(list.Length>2)
                {
                    if(list[list.Length-3]==list.Length)   //是顺子
                    {
                        if (list.Length < 13)    //3,4,5,A,2
                        {
                            rtStr = list.Length.ToString() + ":" + list.Length.ToString();
                            
                        }
                        else
                        {
                            rtStr = list.Length.ToString() + ":" + list[list.Length - 2].ToString();
                        }
                    }
                }
                else   //A,2
                {
                    rtStr = "2:2";     //AA,22,AAA,222
                }
            }else    //不带A，带2
            {
                if(list[list.Length-2]==list.Length+1)   //是顺子,比如 3,4,2(15),那么4=(length=3)+1
                    rtStr = list.Length.ToString() + ":" + list[list.Length - 2].ToString();
            }
        }
        return rtStr;
        
    }
    private string CheckBomb(int[] nums)
    {
        string rtStr = "";
        if(nums.Length>4 && nums[0]==nums[nums.Length-1]) //说明是5头以上的炸弹
        {
            rtStr = "BOMB:" + nums.Length.ToString() + ":" + nums[0].ToString();
            return rtStr;
        }
        Dictionary<int, int> dic =
               new Dictionary<int, int>();

        // 开始统计每个元素重复次数
        foreach (int v in nums)
        {
            if (dic.ContainsKey(v))
            {
                // 数组元素再次，出现次数增加 1
                dic[v]+= 1;
            }
            else
            {
                // 数组元素首次出现，向集合中添加一个新项
                dic.Add(v, 1);
            }
        }
        foreach (var c in dic)
        {
            if (c.Value > 4)
            {
                rtStr = "NO";
                return rtStr;
            }
        }
        return rtStr;

    }
    public  string getCardType(int[] nums)
    {
        //如果是5个以上的炸弹，直接走炸弹逻辑
        string checkBomb = CheckBomb(nums);
        if(checkBomb=="NO") return string.Empty;
        if (checkBomb != "") return checkBomb;
        //然后再开始走分四列的逻辑
        int[][] list = DiffRow(nums);
        int[] counts = new int[4];
        for (int k = 0; k < 4; k++)
        { 
            counts[k] = Array.IndexOf(list[k], 0);
        }
        int MaxValue = 0;
        int listCount = 0;
        string type = string.Empty;
        //当第4行牌的数量为1的时候   
        #region
        if (counts[3] == 1)
        {
            int index = Array.IndexOf(list[2], list[3][0]); //?
            switch (counts[2])
            {
                case 1:
                    MaxValue = list[3][0];
                    if (counts[0] == 1)
                    {
                        type = "BOMB:4:" + MaxValue;
                    }
                    else if (counts[0] + counts[1] == 4)//四张带两张单牌（特殊一对）
                    {
                        type = "";
                        //type = "FOR1:6:" + MaxValue;
                    }
                    else if (counts[0] == counts[1] && counts[0] == 3)//四张带两队
                    {
                        type = "";
                        //type = "FOR2:8:" + MaxValue;
                    }

                    break;
                case 2: //一个飞机3333/444/5
                    if (list[2][0] + 1 == list[2][1] && counts[1] == counts[2] && counts[0] == 3)
                    {
                        MaxValue = list[2][counts[2] - 1];
                        type = "";
                        //type = "FEI1:" + counts[2] + ":" + MaxValue;
                    } break;
                case 3:
					//3333/444/555/67
                    if (checkListCount(list, 2, 0, counts[2]) == counts[2] && counts[0] + counts[1] + counts[3] == 3 * counts[2])
                    {
                        MaxValue = list[2][counts[2] - 1];
                        type = "";
                        //type = "FEI1:" + counts[2] + ":" + MaxValue;
                    }
					//3333/444/555飞机带对3翅膀
                    else if (Array.IndexOf(list[2], list[3][0]) == 0 && counts[0] == counts[2])
                    {
                        if ((listCount = checkListCount(list, 2, 1, counts[2])) == counts[2] - 1)
                        {
                            MaxValue = list[2][counts[2] - 1];
                            type = "FEI2:" + listCount + ":" + MaxValue;
                        }

                    }
					//333/444/5555组成飞机
                    else if (Array.IndexOf(list[2], list[3][0]) == counts[2] - 1 && counts[0] == counts[2])
                    {
                        if ((listCount = checkListCount(list, 2, 0, counts[2] - 1)) == counts[2] - 1)
                        {
                            MaxValue = list[2][counts[2] - 2];
                            type = "FEI2:" + listCount + ":" + MaxValue;
                        }
                    }
                    break;
                case 4:
					//3333/444/555/666/77飞机
                    if (index == 0 && counts[0] == counts[1] && counts[0] == 5)
                    {
                        if ((listCount = checkListCount(list, 2, 1, counts[2])) == counts[2] - 1)
                        {
                            MaxValue = list[2][counts[2] - 1];
                            type = "FEI2:" + listCount + ":" + MaxValue;
                        }
                    }
					//333/444/555/6666/77
                    else if (index == counts[2] - 1 && counts[0] == counts[1] && counts[0] == 5)
                    {
                        if ((listCount = checkListCount(list, 2, 0, counts[2] - 1)) == counts[2] - 1)
                        {
                            MaxValue = list[2][counts[2] - 2];
                            type = "FEI2:" + listCount + ":" + MaxValue;
                        }
                    }
					//3333/444/555/666/7/8/9
                    else if ((listCount = checkListCount(list, 2, 0, counts[2])) == counts[2] && counts[0] + counts[1] + counts[3] == 3 * counts[2])
                    {
                        MaxValue = list[2][counts[2] - 1];
                        type = "";
                        //type = "FEI1:" + listCount + ":" + MaxValue;
                    }
                    break;
                case 5:
                    if (index == 0)
                    {
						//3333/444/555/666/777/8/9/10/J
                        if ((listCount = checkListCount(list, 2, 0, counts[2])) == counts[2] && counts[0] + counts[1] + counts[3] == 3 * counts[2])
                        {
                            MaxValue = list[2][counts[2] - 1];
                            type = "";
                            //type = "FEI1:" + listCount + ":" + MaxValue;
                        }
                        else if ((listCount=checkListCount(list, 2, 1, counts[2])) == counts[2] - 1 && counts[0] == counts[1])
                        {
							//3333/444/555/666/777/88/99四个三代二飞机
                            if (counts[0] + 1 == 2 * (counts[2] - 1))
                            {
                                MaxValue = list[2][counts[2] - 1];
                                type = "FEI2:" + listCount + ":" + MaxValue;
                            }
							//3333/444/555/666/777将四个3拆分分别带给4，5，6，7飞机
                            else if (2 * (counts[0] + 1) == 3 * (counts[2] - 1))
                            {
                                MaxValue = list[2][counts[2] - 1];
                                type = "";
                                //type = "FEI1:" + listCount + ":" + MaxValue;
                            }
                        }
                    }
                    else if (index == counts[2] - 1)
                    {
						//333/444/555/666/7777/8/9/10/J
                        if ((listCount = checkListCount(list, 2, 0, counts[2])) == counts[2] && counts[0] + counts[1] + counts[3] == 3 * counts[2])
                        {
                            MaxValue = list[2][counts[2] - 1];
                            type = "";
                            //type = "FEI1:" + listCount + ":" + MaxValue;
                        }
                        else if ((listCount = checkListCount(list, 2, 0, counts[2] - 1)) == counts[2] - 1 && counts[0] == counts[1])
                        {
							//333/444/555/666/7777/88/99四个三代二的飞机
                            if (counts[0] + 1 == 2 * (counts[2] - 1))
                            {
                                MaxValue = list[2][counts[2] - 2];
                                type = "FEI2:" + listCount + ":" + MaxValue;
                            }
							//333/444/555/666/7777四个三带一的飞机
                            else if (2 * (counts[0] + 1) == 3 * (counts[2] - 1))
                            {
                                MaxValue = list[2][counts[2] - 2];
                                type = "";
                                //type = "FEI1:" + listCount + ":" + MaxValue;
                            }
                        }
                    }
                    else
                    {
                        if ((listCount = checkListCount(list, 2, 0, counts[2])) == counts[2] && counts[0] + counts[1] + counts[3] == 3 * counts[2])
                        {
                            MaxValue = list[2][counts[2] - 1];
                            type = "";
                            //type = "FEI1:" + listCount + ":" + MaxValue;
                        }
                    }
                    break;
                case 6:
                    if ((listCount = checkListCount(list, 2, 0, counts[2])) == counts[2] && counts[0] + counts[1] + counts[3] + 4 == 3 * counts[2])
                    {
                        MaxValue = list[2][counts[2] - 1];
                        type = "";
                        //type = "FEI1:" + listCount + ":" + MaxValue;
                    }
                    else if (index == 0 && (listCount = checkListCount(list, 2, 1, counts[2])) == counts[2] - 1 && counts[0] + counts[1] + 2 == 3 * (counts[2] - 1))
                    {
                        MaxValue = list[2][counts[2] - 1];
                        type = "FEI1:" + listCount + ":" + MaxValue;
                    }
                    else if (index == counts[2] - 1 && (listCount = checkListCount(list, 2, 0, counts[2] - 1)) == counts[2] - 1 && counts[0] + counts[1] + 2 == 3 * (counts[2] - 1))
                    {
                        MaxValue = list[2][counts[2] - 2];
                        type = "FEI1:" + listCount + ":" + MaxValue;
                    }
                    break;
            }
        }
        #endregion
        //当第4行牌的数量为2的时候
        #region
        if (counts[3] == 2)
        {
            switch (counts[2])
            {

                default:
                    if (counts[2] >= 2 && counts[2] < 6)
                    {
                        if ((listCount = checkListCount(list, 2, 0, counts[2])) == counts[2] && counts[0] + counts[1] + counts[3] == 3 * counts[2])
                        {
                            MaxValue = list[2][counts[2] - 1];
                            type = "";  //飞一在两副牌是没有的
                            //type = "FEI1:" + listCount + ":" + MaxValue;
                        }
                    }
                    break;
                case 6:
                    int firstIndex = Array.IndexOf(list[2], list[3][0]);
                    int secIndex = Array.IndexOf(list[2], list[3][1]);
					//将两个炸弹的拆成三带二
					//3333/4444/555/666/777/888
                    if (secIndex == 1)
                    {
                        if ((listCount = checkListCount(list, 2, 2, counts[2])) == counts[2] - 2 && counts[0] == counts[1] && counts[0] + 2 == 2 * (counts[2] - 2))
                        {
                            MaxValue = list[2][counts[2] - 1];
                            type = "FEI2:" + listCount + ":" + MaxValue;
                        }
                    }
                    else if (secIndex == counts[2] - 1)
                    {
						//3333/444/555/666/777/8888
                        if (firstIndex == 0)
                        {
                            if ((listCount = checkListCount(list, 2, 1, counts[2] - 1)) == counts[2] - 2 && counts[0] == counts[1] && counts[0] + 2 == 2 * (counts[2] - 2))
                            {
                                MaxValue = list[2][counts[2] - 2];
                                type = "FEI2:" + listCount + ":" + MaxValue;
                            }
                        }
						//333/444/555/666/7777/8888
                        else if (firstIndex == secIndex - 1)
                        {
                            if ((listCount = checkListCount(list, 2, 0, counts[2] - 2)) == counts[2] - 2 && counts[0] == counts[1] && counts[0] + 2 == 2 * (counts[2] - 2))
                            {
                                MaxValue = list[2][counts[2] - 3];
                                type = "FEI2:" + listCount + ":" + MaxValue;
                            }
                        }
                    }
					//将一个炸弹拆成三代一
					//3333/4444/555/666/777/888两边牌拆分，炸弹位置随便安排
                    if ((listCount = checkListCount(list, 2, 1, counts[2])) == counts[2] - 1 && 2 * counts[0] + 3 == 3 * (counts[2] - 1))
                    {
                        MaxValue = list[2][counts[2] - 1];
                        type = "";
                        //type = "FEI1:" + listCount + ":" + MaxValue;
                    }
					//333/4444/555/666/7777/8888
                    else if ((listCount = checkListCount(list, 2, 0, counts[2] - 1)) == counts[2] - 1 && 2 * counts[0] + 3 == 3 * (counts[2] - 1))
                    {
                        MaxValue = list[2][counts[2] - 2];
                        type = "";
                        //type = "FEI1:" + listCount + ":" + MaxValue;
                    }


                    break;
            }
        }
        #endregion
        //当第4行牌的数量大于2的时候
        #region
        if (counts[3] > 2)
        {
            if ((listCount = checkListCount(list, 2, 0, counts[2])) == counts[2] && counts[0] + counts[1] + counts[3] == 3 * counts[2])
            {
                MaxValue = list[2][counts[2] - 1];
                type = "FEI1:" + listCount + ":" + MaxValue;
            }
        }
        #endregion
        //当第4行牌的数量为0,第三行牌的数量大于0
        #region
        if (counts[3] == 0 && counts[2] > 0)
        {

            if (CheckStraight(CutZero(list[2]))!= "" && ListEqual(CutZero(list[0]),CutZero(list[1])))    //确认是三连三张带东西  
            {
                if (counts[0] == counts[2])
                {
                    string CntAndBig = CheckStraight(CutZero(list[0]));
                    if (CntAndBig==CheckStraight(CutZero(list[2])))
                    {
                        type = "FEI0:" + CntAndBig;
                    }

                }
                else if (counts[0] + counts[1] == 3 * counts[2])
                {

                    MaxValue = list[2][counts[2] - 1];
                    type = "FEI1:" + listCount + ":" + MaxValue;
                }
                else if (counts[0] + counts[1] == 4 * counts[2] && counts[0] == counts[1] )
                {
                    //飞机的条件,这里有个BUG，就是比如 4444455555,这样的牌，不能算是飞机，这种情况目前比较少见
                    //基本认为，5张的炸弹不会去凑一个飞机
                    List<int> listMore = new List<int>();
                    if (list[0][0] == list[2][0])
                    {
                        for (int i = counts[2]; i < counts[0]; i++)
                        {
                            listMore.Add(list[0][i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < counts[0]-counts[2]; i++)
                        {
                            listMore.Add(list[0][i]);
                        }
                    }
                    if (CheckStraight(listMore.ToArray())!="")
                    {
                        string CntAndBig = CheckStraight(CutZero(list[2]));
                        type = "FEI2:" + CntAndBig;
                    }
                    else
                    {
                        type = string.Empty;
                    }
                    
                }
            }
			//333/444/555/666/将3和6拆分
            if ((listCount = checkListCount(list, 2, 1, counts[2])) == counts[2] - 1 && counts[0] + counts[1] + 1 == 3 * (counts[2] - 1))
            {
                MaxValue = list[2][counts[2] - 1];
                type = "FEI1:" + listCount + ":" + MaxValue;
            }
            else if ((listCount = checkListCount(list, 2, 0, counts[2] - 1)) == counts[2] - 1 && counts[0] + counts[1] + 1 == 3 * (counts[2] - 1))
            {
                MaxValue = list[2][counts[2] - 2];
                type = "FEI1:" + listCount + ":" + MaxValue;
            }
        }
        #endregion
        //当第3行牌的数量为0，第二行牌的数量大于0
        #region
        if (counts[2] == 0 && counts[1] > 0)
        {
            if (counts[0] == 1)
            {
                MaxValue = list[1][counts[1] - 1];
                listCount = counts[1];
                type = "DOUB:" + listCount + ":" + MaxValue;
            }
            else
            {
                if(counts[0]==13 && counts[1]==1 && list[1][0]==14)
                {
                    //A到A的顺子,A,2,3,4,5,6,7,8,9,10,J,Q,K,A，14张
                    type = "LIST:14:14";
                }
                if(counts[0]==2 && counts[1]==2 && list[0][0]==16)
                {   
                    //四鬼炸弹，报道逻辑
                    type = "BOMB:9:17";
                }
				//双顺
                if (counts[1] > 2 && counts[0] == counts[1] )
                {
                    string CntAndBig1 = CheckStraight(CutZero(list[0]));
                    string CntAndBig2 = CheckStraight(CutZero(list[1]));
                    if (CntAndBig1 == CntAndBig2)
                    {
                        type = "DOUB:" + CntAndBig2;
                    }
                }
            }
        }
        #endregion
        //当第2行牌的数量为0
        #region
        if (counts[1] == 0)
        {
            if (counts[0] == 1)
            {
                MaxValue = list[0][counts[0] - 1];
                listCount = counts[0];
                type = "LIST:" + listCount + ":" + MaxValue;
            }
            else if (counts[0] == 2 && list[0][0] == 16 && list[0][1] == 17)
            {
                //一副牌的逻辑
                //type = "ROCK:2:17";
                type = "";
            }
            else if (counts[0] >= 5)
            {
                string CntAndBig = CheckStraight(CutZero(list[0]));
                if (CntAndBig != "")
                    type = "LIST:" + CntAndBig;
            }
        }
        #endregion
        String[] cmd= type.Split(new char[]{':'});
        
        if (cmd.Length > 0 && cmd[0]!=string.Empty)
        {
            if ((cmd[0]=="LIST" || cmd[0]=="DOUB" )&& (int.Parse(cmd[1]))>1)
            {
                type = int.Parse(cmd[2]) > 15 ? string.Empty : type;
            }
            else if  ( cmd[0] == "FEI0" && (int.Parse(cmd[1])) > 1)
            {
                type = int.Parse(cmd[2]) > 15 ? string.Empty : type;
            }
            else if (cmd[0] == "FEI2" && (int.Parse(cmd[1])) > 1)
            {
                type = int.Parse(cmd[2]) > 15 ? string.Empty : type;
            }
            else if (cmd[0] == "FOR1" || cmd[0] == "FOR2" || cmd[0]=="FEI1")   //这三种牌型是不可以的
            {
                type = string.Empty;
            }
        }
        return type;
    }
}



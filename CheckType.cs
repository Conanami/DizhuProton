
/***˼·:����ҵ��ư���������.Ȼ���ƽ��в��,�ִ���4��������.��ֹ�������:

��������:333\444\555\789

���ֺ������е���������

arr[0]:345789

arr[1]:345

arr[2]:345

arr[3]:null

���Կ�����ֹ�����:���������ͬ���ֵ�����浽��һ�������ĩβ.

��������Ը��ݸ�����Ĵ洢����ж���ҳ��Ƶ�����,��������arr[3]Ϊ��.�����ų���4��1(2).ը��.���������arr[2]Ϊ˳���Ҹ�������1,��arr[2]�д�ŵ��Ƶ���������3�պõ���arr[0]������+arr[1]������.������ж�������һ�ķɻ�.�������͵���Ҳ�����ƵĹ���.�����Ǹ��㷨�ĺ���Դ����.���㷨��C#��д.
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

/* LIST ����˳.

*DOUB ����˫˳.

*FEI0 ������˳.

*FEI1 ��������һ�ķɻ�

*FEI2 �����������ķɻ�

*FOR1 �����Ĵ�1

*FOR2 �����Ĵ�2

*ROCK �����С��

*/             
        //�жϳ��ƹ��򣬷��ط��ϳ��ƹ���ĳ���
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
        if(list[list.Length-1]!=15)    //����2
        {
            if (list[list.Length - 1] - list[0] == list.Length - 1)
                rtStr = list.Length.ToString() + ":" + list[list.Length - 1].ToString();
        }
        else //��2
        {
            if(list[list.Length-2]==14)  //��A
            {
                if(list.Length>2)
                {
                    if(list[list.Length-3]==list.Length)   //��˳��
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
            }else    //����A����2
            {
                if(list[list.Length-2]==list.Length+1)   //��˳��,���� 3,4,2(15),��ô4=(length=3)+1
                    rtStr = list.Length.ToString() + ":" + list[list.Length - 2].ToString();
            }
        }
        return rtStr;
        
    }
    private string CheckBomb(int[] nums)
    {
        string rtStr = "";
        if(nums.Length>4 && nums[0]==nums[nums.Length-1]) //˵����5ͷ���ϵ�ը��
        {
            rtStr = "BOMB:" + nums.Length.ToString() + ":" + nums[0].ToString();
            return rtStr;
        }
        Dictionary<int, int> dic =
               new Dictionary<int, int>();

        // ��ʼͳ��ÿ��Ԫ���ظ�����
        foreach (int v in nums)
        {
            if (dic.ContainsKey(v))
            {
                // ����Ԫ���ٴΣ����ִ������� 1
                dic[v]+= 1;
            }
            else
            {
                // ����Ԫ���״γ��֣��򼯺������һ������
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
        //�����5�����ϵ�ը����ֱ����ը���߼�
        string checkBomb = CheckBomb(nums);
        if(checkBomb=="NO") return string.Empty;
        if (checkBomb != "") return checkBomb;
        //Ȼ���ٿ�ʼ�߷����е��߼�
        int[][] list = DiffRow(nums);
        int[] counts = new int[4];
        for (int k = 0; k < 4; k++)
        { 
            counts[k] = Array.IndexOf(list[k], 0);
        }
        int MaxValue = 0;
        int listCount = 0;
        string type = string.Empty;
        //����4���Ƶ�����Ϊ1��ʱ��   
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
                    else if (counts[0] + counts[1] == 4)//���Ŵ����ŵ��ƣ�����һ�ԣ�
                    {
                        type = "";
                        //type = "FOR1:6:" + MaxValue;
                    }
                    else if (counts[0] == counts[1] && counts[0] == 3)//���Ŵ�����
                    {
                        type = "";
                        //type = "FOR2:8:" + MaxValue;
                    }

                    break;
                case 2: //һ���ɻ�3333/444/5
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
					//3333/444/555�ɻ�����3���
                    else if (Array.IndexOf(list[2], list[3][0]) == 0 && counts[0] == counts[2])
                    {
                        if ((listCount = checkListCount(list, 2, 1, counts[2])) == counts[2] - 1)
                        {
                            MaxValue = list[2][counts[2] - 1];
                            type = "FEI2:" + listCount + ":" + MaxValue;
                        }

                    }
					//333/444/5555��ɷɻ�
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
					//3333/444/555/666/77�ɻ�
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
							//3333/444/555/666/777/88/99�ĸ��������ɻ�
                            if (counts[0] + 1 == 2 * (counts[2] - 1))
                            {
                                MaxValue = list[2][counts[2] - 1];
                                type = "FEI2:" + listCount + ":" + MaxValue;
                            }
							//3333/444/555/666/777���ĸ�3��ֱַ����4��5��6��7�ɻ�
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
							//333/444/555/666/7777/88/99�ĸ��������ķɻ�
                            if (counts[0] + 1 == 2 * (counts[2] - 1))
                            {
                                MaxValue = list[2][counts[2] - 2];
                                type = "FEI2:" + listCount + ":" + MaxValue;
                            }
							//333/444/555/666/7777�ĸ�����һ�ķɻ�
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
        //����4���Ƶ�����Ϊ2��ʱ��
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
                            type = "";  //��һ����������û�е�
                            //type = "FEI1:" + listCount + ":" + MaxValue;
                        }
                    }
                    break;
                case 6:
                    int firstIndex = Array.IndexOf(list[2], list[3][0]);
                    int secIndex = Array.IndexOf(list[2], list[3][1]);
					//������ը���Ĳ��������
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
					//��һ��ը���������һ
					//3333/4444/555/666/777/888�����Ʋ�֣�ը��λ����㰲��
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
        //����4���Ƶ���������2��ʱ��
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
        //����4���Ƶ�����Ϊ0,�������Ƶ���������0
        #region
        if (counts[3] == 0 && counts[2] > 0)
        {

            if (CheckStraight(CutZero(list[2]))!= "" && ListEqual(CutZero(list[0]),CutZero(list[1])))    //ȷ�����������Ŵ�����  
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
                    //�ɻ�������,�����и�BUG�����Ǳ��� 4444455555,�������ƣ��������Ƿɻ����������Ŀǰ�Ƚ��ټ�
                    //������Ϊ��5�ŵ�ը������ȥ��һ���ɻ�
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
			//333/444/555/666/��3��6���
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
        //����3���Ƶ�����Ϊ0���ڶ����Ƶ���������0
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
                    //A��A��˳��,A,2,3,4,5,6,7,8,9,10,J,Q,K,A��14��
                    type = "LIST:14:14";
                }
                if(counts[0]==2 && counts[1]==2 && list[0][0]==16)
                {   
                    //�Ĺ�ը���������߼�
                    type = "BOMB:9:17";
                }
				//˫˳
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
        //����2���Ƶ�����Ϊ0
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
                //һ���Ƶ��߼�
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
            else if (cmd[0] == "FOR1" || cmd[0] == "FOR2" || cmd[0]=="FEI1")   //�����������ǲ����Ե�
            {
                type = string.Empty;
            }
        }
        return type;
    }
}



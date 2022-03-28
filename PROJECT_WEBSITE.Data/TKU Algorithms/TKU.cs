using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Collections;
using System.IO;
using System.Collections;
using System.Web;

namespace PROJECT_WEBSITE.Data.TKU_Algorithms
{
    public class TKU
    {
        //Số mặt hàng trong cơ sỡ dữ liệu
        public static int itemCount;

        //Số K mà người dùng muốn
        public static int kValue;
        //Border min util
        public static double BorderMinUtil = 0;
        
        //Danh sách hữu ích tối thiểu
        public static int[] arrayMIU;

        //Danh sách hữu ích tối đa
        public static int[] arrayMAU;
        //danh sách TWU
        public static int[] arrayTWUItems;

        public static int CountPKHUIs = 0;
        //danh sách PKHUIs tiềm năng
        public static List<string> PKHUIs = new List<string>(0);
        //Main
        public void RunTKU(string Database, int k, int icount, int dbcount)
        {
            List<int> ulist = new List<int>(0);

            kValue = k;
            itemCount = icount + 1;// icount + 1 , icount là số item count được cho trong data set
            arrayTWUItems = new int[itemCount];
            arrayMIU = new int[itemCount];
            arrayMAU = new int[itemCount];
            //Chiến lược 2
            //Tính toán các dữ liệu MIU,MAU và nâng boderuitl trước khi xây dựng up-tree
            Caculation(Database, arrayTWUItems, itemCount, arrayMIU, arrayMAU, kValue);
            //Xây dựng UP-TREE và sử dụng chiến lượt 3 tăng minutil trong khi xây dựng up-tree
            UPTree tree = BuildUPTree(arrayTWUItems, Database);

            //Chiến lượt 4 tăng minutil trước khi gen ra PKHUIs
            RedBlackTree<int> DSNodeCountHeap = new RedBlackTree<int>(true);
            for (int i = 0; i < tree.root.childlink.Count; i++)
            {
                int[] Sum_DS = new int[itemCount];

                int DSItem = tree.root.childlink[i].item;

                tree.SumDescendent(tree.root.childlink[i], Sum_DS);

                for (int j = 0; j < Sum_DS.Length; j++)
                {
                    if ((Sum_DS[j] != 0) && (j != DSItem))
                    {
                        int DS_Value = (arrayMIU[j] + arrayMIU[DSItem]) * Sum_DS[j];

                        UpdateNodeCountHeap(DSNodeCountHeap, DS_Value);
                    }
                }
            }

            getUlist(arrayTWUItems, ulist);

            //khai thác tập PKHUIs
            string prefix = "";

            //Lưu trữ minutil để nâng mintuil lúc xây dựng uptree
            RedBlackTree<int> ISNodeCountHeap = new RedBlackTree<int>(true);

            tree.UPGrowth(tree, ulist, prefix, ISNodeCountHeap, arrayTWUItems);

            //Xuất ra các tập ứng viên tiềm năng PKHUIs
            for (int i = 0; i < arrayTWUItems.Length; i++)
            {
                if (arrayTWUItems[i] >= BorderMinUtil)
                {
                    Console.WriteLine(i + ":" + arrayTWUItems[i]);

                    PKHUIs.Add(i.ToString() + ":" + arrayTWUItems[i].ToString());
                }
            }

            //ghi file PKHUIs
            string fileLPath = HttpContext.Current.Server.MapPath(@"~/Data/PKHUIs.txt");

            string[] lines = new string[PKHUIs.Count];
            for (int i = 0; i < PKHUIs.Count; i++)
            {
                lines[i] = PKHUIs[i];
            }
            System.IO.File.WriteAllLines(fileLPath, lines);
            CountPKHUIs = PKHUIs.Count();
            Console.WriteLine("So cac tap PKHUIs = " + CountPKHUIs.ToString());

            //Sắp xếp PKHUIs
            string fileLPathSort = HttpContext.Current.Server.MapPath(@"~/Data/SortPKHUIs.txt");
            SortPKHUIs(fileLPath, fileLPathSort, PKHUIs.Count);
            Console.WriteLine("Hoan thanh Sap xep PKHUIs");
            
            //Giai đoạn 2 lấy topk từ PKHUIs
            TKU2 tku2 = new TKU2();
            tku2.RunTKU2(BorderMinUtil, dbcount, k, Database, fileLPathSort);
            
        }

        //gán U list vào list
        public void getUlist(int[] P1, List<int> list)
        {
            for (int i = 0; i < P1.Length; i++)
            {
                if (P1[i] > 0)
                {
                    if (P1[i] >= BorderMinUtil)
                    {
                        InsertItem(list, i, P1);
                    }
                }
            }
        }

        //Hàm insert mặt hàng vào Util List và sắp xếp theo TWU
        public static int InsertItem(List<int> list, int item, int[] Order)
        {
            if (list.Count == 0)
            {
                list.Add(item);
            }
            else if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (Order[item] > Order[list[i]])
                    {
                        list.Insert(i, item);
                        return 0;
                    }
                    else if ((Order[item] == Order[list[i]]) && (item < list[i]))
                    {
                        list.Insert(i, item);
                        return 0;
                    }
                    else if (i == ((list.Count) - 1))
                    {
                        list.Add(item);
                        return 0;
                    }
                }
            }
            return -1;
        }

        //Tính các giá trị TWU, MIU, MAU và nâng minutil trước khi xây dựng up-tree
        public void Caculation(string DB, int[] TWU1, int num_Item, int[] MinBNF, int[] MaxBNF, int pK)
        {

            PEMMatrix matrix = new PEMMatrix(num_Item);
            
            //Đọc dữ liệu từ file
            string[] lines = File.ReadAllLines(@DB);
            foreach (var item in lines)
            {
                string[] t1 = item.Split(':');//Dữ liệu giao dịch
                string[] t2 = t1[0].Split(' ');// danh sách mặt hàng của giao dịch T
                string[] t3 = t1[2].Split(' ');//danh sách hữu ích của từng mặt hàng trong giao dịch T

                for (int i = 0; i < t2.Length; i++)
                {

                    //Tính MIU của từng mặt hàng
                    if (MinBNF[int.Parse(t2[i])] == 0)
                    {
                        if (int.Parse(t3[i]) > 0)
                        {
                            MinBNF[int.Parse(t2[i])] = int.Parse(t3[i]);
                        }
                    }
                    else if (MinBNF[int.Parse(t2[i])] > (int.Parse(t3[i])))
                    {
                        MinBNF[int.Parse(t2[i])] = int.Parse(t3[i]);
                    }

                    //Tính MAU của từng mặt hàng
                    if (MaxBNF[int.Parse(t2[i])] < (int.Parse(t3[i])))
                    {
                        MaxBNF[int.Parse(t2[i])] = int.Parse(t3[i]);
                    }

                    TWU1[int.Parse(t2[i])] += int.Parse(t1[1]);

                    if (i > 0)
                    {
                        matrix.InsertItemMatrix(int.Parse(t2[0]), int.Parse(t2[i]), int.Parse(t3[0]) + int.Parse(t3[i]));
                    }

                }

            }

            getInitialUtility(matrix, num_Item, pK);

        }

        //Hàm cập nhật topk-list
        public static void UpdateNodeCountHeap(RedBlackTree<int> NCH, int newValue)
        {
            if (NCH.Count < kValue)
            {
                NCH.Add(newValue);
            }
            else if (NCH.Count >= kValue)
            {

                if (newValue > BorderMinUtil)
                {
                    NCH.Add(newValue);
                    NCH.Remove(NCH.Min());
                }
            }
            if ((NCH.Min().Value > BorderMinUtil) && (NCH.Count >= kValue))
            {
                BorderMinUtil = NCH.Min().Value;
            }
        }

        //Chiến lượt 2
        public void getInitialUtility(PEMMatrix TM, int nItem, int K)
        {
            RedBlackTree<int> topKlist = new RedBlackTree<int>();

            for (int i = 0; i < nItem; i++)
            {
                for (int j = 0; j < TM.matrix[i].Length; j++)
                {
                    if (TM.matrix[i][j] != 0)
                    {
                        UpdateNodeCountHeap(topKlist, TM.matrix[i][j]);
                    }
                }
            }
        }
        //Sắp xếp giao dịch T từ lớn đến nhỏ theo TWU
        public static void sorttrans(int[] tran, int pre, int tranlen, int[] P1)
        {
            int temp;

            for (int i = pre; i < tranlen - 1; i++)
            {
                for (int j = pre; j < tranlen - 1; j++)
                {
                    if (P1[tran[j]] < P1[tran[j + 1]])
                    {
                        temp = tran[j];
                        tran[j] = tran[j + 1];
                        tran[j + 1] = temp;
                    }
                    else if (P1[tran[j]] == P1[tran[j + 1]])
                    {
                        if (tran[j] > tran[j + 1])
                        {
                            temp = tran[j];
                            tran[j] = tran[j + 1];
                            tran[j + 1] = temp;
                        }
                    }
                }
            }
        }

        //Sắp xếp giao dịch T từ lớn đến nhỏ theo TWU
        public void sorttrans2(int[] tran, string[] bran, int pre, int tranlen, int[] P1)
        {
            int t1;
            string t2;

            for (int i = pre; i < tranlen - 1; i++)
            {
                for (int j = 0; j < tranlen - 1; j++)
                {
                    if (P1[tran[j]] < P1[tran[j + 1]])
                    {
                        t1 = tran[j];
                        t2 = bran[j];

                        tran[j] = tran[j + 1];
                        bran[j] = bran[j + 1];

                        tran[j + 1] = t1;
                        bran[j + 1] = t2;
                    }
                    else if (P1[tran[j]] == P1[tran[j + 1]])
                    {
                        if (tran[j] > tran[j + 1])
                        {
                            t1 = tran[j];
                            t2 = bran[j];

                            tran[j] = tran[j + 1];
                            bran[j] = bran[j + 1];

                            tran[j + 1] = t1;
                            bran[j + 1] = t2;
                        }
                    }
                }
            }
        }

        //Sắp xếp PKHUIs từ lớn đến bé theo EU(x)
        public void SortPKHUIs(string PKHUIs, string SortPKHUIs, int count)
        {
            //PKHUI non sort
            string[] line = File.ReadAllLines(@PKHUIs);
            
            //PKHUI sort
            string[] lineSort = new string[count];

            List<StringPair> Pkhui = new List<StringPair>(0);

            foreach (var item in line)
            {
                string[] temp = item.Split(':');
                Pkhui.Add(new StringPair(temp[0], int.Parse(temp[1])));
            }

            List<StringPair> sort = Pkhui.OrderByDescending(x => x.y).ToList();

            for (int i = 0; i < sort.Count; i++)
            {
                lineSort[i] = sort[i].x+":"+sort[i].y.ToString();
            }
            System.IO.File.WriteAllLines(SortPKHUIs, lineSort);
        }

        //Hàm xây dựng cây UP-TREE
        public UPTree BuildUPTree(int[] p1, string DB)
        {
            //TOP-K LIST

            RedBlackTree<int> NodeCountHeap = new RedBlackTree<int>(true);

            UPTree tree = new UPTree();
            string[] lines = File.ReadAllLines(@DB);
            foreach (var item in lines)
            {
                string[] t1 = item.Split(':');//Dữ liệu giao dịch

                string[] t2 = t1[0].Split(' ');// danh sách mặt hàng của giao dịch T
                string[] b = t1[2].Split(' ');//danh sách hữu ích của từng mặt hàng trong giao dịch T
                string[] b2 = new string[b.Length];

                int tranlen = 0;
                int[] tran = new int[t2.Length];

                for (int i = 0; i < t2.Length; i++)
                {
                    if (p1[int.Parse(t2[i])] >= BorderMinUtil)
                    {
                        b2[tranlen] = b[i];
                        tran[tranlen] = int.Parse(t2[i]);
                        tranlen++;
                    }
                }

                sorttrans2(tran, b2, 0, tranlen, p1);
                tree.Insert_Reorganized_Transaction(tran, b2, tranlen, p1, 1, NodeCountHeap);
            }
            return tree;
        }

        public class TreeNode
        {
            public int item { get; set; }
            public int count { get; set; }
            public int nu { get; set; }
            public TreeNode hlink = null;
            public TreeNode parentlink = null;

            public List<TreeNode> childlink { get; set; }

            public TreeNode(int item, int nu, int count)
            {
                this.item = item;
                this.count = count;
                this.nu = nu;
                this.childlink = new List<TreeNode>(0);
                this.hlink = null;
                this.parentlink = null;
            }
        }
        public class UPTree
        {
            public TreeNode root { get; set; }
            public TreeNode[] HeaderTable = new TreeNode[itemCount];
            public UPTree()
            {
                this.root = new TreeNode(-1, 0, 0);

                for (int i = 0; i < HeaderTable.Length; i++)
                {
                    this.HeaderTable[i] = null;
                }
            }
            //Add cơ sỡ dữ liệu giao dịch vào UP-TREE
            public void InsertPatterBase(int[] tran, int tranlen, int[] L1, int TWU, int IC, int SumBNF)
            {
                TreeNode par = root;
                for (int i = 0; i < tranlen; i++)
                {
                    int target = tran[i];
                    int cs = par.childlink.Count();

                    if (cs == 0)
                    {
                        int M = TWU - (SumBNF - arrayMIU[target] * IC);
                        SumBNF = SumBNF - (arrayMIU[target] * IC);

                        TreeNode nNode = new TreeNode(target, M, IC);

                        par.childlink.Add(nNode);

                        nNode.parentlink = par;

                        if (HeaderTable[target] == null)
                        {
                            HeaderTable[target] = nNode;
                        }
                        else
                        {
                            nNode.hlink = HeaderTable[target];
                            HeaderTable[target] = nNode;
                        }

                        par = nNode;
                    }
                    else
                    {
                        for (int j = 0; j < cs; j++)
                        {
                            TreeNode comp = par.childlink[j];

                            if (target == comp.item)
                            {
                                int M = TWU - (SumBNF - arrayMIU[target] * IC);
                                SumBNF = SumBNF - (arrayMIU[target] * IC);

                                comp.nu += M;
                                comp.count += IC;
                                par = comp;
                                break;
                            }
                            else if (L1[target] > L1[comp.item])
                            {
                                int M = TWU - (SumBNF - arrayMIU[target] * IC);
                                SumBNF = SumBNF - (arrayMIU[target] * IC);

                                TreeNode nNode = new TreeNode(target, M, IC);
                                par.childlink.Insert(j, nNode);

                                nNode.parentlink = par;

                                if (HeaderTable[target] == null)
                                {
                                    HeaderTable[target] = nNode;
                                }
                                else
                                {
                                    nNode.hlink = HeaderTable[target];
                                    HeaderTable[target] = nNode;
                                }

                                par = nNode;
                                break;
                            }
                            else if (((L1[target] == L1[comp.item])) && ((target < comp.item)))
                            {
                                int M = TWU - (SumBNF - arrayMIU[target] * IC);
                                SumBNF = SumBNF - (arrayMIU[target] * IC);

                                TreeNode nNode = new TreeNode(target, M, IC);
                                par.childlink.Insert(j, nNode);

                                nNode.parentlink = par;

                                if (HeaderTable[target] == null)
                                {
                                    HeaderTable[target] = nNode;
                                }
                                else
                                {
                                    nNode.hlink = HeaderTable[target];
                                    HeaderTable[target] = nNode;
                                }

                                par = nNode;
                                break;
                            }
                            else if (j == (cs - 1))
                            {
                                int M = TWU - (SumBNF - arrayMIU[target] * IC);
                                SumBNF = SumBNF - (arrayMIU[target] * IC);

                                TreeNode nNode = new TreeNode(target, M, IC);
                                par.childlink.Add(nNode);

                                nNode.parentlink = par;

                                if (HeaderTable[target] == null)
                                {
                                    HeaderTable[target] = nNode;
                                }
                                else
                                {
                                    nNode.hlink = HeaderTable[target];
                                    HeaderTable[target] = nNode;
                                }

                                par = nNode;
                            }
                        }
                    }
                }
            }

            //Add cơ sỡ dữ liệu giao dịch vào UP-TREE
            public void Insert_Reorganized_Transaction(int[] tran, string[] bran, int tranlen, int[] L1, int IC, RedBlackTree<int> NodeCountHeap)
            {
                int TWU = 0;

                TreeNode par = root;

                for (int i = 0; i < tranlen; i++)
                {
                    TWU += int.Parse(bran[i]);

                    int target = tran[i];
                    int cs = par.childlink.Count();

                    if (cs == 0)
                    {
                        TreeNode nNode = new TreeNode(target, TWU, IC);
                        par.childlink.Add(nNode);

                        if (nNode.nu > BorderMinUtil)
                        {
                           UpdateNodeCountHeap(NodeCountHeap, nNode.nu);
                        }

                        nNode.parentlink = par;

                        if (HeaderTable[target] == null)
                        {
                            HeaderTable[target] = nNode;
                        }
                        else
                        {
                            nNode.hlink = HeaderTable[target];
                            HeaderTable[target] = nNode;
                        }

                        par = nNode;
                    }
                    else
                    {
                        for (int j = 0; j < cs; j++)
                        {
                            TreeNode comp = par.childlink[j];

                            if (target == comp.item)
                            {
                                NodeCountHeap.Remove(comp.nu);
                                UpdateNodeCountHeap(NodeCountHeap, (comp.nu + TWU));

                                comp.nu += TWU;
                                comp.count += IC;
                                par = comp;
                                break;
                            }
                            else if (L1[target] > L1[comp.item])
                            {
                                if (comp.nu > BorderMinUtil)
                                {
                                   UpdateNodeCountHeap(NodeCountHeap, TWU);
                                }

                                TreeNode nNode = new TreeNode(target, TWU, IC);
                                par.childlink.Insert(j, nNode);

                                nNode.parentlink = par;

                                if (HeaderTable[target] == null)
                                {
                                    HeaderTable[target] = nNode;
                                }
                                else
                                {
                                    nNode.hlink = HeaderTable[target];
                                    HeaderTable[target] = nNode;
                                }

                                par = nNode;
                                break;
                            }
                            else if (((L1[target] == L1[comp.item])) && ((target < comp.item)))
                            {
                                if (comp.nu > BorderMinUtil)
                                {
                                   UpdateNodeCountHeap(NodeCountHeap, TWU);
                                }

                                TreeNode nNode = new TreeNode(target, TWU, IC);
                                par.childlink.Insert(j, nNode);

                                nNode.parentlink = par;

                                if (HeaderTable[target] == null)
                                {
                                    HeaderTable[target] = nNode;
                                }
                                else
                                {
                                    nNode.hlink = HeaderTable[target];
                                    HeaderTable[target] = nNode;
                                }

                                par = nNode;
                                break;
                            }
                            else if (j == (cs - 1))
                            {
                                if (comp.nu > BorderMinUtil)
                                {
                                   UpdateNodeCountHeap(NodeCountHeap, TWU);
                                }

                                TreeNode nNode = new TreeNode(target, TWU, IC);
                                par.childlink.Add(nNode);

                                nNode.parentlink = par;

                                if (HeaderTable[target] == null)
                                {
                                    HeaderTable[target] = nNode;
                                }
                                else
                                {
                                    nNode.hlink = HeaderTable[target];
                                    HeaderTable[target] = nNode;
                                }
                                par = nNode;
                            }
                        }
                    }
                }
            }

            //Tính trong chiến lượt 3
            public void SumDescendent(TreeNode cNode, int[] DS_Sum_Table)
            {
                if (cNode != null)
                {
                    DS_Sum_Table[cNode.item] += cNode.count;
                    for (int i = 0; i < cNode.childlink.Count; i++)
                    {
                        SumDescendent(cNode.childlink[i], DS_Sum_Table);
                    }
                }
            }

            //Tìm kiếm PKHUIs bằng UP-GROWTH
            public void UPGrowth(UPTree tree2, List<int> flist2, string prefix, RedBlackTree<int> ISNodeCountHeap, int[] LP1)
            {
                for (int i = 0; i < flist2.Count; i++)
                {
                    if (LP1[flist2[i]] >= BorderMinUtil)
                    {
                        string Nprefix = "";

                        if (prefix == "")
                        {
                            Nprefix = prefix + flist2[i] + "";// sửa lại
                        }
                        else
                        {
                            Nprefix = prefix + " " + flist2[i];
                        }


                        int citem = flist2[i];
                        TreeNode chlink = tree2.HeaderTable[citem];

                        List<List<int>> CPB = new List<List<int>>(0);
                        List<int> CPBW = new List<int>(0);// lưu TWU từng mặt hàng

                        List<int> CPBC = new List<int>(0);// lưu count từng mặt hàng

                        int[] LocalF1 = new int[itemCount];
                        int[] LocalCount = new int[itemCount];

                        while (chlink != null)
                        {
                            List<int> path = new List<int>(0);

                            TreeNode cptr = chlink;

                            while (cptr.parentlink != null)
                            {
                                path.Add(cptr.item);

                                LocalF1[cptr.item] = LocalF1[cptr.item] + chlink.nu;//tính nu
                                LocalCount[cptr.item] = LocalCount[cptr.item] + chlink.count;//tính count
                                cptr = cptr.parentlink;
                            }

                            path.RemoveAt(0);// xóa mặt hàng đầu tiên
                            CPB.Add(path);
                            CPBW.Add(chlink.nu);
                            CPBC.Add(chlink.count);

                            chlink = chlink.hlink;
                        }

                        List<int> localflist = new List<int>(0);

                        for (int j = 0; j < LocalF1.Length; j++)
                        {
                            if (LocalF1[j] < BorderMinUtil)
                            {
                                LocalF1[j] = -1;
                            }
                            else
                            {
                                if (j != citem)
                                {
                                    InsertItem(localflist, j, LocalF1);


                                    string UTI = Nprefix + " " + j;
                                    string[] TempItem = UTI.Split(' ');
                                    int SumMau = 0;
                                    int SumMiu = 0;

                                    for (int u = 0; u < TempItem.Length; u++)
                                    {
                                        SumMau += arrayMAU[int.Parse(TempItem[u])];
                                        SumMiu += arrayMIU[int.Parse(TempItem[u])];
                                    }

                                    int MAU = SumMau * LocalCount[j];

                                    if (MAU >= BorderMinUtil)
                                    {
                                        int MIU = SumMiu * LocalCount[j];

                                        Console.WriteLine(Nprefix + " " + j + ":" + LocalF1[j]);
                                        PKHUIs.Add(Nprefix + " " + j.ToString() + ":" + LocalF1[j].ToString());
                                        if (MIU > BorderMinUtil)
                                        {
                                            UpdateNodeCountHeap(ISNodeCountHeap, MIU);
                                        }
                                    }
                                }
                            }
                        }

                        if (CPB.Count == 0)
                        {

                        }
                        else
                        {
                            UPTree C_FPtree = new UPTree();

                            for (int k = 0; k < CPB.Count; k++)
                            {
                                List<int> ltran = CPB[k];

                                int Sum_MinBNF = 0;

                                int[] tran = new int[ltran.Count];
                                int tranlen = 0;

                                for (int h = 0; h < ltran.Count; h++)
                                {
                                    if (LocalF1[ltran[h]] >= BorderMinUtil)
                                    {
                                        Sum_MinBNF = Sum_MinBNF + CPBC[k] * arrayMIU[ltran[h]];

                                        tran[tranlen++] = ltran[h];
                                    }
                                    else
                                    {
                                        int sum = CPBW[k];
                                        sum = sum - CPBC[k] * arrayMIU[ltran[h]];
                                        CPBW[k] = sum;
                                    }
                                }

                                sorttrans(tran, 0, tranlen, LocalF1);

                                C_FPtree.InsertPatterBase(tran, tranlen, LocalF1, CPBW[k], CPBC[k], Sum_MinBNF);


                            }

                            C_FPtree.UPGrowth_MinBNF(C_FPtree, localflist, Nprefix, ISNodeCountHeap, LocalF1);

                        }
                    }
                }
            }

            // Tiềm kiếm PKHUIs bằng UP-GROWTH lần 2
            public void UPGrowth_MinBNF(UPTree tree2, List<int> flist2, String prefix, RedBlackTree<int> ISNodeCountHeap, int[] LP1)
            {
                for (int i = 0; i < flist2.Count; i++)
                {
                    if (LP1[flist2[i]] >= BorderMinUtil)
                    {
                        string Nprefix = "";
                        if (prefix == "")
                        {
                            Nprefix = prefix + flist2[i] + "";
                        }
                        else
                        {
                            Nprefix = prefix + " " + flist2[i];
                        }


                        int citem = flist2[i];
                        TreeNode chlink = tree2.HeaderTable[citem];


                        List<List<int>> CPB = new List<List<int>>(0);
                        List<int> CPBW = new List<int>(0);// lưu TWU từng mặt hàng

                        List<int> CPBC = new List<int>(0);// lưu count từng mặt hàng

                        int[] LocalF1 = new int[itemCount];
                        int[] LocalCount = new int[itemCount];

                        while (chlink != null)
                        {
                            List<int> path = new List<int>(0);

                            TreeNode cptr = chlink;

                            while (cptr.parentlink != null)
                            {
                                path.Add(cptr.item);

                                LocalF1[cptr.item] = LocalF1[cptr.item] + chlink.nu;
                                LocalCount[cptr.item] = LocalCount[cptr.item] + chlink.count;

                                cptr = cptr.parentlink;
                            }

                            path.RemoveAt(0);

                            CPB.Add(path);
                            CPBW.Add(chlink.nu);
                            CPBC.Add(chlink.count);

                            // turn to next horizontal link
                            chlink = chlink.hlink;
                        }


                        List<int> localflist = new List<int>(0);

                        for (int j = 0; j < LocalF1.Length; j++)
                        {
                            if (LocalF1[j] < BorderMinUtil)
                            {
                                LocalF1[j] = -1;
                            }
                            else
                            {
                                if (j != citem)
                                {
                                    InsertItem(localflist, j, LocalF1);


                                    string UTI = Nprefix + " " + j;
                                    string[] TempItem = UTI.Split(' ');
                                    int SumMau = 0;
                                    int SumMiu = 0;

                                    for (int u = 0; u < TempItem.Length; u++)
                                    {
                                        SumMau += arrayMAU[int.Parse(TempItem[u])];
                                        SumMiu += arrayMIU[int.Parse(TempItem[u])];
                                    }

                                    int MAU = SumMau * LocalCount[j];

                                    if (MAU >= BorderMinUtil)
                                    {
                                        int MIU = SumMiu * LocalCount[j];

                                        Console.WriteLine(Nprefix + " " + j + ":" + LocalF1[j]);
                                        PKHUIs.Add(Nprefix + " " + j.ToString() + ":" + LocalF1[j].ToString());
                                        if (MIU > BorderMinUtil)
                                        {
                                            UpdateNodeCountHeap(ISNodeCountHeap, MIU);
                                        }
                                    }
                                }
                            }
                        }

                        if (CPB.Count == 0)
                        {
                            // không làm gì
                        }
                        else
                        {
                            UPTree C_FPtree = new UPTree();

                            for (int k = 0; k < CPB.Count; k++)
                            {
                                List<int> ltran = CPB[k];

                                int Sum_MinBNF = 0;

                                int[] tran = new int[ltran.Count];
                                int tranlen = 0;

                                for (int h = 0; h < ltran.Count; h++)
                                {
                                    if (LocalF1[ltran[h]] >= BorderMinUtil)
                                    {
                                        Sum_MinBNF = Sum_MinBNF + CPBC[k] * arrayMIU[ltran[h]];

                                        tran[tranlen++] = ltran[h];
                                    }
                                    else
                                    {
                                        int sum = CPBW[k];
                                        sum = sum - CPBC[k] * arrayMIU[ltran[h]];

                                        CPBW[k] = sum;
                                    }
                                }
                                sorttrans(tran, 0, tranlen, LocalF1);

                                C_FPtree.InsertPatterBase(tran, tranlen, LocalF1, CPBW[k], CPBC[k], Sum_MinBNF);
                            }

                            C_FPtree.UPGrowth_MinBNF(C_FPtree, localflist, Nprefix, ISNodeCountHeap, LocalF1);
                        }
                    }
                }
            }
        }

        //Giai đoạn 2
        public class TKU2
        {
            //Ngưỡng hữu ích tối thiểu
            public static double minUtility;

            //Số K cần tìm
            public static int K;

            //Tổng số Số giao dịch
            public static int numTrans;

            //Số tập hữu ích top-k được tiềm thấy
            public static int numTopK;
            public static List<StringPair> hui = new List<StringPair>(0);
            public static List<StringPair> topk = new List<StringPair>();

            public void RunTKU2(double minUtil, int numTran, int k, string inputData, string sortPKHUIs)
            {
                minUtility = minUtil;
                numTrans = numTran;
                K = k;

                List<int>[] HDB = new List<int>[numTrans];
                List<int>[] BNF = new List<int>[numTrans];
                

                //Khởi tạo các HDB, BNF
                init(HDB, BNF, numTrans);

                //Đọc dữ liệu đầu vào
                readData(HDB, BNF, HDB.Length, inputData);

                //HUIs chiến lượt 5
                RedBlackTree<StringPair> hui = new RedBlackTree<StringPair>();
                //Đọc tập PKHUIs đã sắp xếp
                readPKHUIsSort(HDB, BNF, HDB.Length, sortPKHUIs, hui);

                //Console.WriteLine("=======HUIs========");
                ////Cũ, không áp dụng chiến lượt 5
                //foreach (var item in hui)
                //{
                //    Console.WriteLine(item.x + ":" + item.y);
                //}
                ////nếu từng tập hui mà lớn hơn hoặc bằng minutil là top-k
                //foreach (var item in hui)
                //{
                //    if (item.y >= minUtility)
                //    {
                //        topk.Add(item);
                //    }
                //}
                //Console.WriteLine("=======TOP-K========");
                ////Tập top-k
                //foreach (var item in topk)
                //{
                //    Console.WriteLine(item.x + ":" + item.y);
                //}
                //Console.WriteLine("===============");
                //Console.WriteLine("So cac tap PKHUIs = " + CountPKHUIs.ToString());
                //Console.WriteLine("===============");
                //Console.WriteLine("So cac tap HUIs {0}", hui.Count.ToString());
                //Console.WriteLine("===============");
                //Console.WriteLine("Tap huu ich cao top k voi K = " + topk.Count.ToString());
                //Console.WriteLine("===============");

                var dateTime = DateTime.Now.ToString("dd-MM-yyyy");

                string fileLPath = HttpContext.Current.Server.MapPath(@"~/Data/Output" + dateTime + ".txt");

                string[] lines = new string[hui.Count];
                int i = 0;
                Console.WriteLine("=======HUIs And TOP-K========");
                foreach (var item in hui)
                {
                    lines[i] = item.Value.x + ":" + item.Value.y;
                    Console.WriteLine(item.Value.x + ":" + item.Value.y);
                    i++;
                }

                System.IO.File.WriteAllLines(fileLPath, lines);

                //Console.WriteLine("===============");
                //Console.WriteLine("So cac tap PKHUIs = " + CountPKHUIs.ToString());
                //Console.WriteLine("===============");
                //Console.WriteLine("So cac tap HUIs {0}", hui.Count.ToString());
                //Console.WriteLine("===============");
                //Console.WriteLine("Tap huu ich cao top k voi K = " + hui.Count.ToString());
                //Console.WriteLine("===============");

            }
            //Khởi tạo
            public void init(List<int>[] HDB, List<int>[] BNF, int numtrans)
            {
                for (int i = 0; i < numtrans; i++)
                {
                    HDB[i] = new List<int>(0);
                    BNF[i] = new List<int>(0);
                }
            }

            //Đọc dữ liệu đầu vào
            public static void readData(List<int>[] HDB, List<int>[] BNF, int numtrans, string input)
            {
                string[] line = File.ReadAllLines(input);

                //int transcount = 0;

                for (int i = 0; i < line.Length; i++)
                {
                    string[] data = line[i].Split(':');//ghi tập input ra data[0] dữ liệu giao dịch
                                                       //data[1] là TU của giao dịch
                                                       //data[2] là EU của từng mặt hàng
                    string[] tran = data[0].Split(' ');

                    string[] eu = data[2].Split(' ');

                    for (int j = 0; j < tran.Length; j++)
                    {
                        HDB[i].Add(int.Parse(tran[j]));
                        BNF[i].Add(int.Parse(eu[j]));
                    }
                    //transcount++;
                }




            }

            //Đọc tập PKHUIs
            public void readPKHUIsSort(List<int>[] HDB, List<int>[] BNF, int numtrans, string sortPKHUIs,RedBlackTree<StringPair> HUIs)
            {
                //RedBlackTree<StringPair> Heap = new RedBlackTree<StringPair>(true);

                string[] line = File.ReadAllLines(sortPKHUIs);
                //duyệt PKHUIs
                for (int i = 0; i < line.Length; i++)
                {
                    string[] CI = line[i].Split(':');//tách PKHUIs ra làm 2 phần CI[0] là PKHUI, CI[1] là EU của một tập PKHUIs
                    int mathcount = 0;
                    int miu = 0;

                    string[] PKHUIs = CI[0].Split(' ');//tập PKHUIs

                    if (int.Parse(CI[1]) >= minUtility)
                    {                    
                        for (int j = 0; j < numtrans; j++)
                        {
                            if (HDB[j].Count()!=0)
                            {
                                mathcount = 0;
                                int PU = 0;

                                for (int s = 0; s < PKHUIs.Length; s++)
                                {
                                    if (HDB[j].Contains(int.Parse(PKHUIs[s])))
                                    {
                                        mathcount++;

                                        int index = HDB[j].IndexOf(int.Parse(PKHUIs[s]));

                                        List<int> B = BNF[j];

                                        int Ben = B[index];
                                        PU += Ben;
                                    }else
                                    {
                                        PU = 0;
                                        break;
                                    }
                                }

                                if (mathcount == PKHUIs.Length)
                                {
                                    miu += PU;
                                }
                            }
                        }
                        if (miu >= minUtility)
                        {
                            //TKU base
                            //StringPair h = new StringPair(CI[0], miu);
                            //hui.Add(h);
                            //UpdateHeap(HUIs, CI[0], miu);

                            //Chiến lượt 5
                            UpdateHeap(HUIs, CI[0], miu);
                        }
                    }
                }
            }

            //Update minheap chiến lượt 5
            public void UpdateHeap(RedBlackTree<StringPair> NCH, string PKHUI,int Util)
            {
                if (NCH.Count < K)
                {
                    NCH.Add(new StringPair(PKHUI, Util));
                }else if (NCH.Count>=K)
                {
                    if (Util > minUtility)
                    {
                        NCH.Add(new StringPair(PKHUI, Util));

                        NCH.Remove(NCH.Min());
                    }
                }
                if ((NCH.Min().Value.y > minUtility) && (NCH.Count >= K))
                {
                    minUtility = NCH.Min().Value.y;
                }
            }

        }

        public class StringPair : IComparable<StringPair>
        {
            public string x;
            public int y;
            public StringPair(string x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public int CompareTo(StringPair o)
            {
                return y - o.y;
            }
        }
    }
}

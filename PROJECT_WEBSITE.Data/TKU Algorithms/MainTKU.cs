using PROJECT_WEBSITE.Data.EF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PROJECT_WEBSITE.Data.TKU_Algorithms
{
    public class MainTKU
    {

        public string GhiFileTxt()
        {
            try
            {

                DbWebsite db = new DbWebsite();
                var dateTime = DateTime.Now.ToString("dd-MM-yyyy");
                ////ghi file PKHUIs

                string filename = "Database" + dateTime + ".txt";
                string fileLPath = HttpContext.Current.Server.MapPath(@"~/Data/"+ filename);
                int Tranlen = db.Orders.Count();
                List<Order> orders = db.Orders.ToList();
                string[] lines = new string[Tranlen];
                int linecount = 0;

                foreach (var item in orders)
                {

                    // mảng các mặt hàng trong một giao dịch
                    int itemTranlen = db.OrderDetails.Count(x => x.OrderID == item.OrderID);
                    string[] arrayItem = new string[itemTranlen];
                    List<OrderDetail> orderTran = db.OrderDetails.Where(x => x.OrderID == item.OrderID).ToList();
                    for (int i = 0; i < itemTranlen; i++)
                    {
                        arrayItem[i] = orderTran[i].ProductID.ToString();
                    }

                    //TU của một giao dịch
                    decimal? TU = 0;
                    foreach (var item1 in orderTran)
                    {
                        TU += item1.OrderDetailCount * item1.OrderPrice;
                    }

                    //mảng các EU của từng mặt hàng trong một giao dịch
                    string[] arrayEUItem = new string[itemTranlen];
                    for (int i = 0; i < itemTranlen; i++)
                    {
                        decimal? ac = orderTran[i].OrderPrice;
                        arrayEUItem[i] = (orderTran[i].OrderDetailCount * ac).ToString();
                    }


                    //Thêm item vào
                    string[] tran = new string[(itemTranlen * 2) + 3];
                    for (int i = 0; i < itemTranlen; i++)
                    {
                        tran[i] = arrayItem[i];
                    }

                    //Thêm EU vào
                    int k = 0;
                    //for (int i = itemTranlen + 3; i < (itemTranlen * 2) + 3; i++)
                    //{
                    //    tran[i] = arrayEUItem[k];
                    //    k++;
                    //}



                    string t = "";
                    int temp = 0;
                    for (int i = 0; i < itemTranlen; i++)
                    {
                        if (i == itemTranlen - 1)
                        {
                            t = t + tran[i];
                        }
                        else
                        {
                            t = t + tran[i] + " ";
                            temp++;
                        }
                    }
                    //thêm TU vào
                    t = t + ":";

                    t = t + TU.ToString();

                    t = t + ":";

                    for (int i = itemTranlen + 3; i < (itemTranlen * 2) + 3; i++)
                    {
                        if (i == tran.Length - 1)
                        {
                            t = t + arrayEUItem[k];
                        }
                        else
                        {
                            t = t + arrayEUItem[k] + " ";
                            k++;
                        }
                    }
                    lines[linecount] = t;
                    linecount++;
                }

                System.IO.File.WriteAllLines(fileLPath, lines);




                return filename;
            }
            catch (Exception)
            {

                return null;
            }
        }


        public bool Main(string file, int topK)
        {
            DbWebsite db = new DbWebsite();
            ArrayList samplesList = new ArrayList();
            Stopwatch sw;
            sw = Stopwatch.StartNew();

            TKU xl = new TKU();
            //tinh item count, db size
            CalculateDB calcu = new CalculateDB(file);
            calcu.runCal();
            int ItemCountFile = calcu.getMaxID();
            int SizeDatabaseFile = calcu.getDBSize();


            //Số mặt hàng trong giao dịch
            int ItemCount = db.OrderDetails.OrderByDescending(t=>t.ProductID).Take(1).FirstOrDefault().ProductID;
            int SizeDatabase = db.Orders.Count();


            //đọc file data set(file txt trong debug,số k);
            xl.RunTKU(file, topK, ItemCount, SizeDatabase);

            return true;


            ////Số mặt hàng trong giao dịch
            //int ItemCount = db.OrderDetails.GroupBy(x => x.ProductID).Count();
            //int SizeDatabase = db.Orders.Count();
            ////chạy thuật toán
            //xl.RunTKU("Database.txt", 3, ItemCount, SizeDatabase);
            //Console.WriteLine("Thanh cong");


            //Console.WriteLine("Thanh cong");
            //sw.Stop();
            //if (sw.ElapsedMilliseconds > 1000)
            //{
            //    Console.WriteLine("Thoi gian {0}s", sw.ElapsedMilliseconds.ToString("N0"));
            //}
            //else
            //{
            //    Console.WriteLine("Thoi gian {0}ms", sw.ElapsedMilliseconds);
            //}
            //Console.ReadLine();
        }
    }
}

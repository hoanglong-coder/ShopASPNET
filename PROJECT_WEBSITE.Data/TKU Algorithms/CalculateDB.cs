using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.TKU_Algorithms
{
    public class CalculateDB
    {
        public static string inputPath;
        public static int databaseSize = 0;
        public static int maxID = 0;
        public static HashSet<int> allItem = new HashSet<int>();

        public CalculateDB(string input)
        {
            inputPath = input;
        }

        public void runCal()
        {
            string[] lines = File.ReadAllLines(inputPath);

            foreach (var item in lines)
            {
                if (item == null)
                {
                    break;
                }
                databaseSize++;
                string[] t1 = item.Split(':');
                string[] t2 = t1[0].Split(' ');
                for (int i = 0; i < t2.Length; i++)
                {
                    int num = int.Parse(t2[i]);
                    allItem.Add(num);
                }
            }
        }
        public int getMaxID()
        {
            return allItem.Count;
        }
        public int getDBSize()
        {
            return databaseSize;
        }
    }

}


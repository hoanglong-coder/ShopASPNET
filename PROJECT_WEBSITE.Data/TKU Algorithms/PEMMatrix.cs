using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROJECT_WEBSITE.Data.TKU_Algorithms
{
    public class PEMMatrix
    {
        public int[][] matrix { get; set; }

        public int Count;

		public PEMMatrix(int elementCount)
		{
			Count = elementCount;
			matrix = new int[elementCount][];
			for (int i = 0; i < elementCount; i++)
			{
				matrix[i] = new int[elementCount - i];
			}
		}
		public void InsertItemMatrix(int id1, int id2, int sum)
		{
			if (id2 < id1)
			{

				matrix[id2][Count - id1 - 1] += sum;
			}
			else
			{

				matrix[id1][Count - id2 - 1] += sum;

			}
		}

	}
}

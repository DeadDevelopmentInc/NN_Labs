using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadialNetwork
{
    public class Helpers
    {
        public static double[][] MakeMatrix(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new double[cols];
            return result;
        }

        public static void ShowVector(double[] vector, int decimals, int valsPerLine, bool blankLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i > 0 && i % valsPerLine == 0) // max of 12 values per row 
                    Console.WriteLine("");
                if (vector[i] >= 0.0) Console.Write(" ");
                Console.Write(vector[i].ToString("F" + decimals) + " "); // n decimals
            }
            if (blankLine) Console.WriteLine("\n");
        }

        public static void ShowVector(int[] vector, int valsPerLine, bool blankLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i > 0 && i % valsPerLine == 0) // max of 12 values per row 
                    Console.WriteLine("");
                if (vector[i] >= 0.0) Console.Write(" ");
                Console.Write(vector[i] + " ");
            }
            if (blankLine) Console.WriteLine("\n");
        }

        public static void ShowMatrix(double[][] matrix, int numRows, int decimals, bool lineNumbering, bool showLastLine)
        {
            int ct = 0;
            if (numRows == -1) numRows = int.MaxValue; // if numRows == -1, show all rows
            for (int i = 0; i < matrix.Length && ct < numRows; ++i)
            {
                if (lineNumbering == true)
                    Console.Write(i.ToString().PadLeft(3) + ": ");
                for (int j = 0; j < matrix[0].Length; ++j)
                {
                    if (matrix[i][j] >= 0.0) Console.Write(" "); // blank space instead of '+' sign
                    Console.Write(matrix[i][j].ToString("F" + decimals) + " ");
                }
                Console.WriteLine("");
                ++ct;
            }
            if (showLastLine == true && numRows < matrix.Length)
            {
                Console.WriteLine("      ........\n ");
                int i = matrix.Length - 1;
                Console.Write(i.ToString().PadLeft(3) + ": ");
                for (int j = 0; j < matrix[0].Length; ++j)
                {
                    if (matrix[i][j] >= 0.0) Console.Write(" "); // blank space instead of '+' sign
                    Console.Write(matrix[i][j].ToString("F" + decimals) + " ");
                }
            }
            Console.WriteLine("");
        }

    } // class Helpers
}

using System;
using System.Text;

public class Matrix
{
    public double[,] Data { get; set; }

    public Matrix(int size)
    {
        Data = new double[size, size];
    }

    public Matrix(double[,] data)
    {
        Data = data;
    }

    public int Size => Data.GetLength(0);

    public double Determinant()
    {
        if (Data.GetLength(0) != Data.GetLength(1))
        {
            throw new InvalidOperationException("Matrix must be square.");
        }

        return CalculateDeterminant(Data);
    }

    private double CalculateDeterminant(double[,] matrix)
    {
        int n = matrix.GetLength(0);

        if (n == 1)
        {
            return matrix[0, 0];
        }
        if (n == 2)
        {
            return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        }

        double det = 0;
        for (int p = 0; p < n; p++)
        {
            double[,] subMatrix = new double[n - 1, n - 1];
            for (int i = 1; i < n; i++)
            {
                int subCol = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == p)
                    {
                        continue;
                    }
                    subMatrix[i - 1, subCol] = matrix[i, j];
                    subCol++;
                }
            }
            det += matrix[0, p] * Math.Pow(-1, p) * CalculateDeterminant(subMatrix);
        }
        return det;
    }

    public static Matrix GenerateRandomMatrix(int size, int minValue = 0, int maxValue = 10)
    {
        Random random = new Random();
        double[,] data = new double[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                data[i, j] = random.Next(minValue, maxValue);
            }
        }

        return new Matrix(data);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                sb.Append(Math.Round(Data[i, j], 3).ToString("F3")).Append("\t");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }


    private static string MatrixToString(double[,] matrix)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                sb.Append(matrix[i, j].ToString("F4")).Append("\t");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

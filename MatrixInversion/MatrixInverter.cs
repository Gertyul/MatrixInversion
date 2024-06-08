using System.Diagnostics;
using System.Text;
using System;

public static class MatrixInverter
{
    /// <summary>
    /// Інвертує матрицю, використовуючи метод ітерацій Шульца.
    /// </summary>
    /// <param name="matrix">Матриця, яку потрібно інвертувати.</param>
    /// <param name="log">Журнал процесу ітерацій.</param>
    /// <param name="timeElapsed">Час, витрачений на процес інверсії.</param>
    /// <param name="operationCount">Кількість виконаних операцій.</param>
    /// <returns>Інвертована матриця.</returns>
    public static Matrix InvertUsingSchulz(Matrix matrix, out string log, out double timeElapsed, out long operationCount)
    {
        int n = matrix.Size;
        double[,] A = matrix.Data;
        double[,] I = CreateIdentityMatrix(n);
        double[,] X = CreateInitialApproximation(A, n);
        double[,] R;
        double[,] X_next = X;

        log = "Initial approximation (X0):\n";
        log += MatrixToString(X);

        int maxIterations = 100;
        double tolerance = 1e-10;
        bool converged = false;
        operationCount = 0;

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int k = 0; k < maxIterations; k++)
        {
            R = MatrixSubtract(I, MatrixMultiply(A, X, ref operationCount)); // Матриця залишків
            X_next = MatrixAdd(X, MatrixMultiply(X, R, ref operationCount)); // Оновлення наближення

            operationCount += n * n * n;

            if (MatrixNorm(MatrixSubtract(X_next, X)) < tolerance)
            {
                converged = true;
                break;
            }

            X = X_next;

            log += $"\nIteration {k + 1} (X{k + 1}):\n";
            log += MatrixToString(X);
        }

        stopwatch.Stop();
        timeElapsed = stopwatch.Elapsed.TotalMilliseconds;

        if (!converged)
        {
            log += "\nMethod did not converge.";
            throw new Exception("Method did not converge.");
        }

        log += "\nMethod converged successfully.";
        log += $"\nTime elapsed: {timeElapsed} ms";
        log += $"\nIterations count: {operationCount}";
        return new Matrix(X_next);
    }

    /// <summary>
    /// Створює початкове наближення для інверсії матриці.
    /// </summary>
    /// <param name="A">Вхідна матриця.</param>
    /// <param name="n">Розмір матриці.</param>
    /// <returns>Початкове наближення.</returns>
    private static double[,] CreateInitialApproximation(double[,] A, int n)
    {
        double[,] At = MatrixTranspose(A);
        double normA = MatrixNorm(A);
        double normAt = MatrixNorm(At);

        double alpha = 1.0 / (normA * normAt);
        double[,] X0 = MatrixMultiplyByScalar(At, alpha);

        return X0;
    }

    /// <summary>
    /// Додає дві матриці.
    /// </summary>
    /// <param name="A">Перша матриця.</param>
    /// <param name="B">Друга матриця.</param>
    /// <returns>Результат додавання.</returns>
    private static double[,] MatrixAdd(double[,] A, double[,] B)
    {
        int n = A.GetLength(0);
        double[,] result = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                result[i, j] = A[i, j] + B[i, j];
            }
        }

        return result;
    }

    /// <summary>
    /// Віднімає другу матрицю від першої.
    /// </summary>
    /// <param name="A">Перша матриця.</param>
    /// <param name="B">Друга матриця.</param>
    /// <returns>Результат віднімання.</returns>
    private static double[,] MatrixSubtract(double[,] A, double[,] B)
    {
        int n = A.GetLength(0);
        double[,] result = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                result[i, j] = A[i, j] - B[i, j];
            }
        }

        return result;
    }

    /// <summary>
    /// Множить дві матриці.
    /// </summary>
    /// <param name="A">Перша матриця.</param>
    /// <param name="B">Друга матриця.</param>
    /// <param name="operationCount">Лічильник операцій.</param>
    /// <returns>Результат множення.</returns>
    private static double[,] MatrixMultiply(double[,] A, double[,] B, ref long operationCount)
    {
        int n = A.GetLength(0);
        double[,] result = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                result[i, j] = 0;
                for (int k = 0; k < n; k++)
                {
                    result[i, j] += A[i, k] * B[k, j];
                    
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Транспонує матрицю.
    /// </summary>
    /// <param name="A">Вхідна матриця.</param>
    /// <returns>Транспонована матриця.</returns>
    private static double[,] MatrixTranspose(double[,] A)
    {
        int n = A.GetLength(0);
        double[,] At = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                At[j, i] = A[i, j];
            }
        }

        return At;
    }

    /// <summary>
    /// Обчислює норму матриці.
    /// </summary>
    /// <param name="A">Вхідна матриця.</param>
    /// <returns>Норма матриці.</returns>
    private static double MatrixNorm(double[,] A)
    {
        double sum = 0;
        foreach (double value in A)
        {
            sum += value * value;
        }

        return Math.Sqrt(sum);
    }

    /// <summary>
    /// Створює одиничну матрицю заданого розміру.
    /// </summary>
    /// <param name="n">Розмір матриці.</param>
    /// <returns>Одинична матриця.</returns>
    private static double[,] CreateIdentityMatrix(int n)
    {
        double[,] I = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            I[i, i] = 1;
        }

        return I;
    }

    /// <summary>
    /// Множить матрицю на скаляр.
    /// </summary>
    /// <param name="A">Вхідна матриця.</param>
    /// <param name="scalar">Скаляр.</param>
    /// <returns>Результат множення.</returns>
    private static double[,] MatrixMultiplyByScalar(double[,] A, double scalar)
    {
        int n = A.GetLength(0);
        double[,] result = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                result[i, j] = A[i, j] * scalar;
            }
        }

        return result;
    }

    /// <summary>
    /// Перетворює матрицю у рядкове представлення.
    /// </summary>
    /// <param name="matrix">Вхідна матриця.</param>
    /// <returns>Рядкове представлення матриці.</returns>
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

    /// <summary>
    /// Інвертує матрицю, використовуючи метод LU-розкладу.
    /// </summary>
    /// <param name="matrix">Матриця, яку потрібно інвертувати.</param>
    /// <param name="log">Журнал процесу.</param>
    /// <param name="timeElapsed">Час, витрачений на процес інверсії.</param>
    /// <param name="operationCount">Кількість виконаних операцій.</param>
    /// <returns>Інвертована матриця.</returns>
    public static Matrix InvertUsingLUP(Matrix matrix, out string log, out double timeElapsed, out long operationCount)
    {
        int n = matrix.Size;
        double[,] A = matrix.Data;
        double[,] L = new double[n, n];
        double[,] U = new double[n, n];
        int[] P = new int[n];
        log = string.Empty;
        operationCount = 0;

        Stopwatch stopwatch = Stopwatch.StartNew();

        if (!LUPDecompose(A, L, U, P, ref operationCount, ref log))
        {
            timeElapsed = stopwatch.Elapsed.TotalMilliseconds;
            throw new Exception("Matrix is singular and cannot be inverted.");
        }

        double[,] invA = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            double[] e = new double[n];
            e[i] = 1;

            double[] y = ForwardSubstitution(L, P, e, ref operationCount);
            double[] x = BackSubstitution(U, y, ref operationCount);

            for (int j = 0; j < n; j++)
            {
                invA[j, i] = x[j];
            }
        }

        stopwatch.Stop();
        timeElapsed = stopwatch.Elapsed.TotalMilliseconds;

        log += "\nInverted matrix:\n";
        log += MatrixToString(invA);
        log += $"\nTime elapsed: {timeElapsed} ms";
        log += $"\nIterations count: {operationCount}";

        return new Matrix(invA);
    }

    /// <summary>
    /// Виконує LU-розклад матриці з частковим поворотом.
    /// </summary>
    /// <param name="A">Вхідна матриця.</param>
    /// <param name="L">Нижня трикутна матриця.</param>
    /// <param name="U">Верхня трикутна матриця.</param>
    /// <param name="P">Вектор перестановок.</param>
    /// <param name="operationCount">Лічильник операцій.</param>
    /// <param name="log">Журнал процесу розкладу.</param>
    /// <returns>True, якщо розклад успішний, False у випадку виродженої матриці.</returns>
    private static bool LUPDecompose(double[,] A, double[,] L, double[,] U, int[] P, ref long operationCount, ref string log)
    {
        int n = A.GetLength(0);
        double[,] LU = (double[,])A.Clone();

        for (int i = 0; i < n; i++)
        {
            P[i] = i;
        }

        for (int k = 0; k < n; k++)
        {
            double max = 0;
            int kPrime = -1;

            for (int i = k; i < n; i++)
            {
                if (Math.Abs(LU[i, k]) > max)
                {
                    max = Math.Abs(LU[i, k]);
                    kPrime = i;
                }
            }

            if (max < 1e-10)
            {
                return false;
            }

            if (kPrime != k)
            {
                int temp = P[k];
                P[k] = P[kPrime];
                P[kPrime] = temp;

                for (int i = 0; i < n; i++)
                {
                    double t = LU[k, i];
                    LU[k, i] = LU[kPrime, i];
                    LU[kPrime, i] = t;
                }
            }

            for (int i = k + 1; i < n; i++)
            {
                LU[i, k] /= LU[k, k];
                

                for (int j = k + 1; j < n; j++)
                {
                    LU[i, j] -= LU[i, k] * LU[k, j];
                    
                }
            }
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (i > j)
                {
                    L[i, j] = LU[i, j];
                    U[i, j] = 0;
                }
                else
                {
                    L[i, j] = (i == j) ? 1 : 0;
                    U[i, j] = LU[i, j];
                }
            }
        }

        log += "\nL matrix:\n" + MatrixToString(L);
        log += "\nU matrix:\n" + MatrixToString(U);
        log += "\nP vector: " + string.Join(", ", P);

        return true;
    }

    /// <summary>
    /// Виконує пряме підстановлення.
    /// </summary>
    /// <param name="L">Нижня трикутна матриця.</param>
    /// <param name="P">Вектор перестановок.</param>
    /// <param name="b">Вектор правої частини.</param>
    /// <param name="operationCount">Лічильник операцій.</param>
    /// <returns>Результуючий вектор.</returns>
    private static double[] ForwardSubstitution(double[,] L, int[] P, double[] b, ref long operationCount)
    {
        int n = L.GetLength(0);
        double[] y = new double[n];

        for (int i = 0; i < n; i++)
        {
            y[i] = b[P[i]];
            for (int j = 0; j < i; j++)
            {
                y[i] -= L[i, j] * y[j];
                operationCount += 2; 
            }
        }

        return y;
    }

    /// <summary>
    /// Виконує зворотне підстановлення.
    /// </summary>
    /// <param name="U">Верхня трикутна матриця.</param>
    /// <param name="y">Проміжний вектор.</param>
    /// <param name="operationCount">Лічильник операцій.</param>
    /// <returns>Результуючий вектор.</returns>
    private static double[] BackSubstitution(double[,] U, double[] y, ref long operationCount)
    {
        int n = U.GetLength(0);
        double[] x = new double[n];

        for (int i = n - 1; i >= 0; i--)
        {
            x[i] = y[i];
            for (int j = i + 1; j < n; j++)
            {
                x[i] -= U[i, j] * x[j];
                
            }
            x[i] /= U[i, i];
            operationCount++;
        }

        return x;
    }
}
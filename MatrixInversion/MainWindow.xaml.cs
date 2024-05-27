using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MatrixInversion
{
    public partial class MainWindow : Window
    {
        private Matrix originalMatrix;
        private Matrix invertedMatrix;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InputMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            InputMatrixDialog dialog = new InputMatrixDialog();
            if (dialog.ShowDialog() == true)
            {
                originalMatrix = dialog.Matrix;
                OriginalMatrixDataGrid.ItemsSource = ToDataTable(originalMatrix.Data).DefaultView;
            }
        }

        private void GenerateMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int size = int.Parse(MatrixSizeTextBox.Text);

                if (size <= 0)
                {
                    MessageBox.Show("Matrix size must be a positive integer.", "Invalid Matrix Size", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                originalMatrix = Matrix.GenerateRandomMatrix(size);
                OriginalMatrixDataGrid.ItemsSource = ToDataTable(originalMatrix.Data).DefaultView;
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid integer for the matrix size.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InvertMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            if (originalMatrix != null)
            {
                try
                {
                    double determinant = originalMatrix.Determinant();
                    if (Math.Abs(determinant) < 1e-10)
                    {
                        MessageBox.Show("The matrix is not invertible.");
                        return;
                    }

                    string log;
                    double timeElapsed;
                    long operationCount;

                    if (SchulzMethodRadioButton.IsChecked == true)
                    {
                        invertedMatrix = MatrixInverter.InvertUsingSchulz(originalMatrix, out log, out timeElapsed, out operationCount);
                    }
                    else if (LUPMethodRadioButton.IsChecked == true)
                    {
                        invertedMatrix = MatrixInverter.InvertUsingLUP(originalMatrix, out log, out timeElapsed, out operationCount);
                    }
                    else
                    {
                        MessageBox.Show("Please select a method for matrix inversion.");
                        return;
                    }

                    double[,] roundedInvertedMatrix = RoundMatrix(invertedMatrix.Data, 3);
                    InvertedMatrixDataGrid.ItemsSource = ToDataTable(roundedInvertedMatrix).DefaultView;

                    // Save to file
                    string filePath = "MatrixInversionResult.txt";
                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine("Original Matrix:");
                        writer.WriteLine(originalMatrix);
                        writer.WriteLine("\nInverted Matrix:");
                        writer.WriteLine(new Matrix(roundedInvertedMatrix));
                        writer.WriteLine("\nCalculation Log:");
                        writer.WriteLine(log);
                        writer.WriteLine($"\nTime Elapsed: {timeElapsed} ms");
                        writer.WriteLine($"Operation Count: {operationCount}");
                    }

                    MessageBox.Show($"The result has been saved to {filePath}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please input or generate a matrix first.");
            }
        }

        private DataTable ToDataTable(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            DataTable dt = new DataTable();

            for (int i = 0; i < cols; i++)
            {
                dt.Columns.Add(new DataColumn((i + 1).ToString()));
            }

            for (int i = 0; i < rows; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < cols; j++)
                {
                    dr[j] = Math.Round(matrix[i, j], 3);
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private double[,] RoundMatrix(double[,] matrix, int decimals)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            double[,] roundedMatrix = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    roundedMatrix[i, j] = Math.Round(matrix[i, j], decimals);
                }
            }

            return roundedMatrix;
        }
    }
}

using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Windows;

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

        private void SaveResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (originalMatrix == null || invertedMatrix == null)
            {
                MessageBox.Show("Please generate or input a matrix and invert it first.", "No Data to Save", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        writer.WriteLine("Original Matrix:");
                        writer.WriteLine(originalMatrix.ToString());
                        writer.WriteLine("\nInverted Matrix:");
                        writer.WriteLine(new Matrix(RoundMatrix(invertedMatrix.Data, 3)).ToString());

                        if (SchulzMethodRadioButton.IsChecked == true)
                        {
                            string log;
                            double timeElapsed;
                            long operationCount;
                            MatrixInverter.InvertUsingSchulz(originalMatrix, out log, out timeElapsed, out operationCount);
                            writer.WriteLine("\nSchulz Method Log:");
                            writer.WriteLine(log);
                        }
                        else if (LUPMethodRadioButton.IsChecked == true)
                        {
                            string log;
                            double timeElapsed;
                            long operationCount;
                            MatrixInverter.InvertUsingLUP(originalMatrix, out log, out timeElapsed, out operationCount);
                            writer.WriteLine("\nLUP Method Log:");
                            writer.WriteLine(log);
                        }
                    }

                    MessageBox.Show($"The result has been saved to {saveFileDialog.FileName}", "File Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving the file: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
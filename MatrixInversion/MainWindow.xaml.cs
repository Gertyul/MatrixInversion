using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MatrixInversion
{
    public partial class MainWindow : Window
    {
        private Matrix originalMatrix;
        private Matrix invertedMatrix;

        public MainWindow()
        {
            InitializeComponent();

            // Додаємо подію для всіх кнопок
            InputMatrixButton.Click += Button_Click;
            GenerateMatrixButton.Click += Button_Click;
            InvertMatrixButton.Click += Button_Click;
            SaveResultButton.Click += Button_Click;
        }

        // Обробник анімації кнопок
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                Storyboard storyboard = (Storyboard)FindResource("ButtonClickStoryboard");
                storyboard.Begin(button);
            }
        }

        // Обробник кнопки введення матриці
        private void InputMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            InputMatrixDialog dialog = new InputMatrixDialog(originalMatrix);

            if (dialog.ShowDialog() == true)
            {
                if (MatrixHasInvalidValues(dialog.Matrix))
                {
                    MessageBox.Show("Matrix elements must not exceed 5000.", "Invalid Matrix Values", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                originalMatrix = dialog.Matrix;
                OriginalMatrixDataGrid.ItemsSource = ToDataTable(originalMatrix.Data).DefaultView;
            }
        }


        private void LoadMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    int size = lines.Length;
                    double[,] data = new double[size, size];

                    for (int i = 0; i < size; i++)
                    {
                        string[] elements = lines[i].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < size; j++)
                        {
                            double value = double.Parse(elements[j]);
                            if (Math.Abs(value) > 5000)
                            {
                                MessageBox.Show("Matrix elements must not exceed 5000.", "Invalid Matrix Values", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            
                            data[i, j] = value;
                        }
                    }

                    originalMatrix = new Matrix(data);
                    OriginalMatrixDataGrid.ItemsSource = ToDataTable(originalMatrix.Data).DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while loading the file: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Обробник кнопки генерації матриці
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
                if (size > 12)
                {
                    MessageBox.Show("Matrix size can`t over than 12.", "Invalid Matrix Size", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                originalMatrix = Matrix.GenerateRandomMatrix(size);
                if (MatrixHasInvalidValues(originalMatrix))
                {
                    MessageBox.Show("Matrix elements must not exceed 5000.", "Invalid Matrix Values", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                OriginalMatrixDataGrid.ItemsSource = ToDataTable(originalMatrix.Data).DefaultView;
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid integer for the matrix size.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обробник кнопки інверсії матриці
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

        // Обробник кнопки збереження результату
        private void SaveResultButton_Click(object sender, RoutedEventArgs e)
        {
            if (originalMatrix == null || invertedMatrix == null)
            {
                MessageBox.Show("Please generate or input a matrix and invert it first.", "No Data to Save", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text file (*.txt)|*.txt"
            };
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

        // Перетворення матриці в DataTable
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

        // Округлення матриці до вказаної кількості знаків після коми
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

        //валідація при числі матриці більшому за 5000
        private bool MatrixHasInvalidValues(Matrix matrix)
        {
            foreach (double value in matrix.Data)
            {
                if (Math.Abs(value) > 5000)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace MatrixInversion
{
    public partial class InputMatrixDialog : Window
    {
        // Властивість для збереження матриці.
        public Matrix Matrix { get; private set; }

        // Конструктор вікна.
        public InputMatrixDialog(Matrix matrix = null)
        {
            InitializeComponent();

            if (matrix != null)
            {
                Matrix = matrix;
                MatrixDataGrid.ItemsSource = ToDataTable(Matrix.Data).DefaultView;
            }
        }

        // Обробник події натискання кнопки Create.
        private void CreateMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            int size;
            // Перевірка чи введено правильний розмір матриці.
            if (int.TryParse(MatrixSizeTextBox.Text, out size) && size > 0 && size <= 12)
            {
                Matrix = new Matrix(size); // Створення нової матриці.
                MatrixDataGrid.ItemsSource = ToDataTable(Matrix.Data).DefaultView; // Заповнення DataGrid даними матриці.
            }
            else
            {
                MessageBox.Show("Please enter a valid matrix size. It should be integer and not higher than 12"); // Відображення повідомлення про помилку.
            }
        }

        // Обробник події натискання кнопки OK.
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int size = MatrixDataGrid.Items.Count;
                double[,] data = new double[size, size];

                if (Matrix != null)
                {
                    // Заповнення даних матриці з DataGrid.
                    for (int i = 0; i < size; i++)
                    {
                        var row = MatrixDataGrid.Items[i] as DataRowView;
                        if (row != null)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                data[i, j] = double.Parse(row[j].ToString());
                            }
                        }
                    }
                    Matrix = new Matrix(data);
                    DialogResult = true; // Закриття діалогу з позитивним результатом.
                }
                else
                {
                    MessageBox.Show("Please create and input the matrix data."); // Відображення повідомлення про помилку.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while parsing the matrix: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // Обробник події натискання кнопки Cancel.
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Закриття діалогу з негативним результатом.
        }

        // Метод для перетворення двовимірного масиву в DataTable.
        private DataTable ToDataTable(double[,] matrix)
        {
            var dataTable = new DataTable();

            int rows = matrix.GetLength(0); // Кількість рядків.
            int cols = matrix.GetLength(1); // Кількість стовпців.

            for (int i = 0; i < cols; i++)
            {
                dataTable.Columns.Add(i.ToString(), typeof(double)); // Додавання стовпців в DataTable.
            }

            for (int i = 0; i < rows; i++)
            {
                var row = dataTable.NewRow();
                for (int j = 0; j < cols; j++)
                {
                    row[j] = matrix[i, j]; // Заповнення рядків DataTable.
                }
                dataTable.Rows.Add(row); // Додавання рядків в DataTable.
            }

            return dataTable; // Повернення DataTable.
        }
    }
}
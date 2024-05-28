using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace MatrixInversion
{
    public partial class InputMatrixDialog : Window
    {
        public Matrix Matrix { get; private set; }

        public InputMatrixDialog()
        {
            InitializeComponent();
        }

        private void CreateMatrixButton_Click(object sender, RoutedEventArgs e)
        {
            int size;
            if (int.TryParse(MatrixSizeTextBox.Text, out size) && size > 0)
            {
                Matrix = new Matrix(size);
                MatrixDataGrid.ItemsSource = ToDataTable(Matrix.Data).DefaultView;
            }
            else
            {
                MessageBox.Show("Please enter a valid matrix size.");
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (Matrix != null)
            {
                for (int i = 0; i < Matrix.Data.GetLength(0); i++)
                {
                    for (int j = 0; j < Matrix.Data.GetLength(1); j++)
                    {
                        Matrix.Data[i, j] = Convert.ToDouble((MatrixDataGrid.Items[i] as DataRowView)[j]);
                    }
                }
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please create and input the matrix data.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private DataTable ToDataTable(double[,] matrix)
        {
            var dataTable = new DataTable();

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < cols; i++)
            {
                dataTable.Columns.Add(i.ToString(), typeof(double));
            }

            for (int i = 0; i < rows; i++)
            {
                var row = dataTable.NewRow();
                for (int j = 0; j < cols; j++)
                {
                    row[j] = matrix[i, j];
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
using BooksWPF.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.IO;

namespace BooksWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Book> books = new List<Book>();
        List<Country> countries = new List<Country>();
        public MainWindow()
        {
            InitializeComponent();
            PopulateListBooks();
            FillCboCountries();
        }
        private List<Book> GetAllBooks()
        {
            string sql = "SELECT * FROM Book";
            using (IDbConnection connection = new SqlConnection(GetConnectionString.ConStr("WPFBooks")))
            {
                books = connection.Query<Book>(sql).ToList();
                return books;
            }
        }
        private void PopulateListBooks()
        {
            lstBooks.ItemsSource = GetAllBooks();
        }
        private List<Country> GetAllCountries()
        {
            string sql = "SELECT * FROM Country";
            using (IDbConnection connection = new SqlConnection(GetConnectionString.ConStr("WPFBooks")))
            {
                countries = connection.Query<Country>(sql).ToList();
                return countries;
            }
        }
        private void FillCboCountries()
        {
            cboCountry.ItemsSource = GetAllCountries();
            cboCountry.DisplayMemberPath = "CountryName";
            cboCountry.SelectedValuePath = "Id";
        }
        private void btnAddBook_Click(object sender, RoutedEventArgs e)
        {
            lblId.Content = "";
            Book newBook = new Book();
            newBook.Author = txtAuthor.Text;
            newBook.Title = txtTitle.Text;
            newBook.Price = decimal.Parse(txtPrice.Text);
            newBook.CountryId = (int)cboCountry.SelectedValue;
            int id = AddBook(newBook);
            ClearFields();
            PopulateListBooks();
            lblId.Content = id.ToString() + " Created!";
        }
        private void ClearFields()
        {
            lblId.Content = string.Empty;
            txtAuthor.Text = string.Empty;
            txtTitle.Text = string.Empty;
            txtPrice.Text = string.Empty;
            cboCountry.SelectedIndex = -1;
        }
        private int AddBook(Book newbook)
        {
            string sql = "INSERT INTO Book (Author, Title ,Price, CountryId)" +
                "VALUES (@Author, @Title, @Price, @CountryId) " +
                "SELECT CAST(SCOPE_IDENTITY() as int)";
            using (IDbConnection connection = new SqlConnection(GetConnectionString.ConStr("WPFBooks")))
            {
                var returnedId = connection.Query<int>(sql, newbook).Single();
                return returnedId;
            }
        }
        private void lstBooks_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Book selectedBook = new Book();
            selectedBook = lstBooks.SelectedItem as Book;
            if (selectedBook != null)
            {
                lblId.Content = selectedBook.Id;
                txtAuthor.Text = selectedBook.Author;
                txtTitle.Text = selectedBook.Title;
                txtPrice.Text = selectedBook.Price.ToString();
                cboCountry.SelectedValue = selectedBook.CountryId;
            }
        }
        private void UpdateBook(Book updateBook)
        {
            string sql = "UPDATE Book " +
                         "SET Author = @Author, Title = @Title, Price = @Price, CountryId = @CountryId " +
                         "WHERE Id = @Id";
            using (IDbConnection connection = new SqlConnection(GetConnectionString.ConStr("WPFBooks")))
            {
                connection.Execute(sql,
                new
                {
                    Id = updateBook.Id,
                    Author = updateBook.Author,
                    Title = updateBook.Title,
                    Price = updateBook.Price,
                    CountryId = updateBook.CountryId,
                });
            }
        }
        private void btnUpdateBook_Click(object sender, RoutedEventArgs e)
        {
            Book updatedBook = new Book();
            updatedBook.Id = (int)lblId.Content;
            updatedBook.Title = txtTitle.Text;
            updatedBook.Author = txtAuthor.Text;
            updatedBook.Price = decimal.Parse(txtPrice.Text);
            updatedBook.CountryId = (int)cboCountry.SelectedValue;
            UpdateBook(updatedBook);
            PopulateListBooks();
            ClearFields();
            lblId.Content = updatedBook.Id + " Updated!";
        }
        private void DeleteBookById(int id)
        {
            string sql = "DELETE FROM Book WHERE Id = @Id";
            using (IDbConnection connection = new SqlConnection(GetConnectionString.ConStr("WPFBooks")))
            {
                connection.Execute(sql, new { Id = id });
            }
        }
        private void btnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Do you want to delete the book with ID {lblId.Content}",
                                                        "Confirmation",
                                                        MessageBoxButton.YesNo,
                                                        MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DeleteBookById((int)lblId.Content);
                PopulateListBooks();
            }
            else
            {
                return;
            }
        }

        private void btnUploadCSV_Click(object sender, RoutedEventArgs e)
        {
            string filepath = @"C:\Intec\Data\CommaSep.txt";
            List<Book> booksCSV = new List<Book>();
            List<string> lines = File.ReadAllLines(filepath).ToList();
            foreach (string line in lines)
            {
                string[] entries = line.Split(';');
                Book book = new Book();
                book.Author = entries[0];
                book.Price = decimal.Parse(entries[1]);
                book.Title = entries[2];
                book.CountryId = int.Parse(entries[3]);
                booksCSV.Add(book);
            }
            ProcessCSV(booksCSV);
        }
        private void ProcessCSV(List<Book> csvBook)
        {
            // INSERT BOOK per BOOK
            string sql = "INSERT INTO Book (Author, Title ,Price, CountryId)" +
                         "VALUES (@Author, @Title, @Price, @CountryId)";
            foreach (Book book in csvBook)
            {
                using (IDbConnection connection = new SqlConnection(GetConnectionString.ConStr("WPFBooks")))
                {
                    connection.Execute(sql, book);
                }
            }
        }
        private void BulkProcessCSV(List<Book> csvBook)
        {
            // INSERT books as BULK
            string sql = "INSERT INTO Book (Author, Title ,Price, CountryId)" +
                         "VALUES (@Author, @Title, @Price, @CountryId)";
            using (IDbConnection connection = new SqlConnection(GetConnectionString.ConStr("WPFBooks")))
            {
                connection.Execute(sql, csvBook);
            }
        }
    }
}

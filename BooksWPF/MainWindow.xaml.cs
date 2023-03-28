using BooksWPF.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

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
            Book newBook = new Book();
            newBook.Author = txtAuthor.Text;
            newBook.Title = txtTitle.Text;
            newBook.Price = decimal.Parse(txtPrice.Text);
            newBook.CountryId = (int)cboCountry.SelectedValue;
            AddBook(newBook);
            PopulateListBooks();
        }
        private void AddBook(Book newbook)
        {
            string sql = "INSERT INTO Book (Author, Title ,Price, CountryId)" +
                "VALUES (@Author, @Title, @Price, @CountryId)";
            using (IDbConnection connection = new SqlConnection(GetConnectionString.ConStr("WPFBooks")))
            {
                connection.Execute(sql, newbook);
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
                var result = GetAllCountries().Find(item => item.Id == selectedBook.CountryId);
                cboCountry.SelectedValue = result.Id;
            }
        }

        private void btnUpdateBook_Click(object sender, RoutedEventArgs e)
        {

        }
        private void UpdateBook(Book updateBook)
        {
            using (IDbConnection connection = new SqlConnection(GetConnectionString.ConStr("WPFBooks")))
            {
                connection.Execute("UPDATE Book SET Author = @Author, Title = @Title, " +
                    "Price = @Price, CountryId = @CountryId WHERE Id = @Id",
                new
                {
                    id = updateBook.Id,
                    Author = updateBook.Author,
                    Title = updateBook.Title,
                    Price = updateBook.Price,
                    CountryId = updateBook.CountryId,
                });
            }
        }
    }
}

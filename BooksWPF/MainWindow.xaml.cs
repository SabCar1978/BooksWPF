﻿using BooksWPF.Models;
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
            GetAllBooks();
            PopulateListBooks();
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
            lstBooks.ItemsSource = books;
        }
    }
}

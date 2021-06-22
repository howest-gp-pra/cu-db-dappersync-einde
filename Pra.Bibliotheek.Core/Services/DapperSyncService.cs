using System;
using System.Collections.Generic;
using System.Data;
using Pra.Bibliotheek.Core.Entities;
using Pra.Bibliotheek.Core.Interfaces;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Pra.Bibliotheek.Core.Services
{
    public class DapperSyncService : IBookService
    {
        // CRUD AUTEUR
        public List<Author> GetAuthors()
        {
            List<Author> authors;
            string sql = "select * from author order by name";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    authors = connection.Query<Author>(sql).ToList();
                }
                catch
                {
                    return null;
                }
            }
            return authors;
        }
        public bool AddAuthor(Author author)
        {
            string sql = "Insert into author (id, name) values (@id, @name)";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    //int affectedRows = connection.Execute(sql, new { id = author.ID, naam = author.Name });
                    int affectedRows = connection.Execute(sql, author);
                    return affectedRows > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool UpdateAuthor(Author author)
        {
            string sql = "Update author set name = @name Where Id = @id";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    int affectedRows = connection.Execute(sql, author);
                    return affectedRows > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool DeleteAuthor(Author author)
        {
            if (IsAuthorInUse(author))
                return false;
            string sql = "Delete from author Where Id = @id";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    int affectedRows = connection.Execute(sql, author);
                    return affectedRows > 0;
                }
                catch
                {
                    return false;
                }   
            }
        }
        public bool IsAuthorInUse(Author author)
        {
            string sql = "select count(*) from book where authorID = @id";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    int count = connection.ExecuteScalar<int>(sql, author);
                    return count > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool DoesAuthorIDExist(string authorID)
        {
            string sql = "select count(*) from author where id = @id";
            //string sql = $"select count(*) from author where id = '{authorID}'";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    int count = connection.ExecuteScalar<int>(sql, new { id = authorID });
                    //int count = connection.ExecuteScalar<int>(sql);
                    return count > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
        public Author FindAuthorByID(string authorID)
        {
            Author author;
            string sql = $"Select id, name from author where id = '{authorID}' ";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    author = connection.QueryFirst<Author>(sql);
                }
                catch
                {
                    return null;
                }
            }
            return author;
        }
        public Author FindAuthorByName(string name)
        {
            Author author;
            string sql = "Select id, name from author where name = @findname";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    author = connection.QueryFirst<Author>(sql, new { findname = name });
                }
                catch
                {
                    return null;
                }
            }
            return author;
        }
        // CRUD UITGEVER
        public List<Publisher> GetPublishers()
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    List<Publisher> publishers = connection.GetAll<Publisher>().ToList();
                    publishers = publishers.OrderBy(p => p.Name).ToList();
                    return publishers;
                }
                catch
                {
                    return null;
                }
            }
        }
        public bool AddPublisher(Publisher publisher)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {                
                //var newAutoNumberValue = connection.Insert(publisher);
                try
                {
                    connection.Open();
                    connection.Insert(publisher);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool UpdatePublisher(Publisher publisher)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.Update(publisher);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool DeletePublisher(Publisher publisher)
        {
            if (IsPublisherInUse(publisher))
                return false;
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.Delete(publisher);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool IsPublisherInUse(Publisher publisher)
        {
            string sql = $"select count(*) from book where publisherID = '{publisher.ID}'";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    int count = connection.ExecuteScalar<int>(sql);
                    return count > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool DoesPublisherIDExist(string publisherID)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    Publisher publisher = connection.Get<Publisher>(publisherID);
                    return publisher != null;
                }
                catch
                {
                    return false;
                }
            }
        }
        public Publisher FindPublisherByName(string name)
        {
            string sql = $"Select id, name from Publisher where name = '{name}'";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    Publisher publisher = connection.QueryFirst(sql);
                    return publisher;
                }
                catch
                {
                    return null;
                }

            }
        }
        public Publisher FindPublisherByID(string publisherID)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    Publisher publisher = connection.Get<Publisher>(publisherID);
                    return publisher;
                }
                catch
                {
                    return null;
                }
            }
        }

        // CRUD BOEKEN
        public List<Book> GetBooks(Author author = null, Publisher publisher = null)
        {
            List<Book> boeken = new List<Book>();

            string sql = "Select * from book";
            
            List<string> filters = new List<string>();
            if (author != null)
                filters.Add("authorID = @AuthorID");
            if (publisher != null)
                filters.Add("publisherID = @PublisherID");
            if(filters.Count > 0)
            {
                sql += $" where {string.Join(" and ", filters)}";
            }

            sql += " order by title";

            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    boeken = connection.Query<Book>(sql, new { AuthorID = author?.ID, PublisherID = publisher?.ID }).ToList();
                }
                catch
                {
                    return null;
                }
            }
            return boeken;
        }
        public bool AddBook(Book book)
        {
            if (!DoesAuthorIDExist(book.AuthorID))
                return false;
            if (!DoesPublisherIDExist(book.PublisherID))
                return false;

            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.Insert(book);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool UpdateBook(Book book)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.Update(book);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool DeleteBook(Book book)
        {
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    connection.Delete(book);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

    }
}

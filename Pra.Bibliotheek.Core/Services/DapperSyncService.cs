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
                connection.Open();
                authors = connection.Query<Author>(sql).ToList();
            }
            return authors;
        }
        public bool AddAuthor(Author author)
        {
            string sql = "Insert into author (id, name) values (@id, @name)";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                //int affectedRows = connection.Execute(sql, new { id = author.ID, naam = author.Name });
                int affectedRows = connection.Execute(sql, author);
                connection.Close();
                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }
        public bool UpdateAuthor(Author author)
        {
            string sql = "Update author set name = @name Where Id = @id";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                int affectedRows = connection.Execute(sql, author);
                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }
        public bool DeleteAuthor(Author author)
        {
            if (IsAuthorInUse(author))
                return false;
            string sql = "Delete from author Where Id = @id";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                int affectedRows = connection.Execute(sql, author);
                if (affectedRows > 0)
                    return true;
                else
                    return false;
            }
        }
        public bool IsAuthorInUse(Author author)
        {
            string sql = "select count(*) from book where authorID = @id";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                int count = connection.ExecuteScalar<int>(sql, author);
                if (count == 0)
                    return false;
                else
                    return true;
            }
        }
        public bool DoesAuthorIDExist(string authorID)
        {
            string sql = "select count(*) from author where id = @id";
            //string sql = $"select count(*) from author where id = '{authorID}'";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                int count = connection.ExecuteScalar<int>(sql, new { id = authorID });
                //int count = connection.ExecuteScalar<int>(sql);
                if (count == 0)
                    return false;
                else
                    return true;
            }
        }
        public Author FindAuthorByID(string authorID)
        {
            Author author;
            string sql = $"Select id, name from author where id = '{authorID}' ";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                author = connection.QueryFirst<Author>(sql);
            }
            return author;
        }
        public Author FindAuthorByName(string name)
        {
            Author author;
            string sql = "Select id, name from author where name = @findname";
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                author = connection.QueryFirst<Author>(sql, new { findname = name });
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
                connection.Open();
                int count = connection.ExecuteScalar<int>(sql);
                if (count == 0)
                    return false;
                else
                    return true;
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
                    if (publisher == null)
                        return false;
                    else
                        return true;
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
            using (SqlConnection connection = new SqlConnection(Helper.GetConnectionString()))
            {
                connection.Open();
                if (author == null && publisher == null)
                    boeken = connection.Query<Book>("Select * from book order by title").ToList();
                else if (author != null && publisher == null)
                    boeken = connection.Query<Book>("Select * from book where authorID = @autohorID order by title", new { autohorID = author.ID }).ToList();
                else if (author == null && publisher != null)
                    boeken = connection.Query<Book>("Select * from book where publisherID = @publisherID  order by title", new { publisherID = publisher.ID }).ToList();
                else
                    boeken = connection.Query<Book>("Select * from book where authorID = @autohorID and publisherID = @publisherID  order by title", new { autohorID = author.ID, publisherID = publisher.ID }).ToList();
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

        public bool SaveData()
        {
            throw new NotImplementedException();
        }
    }
}

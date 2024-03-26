using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public class DbInitializer
    {
        private readonly SqlConnection _connection;
        public DbInitializer(SqlConnection connection)
        {
            _connection = connection;
        }
        public void InitDb()
        {
            try
            {
                var createDB = $"CREATE DATABASE {Configure.DbName}";
                SqlCommand DBcommand = new SqlCommand(createDB, _connection);

                //COUNT(*) returns the number of rows
                var checkDb = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{Configure.DbName}'";
                SqlCommand checkCommand = new SqlCommand(checkDb, _connection);

                int count = Convert.ToInt32(checkCommand.ExecuteScalar());
                if (count == 0)
                {
                    //db doesnt exists
                    DBcommand.ExecuteNonQuery();
                    _connection.ChangeDatabase(Configure.DbName);
                    Console.WriteLine($"Database created: {_connection.Database}");
                }
                else
                {
                    _connection.ChangeDatabase(Configure.DbName);
                    Console.WriteLine($"Connection Established with the existing Database: {_connection.Database}");
                }
            }catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex);
            }

        }
        public void InitTable()
        {
            int countTable = 0;

            var BooksCreateQuery = $@"CREATE TABLE {Configure.BooksTable}(
                                    id INT IDENTITY(1,1) PRIMARY KEY,
                                    title VARCHAR(20) NOT NULL,
                                    author VARCHAR(20) NOT NULL,
                                    availableCopies INT NOT NULL);";

            var MembersCreateQuery = $@"CREATE TABLE {Configure.MembersTable}(
                                        id INT IDENTITY(1,1) PRIMARY KEY,
                                        name VARCHAR(20) NOT NULL,
                                        phone VARCHAR(10) NOT NULL);";


            var borrowedCreateQuery = $@"CREATE TABLE {Configure.BorrowedTable}(
                                        id INT IDENTITY(1,1) PRIMARY KEY,
                                        member_Id INT NOT NULL,
                                        book_Id INT NOT NULL,
                                        borrowedCopies INT NOT NULL,
                                        FOREIGN KEY (member_Id) REFERENCES {Configure.MembersTable}(id),
                                        FOREIGN KEY (book_Id) REFERENCES {Configure.BooksTable}(id));";



            //books Table
            try
            {
                var checkBookTable = $"SELECT COUNT(*) FROM sys.tables WHERE name = '{Configure.BooksTable}'";
                SqlCommand checkBookCommand = new SqlCommand(checkBookTable, _connection);
                countTable = Convert.ToInt32(checkBookCommand.ExecuteScalar());
                if (countTable == 0)
                {
                    //table books doesnt exists
                    SqlCommand createBookCommand = new SqlCommand(BooksCreateQuery, _connection);
                    createBookCommand.ExecuteNonQuery();
                    Console.WriteLine($"Table: {Configure.BooksTable} Created");
                }
            }catch (Exception ex)
            {
                Console.WriteLine($"Error while Creating {Configure.BooksTable} Table\n"+ex.Message);
            }

            //members Table
            try
            {
                var checkMembersTable = $"SELECT COUNT(*) FROM sys.tables WHERE name = '{Configure.MembersTable}'";
                SqlCommand checkMemberCommand = new SqlCommand(checkMembersTable, _connection);
                countTable = Convert.ToInt32(checkMemberCommand.ExecuteScalar());
                if (countTable == 0)
                {
                    SqlCommand createMember = new SqlCommand(MembersCreateQuery, _connection);
                    createMember.ExecuteNonQuery();
                    Console.WriteLine($"Table: {Configure.MembersTable} Created");
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error while Creating {Configure.MembersTable} Table\n" + ex.Message);
            }

            //borrowed
            try
            {
                var checkBorrowedTable = $"SELECT COUNT(*) FROM sys.tables WHERE name = '{Configure.BorrowedTable}'";
                SqlCommand checkBorrowCommand = new SqlCommand(checkBorrowedTable, _connection);
                countTable = Convert.ToInt32(checkBorrowCommand.ExecuteScalar());
                if (countTable == 0)
                {
                    SqlCommand createBorrowed = new SqlCommand(borrowedCreateQuery, _connection);
                    createBorrowed.ExecuteNonQuery();
                    Console.WriteLine($"Table: {Configure.BorrowedTable} Created");
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error while Creating {Configure.BorrowedTable} Table\n" + ex.Message);
            }
        }
    }
}

using Microsoft.Data.SqlClient;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;

namespace LibraryManagementSystem
{
	public class CRUD_Book
	{
        private string? Title { get; set; }
        private string? Author { get; set; }
		private readonly SqlConnection _connection;


        public CRUD_Book(SqlConnection connection)
        {
			_connection = connection;
        }


		public void DisplayMenu()
		{
			while(true)
			{
                Console.WriteLine("Enter choice\n1.Add new Book\n2.Update Book\n3.Delete Book\n4.Display All Books\n5.Back to menu\n");
				int choice = Convert.ToInt32(Console.ReadLine());
				switch (choice)
				{
					default:
						Console.WriteLine("Enter valid choice");
						break;
					case 1:
						AddBookToDb();
						break;
					case 2:
						UpdateBook();
						break;
					case 3:
						DeleteBook();
						break;
					case 4:
						DisplayAllBooks();
						break;
					case 5:
						return;
                }
            }
		}


		private void AddBookToDb()
		{
			do
			{
				Console.WriteLine("Enter valid book title < 20 chars");
				Title = Console.ReadLine();
			} while (string.IsNullOrEmpty(Title) || Title.Length > 20);
			do
			{
				Console.WriteLine("Enter valid book author < 20 chars");
				Author = Console.ReadLine();
			} while (string.IsNullOrEmpty(Author) || Author.Length > 20);

            Console.WriteLine("Enter the available copies");
			int copies = Convert.ToInt32(Console.ReadLine());

            var query = $"INSERT INTO books(title,author,availableCopies) VALUES ('{Title}','{Author}','{copies}')";

			//sql command is used to convert the query to the DML command for this db
			SqlCommand sqlCommand = new SqlCommand(query, _connection);
			sqlCommand.ExecuteNonQuery();
            Console.WriteLine("Book: {0} added to DB",Title);

        }

		private void UpdateBook()
		{
			int bookId = 0;
			//first check rows exists
			var getQuery = "SELECT * FROM books";
			SqlCommand GetCommand = new SqlCommand(getQuery, _connection);
			SqlDataReader dataReader = GetCommand.ExecuteReader();
			if (!dataReader.HasRows)
			{
				dataReader.Close();
                Console.WriteLine("NO RECORDS IN DB");
				return;
            }
			dataReader.Close();
			do
			{
				Console.WriteLine("Enter valid book ID to Update");
				bookId = Convert.ToInt32(Console.ReadLine());

			} while (CheckBook(bookId));

			//get data for update operation
			do
			{
				Console.WriteLine("Enter valid book title < 20 chars");
				Title = Console.ReadLine();
			} while (string.IsNullOrEmpty(Title) || Title.Length > 20);//get title
			do
			{
				Console.WriteLine("Enter valid book author < 20 chars");
				Author = Console.ReadLine();
			} while (string.IsNullOrEmpty(Author) || Author.Length > 20);//get author

			Console.WriteLine("Enter the available copies");
			int copies = Convert.ToInt32(Console.ReadLine());

			var updateQuery = $"UPDATE books SET title = '{Title}', author = '{Author}', availableCopies = '{copies}' WHERE id = {bookId}";
			SqlCommand UpdateCommand = new SqlCommand(updateQuery, _connection);
			UpdateCommand.ExecuteNonQuery();
			Console.WriteLine("Book ID: {0} details updated", bookId);
		}

		private void DeleteBook()
		{
			int bookId;
			var getQuery = "SELECT * FROM books";
			SqlCommand GetCommand = new SqlCommand(getQuery, _connection);
			SqlDataReader dataReader = GetCommand.ExecuteReader();
			if (!dataReader.HasRows)
			{
				dataReader.Close();
				Console.WriteLine("NO RECORDS IN DB");
				return;
			}
			dataReader.Close();
			do
			{
                Console.WriteLine("enter valid book ID to delete");
				bookId = Convert.ToInt32(Console.ReadLine());
            }while(CheckBook(bookId));

			//perform delete query
			var DeleteQuery = $"DELETE FROM books WHERE id = {bookId}";
			SqlCommand DeleteCommand = new SqlCommand(DeleteQuery, _connection);
			DeleteCommand.ExecuteNonQuery();
            Console.WriteLine("Book ID: {0} successfully deleted !!",bookId);
        }

		public bool CheckBook(int bookId)
		{
			//get book from db
			var GetQuery = $"SELECT id from books b WHERE b.id = {bookId}";
			SqlCommand sqlCommand = new SqlCommand(GetQuery, _connection);

			SqlDataReader reader = sqlCommand.ExecuteReader();
			if (!reader.HasRows)
			{
				reader.Close();
				Console.WriteLine("Book ID:{0} doesn't exists", bookId);
				return true;
			}
			else
			{
				reader.Close();
				return false;
			}
		}

		public void DisplayAllBooks()
		{
			//query
			var GetQuery = $"SELECT * FROM books";
			SqlCommand GetCommand = new SqlCommand(GetQuery, _connection);

			SqlDataReader sqlDataReader = GetCommand.ExecuteReader();

			//check for rows
			if (!sqlDataReader.HasRows)
			{
                Console.WriteLine("NO RECORDS IN DB\n");
            }
			else
			{
				int sl = 0;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("-- Books In Library --").AppendLine();
				while (sqlDataReader.Read())
				{
					sl += 1;
					stringBuilder.Append($"Book ID: {sqlDataReader.GetValue(0)}")
								.AppendLine()
								.Append($"{sl}:Book Title:{sqlDataReader.GetValue(1)}")
								.AppendLine()
								.Append($"Author: {sqlDataReader.GetValue(2)}")
								.AppendLine()
								.Append($"Available Copies: {sqlDataReader.GetValue(3)}")
								.AppendLine()
								.Append("----**----")
								.AppendLine();
				}
				Console.WriteLine(stringBuilder.ToString());
			}
			sqlDataReader.Close();
		}
    }
}

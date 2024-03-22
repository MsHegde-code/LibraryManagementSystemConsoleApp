using Microsoft.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Text;

namespace LibraryManagementSystem
{
	public class CRUD_Book
	{
        private string Title { get; set; }
        private string Author { get; set; }
		private SqlConnection ConnectionObj;


        public CRUD_Book(SqlConnection ConnectionObj)
        {
			this.ConnectionObj = ConnectionObj;
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
			SqlCommand sqlCommand = new SqlCommand(query, ConnectionObj);
			sqlCommand.ExecuteNonQuery();
            Console.WriteLine("Book: {0} added to DB",Title);

        }

		private void UpdateBook()
		{
			int bookId = 0;
			bool flag=true;
			SqlDataReader reader;
			do
			{
				Console.WriteLine("Enter valid book ID to Update");
				bookId = Convert.ToInt32(Console.ReadLine());

				//get book from db
				var GetQuery = $"SELECT id from books b WHERE b.id = {bookId}";

				SqlCommand sqlCommand = new SqlCommand(GetQuery, ConnectionObj);

				//used using statement as the dataReader needs to be closed after it's usage
				//or reader.Close(); can also be used
				using(reader = sqlCommand.ExecuteReader())
				
				if (!reader.HasRows)
				{
                    Console.WriteLine("Book ID:{0} doesn't exists",bookId);
                }
				else
					flag=false;

			} while (flag);

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
			SqlCommand UpdateCommand = new SqlCommand(updateQuery, ConnectionObj);
			UpdateCommand.ExecuteNonQuery();
            Console.WriteLine("Book ID: {0} details updated",bookId);
        }

		private void DeleteBook()
		{
			int bookId;
			bool flag=true;
			do
			{
                Console.WriteLine("enter valid book ID to delete");
				bookId = Convert.ToInt32(Console.ReadLine());

				//write query and convert it to sql command
				var getQuery = $"SELECT id from books WHERE id = {bookId}";
				SqlCommand sqlCommand = new SqlCommand(getQuery, ConnectionObj);

				//get record from db
				SqlDataReader reader = sqlCommand.ExecuteReader();
				if (!reader.HasRows)
				{
					Console.WriteLine("Book Id:{0} doesn't exist",bookId);
				}
				else
				{
					flag = false;
				}
				reader.Close();
            }while(flag);

			//perform delete query
			var DeleteQuery = $"DELETE FROM books WHERE id = {bookId}";
			SqlCommand DeleteCommand = new SqlCommand(DeleteQuery, ConnectionObj);
			DeleteCommand.ExecuteNonQuery();
            Console.WriteLine("Book ID: {0} successfully deleted !!",bookId);
        }

		private void DisplayAllBooks()
		{
			//query
			var GetQuery = $"SELECT * FROM books";
			SqlCommand GetCommand = new SqlCommand(GetQuery, ConnectionObj);

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
					stringBuilder.Append($"{sl}:Book Title:{sqlDataReader.GetValue(1)}")
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

using Microsoft.Data.SqlClient;
using System.Text;

namespace LibraryManagementSystem
{
	public class Borrower
	{
        private int member_Id { get; set; }
        private int book_Id { get; set; }
		private readonly SqlConnection _connection;
        public Borrower(SqlConnection connection)
        {
            _connection = connection;
        }

		

		public void DisplayMenu()
		{
            while(true)
			{
				Console.WriteLine("Enter choice\n1.Borrow\n2.Return\n3.Display Borrowed\n4.Back to menu\n");
				int choice = Convert.ToInt32(Console.ReadLine());
				switch (choice)
				{
					default:
						Console.WriteLine("Enter valid choice\n");
						break;
					case 1:
						BorrowBook();
						break;
					case 2: ReturnBook();
						break;
					case 3:
						DisplayBorrowed();
						break;
					case 4: return;
				}
			}
        }

		private void BorrowBook()
		{
			int Mid, BId, bookCopies=0;
			var MemberObj = new CRUD_Member(_connection);
			do
			{
				Console.WriteLine("Enter valid member Id");
				Mid = Convert.ToInt32(Console.ReadLine());
			}while(MemberObj.CheckMember(Mid));

			var BookObj = new CRUD_Book(_connection);
			//display available books
			BookObj.DisplayAllBooks();
			do
			{
				Console.WriteLine("enter valid Book Id");
				BId = Convert.ToInt32(Console.ReadLine());
			} while (BookObj.CheckBook(BId)); //check bookId

			//get available copies of books
			var getQuery = $"SELECT availableCopies FROM books WHERE id = {BId}";
			SqlCommand getCommand = new SqlCommand(getQuery, _connection);
			SqlDataReader reader = getCommand.ExecuteReader();
			if (reader.Read())
			{
				bookCopies = Convert.ToInt32(reader.GetValue(0));
			}
            if(bookCopies <= 0)
			{
				reader.Close();
				Console.WriteLine("Book Not Available to borrow\n");
				return;
			}
			reader.Close();
            //insert borrower table
            var insertQuery = $"INSERT INTO borrowed(member_Id,book_Id) VALUES ('{Mid}','{BId}')";
			SqlCommand insertCommand = new SqlCommand(insertQuery, _connection);
			insertCommand.ExecuteNonQuery();
            
			//update the available quantity of the book borrowed
			var UpdateQuery = $"UPDATE books SET availableCopies = '{bookCopies-1}' WHERE id = {BId}";
			SqlCommand updateCommand = new SqlCommand(UpdateQuery, _connection);
			updateCommand.ExecuteNonQuery();

			Console.WriteLine("Book is Borrowed");
		}

		private void ReturnBook()
		{
			int mid, copies=0;
			//get list of members from borrowed table
			var memberList = new List<int>();
			var memberQuery = $"SELECT member_Id FROM borrowed";
			SqlCommand memberCommand = new SqlCommand(memberQuery, _connection);
			SqlDataReader memberData = memberCommand.ExecuteReader();
			while (memberData.Read())
			{
				mid = Convert.ToInt32(memberData.GetValue(0));
				if (!memberList.Contains(mid))
				{
					memberList.Add(mid);
				}
			}
			memberData.Close();
			//get vaild member id
			do
			{
				Console.WriteLine("Enter valid member ID, 0 to exit");
				mid = Convert.ToInt32(Console.ReadLine());
				if (memberList.Contains(mid))
				{
					break;
				}
				else if(mid == 0)
				{
					return;
				}
				else
				{
                    Console.WriteLine("Member didn't borrow books");
                }
			} while (true);

			//display the books which he borrowed

			var booksList = new List<int>();
			var getQuery = $"SELECT title, book_Id FROM books AS B, borrowed WHERE B.id = book_Id AND member_Id = {mid}";
			SqlCommand getCommand = new SqlCommand(getQuery, _connection);
			SqlDataReader reader = getCommand.ExecuteReader();
				
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append($"Borrowed Books by Member ID: {mid}").AppendLine();
			while(reader.Read())
			{
				stringBuilder.Append($"Book Title - {reader.GetValue(0)} : Book ID - {reader.GetValue(1)}")
					.AppendLine();
				booksList.Add(Convert.ToInt32(reader.GetValue(1)));
			}
			reader.Close();
            Console.WriteLine(stringBuilder.ToString());

			do
			{
				Console.WriteLine("Enter valid book ID");
				book_Id = Convert.ToInt32(Console.ReadLine());
			} while (!booksList.Contains(book_Id));

			//remove from borrowed table
			var deleteQuery = $"DELETE FROM borrowed WHERE book_Id = '{book_Id}' AND member_Id = '{mid}'";
			SqlCommand deleteCommand = new SqlCommand(deleteQuery, _connection);
			deleteCommand.ExecuteNonQuery();

			//get availableCopies of the book
			var Query = $"SELECT availableCopies FROM books WHERE id = '{book_Id}'";
			SqlCommand command = new SqlCommand(Query, _connection);
			SqlDataReader sqlData = command.ExecuteReader();
			if (sqlData.Read())
			{
				copies = Convert.ToInt32(sqlData.GetValue(0));
			}
			sqlData.Close();

			//update the books table
			var updateQuery = $"UPDATE books SET availableCopies = {copies+1} WHERE id = {book_Id}";
			SqlCommand updateCommand = new SqlCommand(updateQuery, _connection);
			updateCommand.ExecuteNonQuery();

            Console.WriteLine("Book ID: {0} is returned",book_Id);


        }

		private void DisplayBorrowed()
		{
			//get members id
			var getQuery = $"SELECT member_Id FROM borrowed";
			SqlCommand getCommand = new SqlCommand(getQuery, _connection);
			SqlDataReader reader = getCommand.ExecuteReader();

			var membersList = new List<int>();
			while(reader.Read())
			{
				member_Id = Convert.ToInt32(reader.GetValue(0));
				//add unique
				if(!membersList.Contains(member_Id))
					membersList.Add(member_Id);
            }
			reader.Close();

			int sl = 0;
			//get books for each member
			StringBuilder stringBuilder = new StringBuilder();
			foreach (var mid in membersList)
			{
				var borrowQuery = $"SELECT title, book_Id FROM books AS B, borrowed WHERE B.id = book_Id AND member_Id = '{mid}'";

				SqlCommand BorrowCommand = new SqlCommand(borrowQuery, _connection);
				SqlDataReader sqlData = BorrowCommand.ExecuteReader();

				
				sl += 1;
				stringBuilder.Append($"{sl}. Member ID: {mid}")
					.AppendLine()
					.Append($"Borrowed Books")
					.AppendLine();

				//print book id and title for each member id
				while (sqlData.Read())
				{
					stringBuilder.Append($"Book Title: {sqlData.GetValue(0)}")
					.Append(" : ")
					.Append($"Book ID: {sqlData.GetValue(1)}")
					.AppendLine();
				}
				stringBuilder.AppendLine()
					.Append("-- ** --")
					.AppendLine();
				sqlData.Close();
				
			}
			Console.WriteLine(stringBuilder.ToString());
		}

	}
}

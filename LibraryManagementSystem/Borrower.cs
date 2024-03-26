using Microsoft.Data.SqlClient;
using System.Text;

namespace LibraryManagementSystem
{
	public class Borrower
	{
        private int member_Id { get; set; }
        private int book_Id { get; set; }

		private int BorrowedCopies { get; set; }
		private readonly SqlConnection _connection;
        public Borrower(SqlConnection connection)
        {
            _connection = connection;
        }

		

		public void DisplayMenu()
		{
            while(true)
			{
				Console.WriteLine("\n** Borrow or Return **\n1.Borrow\n2.Return\n3.Display Borrowed\n4.Back to menu\nEnter the choice\n");
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
			//before borrowing check books exists in Lib
			var query = $"SELECT COUNT(*) FROM books";
			SqlCommand QueryCommand = new SqlCommand(query, _connection);
			int  count = Convert.ToInt32(QueryCommand.ExecuteScalar());
			if (count == 0)
			{
                //no books in lib
                Console.WriteLine("\nNO BOOKS IN THE LIBRARY");
				return;
            }

			//perform borrow

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
			var getQuery = $"SELECT availableCopies FROM {Configure.BooksTable} WHERE id = {BId}";
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

			//get the borrowed copies
			var borrowedCopiesQuery = $"SELECT borrowedCopies FROM {Configure.BorrowedTable} WHERE member_Id = '{Mid}' AND book_Id = {BId}";
			SqlCommand borrowedCopiesCommand = new SqlCommand(borrowedCopiesQuery, _connection);
			BorrowedCopies = Convert.ToInt32(borrowedCopiesCommand.ExecuteScalar());
			if(BorrowedCopies<= 0)
			{
                //insert to borrower table
                //insert new record, Borrowed book for first time
                var insertQuery = $"INSERT INTO borrowed(member_Id,book_Id,BorrowedCopies) VALUES ('{Mid}','{BId}','{BorrowedCopies+1}')";
                SqlCommand insertCommand = new SqlCommand(insertQuery, _connection);
                insertCommand.ExecuteNonQuery();
            }
			else
			{
				//update the borrowed Copies value, do not insert another record
				var UpdateCopiesBorrowed = $"UPDATE {Configure.BorrowedTable} SET borrowedCopies = '{BorrowedCopies+1}' WHERE member_Id = {Mid} AND book_Id = '{BId}'";
				SqlCommand UpdateCopiesCommand = new SqlCommand(UpdateCopiesBorrowed, _connection);
				UpdateCopiesCommand.ExecuteNonQuery();
			}
            
			//update the available quantity of the book borrowed
			var UpdateQuery = $"UPDATE books SET availableCopies = '{bookCopies-1}' WHERE id = {BId}";
			SqlCommand updateCommand = new SqlCommand(UpdateQuery, _connection);
			updateCommand.ExecuteNonQuery();

			Console.WriteLine($"Book ID:{BId} is Borrowed");
		}

		private void ReturnBook()
		{
            var checkBorrowedRecords = $"SELECT COUNT(*) FROM {Configure.BorrowedTable}";
            SqlCommand checkCommand = new SqlCommand(checkBorrowedRecords, _connection);
            int count = Convert.ToInt32(checkCommand.ExecuteScalar());
            if (count == 0)
            {
                //no records
                Console.WriteLine("NO BOOKS BORROWED FROM LIBRARY");
				return;
            }
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
			var getQuery = $"SELECT title, book_Id, BorrowedCopies FROM books AS B, borrowed WHERE B.id = book_Id AND member_Id = {mid}";
			SqlCommand getCommand = new SqlCommand(getQuery, _connection);
			SqlDataReader reader = getCommand.ExecuteReader();
				
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append($"\nBorrowed Books by Member ID: {mid}").AppendLine();
			while(reader.Read())
			{
				stringBuilder.Append($"Book Title - {reader.GetValue(0)} : Book ID - {reader.GetValue(1)}")
					.AppendLine()
					.Append($"Borrowed Copies: {reader.GetValue(2)}")
					.AppendLine();
				booksList.Add(Convert.ToInt32(reader.GetValue(1)));
			}
			reader.Close();
            Console.WriteLine(stringBuilder.ToString());

			//need to get bookID input based on borrowed bookID
			do
			{
				Console.WriteLine("Enter valid book ID");
				book_Id = Convert.ToInt32(Console.ReadLine());
			} while (!booksList.Contains(book_Id));

			//before removing the record, check the number of borrowed copies
			var checkBorrowedCopies = $"SELECT borrowedCopies FROM {Configure.BorrowedTable} WHERE member_Id = '{mid}' AND book_Id = '{book_Id}'";
			SqlCommand BorrowedCopiesCommand = new SqlCommand(checkBorrowedCopies, _connection);
			int BorrowedCopies = Convert.ToInt32(BorrowedCopiesCommand.ExecuteScalar());

			if(BorrowedCopies > 1)
			{
				//decrement a single copy count from the record
				var UpdateBorrowedCopies = $"UPDATE {Configure.BorrowedTable} SET BorrowedCopies = {BorrowedCopies-1} WHERE member_Id = '{mid}' AND book_Id = '{book_Id}'";
				SqlCommand borrowCopiesCommand = new SqlCommand(UpdateBorrowedCopies , _connection);
				borrowCopiesCommand.ExecuteNonQuery();
                Console.WriteLine("One Copy of Book ID: {0} is returned ", book_Id);
            }
			else
			{
				//there is only one copy of the book, when returned delete the record
                //remove from borrowed table
                var deleteQuery = $"DELETE FROM borrowed WHERE book_Id = '{book_Id}' AND member_Id = '{mid}'";
                SqlCommand deleteCommand = new SqlCommand(deleteQuery, _connection);
                deleteCommand.ExecuteNonQuery();
            }

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
			//before displaying, check borrowed has rows
			var checkBorrowedRecords = $"SELECT COUNT(*) FROM {Configure.BorrowedTable}";
			SqlCommand checkCommand = new SqlCommand(checkBorrowedRecords, _connection);
			int count = Convert.ToInt32(checkCommand.ExecuteScalar());
			if (count == 0)
			{
				//no records
				Console.WriteLine("NO BOOKS BORROWED FROM LIBRARY");
			}
			else
			{
                //get members id
                var getQuery = $"SELECT member_Id FROM borrowed";
                SqlCommand getCommand = new SqlCommand(getQuery, _connection);
                SqlDataReader reader = getCommand.ExecuteReader();

                var membersList = new List<int>();
                while (reader.Read())
                {
                    member_Id = Convert.ToInt32(reader.GetValue(0));
                    //add unique
                    if (!membersList.Contains(member_Id))
                        membersList.Add(member_Id);
                }
                reader.Close();

                int sl = 0;
                //get books for each member
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var mid in membersList)
                {
                    var borrowQuery = $"SELECT title, book_Id, BorrowedCopies FROM books AS B, borrowed WHERE B.id = book_Id AND member_Id = '{mid}'";

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
                        .AppendLine()
						.Append($"-> Borrowed Copies: {sqlData.GetValue(2)}")
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
}

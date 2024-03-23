using Microsoft.Data.SqlClient;

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
				Console.WriteLine("Enter choice\n1.Borrow\n2.Return\n3.Back to menu\n");
				int choice = Convert.ToInt32(Console.ReadLine());
				switch (choice)
				{
					default:
						Console.WriteLine("Enter valid choice\n");
						break;
					case 1:
						BorrowBook();
						break;
					case 2: //ReturnBook();
						break;
					case 3: return;
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

	}
}

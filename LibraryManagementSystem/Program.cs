using Microsoft.Data.SqlClient;

namespace LibraryManagementSystem
{


	internal class Program
	{
		static void Main(string[] args)
		{
            Console.WriteLine("Application Started\n");
            //get connectionString from the DB properties
            var connectionString = Configure.connectionString;

            //connect to the db through the connString, this connection obj is used to perform db related operations
            SqlConnection connection = new SqlConnection(connectionString);

            //handles db connections exception
            try
			{
				//opens the db connection
                connection.Open();
            }catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			//DB initializer
			var init = new DbInitializer(connection);
			init.InitDb();
			init.InitTable();

			///
			while(true)
			{
                Console.WriteLine("\n** Main Menu **\n1.Library\n2.Manage Member\n3.Borrow or Return\n4.Exit App\n");
                Console.WriteLine("Enter the choice");
                int choice = Convert.ToInt32(Console.ReadLine());	
				switch (choice)
				{
					case 1: var book = new CRUD_Book(connection);
							book.DisplayMenu();
							break;
					case 2: var member = new CRUD_Member(connection);
							member.DisplayMenu();
							break;
					case 3:	var borrow = new Borrower(connection);
							borrow.DisplayMenu();
							break;
					case 4:
							Console.WriteLine("Connection Closed");
							connection.Close();
							Environment.Exit(0);
							break;
					default: Console.WriteLine("enter valid choice");
							break;
				}
            }
		}
	}
}

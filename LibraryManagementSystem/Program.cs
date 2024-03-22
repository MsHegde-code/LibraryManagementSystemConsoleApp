using Microsoft.Data.SqlClient;

namespace LibraryManagementSystem
{
	internal class Program
	{
		static void Main(string[] args)
		{

			//get connectionString from the DB properties
			var connectionString = @"Data Source=MANIT;Initial Catalog=LibraryManagement;Integrated Security=True;Trust Server Certificate=True";

			//connect to the db through the connString, this connection obj is used to perform db related operations
			SqlConnection connection = new SqlConnection(connectionString);

			connection.Open();
			Console.WriteLine("Connection Established");
			

			///
			while(true)
			{
                Console.WriteLine("Main Menu\n1.Library\n2.Manage Member\n3.Borrow\n4.Exit\n");
				int choice = Convert.ToInt32(Console.ReadLine());	
				switch (choice)
				{
					case 1: var book = new CRUD_Book(connection);
							book.DisplayMenu();
							break;
					case 2: var member = new CRUD_Member(connection);
							member.DisplayMenu();
							break;
					case 3:	break;
					case 4:
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

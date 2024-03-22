using Microsoft.Data.SqlClient;
using System.Text;

namespace LibraryManagementSystem
{
	public class CRUD_Member
	{
		private readonly SqlConnection _connection;
        private string Name { get; set; }
        private string Phone { get; set; }
        public CRUD_Member(SqlConnection connection)
        {
            _connection = connection;
        }

		public void DisplayMenu()
		{
			while(true)
			{
                Console.WriteLine("Enter Choice\n1.Add new Member\n2.Update member\n3.Delete member\n4.Display all members\n5.Back to menu");
				var choice = Convert.ToInt32(Console.ReadLine());
				switch (choice)
				{
					default: Console.WriteLine("Enter valid choice");
						break;
					case 1: AddNewMember();
							break;
					case 2: UpdateMember();
							break;
					case 3: DeleteMember();
							break;
					case 4: DisplayAllMembers();
							break;
					case 5:	return;

				}
            }
		}
		private void AddNewMember()
		{
			do
			{
				Console.WriteLine("Enter member name <25 chars");
				Name = Console.ReadLine();
			} while (string.IsNullOrEmpty(Name) || Name.Length > 25);
			do
			{
				Console.WriteLine("Enter valid phone number");
				Phone = Console.ReadLine();
			}while(string.IsNullOrEmpty(Phone) || Phone.Length < 10 || Phone.Length >10);

			var AddQuery = $"INSERT INTO members(name, phone) VALUES ('{Name}','{Phone}')";
			SqlCommand sqlCommand = new SqlCommand(AddQuery, _connection);
			sqlCommand.ExecuteNonQuery();
            Console.WriteLine("Member: {0} Added to Db",Name);
        }
		
		private void UpdateMember()
		{
			int memberId;
			bool flag = true;
			do
			{
                Console.WriteLine("Enter valid member ID to Update");
				memberId = Convert.ToInt32(Console.ReadLine());

				var GetQuery = $"SELECT id from members WHERE id = {memberId}";
				SqlCommand getCommand = new SqlCommand(GetQuery, _connection);
				SqlDataReader reader = getCommand.ExecuteReader();

				if (!reader.HasRows)
				{
                    Console.WriteLine("Member Id: {0} doesn't exists",memberId);
                }
				else
				{
					flag = false;
				}
				reader.Close();
			} while (flag);

			//perform update 
			do
			{
				Console.WriteLine("Enter member name <25 chars");
				Name = Console.ReadLine();
			} while (string.IsNullOrEmpty(Name) || Name.Length > 25);
			do
			{
				Console.WriteLine("Enter valid phone number");
				Phone = Console.ReadLine();
			} while (string.IsNullOrEmpty(Phone) || Phone.Length < 10 || Phone.Length > 10);

			var UpdateQuery = $"UPDATE members SET name = '{Name}', phone = '{Phone}' WHERE id = {memberId}";
			SqlCommand sqlCommand = new SqlCommand(UpdateQuery, _connection);
			sqlCommand.ExecuteNonQuery();
			Console.WriteLine("Member ID: {0} updated Successfully", memberId);
		}

		private void DeleteMember()
		{
			int memberId; bool flag = true;
			do
			{
                Console.WriteLine("Enter valid member Id to delete");
				memberId = Convert.ToInt32(Console.ReadLine());

				var GetQuery = $"SELECT id from members WHERE id = {memberId}";
				SqlCommand getCommand = new SqlCommand(GetQuery, _connection);
				SqlDataReader reader = getCommand.ExecuteReader();

				if (!reader.HasRows)
				{
                    Console.WriteLine("Member Id: {0} doesn't exists", memberId);
                }
				else
				{
					flag = false;
				}
				reader.Close();
            } while (flag);

			//delete operation
			var DeleteQuery = $"DELETE from members WHERE id = {memberId}";
			SqlCommand sqlCommand = new SqlCommand(DeleteQuery, _connection);
			sqlCommand.ExecuteNonQuery();
			Console.WriteLine("Member ID: {0} successfully deleted",memberId);
		}

		private void DisplayAllMembers()
		{
			var GetAllQuery = $"SELECT * FROM members";
			SqlCommand getCommand = new SqlCommand(GetAllQuery, _connection);

			//read data
			SqlDataReader reader = getCommand.ExecuteReader();

			if (!reader.HasRows)
			{
				Console.WriteLine("NO RECORDS IN DB\n");
			}
			else
			{
				int sl = 0;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("-- Active Members --").AppendLine();
				while (reader.Read())
				{
					sl += 1;
					stringBuilder.Append($"{sl}.Member Name: {reader.GetValue(1)}")
						.AppendLine()
						.Append($"Phone: {reader.GetValue(2)}")
						.AppendLine()
						.Append("----**----")
						.AppendLine();
				}

				Console.WriteLine(stringBuilder.ToString());
			}
			reader.Close();
		}
	}
}

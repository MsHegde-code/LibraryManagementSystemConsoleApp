using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem
{
    public class Configure
    {
        public const string connectionString = @"Data Source=MANIT;Integrated Security=True;Trust Server Certificate=True";
        public const string DbName = "LibraryManagement";


        public const string BooksTable = "books";
        public const string MembersTable = "members";
        public const string BorrowedTable = "borrowed";
    }
}

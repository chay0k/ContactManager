using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class UserListViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }
}

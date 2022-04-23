using System;
namespace WebApplication2.Models
{

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime DateOfBirh { get; set; }
        public bool Married { get; set; }
        public string Phone { get; set; } = "";
        public decimal Salary { get; set; }
    }
}
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebApplication2.Models;


namespace WebApplication2.Resources
{
    public class LoadUserData
    {
        private UserContext db;
        public LoadUserData(UserContext context)
        {
            db = context;
        }

        public List<User> _users;

        public List<User> ReadData(string path)
        {
            _users = new List<User>();

            string[] Rows = File.ReadAllLines(path, Encoding.Default);

            //string[] Rows = File.ReadAllLines(path, Encoding.Default);

            for (int i = 0; i < Rows.Length; i++)
            {
                var Elements = Rows[i].Split(',');

                User user = new User()
                {
                    Name = Elements[0],
                    DateOfBirh = Convert.ToDateTime(Elements[1]),
                    Married = Convert.ToBoolean(Elements[2]),
                    Phone = Elements[3],
                    Salary = Convert.ToDecimal(Elements[4])
                };

                _users.Add(user);
            }
            return _users;
        }
        public void AddUserDataToDatabase()
        {
            foreach (User user in _users)
            {
                db.Users.Add(user);
            }
            db.SaveChanges();
        }
    }
}

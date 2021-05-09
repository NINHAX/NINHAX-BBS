using NHX.BBS.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.Logic
{
    public static class UserLogic
    {
        public static User Auth(string email, string password)
        {
            using var context = new BBSDatabaseContext();
            User user = (from u in context.Users
                         where u.Email.Equals(email) && u.Password.Equals(password)
                         select u).Single();
            return user;
        }

        public static void CreateUser(string email, string password, string displayName)
        {
            using (var context = new BBSDatabaseContext())
            {
                User user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Password = password,
                    DisplayName = displayName
                };

                context.Users.Add(user);
                context.SaveChanges();
            }
        }
    }
}

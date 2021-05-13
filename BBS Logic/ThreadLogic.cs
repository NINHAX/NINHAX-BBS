using NHX.BBS.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.Logic
{
    public static class ThreadLogic
    {
        public static Thread GetThread(Guid id)
        {
            using var context = new BBSDatabaseContext();
            Thread thread = (from t in context.Threads
                         where t.Id.Equals(id)
                         select t).Single();
            return thread;
        }

        public static void SetThread(string title, User user)
        {
            using var context = new BBSDatabaseContext();
            Post Post = new()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Timestamp = BitConverter.GetBytes(DateTime.Now.ToBinary()),
                Creator = user.Id
            };

            context.Posts.Add(Post);
            context.SaveChanges();
        }
    }
}

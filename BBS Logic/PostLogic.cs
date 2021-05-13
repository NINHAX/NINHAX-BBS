using NHX.BBS.Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHX.BBS.Logic
{
    public static class PostLogic
    {
        public static Post GetPost(Guid id)
        {
            using var context = new BBSDatabaseContext();
            Post post = (from p in context.Posts
                         where p.Id.Equals(id)
                         select p).Single();
            return post;
        }

        public static void SetPost(string title, Thread thread, User user)
        {
            using var context = new BBSDatabaseContext();
            Post Post = new()
            {
                Id = Guid.NewGuid(),
                Title = title,
                Timestamp = BitConverter.GetBytes(DateTime.Now.ToBinary()),
                Thread = thread.Id,
                Creator = user.Id
            };

            context.Posts.Add(Post);
            context.SaveChanges();
        }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace NHX.BBS.Logic.Model
{
    public partial class User
    {
        public User()
        {
            Posts = new HashSet<Post>();
            Threads = new HashSet<Thread>();
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Thread> Threads { get; set; }
    }
}

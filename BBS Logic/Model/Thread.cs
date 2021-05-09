using System;
using System.Collections.Generic;

#nullable disable

namespace NHX.BBS.Logic.Model
{
    public partial class Thread
    {
        public Thread()
        {
            Posts = new HashSet<Post>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public byte[] Timestamp { get; set; }
        public Guid Creator { get; set; }

        public virtual User CreatorNavigation { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}

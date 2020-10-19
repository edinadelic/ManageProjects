using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageProjects.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<MyUser> MyUsers { get; set; }

    }
}

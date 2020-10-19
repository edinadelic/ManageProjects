using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageProjects.Models
{
    public partial class MyUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Team Teams { get; set; }
        public int? TeamId { get; set; }
        public string IdentityId { get; set; }
        public virtual ApplicationUser ApplicationUsers { get; set; }

    }
}


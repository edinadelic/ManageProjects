using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageProjects.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual MyUser MyUser { get; set; }

    }
}


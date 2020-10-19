using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageProjects.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool? isApproved { get; set; }
        public MyUser User { get; set; }
        public string UserId { get; set; }
        public ICollection<Time> Times { get; set; }
    }
}

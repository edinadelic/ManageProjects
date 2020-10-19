using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageProjects.Models
{
    public class Time
    {
        public int Id { get; set; }
        public float EstimetedTime { get; set; }
        public float? LoggedTime { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? DeadLine { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}

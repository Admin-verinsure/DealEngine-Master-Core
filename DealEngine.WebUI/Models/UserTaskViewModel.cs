using System;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{
    public class UserTaskViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        public string ClientName { get; set; }

        public string Details { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public DateTime DueDate { get; set; }

        public bool Completed { get; set; }

        public User CompletedBy { get; set; }

    }
}
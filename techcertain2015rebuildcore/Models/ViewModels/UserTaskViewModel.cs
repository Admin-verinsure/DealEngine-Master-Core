using System;
using TechCertain.Domain.Entities;

namespace techcertain2015rebuildcore.Models.ViewModels
{
    public class UserTaskViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        public Organisation For { get; set; }

        public string ClientName { get; set; }

        public string Details { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public DateTime DueDate { get; set; }

        public bool Completed { get; set; }

        public User CompletedBy { get; set; }

    }
}
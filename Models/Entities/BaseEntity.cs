﻿using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Models.Entities
{
    public class BaseEntity
    {
        [Key]public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }


    }
}

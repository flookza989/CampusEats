﻿using System.ComponentModel.DataAnnotations;

namespace CampusEats.API.DTOs.User
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
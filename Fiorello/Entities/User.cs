﻿using Fiorello.Models;
using Microsoft.AspNetCore.Identity;

namespace Fiorello.Entities
{
    public class User : IdentityUser
    {
        public string Fullname { get; set; }
        public string Country { get; set; }
        public Basket Basket { get; set; }
    }
}

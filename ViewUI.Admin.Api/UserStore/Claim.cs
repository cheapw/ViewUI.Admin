﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ViewUI.Admin.Api.UserStore
{
    public class Claim
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [MaxLength(32)]
        public string Type { get; set; }

        [MaxLength(100)]
        public string Value { get; set; }

        public User User { get; set; }
    }
}

﻿using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications
{
    public class NotificationCreateForRoleBased: CreateCommand
    {
        public string? Description { get; set; }

        public string? Role { get; set; }
    }
}

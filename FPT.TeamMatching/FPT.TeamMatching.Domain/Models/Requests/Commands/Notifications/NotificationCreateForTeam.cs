﻿using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications
{
    public class NotificationCreateForTeam: CreateCommand
    {
        public Guid? ProjectId { get; set; }

        public string? Description { get; set; }
    }
}

﻿using FPT.TeamMatching.Domain.Entities.Base;

namespace FPT.TeamMatching.Domain.Entities;

public class UserXRole : BaseEntity
{
    public Guid? UserId { get; set; }

    public Guid? RoleId { get; set; }
    
    public Guid? SemesterId { get; set; }  

    public bool IsPrimary { get; set; }   

    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }
    
    public virtual Semester? Semester { get; set; } 
}
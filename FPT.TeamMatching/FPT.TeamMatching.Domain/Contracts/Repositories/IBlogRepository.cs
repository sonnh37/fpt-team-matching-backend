﻿using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IBlogRepository : IBaseRepository<Blog>
{
    Task<List<Blog>?> GetBlogFindMemberInCurrentSemester(Guid id);
    Task<Blog> ChangeStatusBlog(Guid id);



}
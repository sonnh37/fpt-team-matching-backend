using FPT.TeamMatching.Domain.Contracts.Repositories;

namespace FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

public interface IUnitOfWork : IBaseUnitOfWork
{
    IUserRepository UserRepository { get; }
    IBlogRepository BlogRepository { get; }
    ILikeRepository LikeRepository { get; }
    ICommentRepository CommentRepository { get; }
    IRateRepository RateRepository { get; }
    IJobPositionRepository JobPositionRepository { get; }
    ITeamMemberRepository TeamMemberRepository { get; }



}   
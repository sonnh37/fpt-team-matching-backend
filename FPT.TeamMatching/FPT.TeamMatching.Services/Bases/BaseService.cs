using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FPT.TeamMatching.Services.Bases;

public abstract class BaseService
{
}

public abstract class BaseService<TEntity> : BaseService, IBaseService
    where TEntity : BaseEntity
{
    private readonly IBaseRepository<TEntity> _baseRepository;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly IMapper _mapper;
    protected readonly IUnitOfWork _unitOfWork;

    protected BaseService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _baseRepository = _unitOfWork.GetRepositoryByEntity<TEntity>();
        _httpContextAccessor ??= new HttpContextAccessor();
    }

    #region Queries

    public async Task<BusinessResult> GetById<TResult>(Guid id) where TResult : BaseResult
    {
        try
        {
            var entity = await _baseRepository.GetById(id, true);
            var result = _mapper.Map<TResult>(entity);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG)
                    ;

            return new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(TResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetAll<TResult>() where TResult : BaseResult
    {
        try
        {
            var entities = await _baseRepository.GetAll();
            var results = _mapper.Map<List<TResult>>(entities);
            if (!results.Any())
                return new ResponseBuilder()
                    .WithData(results)
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG)
                    ;

            return new ResponseBuilder()
                .WithData(results)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error {typeof(TResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> GetAll<TResult>(GetQueryableQuery query) where TResult : BaseResult
    {
        try
        {
            List<TResult>? results;

            var (data, total) = await _baseRepository.GetData(query);

            results = _mapper.Map<List<TResult>>(data);

            if (results.Count == 0) 
                return new ResponseBuilder()
                    .WithData(results)
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG)
                    ;

            // GetAll 
            if (!query.IsPagination)
                return new ResponseBuilder()
                    .WithData(results)
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG)
                    ;
            
            // GetAll with pagination
            var tableResponse = new PaginatedResult(query, results, total);

            return new ResponseBuilder()
                .WithData(tableResponse)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_READ_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred in {typeof(TResult).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    #endregion

    #region Commands

    public async Task<BusinessResult> CreateOrUpdate<TResult>(CreateOrUpdateCommand createOrUpdateCommand)
        where TResult : BaseResult
    {
        try
        {
            var entity = await CreateOrUpdateEntity(createOrUpdateCommand);
            var result = _mapper.Map<TResult>(entity);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG);

            var msg = new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);

            return msg;
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating {typeof(TEntity).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> Restore<TResult>(UpdateCommand updateCommand)
        where TResult : BaseResult
    {
        try
        {
            var entity = await RestoreEntity(updateCommand);
            var result = _mapper.Map<TResult>(entity);
            if (result == null)
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG)
                    ;


            return new ResponseBuilder()
                .WithData(result)
                .WithStatus(Const.SUCCESS_CODE)
                .WithMessage(Const.SUCCESS_SAVE_MSG);
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while updating {typeof(TEntity).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public async Task<BusinessResult> DeleteById(Guid id, bool isPermanent = false)
    {
        try
        {
            if (isPermanent)
            {
                var isDeleted = await DeleteEntityPermanently(id);
                return isDeleted
                    ? new ResponseBuilder()
                        .WithStatus(Const.SUCCESS_CODE)
                        .WithMessage(Const.SUCCESS_DELETE_MSG)
                        
                    : new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_DELETE_MSG)
                        ;
            }

            var entity = await DeleteEntity(id);

            return entity != null
                ? new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG)
                    
                : new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(Const.FAIL_SAVE_MSG)
                    ;
        }
        catch (DbUpdateException dbEx)
        {
            if (dbEx.InnerException?.Message.Contains("FOREIGN KEY") == true)
            {
                var errorMessage = "Không thể xóa vì dữ liệu đang được tham chiếu ở bảng khác.";
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(errorMessage)
                    ;
            }

            throw;
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while deleting {typeof(TEntity).Name} with ID {id}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    protected async Task<TEntity?> CreateOrUpdateEntity(CreateOrUpdateCommand createOrUpdateCommand)
    {
        TEntity? entity;
        if (createOrUpdateCommand is UpdateCommand updateCommand)
        {
            entity = await _baseRepository.GetById(updateCommand.Id);
            if (entity == null) return null;

            _mapper.Map(updateCommand, entity);

            InitializeBaseEntityForUpdate(entity);
            _baseRepository.Update(entity);
        }
        else
        {
            entity = _mapper.Map<TEntity>(createOrUpdateCommand);
            if (entity == null) return null;
            entity.Id = Guid.NewGuid();
            InitializeBaseEntityForCreate(entity);
            _baseRepository.Add(entity);
        }

        var saveChanges = await _unitOfWork.SaveChanges();
        return saveChanges ? entity : default;
    }

    protected async Task<TEntity?> RestoreEntity(UpdateCommand updateCommand)
    {
        TEntity? entity;

        entity = await _baseRepository.GetById(updateCommand.Id);
        if (entity == null) return null;

        // update isdeleted
        entity.IsDeleted = false;

        InitializeBaseEntityForUpdate(entity);
        _baseRepository.Update(entity);


        var saveChanges = await _unitOfWork.SaveChanges();
        return saveChanges ? entity : default;
    }


    protected void InitializeBaseEntityForCreate(TEntity? entity)
    {
        if (entity == null) return;

        var user = GetUser();

        entity.CreatedDate = DateTime.UtcNow;
        entity.UpdatedDate = DateTime.UtcNow;
        entity.IsDeleted = false;

        if (user == null) return;
        entity.CreatedBy = user.Email;
        entity.UpdatedBy = user.Email;
    }

    protected void InitializeBaseEntityForUpdate(TEntity? entity)
    {
        if (entity == null) return;

        var user = GetUser();

        entity.UpdatedDate = DateTime.UtcNow;

        if (user == null) return;
        entity.UpdatedBy = user.Email;
    }

    private async Task<TEntity?> DeleteEntity(Guid id)
    {
        var entity = await _baseRepository.GetById(id);
        if (entity == null) return null;

        _baseRepository.Delete(entity);

        var saveChanges = await _unitOfWork.SaveChanges();
        return saveChanges ? entity : default;
    }

    private async Task<bool> DeleteEntityPermanently(Guid id)
    {
        var entity = await _baseRepository.GetById(id);
        if (entity == null) return false;

        _baseRepository.DeletePermanently(entity);

        var saveChanges = await _unitOfWork.SaveChanges();
        return saveChanges;
    }

    #endregion

    // public BusinessResult GetUserByCookie()
    // {
    //     try
    //     {
    //         if (_httpContextAccessor?.HttpContext == null ||
    //             !_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
    //             return HandlerNotFound("Not login yet");
    //
    //         // Lấy thông tin UserId từ Claims
    //         var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
    //         if (string.IsNullOrEmpty(userIdClaim))
    //             return HandlerNotFound("No user claim found");
    //
    //         // Lấy thêm thông tin User từ database nếu cần
    //         var userId = Guid.Parse(userIdClaim);
    //         var user = _unitOfWork.UserRepository.GetById(userId).Result;
    //         var userResult = _mapper.Map<UserResult>(user);
    //
    //         if (userResult == null) return HandlerNotFound();
    //
    //         return new ResponseBuilder<UserResult>()
    //             .WithData(userResult)
    //             .WithStatus(Const.SUCCESS_CODE)
    //             .WithMessage(Const.SUCCESS_READ_MSG)
    //             ;
    //     }
    //     catch (Exception ex)
    //     {
    //         return HandlerError(ex.Message);
    //     }
    // }

    public User? GetUser()
    {
        try
        {
            if (_httpContextAccessor?.HttpContext == null ||
                !_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                return null;

            // Lấy thông tin UserId từ Claims
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            // Lấy thêm thông tin User từ database nếu cần
            var userId = Guid.Parse(userIdClaim);
            var user = _unitOfWork.UserRepository.GetById(userId, true).Result;

            return user;
        }
        catch (Exception ex)
        {
            // Log lỗi nếu cần thiết
            return null;
        }
    }

    public BusinessResult HandlerError(string message)
    {
        var errorMessage = $"An error {typeof(TEntity).Name}: {message}";
        return new ResponseBuilder()
            .WithStatus(Const.FAIL_CODE)
            .WithMessage(errorMessage)
            ;
    }

    public BusinessResult HandlerNotFound(string message = Const.NOT_FOUND_MSG)
    {
        return new ResponseBuilder()
            .WithStatus(Const.NOT_FOUND_CODE)
            .WithMessage(message)
            ;
    }

    public BusinessResult HandlerFail(string message)
    {
        return new ResponseBuilder()
            .WithStatus(Const.FAIL_CODE)
            .WithMessage(message)
            ;
    }
}
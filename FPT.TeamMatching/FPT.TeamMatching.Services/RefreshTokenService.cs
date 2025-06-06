﻿using System.Net;
using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.RefreshTokens;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results;
using FPT.TeamMatching.Domain.Models.Results.Bases;
using FPT.TeamMatching.Domain.Utilities;
using FPT.TeamMatching.Services.Bases;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FPT.TeamMatching.Services;

public class RefreshTokenService : BaseService<RefreshToken>, IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly TokenSetting _tokenSetting;

    public RefreshTokenService(IMapper mapper,
        IUnitOfWork unitOfWork,
        IUserService userService,
        IOptions<TokenSetting> tokenSetting
    ) : base(mapper, unitOfWork)
    {
        _tokenSetting = tokenSetting.Value;
        _refreshTokenRepository = _unitOfWork.RefreshTokenRepository;
    }

    public new async Task<BusinessResult> CreateOrUpdate<TResult>(CreateOrUpdateCommand createOrUpdateCommand)
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
            var errorMessage = $"An error occurred while updating {typeof(RefreshToken).Name}: {ex.Message}";
            return new ResponseBuilder()
                .WithStatus(Const.FAIL_CODE)
                .WithMessage(errorMessage);
        }
    }

    public BusinessResult ValidateRefreshTokenIpMatch()
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                        ?? _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken)) return HandlerFail(Const.FAIL_UNAUTHORIZED_MSG);

        ipAddress = NormalizeIpAddress(ipAddress);
        // Kiểm tra refreshToken và IP address
        var storedRefreshToken = _refreshTokenRepository.GetByRefreshTokenAsync(refreshToken).Result;

        if (storedRefreshToken == null || storedRefreshToken.Expiry < DateTime.UtcNow) return HandlerFail("Hết phiên đăng nhập!");

        if (storedRefreshToken.IpAddress != ipAddress)
            return HandlerFail("Có ai đó đang cố gắng truy cập nick bạn!");

        var refreshTokenResult = _mapper.Map<RefreshTokenResult>(storedRefreshToken);
        return new ResponseBuilder()
            .WithData(refreshTokenResult)
            .WithStatus(Const.SUCCESS_CODE)
            .WithMessage(Const.SUCCESS_READ_MSG);
    }

    private string NormalizeIpAddress(string ipAddress)
    {
        if (ipAddress.Contains(",")) ipAddress = ipAddress.Split(',')[0].Trim();

        if (IPAddress.TryParse(ipAddress, out var ip))
        {
            if (ip.IsIPv4MappedToIPv6) return ip.MapToIPv4().ToString();

            // Chuyển loopback IPv6 (::1) về loopback IPv4 (127.0.0.1)
            if (IPAddress.IPv6Loopback.Equals(ip)) return IPAddress.Loopback.ToString(); // Trả về 127.0.0.1
        }

        return ipAddress;
    }

    protected new async Task<RefreshToken?> CreateOrUpdateEntity(CreateOrUpdateCommand createOrUpdateCommand)
    {
        RefreshToken? entity = null;
        if (createOrUpdateCommand is RefreshTokenUpdateCommand updateCommand)
        {
            entity = await _refreshTokenRepository.GetById(updateCommand.Id);
            if (entity == null) return null;

            _mapper.Map(updateCommand, entity);

            await SetBaseEntityForUpdate(entity);
            _refreshTokenRepository.Update(entity);
        }
        else if (createOrUpdateCommand is RefreshTokenCreateCommand createCommand)
        {
            
            
            var httpContext = _httpContextAccessor.HttpContext;
            createCommand.UserAgent = httpContext.Request.Headers["User-Agent"].ToString() ?? "Unknown";
            createCommand.IpAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                                      ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            createCommand.IpAddress = NormalizeIpAddress(createCommand.IpAddress);
            createCommand.Expiry = DateTime.UtcNow.AddDays(_tokenSetting.RefreshTokenExpiryDays);
            entity = _mapper.Map<RefreshToken>(createCommand);
            if (entity == null) return null;
            
            var queryable = _refreshTokenRepository.GetQueryable();
            var refreshTokens = queryable.Where(rs => 
                rs.UserId == createCommand.UserId && rs.IpAddress == createCommand.IpAddress).ToList();

            if (refreshTokens.Any())
            {
                // Logout any UserId ad IpAddress dup
                foreach (var refreshToken in refreshTokens)
                {
                    _refreshTokenRepository.DeletePermanently(refreshToken);
                }
                var saveChanges_ = await _unitOfWork.SaveChanges();
                if (!saveChanges_) return null;
            }
            
            entity.Id = Guid.NewGuid();
            await SetBaseEntityForCreation(entity);
            _refreshTokenRepository.Add(entity);
        }

        var saveChanges = await _unitOfWork.SaveChanges();
        return saveChanges ? entity : null;
    }
}
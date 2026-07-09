using Application.Dtos;

namespace Api.Auth.Dto;

/// <summary>
/// 
/// </summary>
/// <param name="NewAccessToken"></param>
/// <param name="NewRefreshToken"></param>
/// <param name="User"></param>
public record LoginResult(UpdateTokenEvent NewAccessToken, UpdateTokenEvent NewRefreshToken, UserDto User);

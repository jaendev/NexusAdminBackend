using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NexusAdmin.Core.Entities;
using NexusAdmin.Core.Exceptions;
using NexusAdmin.Core.Interfaces.Repositories;
using NexusAdmin.Core.UseCases.Users.UpdateUser;
using NexusAdmin.Functions.Configuration;
using NexusAdmin.Functions.DTO.User;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace NexusAdmin.Functions.Users;

public class UpdateUserFunction
{
    private readonly ILogger<UpdateUserFunction> _logger;
    private readonly UpdateUserUseCase _updateUserUseCases;

    public UpdateUserFunction(ILogger<UpdateUserFunction> logger, UpdateUserUseCase updateUserUseCases)
    {
        this._logger = logger;
        this._updateUserUseCases = updateUserUseCases;
    }

    [Function("UpdateUser")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "patch", Route = "users/{userId}")]
        HttpRequestData req,
        string userId
        )
    {
        _logger.LogInformation($"Updating user: {userId}");

        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UpdateUserDto? dto = JsonSerializer.Deserialize<UpdateUserDto>(requestBody, JsonOptions.Default);

            if (dto == null)
            {
                HttpResponseData badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badRequest;
            }

            UpdateUserRequest request = new UpdateUserRequest
            {
                Name = dto.Name,
                Role = !string.IsNullOrWhiteSpace(dto.Role) && Enum.TryParse<User.UserRole>(dto.Role, out var userRole)
                    ? userRole
                    : null
            };

            UpdateUserResponse result = await this._updateUserUseCases.ExecuteAsync(userId, request);
            
            this._logger.LogInformation($"User updated successfully: {userId}");
            
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                success = true,
                message = "User updated successfully",
                data = result
            });

            return response;
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning($"User not found: {ex.Message}");
            var notFound = req.CreateResponse(HttpStatusCode.NotFound);
            await notFound.WriteAsJsonAsync(new { error = ex.Message });
            return notFound;
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning($"Validation error: {ex.Message}");
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteAsJsonAsync(new { error = ex.Message });
            return badRequest;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error: {ex.Message}");
            var error = req.CreateResponse(HttpStatusCode.InternalServerError);
            await error.WriteAsJsonAsync(new { error = "Internal server error" });
            return error;
        }
    }
}

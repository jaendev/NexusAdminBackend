using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NexusAdmin.Core.Exceptions;
using NexusAdmin.Core.UseCases.Users.DeactivateUser;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace NexusAdmin.Functions.Users;

public class DeactivateUserFunction
{
    private readonly ILogger<DeactivateUserFunction> _logger;
    private readonly DeactivateUserUseCase _deactivateUserUseCase;

    public DeactivateUserFunction(ILogger<DeactivateUserFunction> logger, 
        DeactivateUserUseCase deactivateUserUseCase)
    {
        this._logger = logger;
        this._deactivateUserUseCase = deactivateUserUseCase;
    }

    [Function("DeactivateUser")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/{userId}/deactivate")]
        HttpRequestData req,
        string userId)
    {
        this._logger.LogInformation($"Deactivating user: {userId}");

        try
        {
            DeactivateUserResponse result = await this._deactivateUserUseCase.ExecuteAsync(userId);
            
            this._logger.LogInformation($"User deactivated successfully: {userId}");
            
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                success = true,
                message = "User deactivated successfully",
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

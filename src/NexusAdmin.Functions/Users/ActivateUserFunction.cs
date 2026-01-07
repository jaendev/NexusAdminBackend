using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NexusAdmin.Core.Exceptions;
using NexusAdmin.Core.UseCases.Users.ActivateUser;

namespace NexusAdmin.Functions.Users;

public class ActivateUserFunction
{
    private readonly ILogger<ActivateUserFunction> _logger;
    private readonly ActivateUserUseCase _activateUserUseCase;

    public ActivateUserFunction(ILogger<ActivateUserFunction> logger, ActivateUserUseCase activateUserUseCase)
    {
        this._logger = logger;
        this._activateUserUseCase = activateUserUseCase;
    }

    [Function("ActivateUser")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/{userId}/activate")]
        HttpRequestData req,
        string userId
        )
    {
        this._logger.LogInformation($"Activating user: {userId}");
        
        try
        {
            ActivateUserResponse result = await this._activateUserUseCase.ExecuteAsync(userId);

            this._logger.LogInformation($"User activated successfully: {userId}");

            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(new
            {
                success = true,
                message = "User activated successfully",
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

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NexusAdmin.Core.Exceptions;
using NexusAdmin.Core.UseCases.Users.DeleteUser;

namespace NexusAdmin.Functions.Users;

public class DeleteUserFunction
{
    private readonly ILogger<DeleteUserFunction> _logger;
    private readonly DeleteUserUseCase _deleteUserUseCase;

    public DeleteUserFunction(ILogger<DeleteUserFunction> logger, DeleteUserUseCase deleteUserUseCase)
    {
        this._logger = logger;
        this._deleteUserUseCase = deleteUserUseCase;
    }

    [Function("DeleteUser")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "users/{userId}")]
        HttpRequestData req,
        string userId
        )
    {
        this._logger.LogInformation($"Deleting user: {userId}");

        try
        {
            await this._deleteUserUseCase.ExecuteAsync(userId);
            
            this._logger.LogInformation($"User deleted successfully: {userId}");
            
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(new
            {
                success = true,
                message = "User deleted successfully"
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
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error: {ex.Message}");
            var error = req.CreateResponse(HttpStatusCode.InternalServerError);
            await error.WriteAsJsonAsync(new { error = "Internal server error" });
            return error;
        }
    }
}

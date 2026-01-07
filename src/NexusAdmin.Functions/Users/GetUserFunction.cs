using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NexusAdmin.Core.Exceptions;
using NexusAdmin.Core.UseCases.Users.GetUser;

namespace NexusAdmin.Functions.Users;

public class GetUserFunction
{
    private readonly ILogger<GetUserFunction> _logger;
    private readonly GetUserByIdUseCase _getUserByIdUseCase;

    public GetUserFunction(ILogger<GetUserFunction> logger, GetUserByIdUseCase getUserByIdUseCase)
    {
        this._logger = logger;
        this._getUserByIdUseCase = getUserByIdUseCase;
    }

    [Function("GetUser")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{userId}")]
        HttpRequestData req,
        string userId
    )
    {
        this._logger.LogInformation($"Getting user: {userId}");

        try
        {
            GetUserResponse result = await this._getUserByIdUseCase.ExecuteAsync(userId);
            
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                success = true,
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
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error: {ex.Message}");
            var error = req.CreateResponse(HttpStatusCode.InternalServerError);
            await error.WriteAsJsonAsync(new { error = "Internal server error" });
            return error;
        }
    }
}

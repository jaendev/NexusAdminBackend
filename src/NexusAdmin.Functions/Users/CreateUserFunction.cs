using System;
using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using NexusAdmin.Core.UseCases.Users.CreateUser;
using NexusAdmin.Functions.Configuration;

namespace NexusAdmin.Functions.Users;

public class CreateUserFunction
{
    private readonly ILogger<CreateUserFunction> _logger;

    public CreateUserFunction(ILogger<CreateUserFunction> logger)
    {
        this._logger = logger;
    }

    [Function("CreateUser")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users")]
        HttpRequestData req
    )
    {
        this._logger.LogInformation("Process user creation");
        // Read body
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CreateUserRequest? data = JsonSerializer.Deserialize<CreateUserRequest>(
                requestBody,
                JsonOptions.Default
            );

            if (data == null)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badRequest;
            }

            // TODO: Call here Use Case

            // var result = await _createUserUseCase.ExecuteAsync(data);

            // For now, test request
            HttpResponseData? response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                message = "Created user",
                data = new
                {
                    id = Guid.NewGuid().ToString(),
                    email = data.Email,
                    name = data.Name
                }
            });

            return response;
        }
        catch (Exception ex)
        {
            this._logger.LogError($"Error: {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "Inside server error" });
            return errorResponse;
        }
    }
}
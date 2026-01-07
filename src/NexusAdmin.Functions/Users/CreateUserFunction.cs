using System;
using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using NexusAdmin.Core.Entities;
using NexusAdmin.Core.Exceptions;
using NexusAdmin.Core.UseCases.Users.CreateUser;
using NexusAdmin.Functions.Configuration;
using NexusAdmin.Functions.DTO.User;

namespace NexusAdmin.Functions.Users;

public class CreateUserFunction
{
    private readonly ILogger<CreateUserFunction> _logger;
    private readonly CreateUserUseCase _createUserUseCase;

    public CreateUserFunction(ILogger<CreateUserFunction> logger, CreateUserUseCase createUserUseCase)
    {
        this._logger = logger;
        this._createUserUseCase = createUserUseCase;
    }

    [Function("CreateUser")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users")]
        HttpRequestData req
    )
    {
        this._logger.LogInformation("Processing user creation request");
        
        try
        {
            // Read and deserialize request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CreateUserDto? clientDto = JsonSerializer.Deserialize<CreateUserDto>(requestBody, JsonOptions.Default);

            if (clientDto == null) 
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badResponse;
            }
            
            // Map DTO to use case request
            var request = new CreateUserRequest
            {
                Email = clientDto.Email,
                Name = clientDto.Name,
                Role = Enum.TryParse<User.UserRole>(clientDto.Role, out var role) ? role : User.UserRole.User
            };

            // Execute use case
            var result = await this._createUserUseCase.ExecuteAsync(request);

            _logger.LogInformation($"User created successfully: {result.Id}");

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(new
            {
                success = true,
                message = "User created successfully",
                data = result
            });

            return response;
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning($"Validation error: {ex.Message}");
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteAsJsonAsync(new { error = ex.Message });
            return badRequest;
        }
        catch (UserAlreadyExistsException ex)
        {
            _logger.LogWarning($"User already exists: {ex.Message}");
            var conflict = req.CreateResponse(HttpStatusCode.Conflict);
            await conflict.WriteAsJsonAsync(new { error = ex.Message });
            return conflict;
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

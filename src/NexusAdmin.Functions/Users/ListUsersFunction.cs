using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NexusAdmin.Core.Interfaces.Repositories;
using NexusAdmin.Core.UseCases.Users.ListUsers;

namespace NexusAdmin.Functions.Users;

public class ListUsersFunction
{
    private readonly ILogger<ListUsersFunction> _logger;
    private readonly ListUsersUseCase _listUsersUseCase;

    public ListUsersFunction(ILogger<ListUsersFunction> logger, ListUsersUseCase listUsersUseCase)
    {
        this._logger = logger;
        this._listUsersUseCase = listUsersUseCase;
    }

    [Function("ListUsers")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users")]
        HttpRequestData req
        )
    {
        _logger.LogInformation("Listing users");

        try
        {
            // Read query parameters for pagination
            NameValueCollection query = HttpUtility.ParseQueryString(req.Url.Query);
            int.TryParse(query["page"], out int page);
            int.TryParse(query["pageSize"], out int pageSize);

            ListUsersRequest request = new ListUsersRequest
            {
                Page = page > 0 ? page : 1,
                PageSize = pageSize > 0 ? pageSize : 10
            };
            
            ListUsersResponse result = await this._listUsersUseCase.ExecuteAsync(request);
            
            _logger.LogInformation($"Retrieved {result.Users.Count} users (page {result.Page} of {result.TotalPages})");
            
            HttpResponseData response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                success = true,
                data = result
            });

            return response;
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

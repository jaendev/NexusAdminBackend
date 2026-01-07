namespace NexusAdmin.Core.UseCases.Users.ListUsers;

public class ListUsersRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
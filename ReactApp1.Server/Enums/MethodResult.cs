namespace ReactApp1.Server.Enums
{
    public enum MethodResult
    {
        Success = 200,
        Created = 201,
        NoContent = 204,
        NotFound = 404,
        ValidationError = 422,
        Unauthorized = 401,
        Forbidden = 403,
        Conflict = 409,
        InternalError = 500,
    }
}

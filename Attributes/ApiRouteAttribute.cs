using Microsoft.AspNetCore.Mvc;

public class ApiRouteAttribute : RouteAttribute
{
    // Constructor that adds "api/" prefix to the route
    public ApiRouteAttribute(string template) : base($"api/{template}") {}
}
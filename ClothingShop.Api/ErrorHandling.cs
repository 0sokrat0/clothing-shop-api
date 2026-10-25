using ClothingShop.Application.Exceptions;
using ClothingShop.Domain;
using Microsoft.AspNetCore.Diagnostics;

namespace ClothingShop.Api;

// Переводит исключения слоёв Domain/Application в HTTP-коды
public class ErrorHandling : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception ex, CancellationToken ct)
    {
        var status = ex switch
        {
            DomainException => StatusCodes.Status400BadRequest,
            BadHttpRequestException => StatusCodes.Status400BadRequest,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        http.Response.StatusCode = status;
        var message = status == 500 ? "Внутренняя ошибка сервера" : ex.Message;
        await http.Response.WriteAsJsonAsync(new { error = message }, ct);
        return true;
    }
}

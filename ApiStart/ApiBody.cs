using Microsoft.AspNetCore.Http.HttpResults;
 
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiStart;


public static class ApiBody
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => "Hello, World!"); 
        app.MapGet("/test", (string arg) => $"Test "+arg); 
        app.MapGet("/method", (string arg, string age) => GetApIMethod(arg, age));  
        return app;
    }

    public static IResult GetApIMethod(string arg, string age)
    {
        int iage = 0;        
        if(!int.TryParse(age, out iage))
            return Results.BadRequest("age는 숫자여야 합니다.");

        return Results.Ok($"{arg}는 {iage}살 입니다.");
    }
}
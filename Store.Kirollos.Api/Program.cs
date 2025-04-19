
using Domain.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Data;
using Services;
using Services.Abstractions;
using Shared.ErrorModels;
using Store.Kirollos.Api.Extentions;
using Store.Kirollos.Api.Middlewares;
using System.Threading.Tasks;



namespace Store.Kirollos.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.RegisterAllServices(builder.Configuration);

            var app = builder.Build();

            await app.ConfigureMiddlewares();

            app.Run();
        }
    }
}

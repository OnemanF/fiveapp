using System.Data.Common;
using api;
using efscaffold;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var appOptions = builder.Services.AddAppOptions(builder.Configuration);

builder.Services.AddDbContext<MyDbContext>(conf =>
{
    conf.UseNpgsql(appOptions.DbConnectionString);
});

var app = builder.Build();

app.MapGet("/", ([FromServices] MyDbContext db) =>
{   
    var objects = db.Authors.ToList();
});
    
app.Run();

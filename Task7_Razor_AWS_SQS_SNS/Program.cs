using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Task7_Razor_AWS_SQS_SNS;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAWSService<IAmazonS3>(new AWSOptions
{
    Credentials = new BasicAWSCredentials("AKIARKDSGBF6YGLNWV77", "I1PJCBwgV7B4abNb6Qb4aXlw60yBWmq8om/DmjEM"),
    Region = Amazon.RegionEndpoint.EUNorth1 // Replace with your AWS region, e.g., USWest2
});

// Add Amazon Simple Notification Service
builder.Services.AddAWSService<IAmazonSimpleNotificationService>(new AWSOptions
{
    Credentials = new BasicAWSCredentials("AKIARKDSGBF6YGLNWV77", "I1PJCBwgV7B4abNb6Qb4aXlw60yBWmq8om/DmjEM"),
    Region = Amazon.RegionEndpoint.EUNorth1 // Replace with your AWS region
});

builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddLogging();


builder.Logging.ClearProviders();
builder.Logging.AddConsole(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();



app.MapRazorPages();

app.Run();

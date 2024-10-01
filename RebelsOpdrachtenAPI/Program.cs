using Amazon.DynamoDBv2;
using Amazon.Runtime;
using RebelsOpdrachtenAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

var awsOptions = builder.Configuration.GetAWSOptions();

var awsAccessKey = builder.Configuration["AWS:AccessKey"];
var awsSecretKey = builder.Configuration["AWS:SecretKey"];
var awsRegion = builder.Configuration["AWS:Region"];
var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
awsOptions.Credentials = credentials;
awsOptions.Region = Amazon.RegionEndpoint.GetBySystemName(awsRegion);

builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddControllers();
builder.Services.AddCors(options =>{
    options.AddPolicy("AllowSpecificOrigins",
    builder => {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
        
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowSpecificOrigins");
app.MapControllers();
app.Run();

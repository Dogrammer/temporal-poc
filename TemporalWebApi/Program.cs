using Temporalio.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Temporal client as singleton
builder.Services.AddSingleton<ITemporalClient>(sp =>
{
    var temporalAddress = builder.Configuration.GetValue<string>("Temporal:Address") ?? "localhost:7233";
    return TemporalClient.ConnectAsync(new TemporalClientConnectOptions(temporalAddress)).Result;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Commented out for local development
// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

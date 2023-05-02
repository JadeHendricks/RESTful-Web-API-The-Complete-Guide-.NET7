//logging is already registered here for us automatically - DI
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//AddNewtonsoftJson adding this for the api to allow PUT endpoint fallbacks
//ReturnHttpNotAcceptable only allows data in a format our api accepts
builder.Services.AddControllers(option => {
    //option.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using APICatalogo.Context;
using APICatalogo.DTO.Mappings;
using APICatalogo.Filters;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
           {
               options.AddPolicy("EnableCORS", builder =>
               {
                   builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build();
               });
           });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseMySql(mySqlConnection,
                    ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
      .AddEntityFrameworkStores<AppDbContext>()
      .AddDefaultTokenProviders();

builder.Services.AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme).
        AddJwtBearer(options =>
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidAudience = builder.Configuration["TokenConfiguration:Audience"],
             ValidIssuer = builder.Configuration["TokenConfiguration:Issuer"],
             ValidateIssuerSigningKey = true,
             IssuerSigningKey = new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
         });


builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "APICatalogo",
        Description = "Cat�logo de Produtos e Categorias",
        TermsOfService = new Uri("https://localhost:5001/terms"),
        Contact = new OpenApiContact
        {
            Name = "Lauren",
            Email = "laurengoncalves.dev@gmail.com",
            Url = new Uri("https://localhost:5001"),
        },
        License = new OpenApiLicense
        {
            Name = "Usar sobre LICX",
            Url = new Uri("https://localhost:5001/terms"),
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Header do JWT Authorization usando o schema Bearer.Informe 'Bearer'[espa�o] e a seguir o seu token. Exemplo: \"Bearer 12345abcdef\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddControllers()
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling
      = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});


var app = builder.Build();

// // Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //Swagger
    app.UseSwagger();
    //SwaggerUI
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalogo");
    });
}
else
{
    app.UseSwagger();
    //SwaggerUI
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalogo");
        c.InjectStylesheet("/swagger/custom.css");
        c.RoutePrefix = String.Empty;
    });
    app.UseHsts();
}
app.UseHttpsRedirection();

//adiciona o middleware de roteamento 
app.UseRouting();

app.UseCors("EnableCORS");

//adiciona o middleware de autenticacao
app.UseAuthentication();

//adiciona o middleware que habilita a autorizacao
app.UseAuthorization();

//Adiciona o middleware que executa o endpoint 
//do request atual
app.UseEndpoints(endpoints =>
{
    // adiciona os endpoints para as Actions
    // dos controladores sem especificar rotas
    endpoints.MapControllers();
});
app.Run();
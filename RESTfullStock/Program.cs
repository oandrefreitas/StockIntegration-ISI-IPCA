using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using RESTfullStock.Services;



var builder = WebApplication.CreateBuilder(args);

// <summary>
// Adiciona controladores ao contêiner de dependências.
// </summary>
builder.Services.AddControllers();

// <summary>
// Configura o serviço de autenticação JWT baseado no segredo armazenado no appsettings.json.
// Define o tempo de expiração do token.
// </summary>
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
var tokenExpirationMinutes = int.Parse(builder.Configuration["JwtSettings:TokenExpirationMinutes"]);

// <summary>
// Regtisto do AuthService no contêiner de dependências.
// 
// </summary>
builder.Services.AddTransient<AuthService>(provider =>
    new AuthService(jwtSecret, tokenExpirationMinutes));

// <summary>
// Registra o serviço HTTP para realizar chamadas a APIs externas.
// </summary>
builder.Services.AddHttpClient<CurrencyService>();

// <summary>
// Configura autenticação baseada em JWT e define os parâmetros de validação.
// </summary>
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; // Permite chamadas HTTP em ambientes de teste.
    x.SaveToken = true; // Salva o token gerado.
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Valida a chave de assinatura.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)), // Configura a chave secreta.
        ValidateIssuer = false, // Ignora validação do emissor.
        ValidateAudience = false // Ignora validação do público.
    };
});

// <summary>
// Define políticas de autorização para diferentes roles (admin, gestor, operador).
// </summary>
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
    options.AddPolicy("Gestor", policy => policy.RequireRole("ges"));
    options.AddPolicy("Operador", policy => policy.RequireRole("op"));
    options.AddPolicy("AdminOrGestor", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("admin") || context.User.IsInRole("ges")));
});

// <summary>
// Configura o Swagger para gerar documentação da API e suporte a autenticação JWT.
// </summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RESTfullStock API", Version = "v1" });

    // Configura autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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

    // Adiciona suporte para comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// <summary>
// Constrói e inicializa o aplicativo com os middlewares necessários.
// </summary>
var app = builder.Build();

// Middleware para usar autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Configura o Swagger e a interface SwaggerUI em ambientes de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mapeia controladores para os endpoints correspondentes
app.MapControllers();

// Inicia o aplicativo
app.Run();

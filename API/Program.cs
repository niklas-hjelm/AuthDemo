using API.DataAccess;
using API.DataAccess.DemoData;
using API.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
builder.Services.AddAuthorizationBuilder();
builder.Services.AddDbContext<ApplicationDbContext>(
	options =>
	{
		options.UseInMemoryDatabase("AppDb");
	});

builder.Services.AddScoped<ThingRepository>();

builder.Services.AddIdentityCore<ApplicationUser>()
	.AddRoles<ApplicationRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddApiEndpoints();

builder.Services.AddCors(
	options => options.AddPolicy(
		"wasm",
		policy => policy.WithOrigins([builder.Configuration["BackendUrl"] ?? "http://localhost:5161",
			builder.Configuration["FrontendUrl"] ?? "http://localhost:5241"])
			.AllowAnyMethod()
			.AllowAnyHeader()
			.AllowCredentials()));

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
	// Seed the database
	await using var scope = app.Services.CreateAsyncScope();
	await SeedData.InitializeAsync(scope.ServiceProvider);
}

app.MapIdentityApi<ApplicationUser>();

app.UseCors("wasm");

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager, object empty) =>
{
	if (empty is not null)
	{
		await signInManager.SignOutAsync();

		return Results.Ok();
	}

	return Results.Unauthorized();
}).RequireAuthorization();

app.UseHttpsRedirection();

app.MapGet("/roles", (ClaimsPrincipal user) =>
{
	if (user.Identity is not null && user.Identity.IsAuthenticated)
	{
		var identity = (ClaimsIdentity)user.Identity;
		var roles = identity.FindAll(identity.RoleClaimType)
			.Select(c =>
				new
				{
					c.Issuer,
					c.OriginalIssuer,
					c.Type,
					c.Value,
					c.ValueType
				});

		return TypedResults.Json(roles);
	}

	return Results.Unauthorized();
}).RequireAuthorization();

app.MapGet("/things", async (ThingRepository repo) =>
{
	return await repo.GetAllThingsAsync();
}).RequireAuthorization();

app.MapPost("/things", async (ThingRepository repo, string newThing) => 
{
	await repo.AddThingAsync(newThing);
}).RequireAuthorization(policy => policy.RequireRole("Administrator"));

app.Run();
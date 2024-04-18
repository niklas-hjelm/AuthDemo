using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using API.DataAccess.Entities;

namespace API.DataAccess.DemoData;

public class SeedData
{
	private static readonly IEnumerable<SeedUser> seedUsers =
	[
		new SeedUser()
		{
			Email = "admin@admin.admin",
			NormalizedEmail = "ADMIN@ADMIN.ADMIN",
			NormalizedUserName = "ADMIN@ADMIN.ADMIN",
			RoleList = [ "Administrator", "Manager" ],
			UserName = "admin@admin.admin"
		},
		new SeedUser()
		{
			Email = "user@user.user",
			NormalizedEmail = "USER@USER.USER",
			NormalizedUserName = "USER@USER.USER",
			RoleList = [ "User" ],
			UserName = "user@user.user"
		},
	];

	private static readonly IEnumerable<string> seedThings =
		[
			"Apple",
			"Banana"
		];

	public static async Task InitializeAsync(IServiceProvider serviceProvider)
	{
		using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

		if (context.Users.Any())
		{
			return;
		}

		var userStore = new UserStore<ApplicationUser>(context);
		var password = new PasswordHasher<ApplicationUser>();

		using var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

		string[] roles = ["Administrator", "Manager", "User"];

		foreach (var role in roles)
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new ApplicationRole(role));
			}
		}

		using var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

		foreach (var user in seedUsers)
		{
			var hashed = password.HashPassword(user, "Passw0rd!");
			user.PasswordHash = hashed;
			await userStore.CreateAsync(user);

			if (user.Email is not null)
			{
				var appUser = await userManager.FindByEmailAsync(user.Email);

				if (appUser is not null && user.RoleList is not null)
				{
					await userManager.AddToRolesAsync(appUser, user.RoleList);
				}
			}
		}

		foreach (var thing in seedThings)
		{
			var newThing = new Thing() { Name = thing };
			context.Things.Add(newThing);
		}

		await context.SaveChangesAsync();
	}

	private class SeedUser : ApplicationUser
	{
		public string[]? RoleList { get; set; }
	}
}

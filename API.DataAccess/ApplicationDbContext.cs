using API.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess;

public class ApplicationDbContext :
		IdentityDbContext<ApplicationUser, ApplicationRole, string,
			IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
			IdentityRoleClaim<string>, IdentityUserToken<string>>
{

    public DbSet<Thing> Things { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
    }

    protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<ApplicationUser>(b =>
		{
			// Each User can have many UserClaims
			b.HasMany(e => e.Claims)
				.WithOne()
				.HasForeignKey(uc => uc.UserId)
				.IsRequired();

			// Each User can have many UserLogins
			b.HasMany(e => e.Logins)
				.WithOne()
				.HasForeignKey(ul => ul.UserId)
				.IsRequired();

			// Each User can have many UserTokens
			b.HasMany(e => e.Tokens)
				.WithOne()
				.HasForeignKey(ut => ut.UserId)
				.IsRequired();

			b.HasMany(e => e.UserRoles)
				.WithOne(e => e.User)
				.HasForeignKey(ur => ur.UserId)
				.IsRequired();

		});


		builder.Entity<ApplicationRole>(b =>
			b.HasMany(e => e.UserRoles)
				.WithOne(e => e.Role)
				.HasForeignKey(ur => ur.RoleId)
				.IsRequired()
			);
	}
}

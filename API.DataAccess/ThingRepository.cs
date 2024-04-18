using API.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess;

public class ThingRepository(ApplicationDbContext context)
{
	public async Task<IEnumerable<Thing>> GetAllThingsAsync()
	{
		return await context.Things.ToListAsync();
	}

	public async Task AddThingAsync(string thingName)
	{
		Thing thing = new() { Name=thingName };
		await context.Things.AddAsync(thing);
		await context.SaveChangesAsync();
	}
}

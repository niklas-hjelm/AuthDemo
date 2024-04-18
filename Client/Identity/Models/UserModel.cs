namespace Client.Identity.Models;

public class UserModel
{
	public string Email { get; set; } = string.Empty;

	public bool IsEmailConfirmed { get; set; }

	public Dictionary<string, string> Claims { get; set; } = [];
}

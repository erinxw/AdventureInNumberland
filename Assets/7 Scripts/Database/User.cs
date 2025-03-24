

public class User
{
    public string email;
    public string password;
    public int guardianPasscode;

    public User(string email, string password, int guardianPasscode)
    {
	this.email = email;
	this.password = password;
	this.guardianPasscode = guardianPasscode;
    }
}

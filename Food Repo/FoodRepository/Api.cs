using System.Security.Cryptography;
using System.Text;


namespace FoodRepository;



// Api class that contains methods to configure the API endpoints.
public static class Api
{
    public static void ConfigureApi(this WebApplication app)
    {
        // All of my api endpoint mapping
        app.MapPost("/signup", SignUp);
        app.MapPost("/signin", SignIn);
        app.MapGet("/logout", Logout);
        app.MapGet("/Users", GetUsers);
        app.MapGet("/Users/{id}", GetUser);
        app.MapPost("/Users", InsertUser);
        app.MapPut("/Users", UpdateUser);
        app.MapDelete("/Users", DeleteUser);
    }

    // GetUsers is a method that returns a list of UserModel objects.
    private static async Task<IResult> GetUsers(IUserData data)
    {
        // Try to get a list of UserModel objects.
        try
        {
            return Results.Ok(await data.GetUsers());
        }

        // If an exception is thrown, return a problem result.
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    // GetUser is a method that returns a UserModel object.
    private static async Task<IResult> GetUser(int id, IUserData data)
    {
        // Try to get a UserModel object.
        try
        {
            var results = await data.GetUser(id);
            if (results == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(results);
        }

        // If an exception is thrown, return a problem result.
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    // InsertUser is a method that inserts a UserModel object.
    private static async Task<IResult> InsertUser(UserModel user, IUserData data)
    {
        // Try to insert a UserModel object.
        try
        {
            await data.InsertUser(user);
            return Results.Ok();
        }

        // If an exception is thrown, return a problem result.
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    // UpdateUser is a method that updates a UserModel object.
    private static async Task<IResult> UpdateUser(UserModel user, IUserData data)
    {
        // Try to update a UserModel object.
        try
        {
            await data.UpdateUser(user);
            return Results.Ok();
        }

        // If an exception is thrown, return a problem result.
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    // DeleteUser is a method that deletes a UserModel object.
    private static async Task<IResult> DeleteUser(int id, IUserData data)
    {
        // Try to delete a UserModel object.
        try
        {
            await data.DeleteUser(id);
            return Results.Ok();
        }

        // If an exception is thrown, return a problem result.
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> SignUp(UserModel user, IUserData data)
    {
        try
        {
            // Check if username or email already exists
            var existingUser = await data.GetUserByUsernameOrEmail(user.Username, user.Email);
            if (existingUser != null)
                return Results.Conflict("Username or email already exists.");

            // Hash the password before storing it
            user.Password = HashPassword(user.Password);

            // Insert user into the database
            await data.InsertUser(user);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> SignIn(UserModel user, IUserData data)
    {
        try
        {
            // Retrieve user from the database by username
            var existingUser = await data.GetUserName(user.Username);
            if (existingUser == null)
                return Results.NotFound("User not found.");

            // Verify password
            if (!VerifyPassword(user.Password, existingUser.Password))
                return Results.BadRequest("Invalid username or password.");

            // Generate token for session
            string token = GenerateToken();

            // Update user token in the database
            await data.UpdateUserToken(existingUser.Id, token);

            // Return token
            return Results.Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    private static async Task<IResult> Logout(string token, IUserData data)
    {
        try
        {
            // Clear token in the database
            await data.ClearUserToken(token);
            return Results.Ok();
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    public static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            // ComputeHash returns byte array, convert it to a string
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Convert byte array to a string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                builder.Append(hashedBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        // Hash the incoming password and compare it with the hashed password
        return HashPassword(password) == hashedPassword;
    }

    public static string GenerateToken()
    {
        byte[] tokenBytes = new byte[64];
        using (var rngProvider = new RNGCryptoServiceProvider())
        {
            rngProvider.GetBytes(tokenBytes);
        }
        return Convert.ToBase64String(tokenBytes);
    }
}


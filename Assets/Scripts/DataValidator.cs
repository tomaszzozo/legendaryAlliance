using System.Text.RegularExpressions;
using System.Linq;

public class DataValidator
{
    public const string statusOk = "OK";

    private static readonly Regex containsLowercase = new("[a-z]");
    private static readonly Regex containsUppercase = new("[A-Z]");
    private static readonly Regex containsDigit = new("[0-9]");
    private static readonly Regex containsSpecialCharacter = new("[^a-zA-Z0-9]");

    public static string ValidateSignUpData(string username, string password, string repeatPassword)
    {
        if (username.Length == 0) return "Enter username first.";
        if (password.Length == 0) return "Enter password first.";
        if (repeatPassword.Length == 0) return "Repeat your password.";

        string verificationResult = ValidateUsername(username);
        if (!verificationResult.Equals(statusOk)) return verificationResult;

        verificationResult = ValidatePassword(password);
        if (!verificationResult.Equals(statusOk)) return verificationResult;

        verificationResult = VerifyPasswords(password, repeatPassword);
        return verificationResult;
    }

    private static string ValidatePassword(string password)
    {
        if (password.Length < 5) return "Password must have at least 5 characters.";
        if (password.Length > 20) return "Password must have less than 20 characters.";
        if (!containsLowercase.IsMatch(password)) return "Password must contain at least one lowercase letter.";
        if (!containsUppercase.IsMatch(password)) return "Password must contain at least one uppercase letter.";
        if (!containsDigit.IsMatch(password)) return "Password must contain at least one digit.";
        if (!containsSpecialCharacter.IsMatch(password)) return "Password must contain at least one special non-white character.";
        if (password.Any(x => char.IsWhiteSpace(x))) return "Password can not contain white characters.";
        return statusOk;
    }

    private static string ValidateUsername(string username)
    {
        if (username.Length < 5) return "Username must have at least 5 characters.";
        if (username.Length > 20) return "Username must have less than 20 characters.";
        if (!containsLowercase.IsMatch(username) && !containsUppercase.IsMatch(username)) return "Username must contain uppercase or lowercase letters.";
        if (containsSpecialCharacter.IsMatch(username)) return "Username can only consist of digits and characters";
        return statusOk;
    }

    private static string VerifyPasswords(string password1, string password2)
    {
        return password1.Equals(password2) ? statusOk : "Passwords do not match.";
    }
}

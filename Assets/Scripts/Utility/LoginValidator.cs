using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginValidator
{
    private static int minEmailLength = 3;
    private static int maxEmailLength = 254;

    private static int minPasswordLength = 6;

    public static bool ValidateRegistration(string username, string email, string password,out string errormessage)
    {
        if (username == string.Empty)
        {
            errormessage = "Invalid username: Username field is empty";
            return false;
        }
        if (email == string.Empty)
        {
            errormessage = "Invalid email: Email field is empty";
            return false;
        }
        if (!email.Contains("@"))
        {
            errormessage = "Invalid email address";
            return false;
        }
        if (!email.Contains(".")) {
            errormessage = "Invalid email address";
            return false;
        }
        if (email.Length < minEmailLength || email.Length > maxEmailLength)
        {
            errormessage = "Invalid email address";
            return false;
        }
        if (password.Length < minPasswordLength)
        {
            errormessage = "Invalid password: must have at least 6 characters";
            return false;
        }
        errormessage = "";
        return true;
    }
}

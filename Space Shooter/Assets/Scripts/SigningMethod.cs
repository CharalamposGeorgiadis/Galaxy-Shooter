using UnityEngine;

public class SigningMethod : MonoBehaviour
{
    // Chosen signing method
    public static string signingMethod;

    /// <summary>
    /// Sets the signing method to "SignIn"
    /// </summary>
    public void SignInMethod()
    {
        signingMethod = "SignIn";
    }

    /// <summary>
    /// Sets the signing method to "SignUp"
    /// </summary>
    public void SignUpMethod()
    {
        signingMethod = "SignUp";
    }
}

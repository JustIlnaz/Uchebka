using System;
using System.Security.Cryptography;
using System.Text;

namespace AutoService.Models;

static class AuthHelper
{
    public static string HashPassword(string password)
    {
        if (password == null) password = string.Empty;
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}

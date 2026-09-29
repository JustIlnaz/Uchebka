using AutoService.Data;

namespace AutoService.Models;

static class UserSession
{
    public static User? CurrentUser { get; set; }
}

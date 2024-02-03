namespace PermissionApp.Infrastructure;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.SaveChanges();
    }
}
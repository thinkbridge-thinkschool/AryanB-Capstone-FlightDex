namespace FlightDex.Api.Modules;

/// <summary>Registers each bounded context's services and the in-process event bus.</summary>
public static class ModuleRegistration
{
    public static IServiceCollection AddModules(this IServiceCollection services, IConfiguration configuration)
        => throw new NotImplementedException();
}

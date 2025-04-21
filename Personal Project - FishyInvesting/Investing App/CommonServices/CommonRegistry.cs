using Autofac;
using System.Reflection;

namespace CommonServices;

public class CommonRegistry : Autofac.Module
{
    private readonly string _environmentName;

    // Accept the environment name in the constructor
    public CommonRegistry(string environmentName)
    {
        _environmentName = environmentName;
    }
    protected override void Load(ContainerBuilder builder)
    {
        // Does not matter what class is chosen
        var assembly = typeof(CommonRegistry).GetTypeInfo().Assembly;

        builder.RegisterAssemblyTypes(assembly)
            .AsImplementedInterfaces().AsSelf();
        
        // Register the environment name as a singleton
        builder.RegisterInstance(_environmentName)
            .As<string>(); // Register as a string service
    }
}
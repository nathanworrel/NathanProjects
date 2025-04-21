using Autofac;
using WebApi.Template.Controllers;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApi.Template.Contexts;

namespace WebApi.Template;

public class ProgramRegistry : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Does not matter what class is chosen
        var assembly = typeof(CharlesSchwabController).GetTypeInfo().Assembly;
        
        builder.Register(c =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<CharlesSchwabContext>();
            return optionsBuilder.Options;
        }).As<DbContextOptions<CharlesSchwabContext>>().InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assembly)
            .Except<CharlesSchwabContext>(t => t.AsSelf().InstancePerLifetimeScope())
            .AsImplementedInterfaces().AsSelf();
    }
}
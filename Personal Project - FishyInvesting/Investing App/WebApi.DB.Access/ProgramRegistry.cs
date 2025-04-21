using Autofac;
using WebApi.DB.Access.Controllers;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApi.DB.Access.Contexts;

namespace WebApi.DB.Access;

public class ProgramRegistry : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Does not matter what class is chosen
        var assembly = typeof(AccountController).GetTypeInfo().Assembly;
        
        builder.Register(c =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<DBAccessContext>();
            return optionsBuilder.Options;
        }).As<DbContextOptions<DBAccessContext>>().InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assembly)
            .Except<DBAccessContext>(t => t.AsSelf().InstancePerLifetimeScope())
            .AsImplementedInterfaces().AsSelf();
    }
}
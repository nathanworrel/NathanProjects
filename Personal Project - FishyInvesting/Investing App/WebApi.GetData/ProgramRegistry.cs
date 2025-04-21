using System.Reflection;
using Autofac;
using Microsoft.EntityFrameworkCore;
using WebApi.GetData.Context;
using WebApi.GetData.Controllers;

namespace WebApi.GetData;

public class ProgramRegistry : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Does not matter what class is chosen
        var assembly = typeof(GetDataController).GetTypeInfo().Assembly;
        
        builder.Register(c =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<GetDataContext>();
            return optionsBuilder.Options;
        }).As<DbContextOptions<GetDataContext>>().InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assembly)
            .Except<GetDataContext>(t => t.AsSelf().InstancePerLifetimeScope())
            .AsImplementedInterfaces().AsSelf();
    }
}
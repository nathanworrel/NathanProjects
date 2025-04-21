using System.Reflection;
using Autofac;
using Microsoft.EntityFrameworkCore;
using WebApi.UpdateTrades.Contexts;
using WebApi.UpdateTrades.Controllers;

namespace WebApi.UpdateTrades;

public class ProgramRegistry : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Does not matter what class is chosen
        var assembly = typeof(UpdateTradesController).GetTypeInfo().Assembly;
        
        builder.Register(c =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<UpdateTradesContext>();
            return optionsBuilder.Options;
        }).As<DbContextOptions<UpdateTradesContext>>().InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assembly)
            .Except<UpdateTradesContext>(t => t.AsSelf().InstancePerLifetimeScope())
            .AsImplementedInterfaces().AsSelf();
    }
}
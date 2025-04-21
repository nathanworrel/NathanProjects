using System.Reflection;
using Autofac;
using Microsoft.EntityFrameworkCore;
using WebApi.StrategyService.Contexts;
using WebApi.StrategyService.Controllers;
using WebApi.StrategyService.Services;

namespace WebApi.StrategyService;

public class ProgramRegistry : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Does not matter what class is chosen
        var assembly = typeof(StrategyServiceController).GetTypeInfo().Assembly;
        
        builder.Register(c =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<StrategyServiceContext>();
            return optionsBuilder.Options;
        }).As<DbContextOptions<StrategyServiceContext>>().InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assembly)
            .Except<StrategyServiceContext>(t => t.AsSelf().InstancePerLifetimeScope())
            .AsImplementedInterfaces().AsSelf();
    }
}
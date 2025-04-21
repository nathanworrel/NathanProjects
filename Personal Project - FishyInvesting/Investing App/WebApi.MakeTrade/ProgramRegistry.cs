using System.Reflection;
using Autofac;
using MakeTrade.Controllers;
using Microsoft.EntityFrameworkCore;
using WebApi.GetData.Context;

namespace MakeTrade;

public class ProgramRegistry : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Does not matter what class is chosen
        var assembly = typeof(MakeTradeController).GetTypeInfo().Assembly;
        
        builder.Register(c =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<MakeTradeContext>();
            return optionsBuilder.Options;
        }).As<DbContextOptions<MakeTradeContext>>().InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assembly)
            .Except<MakeTradeContext>(t => t.AsSelf().InstancePerLifetimeScope())
            .AsImplementedInterfaces().AsSelf();
    }
}
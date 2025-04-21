using System.Reflection;
using Autofac;
using Microsoft.EntityFrameworkCore;
using WebApi.GetData.Context;
using WebApi.Scheduler.Controllers;

namespace WebApi.Scheduler;

public class ProgramRegistry : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Does not matter what class is chosen
        var assembly = typeof(SchedulerController).GetTypeInfo().Assembly;
        
        builder.Register(c =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<SchedulerContext>();
            return optionsBuilder.Options;
        }).As<DbContextOptions<SchedulerContext>>().InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(assembly)
            .Except<SchedulerContext>(t => t.AsSelf().InstancePerLifetimeScope())
            .AsImplementedInterfaces().AsSelf();
    }
    
}
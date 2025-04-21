// using System.Diagnostics;
// using System.Reflection;
// using AutoMapper;
// using FishyLibrary.Helpers;
// using FishyLibrary.Models;
// using FishyLibrary.Models.Parameters;
// using FishyLibrary.Models.StrategyType;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
// using Microsoft.Extensions.Logging.Abstractions;
// using WebApi.DB.Access.Controllers;
// using Xunit.Sdk;
//
// namespace UnitTests.WebApi.DB.Access;
//
// public class ParametersControllerTest : IClassFixture<DevDatabaseFixture>, IDisposable
// {
//     public ParametersControllerTest(DevDatabaseFixture fixture)
//         => Fixture = fixture;
//
//     public DevDatabaseFixture Fixture { get; }
//     
//     private readonly IMapper _mapper = new Mapper(new MapperConfiguration(cfg => {
//         cfg.CreateMap<Parameters, ParametersGet>();
//         cfg.CreateMap<Parameters, ParametersPost>();
//         cfg.CreateMap<StrategyType, StrategyTypeGet>();
//     }));
//
//     public void Dispose()
//     {
//         Fixture.Dispose();
//     }
//     
//     [Fact]
//     public void GetParameter_NotFound()
//     {
//         using var db = Fixture.CreateContext();
//         var controller = new ParametersController(NullLogger<ParametersController>.Instance, db, _mapper);
//         ActionResult<ParametersGet> received = controller.Get(100);
//         Assert.Null(received.Value);
//     }
//     
//     [Fact]
//     public void AddParameter_Ok()
//     {
//         using var db = Fixture.CreateContext();
//         db.StrategyTypes.Add(new StrategyType(1, "TEST"));
//         var controller = new ParametersController(NullLogger<ParametersController>.Instance, db, _mapper);
//         ActionResult<ParametersPost> received = controller.Post(1, " { 'PeriodShort': 10 } ");
//         Assert.NotNull(received.Value);
//         Assert.Equal(1, received.Value.StrategyTypeId);
//     }
//     
//     [Fact]
//     public void GetParameter_Found()
//     {
//         AddParameter_Ok();
//         using var db = Fixture.CreateContext();
//         var controller = new ParametersController(NullLogger<ParametersController>.Instance, db, _mapper);
//         ActionResult<ParametersGet> received = controller.Get(1);
//         Assert.NotNull(received.Value);
//     }
// }
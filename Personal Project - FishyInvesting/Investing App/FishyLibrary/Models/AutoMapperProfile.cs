using AutoMapper;
using FishyLibrary.Models.Account;
using FishyLibrary.Models.Parameters;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategyRuntime;
using FishyLibrary.Models.StrategySecondaryProduct;
using FishyLibrary.Models.StrategyType;
using FishyLibrary.Models.Trade;
using FishyLibrary.Models.User;

namespace FishyLibrary.Models;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Strategy Types
        CreateMap<StrategyType.StrategyType, StrategyTypeGet>();
        CreateMap<StrategyTypePost, StrategyType.StrategyType>();
        
        // Parameters
        CreateMap<Parameters.Parameters, ParametersGet>();
        CreateMap<ParametersPost, Parameters.Parameters>();
        CreateMap<Parameters.Parameters, ParametersPost>();
        
        // Strategy
        CreateMap<Strategy.Strategy, StrategyGet>();
        CreateMap<StrategyPost, Strategy.Strategy>();
        
        // Strategy Runtime
        CreateMap<StrategyRuntime.StrategyRuntime, StrategyRuntimeGet>();
        CreateMap<StrategyRuntimePost, StrategyRuntime.StrategyRuntime>();
        CreateMap<StrategyRuntime.StrategyRuntime, StrategyRuntimePost>();

        // Account
        CreateMap<Account.Account, AccountGet>();
        CreateMap<AccountPost, Account.Account>();
        CreateMap<Account.Account, AccountPost>();
        
        // User
        CreateMap<User.User, UserGet>();
        CreateMap<UserPost, User.User>();
        
        // Strategy Secondary Product
        CreateMap<StrategySecondaryProduct.StrategySecondaryProduct, StrategySecondaryProductGet>();
        CreateMap<StrategySecondaryProductPost, StrategySecondaryProduct.StrategySecondaryProduct>();
        CreateMap<StrategySecondaryProduct.StrategySecondaryProduct, StrategySecondaryProductPost>();
        
        // Trade
        CreateMap<Trade.Trade, TradeGet>();
        CreateMap<TradePost, Trade.Trade>();
        CreateMap<Trade.Trade, TradePost>();
    }
}
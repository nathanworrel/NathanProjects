using FishyLibrary.Models.Parameters;

namespace CommonServices.Retrievers.DbAccess;

public interface IDbAccessRetriever
{
    Task<bool> AddParameters(Parameters parameters);
}
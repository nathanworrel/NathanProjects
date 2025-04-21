using FishyLibrary.Models;
using FishyLibrary.Utils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace FishyLibrary.Extensions;

public static class ServiceCollectionExtenstions
{
    public static IServiceCollection AddSerilogWithSeq(this IServiceCollection services)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            services.AddSerilog(lc => lc 
                .WriteTo.Seq("http://logs:5341"));
        }
        else
        {
            services.AddSerilog(lc => lc.WriteTo.Console());
        }
        return services;
    }
}

public class Solution {
    public bool IsAnagram(string s, string t) {
        Dictionary<char,int> dict = new Dictionary<char,int>();
        if (s.Length != t.Length)
        {
            return false;
        }

        foreach (var letter in s)
        {
            if (dict.ContainsKey(letter))
            {
                dict[letter]++;
            }
            else
            {
                dict.Add(letter, 1);
            }
        }

        foreach (var letter in t)
        {
            if (dict.ContainsKey(letter))
            {
                dict[letter]--;
                if (dict[letter] < 0)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
}
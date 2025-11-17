using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace ServiceContracts.DTO
{
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj is not CountryResponse)
                return false;

            CountryResponse other = (CountryResponse)obj;
            return CountryID == other.CountryID && CountryName == other.CountryName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CountryID, CountryName);
        }
    }

    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse { CountryID = country.CountryID, CountryName = country.CountryName };
        }
    }
}

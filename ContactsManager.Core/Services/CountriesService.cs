using Entities;
//using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }
        
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest is null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            if (countryAddRequest.CountryName is null)
                throw new ArgumentException(nameof(countryAddRequest.CountryName));

            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) is not null)
                throw new ArgumentException("Given country already exists");

            Country country = countryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();
            await _countriesRepository.AddCountry(country);
            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            List<Country> countries = await _countriesRepository.GetAllCountries();
            return countries.Select(c => c.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID is null)
                return null;

            Country? country_from_list = await _countriesRepository.GetCountryByCountryID(countryID.Value);

            return country_from_list?.ToCountryResponse();
        }
    }
}

using AutoFixture;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;
        private readonly IFixture _fixture;

        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesService = new CountriesService(_countriesRepository);
        }

        #region AddCountry
        [Fact]
        public async Task AddCountry_NullRequest_ToThrowArgumentNullException()
        {
            CountryAddRequest? request = null;
            Func<Task> action = async () => await _countriesService.AddCountry(request);
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task AddCountry_NullCountryName_ToThrowArgumentException()
        {
            CountryAddRequest request = _fixture.Build<CountryAddRequest>().With(c => c.CountryName, null as string).Create();
            Func<Task> action = async () => await _countriesService.AddCountry(request);
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToThrowArgumentException()
        {
            CountryAddRequest request = _fixture.Create<CountryAddRequest>();
            var country = request.ToCountry();
            country.CountryID = Guid.NewGuid();

            _countriesRepositoryMock.Setup(r => r.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(country);

            Func<Task> action = async () => await _countriesService.AddCountry(request);
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddCountry_ValidRequest_ToReturnCountryResponse()
        {
            CountryAddRequest request = _fixture.Create<CountryAddRequest>();
            var country = request.ToCountry();
            country.CountryID = Guid.NewGuid();

            _countriesRepositoryMock.Setup(r => r.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync((Entities.Country?)null);
            _countriesRepositoryMock.Setup(r => r.AddCountry(It.IsAny<Entities.Country>())).ReturnsAsync(country);

            CountryResponse response = await _countriesService.AddCountry(request);
            response.CountryID.Should().NotBe(Guid.Empty);
            response.CountryName.Should().Be(request.CountryName);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            _countriesRepositoryMock.Setup(r => r.GetAllCountries()).ReturnsAsync(new List<Entities.Country>());
            List<CountryResponse> responses = await _countriesService.GetAllCountries();
            responses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCountries_WithCountries_ReturnList()
        {
            var countries = _fixture.Create<List<Entities.Country>>();
            _countriesRepositoryMock.Setup(r => r.GetAllCountries()).ReturnsAsync(countries);
            List<CountryResponse> responses = await _countriesService.GetAllCountries();
            responses.Should().BeEquivalentTo(countries.ConvertAll(c => c.ToCountryResponse()));
        }
        #endregion

        #region GetCountryByCountryID
        [Fact]
        public async Task GetCountryByCountryID_NullId_ReturnNull()
        {
            CountryResponse? response = await _countriesService.GetCountryByCountryID(null);
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetCountryByCountryID_InvalidId_ReturnNull()
        {
            _countriesRepositoryMock.Setup(r => r.GetCountryByCountryID(It.IsAny<Guid>())).ReturnsAsync((Entities.Country?)null);
            CountryResponse? response = await _countriesService.GetCountryByCountryID(Guid.NewGuid());
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetCountryByCountryID_ValidId_ReturnCountry()
        {
            var country = _fixture.Create<Entities.Country>();
            _countriesRepositoryMock.Setup(r => r.GetCountryByCountryID(It.IsAny<Guid>())).ReturnsAsync(country);
            CountryResponse? response = await _countriesService.GetCountryByCountryID(country.CountryID);
            response.Should().Be(country.ToCountryResponse());
        }
        #endregion
    }
}

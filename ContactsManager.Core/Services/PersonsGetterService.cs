using Entities;
//using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exceptions;

namespace Services
{
    public class PersonsGetterService : IPersonsGetterService
    {
        private readonly IPersonsRepository _personsRepository;

        public PersonsGetterService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        public async Task<PersonResponse> ConvertPersonToPersonResponseWithCountry(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = person?.Country?.CountryName;
            return personResponse;
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var persons = await _personsRepository.GetAllPersons();
            //return (await Task.WhenAll(persons.Select(p => ConvertPersonToPersonResponseWithCountry(p)))).ToList();
            return persons.Select(p => p.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonID(Guid? personID)
        {
            if (personID == null) return null;

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person == null) return null;

            return await ConvertPersonToPersonResponseWithCountry(person);
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<Person> persons = searchBy switch
            {
                nameof(PersonResponse.PersonName) =>
                    await _personsRepository.GetFilteredPersons(p =>
                        p.PersonName.Contains(searchString)),

                nameof(PersonResponse.Email) =>
                    await _personsRepository.GetFilteredPersons(p =>
                        p.Email.Contains(searchString)),

                nameof(PersonResponse.DateOfBirth) =>
                    await _personsRepository.GetFilteredPersons(p =>
                        p.DateOfBirth.Value.ToString("dd MMMM yyyy")
                            .Contains(searchString)),

                nameof(PersonResponse.Gender) =>
                    await _personsRepository.GetFilteredPersons(p =>
                        p.Gender.Contains(searchString)),

                nameof(PersonResponse.CountryID) =>
                    await _personsRepository.GetFilteredPersons(p =>
                        p.Country.CountryName.Contains(searchString)),

                nameof(PersonResponse.Address) =>
                    await _personsRepository.GetFilteredPersons(p =>
                        p.Address.Contains(searchString)),

                _ => await _personsRepository.GetAllPersons()

            };

            return persons.Select(p => p.ToPersonResponse()).ToList();
        }
    }
}

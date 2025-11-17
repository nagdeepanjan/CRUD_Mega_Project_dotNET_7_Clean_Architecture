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
    public class PersonsDeleterService : IPersonsDeleterService
    {
        private readonly IPersonsRepository _personsRepository;

        public PersonsDeleterService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        public async Task<PersonResponse> ConvertPersonToPersonResponseWithCountry(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = person?.Country?.CountryName;
            return personResponse;
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID is null)
                throw new ArgumentNullException(nameof(personID));

            Person? person = await _personsRepository.GetPersonByPersonID(personID.Value);
            if (person is null)
                return false;

            await _personsRepository.DeletePersonByPersonID(personID.Value);
            return true;
        }
    }
}

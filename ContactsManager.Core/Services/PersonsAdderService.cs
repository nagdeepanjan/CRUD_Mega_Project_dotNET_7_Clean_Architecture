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
    public class PersonsAdderService : IPersonsAdderService
    {
        private readonly IPersonsRepository _personsRepository;

        public PersonsAdderService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        public async Task<PersonResponse> ConvertPersonToPersonResponseWithCountry(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = person?.Country?.CountryName;
            return personResponse;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            //Check if argument is null
            if (personAddRequest is null)
                throw new ArgumentNullException(nameof(personAddRequest));

            //Model Validation
            ValidationHelper.ModelValidation(personAddRequest);

            //convert personAddRequest to Person type
            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();

            //add Person to persons list
            await _personsRepository.AddPerson(person);

            return await ConvertPersonToPersonResponseWithCountry(person);
        }

    }
}

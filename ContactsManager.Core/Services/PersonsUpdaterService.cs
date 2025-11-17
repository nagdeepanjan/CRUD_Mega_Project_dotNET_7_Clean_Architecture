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
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;

        public PersonsUpdaterService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        public async Task<PersonResponse> ConvertPersonToPersonResponseWithCountry(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = person?.Country?.CountryName;
            return personResponse;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            // Validate input
            if (personUpdateRequest is null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            ValidationHelper.ModelValidation(personUpdateRequest);

            // Find the person to update
            Person? existingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (existingPerson is null)
                //throw new ArgumentException("Person with the given ID does not exist.");
                throw new InvalidPersonIDException("Person with the given ID does not exist.");

            // Update properties
            existingPerson.PersonName = personUpdateRequest.PersonName;
            existingPerson.Email = personUpdateRequest.Email;
            existingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            existingPerson.Gender = personUpdateRequest.Gender?.ToString();
            existingPerson.Address = personUpdateRequest.Address;
            existingPerson.ReceiveNewsletters = personUpdateRequest.ReceiveNewsletters;
            existingPerson.CountryID = personUpdateRequest.CountryID;

            await _personsRepository.UpdatePerson(existingPerson);
            // Return updated response
            //return ConvertPersonToPersonResponse(existingPerson);
            return await ConvertPersonToPersonResponseWithCountry(existingPerson);
        }
    }
}

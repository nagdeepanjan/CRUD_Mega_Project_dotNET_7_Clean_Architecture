using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsletters { get; set; }
        public double? Age { get; set; }

        public Guid? CountryID { get; set; }
        public string? Country { get; set; }


        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (obj is not PersonResponse)
                return false;

            PersonResponse other = (PersonResponse)obj;

            return PersonID == other.PersonID
                   && PersonName == other.PersonName
                   && Email == other.Email
                   && DateOfBirth == other.DateOfBirth
                   && Gender == other.Gender
                   && Address == other.Address
                   && ReceiveNewsletters == other.ReceiveNewsletters
                   && CountryID == other.CountryID
                   && Country == other.Country;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"PersonID: {PersonID}, PersonName: {PersonName}, Email: {Email}, DateOfBirth: {DateOfBirth?.ToString()}, Gender: {Gender}, Address: {Address}, ReceiveNewsLetters: {ReceiveNewsletters}, Age: {Age}, CountryID: {CountryID}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                Address = Address,
                CountryID = CountryID,
                DateOfBirth = DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                ReceiveNewsletters = ReceiveNewsletters
            };
        }
    }
    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                Address = person.Address,
                ReceiveNewsletters = person.ReceiveNewsletters,
                CountryID = person.CountryID,
                Country = person.Country?.CountryName,
                Age = person.DateOfBirth.HasValue
                    ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25, 2)
                    : null
            };
        }


    }
}

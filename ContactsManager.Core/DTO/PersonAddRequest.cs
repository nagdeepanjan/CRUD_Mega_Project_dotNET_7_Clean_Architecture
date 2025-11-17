using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person name cannot be blank")]
        public string? PersonName { get; set; }


        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Email should be in correct format")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        
        
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Choose a gender")]
        public GenderOptions? Gender { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsletters { get; set; }

        [Required(ErrorMessage = "Please choose a country")]
        public Guid? CountryID { get; set; }

        public Person ToPerson()
        {
            return new Person
            {
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                Address = Address,
                ReceiveNewsletters = ReceiveNewsletters,
                CountryID = CountryID
            };
        }
    }
}

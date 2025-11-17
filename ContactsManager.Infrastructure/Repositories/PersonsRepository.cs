using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;

        public PersonsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Person> AddPerson(Person person)
        {
            await _db.Persons.AddAsync(person);
            await _db.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonByPersonID(Guid personID)
        {   
            var person = await _db.Persons.FirstOrDefaultAsync(p => p.PersonID == personID);

            if (person == null)
                return false; // Person not found

            _db.Persons.Remove(person);
            int rowsDeleted = await _db.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            var persons = await _db.Persons.Include(p => p.Country).ToListAsync();
            return persons;
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            var persons = await _db.Persons.Include(p => p.Country).Where(predicate).ToListAsync();
            return persons;
        }

        public async Task<Person?> GetPersonByPersonID(Guid personID)
        {
            return await _db.Persons.Include(p=>p.Country).FirstOrDefaultAsync(p => p.PersonID == personID);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(p => p.PersonID == person.PersonID);

            if (matchingPerson == null)
                return person;


            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Address = person.Address;
            matchingPerson.CountryID = person.CountryID;
            matchingPerson.DateOfBirth = person.DateOfBirth;
            matchingPerson.Email = person.Email;
            matchingPerson.Gender = person.Gender;
            matchingPerson.ReceiveNewsletters = person.ReceiveNewsletters;

            await _db.SaveChangesAsync();

            return matchingPerson;
        }
    }
}

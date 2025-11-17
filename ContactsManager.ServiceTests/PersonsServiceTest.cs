using Entities;
//using EntityFrameworkCoreMock;
//using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Formatters;
using Moq;
using RepositoryContracts;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Tests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsDeleterService _personsDeleterService;

        private readonly IPersonsRepository _personsRepository;
        private readonly Mock<IPersonsRepository> _personRepositoryMock;

        private readonly IFixture _fixture;

        public PersonsServiceTest()
        {
            //Adding AutoFixture
            _fixture = new Fixture();

            //Mock the REPOSITORY
            _personRepositoryMock = new Mock<IPersonsRepository>();     //Mock object used to finetune  the behavior of the mocked repo inside each test method
            _personsRepository = _personRepositoryMock.Object;          //Mocked repository object

            //Instantiate the countries service & persons service
            _personsGetterService = new PersonsGetterService(_personsRepository);
            _personsAdderService = new PersonsAdderService(_personsRepository);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepository);
            _personsSorterService = new PersonsSorterService(_personsRepository);
            _personsDeleterService = new PersonsDeleterService(_personsRepository);
        }

        #region AddPerson()
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            // Arrange
            PersonAddRequest? personAddRequest = null;
            
            // Act & Assert
            Func<Task> action = async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }
        

        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBe_ArgumentException()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(p => p.PersonName, null as string).With(p => p.Email, "alpha@deepz.com").Create();

            Person person = personAddRequest.ToPerson();                        //We need person because PersonAddRequest is a DTO confined to Service. Repo has no access to it.
            _personRepositoryMock.Setup(p => p.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);     //Defining the mocked repo's function

            // Act & Assert
            Func<Task> action = async () =>
            {
                await _personsAdderService.AddPerson(personAddRequest);
            };
            await action.Should().ThrowAsync<ArgumentException>();

        }

        [Fact]
        public async Task AddPerson_FullPersonsDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(p => p.Email, "test@test.com").Create();    //Argument to the service function


            Person person = personAddRequest.ToPerson();                            //Repo works only with Entity classes, not DTOs    
            PersonResponse person_response_expected = person.ToPersonResponse();    //Converting to DTO 
            
            _personRepositoryMock.Setup(p => p.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);         //Defining the behavior of the mocked repo's method

            // Act
            PersonResponse personResponse_from_add = await _personsAdderService.AddPerson(personAddRequest);
            person_response_expected.PersonID = personResponse_from_add.PersonID;                           //Because PersonID is generated when adding the person

            //Assert
            personResponse_from_add.PersonID.Should().NotBe(Guid.Empty);
            personResponse_from_add.Should().Be(person_response_expected);
        }
        #endregion

        #region GetPersonByPersonID

        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBe_Null()
        {
            //Arrange
            Guid personID = Guid.Empty;

            //Act
            PersonResponse? personResponse_from_get= await _personsGetterService.GetPersonByPersonID(personID);

            //Assert
            personResponse_from_get.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>().With(p => p.Email, "test@gmail.com").With(p=>p.Country, null as Country).Create();
            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(p => p.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse? person_response_from_get = await _personsGetterService.GetPersonByPersonID(person.PersonID);

            //Assert
            person_response_from_get.Should().Be(person_response_expected);
        }

        #endregion

        #region GetAllPersons()
        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            //Arrange
            _personRepositoryMock.Setup(p => p.GetAllPersons()).ReturnsAsync(new List<Person>());

            //Act
            List<PersonResponse> persons_from_get = await _personsGetterService.GetAllPersons();

            //Assert
            persons_from_get.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(p => p.Email, "b@b.com").With(p=>p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.Email, "a@b.com").With(p=>p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.Email, "c@b.com").With(p=>p.Country, null as Country).Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(p=>p.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(p => p.GetAllPersons()).ReturnsAsync(persons);



            //Act
            List<PersonResponse> persons_list_from_get = await _personsGetterService.GetAllPersons();


            //Assert
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(p => p.Email, "b@b.com").With(p=>p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.Email, "a@b.com").With(p=>p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.Email, "c@b.com").With(p=>p.Country, null as Country).Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(p => p.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(p => p.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
           
            //Act
            List<PersonResponse> persons_list_from_search =
                await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "");

            //Assert
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(p => p.Email, "b@b.com").With(p=>p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.Email, "a@b.com").With(p=>p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.Email, "c@b.com").With(p=>p.Country, null as Country).Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(p => p.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(p => p.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_search =
                await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "sa");

            //Assert
            persons_list_from_search.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(p => p.Email, "b@b.com").With(p=>p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.Email, "a@b.com").With(p=>p.Country, null as Country).Create(),
                _fixture.Build<Person>().With(p => p.Email, "c@b.com").With(p=>p.Country, null as Country).Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(p => p.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(p => p.GetAllPersons()).ReturnsAsync(persons);
            List<PersonResponse> allPersons = await _personsGetterService.GetAllPersons();
            
            //Act
            List<PersonResponse> persons_list_from_sort = await _personsSorterService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //Assert
            persons_list_from_sort.Should().BeInDescendingOrder(p => p.PersonName);

        }

        #endregion

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_NullPerson_ToBe_ArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;
            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                //Act
                await _personsUpdaterService.UpdatePerson(person_update_request);
            });
            Func<Task> action = async () => { await _personsUpdaterService.UpdatePerson(person_update_request); };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonId_ToGiveArgumentException()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = new PersonUpdateRequest { PersonID = Guid.NewGuid() };

            //Assert
            Func<Task> action = async () => { await _personsUpdaterService.UpdatePerson(person_update_request); };

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToGetArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>().With(p => p.PersonName, null as string).With(p=>p.Email, "test@gmail.com").With(p=>p.Country, null as Country).With(p=>p.Gender, "Male").Create();
            PersonResponse person_response_from_add = person.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            //Act
            Func<Task> action = async () => { await _personsUpdaterService.UpdatePerson(person_update_request); };

            //Assert

            action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdate_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>().With(p => p.Country, null as Country ).With(p => p.Email, "a@b.com").With(p=>p.Gender, "Male").Create();
            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();
            _personRepositoryMock.Setup(p => p.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);
            _personRepositoryMock.Setup(p => p.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_update = await _personsUpdaterService.UpdatePerson(person_update_request);

            //Assert
            person_response_from_update.Should().Be(person_response_expected);
        }

        #endregion

        #region TEST DeletePerson()

        //If you supply valid PersonID, it returns true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>().With(p => p.PersonName, "Junie").With(p => p.Country, null as Country).With(p => p.Email, "a@b.com").With(p=>p.Gender, "Male").Create();
            
            _personRepositoryMock.Setup(p => p.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);
            _personRepositoryMock.Setup(p => p.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

            //Act
            bool isDeleted = await _personsDeleterService.DeletePerson(person.PersonID);

            //Assert
            isDeleted.Should().BeTrue();
        }


        //If you supply invalid PersonID, it returns false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Arrange
            _personRepositoryMock.Setup(repo => repo.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(false);

            //Act
            bool isDeleted = await _personsDeleterService.DeletePerson(Guid.NewGuid());

            //Assert
            isDeleted.Should().BeFalse();
        }
        #endregion
    }
}

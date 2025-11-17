using CRUD_Last.Filters.ActionFilters;
using CRUD_Last.Filters.AuthorizationFilters;
using CRUD_Last.Filters.ExceptionFilters;
using CRUD_Last.Filters.ResultFilter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUD_Last.Controllers
{
    //[TypeFilter(typeof(HandleExceptionFilter))]
    public class PersonsController : Controller
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsSorterService _personsSorterService;

        public PersonsController(ICountriesService countriesService, IPersonsGetterService personsGetterService, IPersonsAdderService personsAdderService, IPersonsDeleterService personsDeleterService, IPersonsUpdaterService personsUpdaterService, IPersonsSorterService personsSorterService)
        {
            _countriesService = countriesService;
            _personsGetterService = personsGetterService;
            _personsAdderService = personsAdderService;
            _personsDeleterService = personsDeleterService;
            _personsUpdaterService = personsUpdaterService;
            _personsSorterService = personsSorterService;
        }
        
        [Route("/persons/index")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter))]
        [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[]{"X-Lucky-Person", "Mozart"})]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy=nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            ViewBag.SearchFields = new Dictionary<string, string>
            {
                {nameof(PersonResponse.PersonName), "Person Name"},
                {nameof(PersonResponse.Email), "Email"},
                {nameof(PersonResponse.DateOfBirth), "Date of Birth"},
                {nameof(PersonResponse.Gender), "Gender"},
                {nameof(PersonResponse.Address), "Address"},
                {nameof(PersonResponse.Country), "Country"}
            };
            //List<PersonResponse> persons = _personsService.GetAllPersons();
            List<PersonResponse> persons = await _personsGetterService.GetFilteredPersons(searchBy, searchString);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sort
            List<PersonResponse> sortedPersons = await _personsSorterService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(sortedPersons);
        }

        [HttpGet("/persons/create")]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries;

            return View();
        }

        [HttpPost("persons/create")]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personAddRequest);
            }

            //call the service method
            PersonResponse personResponse = await _personsAdderService.AddPerson(personAddRequest);

            //navigate to Index() action method (it makes another get request to "persons/index"
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet("/persons/[action]/{personID}")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personID);
            if (personResponse is null)
                return RedirectToAction("Index");
            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            //create the countries list to be used by the dropdown
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries;

            return View(personUpdateRequest);
        }

        [HttpPost("/persons/[action]/{personID}")]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse personResponse = await _personsGetterService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse is null)
                return RedirectToAction("Index");

            if (ModelState.IsValid)
            {
                PersonResponse updatedPerson = await _personsUpdaterService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries;

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
            }
        }

        [HttpGet("/persons/[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personID);
            if (personResponse is null)
                return RedirectToAction("Index");

            return View(personResponse);
        }

        [HttpPost("/persons/[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid personID)
        {
            PersonResponse personresponse = await _personsGetterService.GetPersonByPersonID(personID);
            if (personresponse is null)
                return RedirectToAction("Index");

            await _personsDeleterService.DeletePerson(personID);
            return RedirectToAction("Index");
        }
    }
}

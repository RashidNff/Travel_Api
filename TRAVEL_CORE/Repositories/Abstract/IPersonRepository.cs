using System.Data;
using TRAVEL_CORE.Entities.Person;
using TRAVEL_CORE.Entities;

namespace TRAVEL_CORE.Repositories.Abstract
{
    public interface IPersonRepository
    {
        DataTable GetPersonBrowseData(FilterParameter filterParameter);
        ResponseModel SavePerson(PersonData savePerson);
        PersonData GetPersonById(int contractId);
        ResponseModel ChangeStatus(ChangeStatus model);
    }
}

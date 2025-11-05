using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models.Security
{
    public class Person : BaseModel
    {
        public string TypeDocument { get; set; } = null!;
        public string Document { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FirstSurname { get; set; } = null!;
        public string SecondSurname { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public int? CityId { get; set; }


    }
}

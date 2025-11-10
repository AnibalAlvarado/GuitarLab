

using Entity.Dtos;
using Entity.Models;

namespace test.Dtos
{
    public class FakeEntity : BaseModel
    {
        public string? Name { get; set; }
    }

    public class FakeDto : BaseDto
    {
        public string? Name { get; set; }
    }
}

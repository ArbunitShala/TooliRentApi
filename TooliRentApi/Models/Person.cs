using System.ComponentModel.DataAnnotations;

namespace TooliRentApi.Models
{
    public class Person
    {
            public int Id { get; set; }
            public string FirstName { get; set; } = default!;
            public string LastName { get; set; } = default!;
            public string Email { get; set; } = default!;
        }
}

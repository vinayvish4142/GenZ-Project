namespace GenZApp.Models
{
    public class StudentModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Address { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}

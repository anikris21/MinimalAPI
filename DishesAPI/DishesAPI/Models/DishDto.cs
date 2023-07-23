namespace DishesAPI.Models
{
    public class DishDto
    {
        public Guid Id { get; set; }

        // required
        public required string Name { get; set; }
    }
}

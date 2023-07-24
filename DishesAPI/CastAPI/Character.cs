namespace CastAPI
{
    public class Character
    {
        public Character() { }

        public Guid Id { get; set; } 
        public string Name { get; set; }


        // 1: many user : character
        public User User { get; set; }

        public int UserId { get; set; }
    }
}

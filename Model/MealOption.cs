using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Model
{
    public class MealOption : IHasId
    {
        public int Id { get; init; } = IdGenerator.Instance.GetNextId<MealOption>();
        public required string Name { get; set; }
        public bool IsBreakfast { get; set; }
        public bool IsLunch { get; set; }
        public bool IsSnack { get; set; }
        public bool IsDinner { get; set; }
    }
}

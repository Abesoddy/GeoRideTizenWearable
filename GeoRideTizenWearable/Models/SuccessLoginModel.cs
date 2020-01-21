namespace GeoRideTizenWearable.Models
{
    public class SuccessLoginModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public string AuthToken { get; set; }
    }
}
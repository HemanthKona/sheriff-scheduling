namespace SS.Api.Models.Dto
{
    public partial class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}
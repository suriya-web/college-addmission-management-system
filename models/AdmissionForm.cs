namespace collegeAdmission.Models
{
    public class AdmissionForm
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Program { get; set; }
        public string Course { get; set; }

        public byte[] Marksheet10 { get; set; }
        public byte[] Marksheet12 { get; set; }
        public byte[] UGMarksheet { get; set; }
        public string Status { get; set; }
    }
}

using System;

namespace API.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string UserRole { get; set; }
    }
}
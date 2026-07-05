using System;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.Entities
{
    public class ContainerInfo
    {
        public string Id { get; set; } = string.Empty;
        public string ShortId => Id.Length >= 12 ? Id[..12] : Id;
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;   // "running", "exited", …
        public string Status { get; set; } = string.Empty;  // "Up 2 hours", "Exited (0) 3 days ago"
        public DateTime Created { get; set; }
    }
}

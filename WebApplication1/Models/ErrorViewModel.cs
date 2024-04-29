using System;

namespace YourNamespace.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => RequestId is not null and not empty;
    }
}

using System;

namespace MVCApplication.Models.Config
{
    public class Feature
    {
        public string Id { get; set; } = string.Empty; // Unique identifier for the feature
        public bool Enabled { get; set; } // Global enable/disable flag

        //PROBABLY DONT NEED THESE, BUT LEAVING THEM HERE FOR NOW
        // public int Rollout { get; set; }
        // public string? RawValue { get; set; }
    }
}

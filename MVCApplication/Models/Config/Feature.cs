using System;

namespace MVCApplication.Models.Config
{
    //Singleton class to represent a feature flag that can be enabled or disabled globally
    public class Feature
    {
        public string Id { get; set; } = string.Empty; // Unique identifier for the feature
        public bool Enabled { get; set; } // Global enable/disable flag
    }
}

using Registry.Abstractions;
using RegistryPluginBase.Classes;
using RegistryPluginBase.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace RegistryPlugin.DsrmAdminLogonBehaviour
{
    /// <summary>
    /// Registry Explorer plugin to check the 'DsrmAdminLogonBehaviour' registry value
    /// As if the key value is set to any value really or exists then thats a sign of manual setting. This account on the DC can lead to persistent access as it wont show up on the AD schema
    /// and would also allow an actor to perform a DC sync attack to regain access to the KRBTGT, set a delegation attack since the account is also a local admin; among others
    public class DsrmAdminLogonBehaviour : IRegistryPluginGrid
    {
        private readonly BindingList<ValuesOut> _values;

        
        public DsrmAdminLogonBehaviour()
        {
            _values = new BindingList<ValuesOut>();
            Errors = new List<string>();
        }

      
        public string InternalGuid => "882FB24E-148D-4FB0-8DF7-735ECCF452A4";


        // a hive has ControlSet001 (CurrentControlSet exists only on a live system), and the value is spelled Behavior
        public List<string> KeyPaths => new List<string> { @"ControlSet00*\Control\Lsa" };


        public string ValueName => "DsrmAdminLogonBehavior";

        public string AlertMessage { get; private set; }

        public RegistryPluginType.PluginType PluginType => RegistryPluginType.PluginType.Grid;

        public string Author => "Abdul Mhanni";

        public string Email => "abdul.mhanni@gmail.com";

        
        public string Phone => "";

        public string PluginName => "DsrmAdminLogonBehaviour Value";

    
        public string ShortDescription => "Detects the DsrmAdminLogonBehavior value as it is non existent or without an entry by default. It's a break glass account and thus if it's a value of 1 or 2, then that indicates persistence";

        public string LongDescription => ShortDescription;


        public double Version => 1.1;

      
        public List<string> Errors { get; }

   
       
        public void ProcessValues(RegistryKey key)
        {
            _values.Clear(); // Clear previous results
            Errors.Clear(); // Clear previous errors
            AlertMessage = string.Empty; // Reset alert message

            // Iterate through all values in the current registry key.
            foreach (var value in key.Values)
            {
                // Check if the current value's name matches "DsrmAdminLogonBehavior" (case-insensitive; also the British spelling).
                if (value.ValueName.Equals(ValueName, StringComparison.OrdinalIgnoreCase) ||
                    value.ValueName.Equals("DsrmAdminLogonBehaviour", StringComparison.OrdinalIgnoreCase))
                {
                    bool isPersistenceDetected = false;
                    int intValue;

                    // Attempt to parse the value data to an integer.
                    if (int.TryParse(value.ValueData, out intValue))
                    {
                        // If the integer value is 1 or 2, potential persistence is detected.
                        if (intValue == 1 || intValue == 2)
                        {
                            isPersistenceDetected = true;
                            // Set the alert message to inform the user about the detection.
                            AlertMessage = $"Potential persistence detected: '{value.ValueName}' value is '{intValue}' in key '{key.KeyPath}'.";
                        }
                    }
                    else
                    {
                        // Add an error if the value data could not be parsed as an integer.
                        Errors.Add($"Error: Could not parse '{value.ValueName}' value data '{value.ValueData}' to an integer in key '{key.KeyPath}'.");
                    }

                    // Create a new ValuesOut object with the extracted information.
                    var v = new ValuesOut(
                         value.ValueName,
                         value.ValueData,
                         key.LastWriteTime            
                  
                     );

                    // Manually assign the IsPersistenceDetected property after object creation.
       
                    v.PersistenceDetection = isPersistenceDetected;

                    v.BatchKeyPath = key.KeyPath; // Set key path for batch reporting
                    v.BatchValueName = value.ValueName; // Set value name for batch reporting
                    _values.Add(v); // Add the processed value to the results list
                }
            }
        }

 
        /// Gets the binding list of output values, which is displayed in the Registry Explorer UI.
        public IBindingList Values => _values;
    }
}

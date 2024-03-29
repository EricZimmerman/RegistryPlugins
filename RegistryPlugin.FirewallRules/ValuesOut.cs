﻿using RegistryPluginBase.Interfaces;

namespace RegistryPlugin.FirewallRules
{
    public class ValuesOut : IValueOut
    {
        public ValuesOut(string action, string active, string dir, string protocol, string lport, string rport, string name, string desc, string app)
        {
            Action = action;
            Active = active;
            Dir = dir;
            Protocol = protocol;
            LPort = lport;
            RPort = rport;
            Name = name;
            Desc = desc;
            App = app;
        }
        public string Action { get; }
        public string Active { get; }
        public string Dir { get; }
        public string Protocol { get; }
        public string LPort { get; }
        public string RPort { get; }
        public string Name { get; }
        public string Desc { get; }
        public string App { get; }
        public string BatchKeyPath { get; set; }
        public string BatchValueName { get; set; }
        public string BatchValueData1 => $"Action: {Action} Active: {Active}";
        public string BatchValueData2 => $"Dir: {Dir} Protocol: {Protocol} Name: {Name}";
        public string BatchValueData3 => $"LPort: {LPort} RPort: {RPort} Desc: {Desc} App: {App}";
    }
}

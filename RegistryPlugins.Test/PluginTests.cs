using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using NFluent;
using NUnit.Framework;
using Registry;
using RegistryPlugin.Adobe;
using RegistryPlugin.AppCompatCache;
using RegistryPlugin.AppCompatFlags;
using RegistryPlugin.CIDSizeMRU;
using RegistryPlugin.DHCPNetworkHint;
using RegistryPlugin.FirstFolder;
using RegistryPlugin.KnownNetworks;
using RegistryPlugin.LastVisitedMRU;
using RegistryPlugin.LastVisitedPidlMRU;
using RegistryPlugin.OpenSaveMRU;
using RegistryPlugin.OpenSavePidlMRU;
using RegistryPlugin.RecentDocs;
using RegistryPlugin.RunMRU;
using RegistryPlugin.SAM;
using RegistryPlugin.Services;
using RegistryPlugin.TerminalServerClient;
using RegistryPlugin.TypedURLs;
using RegistryPlugin.WordWheelQuery;
using RegistryPlugin.UserAssist;
using ValuesOut = RegistryPlugin.AppCompatCache.ValuesOut;
using RegistryPlugin.RecentApps;
using RegistryPlugin.BamDam;
using RegistryPlugin.JumplistData;
using RegistryPlugin.MountedDevices;
using RegistryPlugin.SyscacheObjectTable;
using RegistryPlugin.Taskband;
using RegistryPlugin.TaskFlowShellActivities;

namespace RegistryPlugins.Test
{
    [TestFixture]
    public class PluginTests
    {

        [Test]
        public void TaskShellitem()
        {
            var r = new TaskFlowShellActivities();

         //  var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_RecentAppsERZ.DAT");
            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\!Private\NTUSER_ERZ_Win10.DAT");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\CloudStore\Store\Cache\DefaultAccount\$$windows.data.taskflow.shellactivities\Current");

            Check.That(key).IsNotNull();

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

          

            Check.That(r.Values.Count).IsEqualTo(545);

            var ff = (RegistryPlugin.TaskFlowShellActivities.ValuesOut) r.Values[0];

            Check.That(ff.Other).IsNotEmpty();
            Check.That(ff.Other).Contains("Timestamp");

            Debug.WriteLine(ff.Other);

            foreach (var rValue in r.Values)
            {
                Debug.WriteLine(rValue);
            }
        }

        [Test]
        public void JumplistData()
        {
            var r = new JumplistData();

            //  var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_RecentAppsERZ.DAT");
            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_MarkElliot_RecentApps.DAT");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Search\JumplistData");

            Check.That(key).IsNotNull();

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);
            
            Check.That(r.Values.Count).IsEqualTo(44);

            var ff = (RegistryPlugin.JumplistData.ValuesOut) r.Values[0];

            Check.That(ff.JumpListName).IsNotEmpty();
            Check.That(ff.ExecutedOn).IsNotEqualTo(null);

            foreach (var rValue in r.Values)
            {
                Debug.WriteLine(rValue);
            }
        }

        [Test]
        public void Taskband()
        {
            var r = new Taskband();

            //  var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_RecentAppsERZ.DAT");
            var reg = new RegistryHive(@"C:\Temp\ntclean.dat");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband");

            Check.That(key).IsNotNull();

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(15);

            var ff = (RegistryPlugin.Taskband.ValuesOut) r.Values[0];

         //   Check.That(ff.Other).IsNotEmpty();
          //  Check.That(ff.Other).Contains("Timestamp");

            //Debug.WriteLine(ff.Other);

            foreach (var rValue in r.Values)
            {
                Debug.WriteLine(rValue);
            }
        }

        [Test]
        public void Taskband2()
        {
            var r = new Taskband();

            //  var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_RecentAppsERZ.DAT");
            var reg = new RegistryHive(@"C:\Temp\ntuser.dat");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband");

            Check.That(key).IsNotNull();

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(17);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var ff = (RegistryPlugin.Taskband.ValuesOut) r.Values[0];

            //   Check.That(ff.Other).IsNotEmpty();
            //  Check.That(ff.Other).Contains("Timestamp");

            //Debug.WriteLine(ff.Other);

            foreach (var rValue in r.Values)
            {
                Debug.WriteLine(rValue);
            }
        }



        [Test]
        public void Adobe()
        {
            /*var f = new List<string>();
            f.Add("D:20200618035802+05'30'");
            f.Add("D:20200618035756+05'30'");
            f.Add("D:20200618035751+05'30'");
            f.Add("D:20200618035744+05'30'");
            f.Add("D:20200618035737+05'30'");
            f.Add("D:20200617182700-04'00'");
            f.Add("D:20200617182636-04'00'");
            f.Add("D:20200617223217Z");
            f.Add("D:20200617182628-04'00'");
            f.Add("D:20200617181449-04'00'");
            f.Add("D:20200617181442-04'00'");
            f.Add("D:20200617181427-04'00'");
            f.Add("D:20200617221353Z");
            f.Add("D:20200617221332Z");
            f.Add("D:20200617221318Z");
            f.Add("D:20200617230951+01'00'");
            f.Add("D:20200617230941+01'00'");
            f.Add("D:20200617230930+01'00'");
            f.Add("D:20200617230922+01'00'");
            f.Add("D:20200617230912+01'00'");
            f.Add("D:20200617230902+01'00'");
            f.Add("D:20200617230854+01'00'");
            f.Add("D:20200617223156Z");
            f.Add("D:20200617230846+01'00'");
            f.Add("D:20200617230730+01'00'");
            f.Add("D:20200617230724+01'00'");
            f.Add("D:20200617230718+01'00'");
            f.Add("D:20200617223147Z");
            f.Add("D:20200617230712+01'00'");
            f.Add("D:20200617230702+01'00'");
            f.Add("D:20200617230655+01'00'");
            f.Add("D:20200617230647+01'00'");
            f.Add("D:20200617230640+01'00'");
            f.Add("D:20200617230217+01'00'");
            f.Add("D:20200617230020+01'00'");
            f.Add("D:20200617230016+01'00'");
            f.Add("D:20200617223137Z");
            f.Add("D:20200617223126Z");
            f.Add("D:20200617223024Z");
            f.Add("D:20200617223003Z");

            foreach (var f1 in f)
            {
                var start = f1.Substring(2);
                Console.WriteLine(start);

                var year = start.Substring(0, 4);
                var month = start.Substring(4, 2);
                var day = start.Substring(6, 2);
                var hours = start.Substring(8, 2);
                var mins = start.Substring(10, 2);
                var sec = start.Substring(12, 2);
                var tzi = start.Substring(14);
                tzi = tzi.Replace("'", ":").TrimEnd(':');

                var dateString = $"{month}/{day}/{year}";
                var timeString = $"{hours}:{mins}:{sec}";

                Console.WriteLine($"{dateString} {timeString} {tzi}");

                if (tzi.EndsWith("Z"))
                {
                    tzi = "+0:00";
                }

                var aaa = DateTimeOffset.ParseExact($"{dateString} {timeString} {tzi}","MM/dd/yyyy HH:mm:ss zzz",CultureInfo.InvariantCulture);

                var dto = new DateTimeOffset(int.Parse(year),int.Parse(month),int.Parse(day),int.Parse(hours),int.Parse(mins),int.Parse(sec),TimeSpan.Zero);

                Console.WriteLine($"dto: {dto:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"As UTC: {aaa.ToUniversalTime():yyyy-MM-dd HH:mm:ss}");
            }*/


            var r = new Adobe();

            var reg = new RegistryHive(@"D:\Temp\Adobe_cRecentFiles_NTUSER.DAT");
            
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Adobe");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(114);
            Check.That(r.Errors.Count).IsEqualTo(0);

        }

        [Test]
        public void AppCompatFlags()
        {
            var r = new AppCompatFlags();

            var reg = new RegistryHive(@"D:\Temp\SoftwareCID\Software");
            reg.ParseHive();

            var key = reg.GetKey(@"Microsoft\Windows NT\CurrentVersion\AppCompatFlags\CIT\System");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(37);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var ff = (RegistryPlugin.AppCompatFlags.ValuesOut) r.Values[0];

            Check.That(ff.ValueName).IsEqualTo("746E9C8C08A390000004EC55A1F64D10");
            Check.That(ff.Executable).Contains("\\DEVICE");
        }

          
        [Test]
        public void Syscache()
        {
            var r = new SyscacheObjectTable();

            var reg = new RegistryHive(@"D:\Temp\SoftwareCID\Syscache.hve");
            reg.ParseHive();

            var key = reg.GetKey(@"DefaultObjectStore\ObjectTablere");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(1585);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var ff = (RegistryPlugin.SyscacheObjectTable.ValuesOut) r.Values[0];

            Check.That(ff.Sha1).IsEqualTo("f34bbe523cf4b187b2c27da2bcd267412301745d");
            Check.That(ff.MftEntryNumber).IsEqualTo(26442);
            Check.That(ff.MftEntryNumber).IsEqualTo(26442);
            Check.That(ff.KeyPath).IsEqualTo("DefaultObjectStore\\ObjectTable\\1");
        }

        [Test]
        public void AppCompatTestOneOff()
        {
            var r = new AppCompat();

            var reg = new RegistryHive(@"C:\Users\eric\Desktop\SYSTEM");
            reg.ParseHive();

            var key = reg.GetKey(@"ControlSet001\Control\Session Manager\AppCompatCache");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(325);

            var ff = (ValuesOut) r.Values[0];

            Check.That(ff.CacheEntryPosition).IsEqualTo(0);
            Check.That(ff.ProgramName).Contains("Logon");
        }

        [Test]
        public void BamDam()
        {
            var r = new BamDam();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\SYSTEM_Creators");
            reg.ParseHive();

            var key = reg.GetKey(@"ControlSet001\Services\dam\UserSettings\S-1-5-21-238543598-4054144643-4261915534-1114");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(112);

            var ff = (RegistryPlugin.BamDam.ValuesOut) r.Values[0];

            Check.That(ff.ExecutionTime.Year).IsEqualTo(2017);
            Check.That(ff.ExecutionTime.Month).IsEqualTo(3);
            Check.That(ff.ExecutionTime.Day).IsEqualTo(18);
            Check.That(ff.Program).Contains("Skype");


            key = reg.GetKey(@"ControlSet001\Services\dam\UserSettings\S-1-5-18");

            r = new BamDam();

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(4);

             ff = (RegistryPlugin.BamDam.ValuesOut) r.Values[0];

            Check.That(ff.ExecutionTime.Year).IsEqualTo(2017);
            Check.That(ff.ExecutionTime.Month).IsEqualTo(3);
            Check.That(ff.ExecutionTime.Day).IsEqualTo(18);
            Check.That(ff.Program).Contains("Start10");


        }



        [Test]
        public void AppCompatTest()
        {
            var r = new AppCompat();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\SYSTEM");
            reg.ParseHive();

            var key = reg.GetKey(@"ControlSet001\Control\Session Manager\AppCompatCache");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(1024);

            var ff = (ValuesOut) r.Values[0];

            Check.That(ff.CacheEntryPosition).IsEqualTo(0);
            Check.That(ff.ProgramName).Contains("java");
        }

        [Test]
        public void AppCompatTestCreators()
        {
            var r = new AppCompat();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\SYSTEM_Creators");
            reg.ParseHive();

            var key = reg.GetKey(@"ControlSet001\Control\Session Manager\AppCompatCache");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(506);

            var ff = (ValuesOut) r.Values[0];

            Check.That(ff.CacheEntryPosition).IsEqualTo(0);
            Check.That(ff.ProgramName).Contains("nvstreg.exe");
        }


        [Test]
        public void BlakeKnownNetworks()
        {
            var r = new KnownNetworks();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\SOFTWARE_dblake");
            reg.ParseHive();

            var key = reg.GetKey(@"Microsoft\Windows NT\CurrentVersion\NetworkList");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(27);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var ff = (KnownNetwork) r.Values[0];

            Check.That(ff.NetworkName).IsEqualTo(@"gogoinflight");
            Check.That(ff.DNSSuffix).IsEqualTo(@"<none>");
            Check.That(ff.ProfileGUID).IsEqualTo("{167B2E5E-29EA-429E-8D43-E82043F0D3CF}");
            Check.That(ff.FirstConnectLOCAL.Year).IsEqualTo(2013);
            Check.That(ff.FirstConnectLOCAL.Day).IsEqualTo(3);
        }

        [Test]
        public void BlakeOpenSavePidlMRU()
        {
            var r = new OpenSavePidlMRU();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_dblake.DAT");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(57);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var ff = (RegistryPlugin.OpenSavePidlMRU.ValuesOut) r.Values[0];

            Check.That(ff.AbsolutePath)
                .IsEqualTo(
                    @"Web sites\https://asgardventurecapital.sharepoint.com\Shared Documents\Confidential Analysis Data\NETFLIX_10-K_20130201.xlsx");
            Check.That(ff.ValueName).IsEqualTo("17");
        }

        [Test]
        public void BlakeRecentDocs()
        {
            var r = new RecentDocs();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_dblake.DAT");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(192);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var ff = (RecentDoc) r.Values[0];

            Check.That(ff.ValueName).IsEqualTo("83");
            Check.That(ff.Extension).Contains("RecentDocs");
        }

        [Test]
        public void BlakeServices()
        {
            var r = new Services();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\SYSTEM_dblake");
            reg.ParseHive();

            var key = reg.GetKey(@"ControlSet001\Services");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(553);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var ff = (Service) r.Values[0];

            Check.That(ff.Name).IsEqualTo(".NET CLR Data");
            Check.That(ff.Description).IsEqualTo("");
            Check.That(ff.NameKeyLastWrite.Year).IsEqualTo(2013);

            ff = (Service) r.Values[8];

            Check.That(ff.Name).IsEqualTo("3ware");
            Check.That(ff.Description).IsEqualTo("");
            Check.That(ff.NameKeyLastWrite.Year).IsEqualTo(2013);

            ff = (Service) r.Values[263];

            Check.That(ff.Name).IsEqualTo("napagent");
            Check.That(ff.Description).IsEqualTo(@"@%SystemRoot%\system32\qagentrt.dll,-7");
            Check.That(ff.StartMode).IsEqualTo(ServiceStartMode.Manual);
            Check.That(ff.ServiceType).IsEqualTo(ServiceType.Win32ShareProcess);
            Check.That(ff.ServiceDLL).IsEqualTo(@"%SystemRoot%\system32\qagentRT.dll");
            Check.That(ff.NameKeyLastWrite.Year).IsEqualTo(2013);
        }

        [Test]
        public void BlakeTypedURLs()
        {
            var r = new TypedURLs();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_dblake.DAT");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Internet Explorer\TypedURLs");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(18);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var ff = (TypedURL) r.Values[0];

            Check.That(ff.Url).IsEqualTo("http://dropbox.com/");
            Check.That(ff.Timestamp.Value.Year).IsEqualTo(2013);

            ff = (TypedURL) r.Values[5];

            Check.That(ff.Url).IsEqualTo("https://asgardventurecapital-my.sharepoint.com/");
            Check.That(ff.Timestamp.Value.Day).IsEqualTo(23);
        }

        [Test]
        public void BlakeWordWheel()
        {
            var r = new WordWheelQuery();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_dblake.DAT");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\WordWheelQuery");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(6);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var ff = (RegistryPlugin.WordWheelQuery.ValuesOut) r.Values[0];

            Check.That(ff.MruPosition).IsEqualTo(0);
            Check.That(ff.SearchTerm).Contains("defrag");

            ff = (RegistryPlugin.WordWheelQuery.ValuesOut) r.Values[1];

            Check.That(ff.MruPosition).IsEqualTo(0);
            Check.That(ff.SearchTerm).Contains("cc");

            ff = (RegistryPlugin.WordWheelQuery.ValuesOut) r.Values[2];

            Check.That(ff.MruPosition).IsEqualTo(1);
            Check.That(ff.SearchTerm).Contains("jboone");
        }

        [Test]
        public void BlakeUserAssist()
        {
            var r = new UserAssist();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_dblake.DAT");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist\{CEBFF5CD-ACE2-4F4F-9178-9926F41749EA}\Count");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(205);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var ff = (RegistryPlugin.UserAssist.ValuesOut)r.Values[1];

            Check.That(ff.RunCounter).IsEqualTo(0);
            Check.That(ff.ProgramName).IsEqualTo("Microsoft.Windows.Explorer");
            Check.That(ff.FocusCount).IsEqualTo(619);
            Check.That(ff.FocusTime).IsEqualTo("0d, 3h, 46m, 24s");


        }

        [Test]
        public void CidSizeTest()
        {
            var r = new CIDSizeMRU();

            var reg = new RegistryHive(@"..\..\Hives\ntuser1.dat");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\CIDSizeMRU");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(8);

            var ff = (CIDSizeInfo) r.Values[0];

            Check.That(ff.MRUPosition).IsEqualTo(0);
            Check.That(ff.Executable).Contains("AcroRd32.exe");
        }

        [Test]
        public void DhcpNetworkHint()
        {
            var r = new DHCPNetworkHint();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\SYSTEM_dblake");
            reg.ParseHive();

            var key = reg.GetKey(@"ControlSet001\Services\Tcpip\Parameters\Interfaces");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(6);

            var ff = (RegistryPlugin.DHCPNetworkHint.ValuesOut) r.Values[1];

            Check.That(ff.DHCPServer).IsEqualTo("10.17.0.7");
            Check.That(ff.DHCPAddress).Contains("10.17.14.218");
            Check.That(ff.DefaultGateway).Contains("10.17.0.1");
            Check.That(ff.Interface).Contains("{5185491C-401D-491E-8C6F-07F6AFFF1A64}");
            Check.That(ff.InterfaceSubkey).Contains("072776E2165627F6D266275656");
            Check.That(ff.LeaseExpires.Year).IsEqualTo(2013);
            Check.That(ff.LeaseObtained.Month).IsEqualTo(10);
            Check.That(ff.NetworkHint).Contains("prg.aero-fre");
        }


        //        [Test]
        //        public void LastVisitedPidlMruTest2()
        //        {
        //            var r = new LastVisitedPidlMRU();
        //            var reg = new RegistryHive(@"D:\Temp\win10ERZamcachepreso\NTUSER.DAT");
        //            reg.ParseHive();
        //
        //            var key = reg.GetKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRU");
        //
        //            Check.That(r.Values.Count).IsEqualTo(0);
        //
        //            r.ProcessValues(key);
        //
        //            Check.That(r.Values.Count).IsEqualTo(6);
        //
        //        }


        [Test]
        public void FirstFolderTest()
        {
            var r = new FirstFolder();

            var reg = new RegistryHive(@"..\..\Hives\ntuser1.dat");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\FirstFolder");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(3);

            var ff = (FolderInfo) r.Values[0];

            Check.That(ff.MRUPosition).IsEqualTo(0);
            Check.That(ff.Executable).Contains(@"C:\Program Files (x86)\Canon\MP Navigator EX 2.0\mpnex20.exe");
        }

        [Test]
        public void KnownNetwork()
        {
            var r = new KnownNetworks();
            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\SOFTWARE_HPSpectre_EST");
            reg.ParseHive();

            var key = reg.GetKey(@"Microsoft\Windows NT\CurrentVersion\NetworkList");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(23);
        }

        [Test]
        public void LastVisitedMRUTest()
        {
            var r = new LastVisitedMRU();
            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\ntuserNokiaShellBags.dat");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedMRU");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(11);
        }

        [Test]
        public void LastVisitedPidlMruTest()
        {
            var r = new LastVisitedPidlMRU();
            var reg = new RegistryHive(@"..\..\Hives\ntuser1.dat");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRU");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(7);
        }

        [Test]
        public void MountedDevicesTest()
        {
            var r = new MountedDevices();

            var reg = new RegistryHive(@"C:\temp\tout\c\Windows\System32\config\SYSTEM");
            reg.ParseHive();

            var key = reg.GetKey(@"MountedDevices");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(55);
        }

        [Test]
        public void OpenSaveMRUTest()
        {
            var r = new OpenSaveMRU();
            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_Loveall.DAT");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSaveMRU");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(6);
        }

        [Test]
        public void OpenSavePidlMruTest()
        {
            var r = new OpenSavePidlMRU();
            //var reg = new RegistryHive(@"c:\temp\ntuser.dat");
          var reg = new RegistryHive(@"..\..\Hives\ntuser1.dat");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(16);
        }

        [Test]
        public void RunMRUTest()
        {
            var r = new RunMRU();
            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\ALL\NTUSER (8).DAT");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(4);
        }

        [Test]
        public void SamDBlakeGroups()
        {
            var r = new UserAccounts();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\SAM_dblake");
            reg.RecoverDeleted = true;
            reg.ParseHive();

            var key = reg.GetKey(@"SAM\Domains\Account\Users");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsStrictlyGreaterThan(0);
            Check.That(r.Errors.Count).IsEqualTo(0);


            var users = (BindingList<UserOut>) r.Values;

            var user = users.Single(t => t.UserId == 1001);

            Check.That(user.UserName.ToLowerInvariant()).IsEqualTo("donald");
        }


        [Test]
        public void SamPluginExploding()
        {
            var r = new UserAccounts();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\SAM_brokenPlugin");
            reg.RecoverDeleted = true;
            reg.ParseHive();

            var key = reg.GetKey(@"SAM\Domains\Account\Users");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsStrictlyGreaterThan(0);
            Check.That(r.Errors.Count).IsEqualTo(0);
        }

        [Test]
        public void SamPluginPWHint()
        {
            var r = new UserAccounts();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\SAM_hasBigEndianDWord");
            reg.RecoverDeleted = true;
            reg.ParseHive();

            var key = reg.GetKey(@"SAM\Domains\Account\Users");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsStrictlyGreaterThan(0);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var u = (UserOut) r.Values[2];

            Check.That(u.PasswordHint).Equals("G");
        }

        [Test]
        public void RecentApps()
        {
            var r = new RecentApps();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_RecentAppsERZ.DAT");
            reg.RecoverDeleted = true;
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Search\RecentApps");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsStrictlyGreaterThan(0);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var u = (RegistryPlugin.RecentApps.ValuesOut)r.Values[2];

            Check.That(u.AppPath).Contains("chrome.exe");
            Check.That(u.RecentItems.Count).Equals(10);
            Check.That(u.RecentDocs).Contains("drivepr");
        }


        [Test]
        public void SamPluginShouldFindEricAccount()
        {
            var r = new UserAccounts();

            var reg = new RegistryHive(@"..\..\Hives\SAM");
            reg.ParseHive();

            var key = reg.GetKey(@"SAM\Domains\Account\Users");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsStrictlyGreaterThan(0);
            Check.That(r.Values.Count).IsEqualTo(4);

            var users = (BindingList<UserOut>) r.Values;

            var user = users.Single(t => t.UserId == 1000);

            Check.That(user.UserName.ToLowerInvariant()).IsEqualTo("eric");

            var lastLogin = DateTimeOffset.Parse("3/25/2015 2:31:24 PM +00:00");

            Check.That(user.LastLoginTime?.ToString("G")).Equals(lastLogin.ToString("G"));

            Check.That(user.LastLoginTime?.Year).Equals(lastLogin.Year);
            Check.That(user.LastLoginTime?.Month).Equals(lastLogin.Month);
            Check.That(user.LastLoginTime?.Day).Equals(lastLogin.Day);
            Check.That(user.LastLoginTime?.Hour).Equals(lastLogin.Hour);
            Check.That(user.LastLoginTime?.Minute).Equals(lastLogin.Minute);
            Check.That(user.LastLoginTime?.Second).Equals(lastLogin.Second);

            var lastPwChange = DateTimeOffset.Parse("3/23/2015 9:15:55 PM +00:00");

            Check.That(user.LastPasswordChange?.ToString("G")).Equals(lastPwChange.ToString("G"));

            Check.That(user.LastIncorrectPassword).IsNull();
            Check.That(user.ExpiresOn).IsNull();

            Check.That(user.TotalLoginCount).IsEqualTo(3);
            Check.That(user.InvalidLoginCount).IsEqualTo(0);

            user = users.Single(t => t.UserId == 500);

            Check.That(user.UserName.ToLowerInvariant()).IsEqualTo("administrator");
        }

        [Test]
        public void TerminalServers()
        {
            var r = new TerminalServerClient();

            var reg = new RegistryHive(@"D:\SynologyDrive\Registry\ALL\NTUSER.DAT");
            reg.ParseHive();

            var key = reg.GetKey(@"Software\Microsoft\Terminal Server Client");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(6);
            Check.That(r.Errors.Count).IsEqualTo(0);

            var ff = (RegistryPlugin.TerminalServerClient.ValuesOut) r.Values[0];

            Check.That(ff.MRUPosition).IsEqualTo(1);
            Check.That(ff.HostName).Contains("GOON");

            r = new TerminalServerClient();

            reg = new RegistryHive(@"D:\SynologyDrive\Registry\ALL\NTUSER3.DAT");
            reg.ParseHive();

            key = reg.GetKey(@"Software\Microsoft\Terminal Server Client");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(7);
            Check.That(r.Errors.Count).IsEqualTo(0);

            ff = (RegistryPlugin.TerminalServerClient.ValuesOut) r.Values[0];

            Check.That(ff.MRUPosition).IsEqualTo(-1);
            Check.That(ff.HostName).Contains("GOON");
            Check.That(ff.LastModified.Month).IsEqualTo(5);
            Check.That(ff.LastModified.Minute).IsEqualTo(32);
            Check.That(ff.LastModified.Second).IsEqualTo(8);
            Check.That(ff.LastModified.Day).IsEqualTo(28);
            

            ff = (RegistryPlugin.TerminalServerClient.ValuesOut) r.Values[3];

            Check.That(ff.MRUPosition).IsEqualTo(-1);
            Check.That(ff.HostName).Contains("SVR01");

            r = new TerminalServerClient();

            reg = new RegistryHive(@"D:\SynologyDrive\Registry\NTUSER_dblake.DAT");
            reg.ParseHive();

            key = reg.GetKey(@"Software\Microsoft\Terminal Server Client");

            Check.That(r.Values.Count).IsEqualTo(0);

            r.ProcessValues(key);

            Check.That(r.Values.Count).IsEqualTo(0);
            Check.That(r.Errors.Count).IsEqualTo(0);

          
        }
    }
}
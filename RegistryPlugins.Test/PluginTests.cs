using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Registry;
using RegistryPlugin.Adobe;
using RegistryPlugin.AppCompatCache;
using RegistryPlugin.AppCompatFlags;
using RegistryPlugin.BamDam;
using RegistryPlugin.CIDSizeMRU;
using RegistryPlugin.DHCPNetworkHint;
using RegistryPlugin.FirstFolder;
using RegistryPlugin.JumplistData;
using RegistryPlugin.KnownNetworks;
using RegistryPlugin.LastVisitedMRU;
using RegistryPlugin.LastVisitedPidlMRU;
using RegistryPlugin.MountedDevices;
using RegistryPlugin.OfficeMRU;
using RegistryPlugin.OpenSaveMRU;
using RegistryPlugin.OpenSavePidlMRU;
using RegistryPlugin.RecentApps;
using RegistryPlugin.RecentDocs;
using RegistryPlugin.RunMRU;
using RegistryPlugin.SAM;
using RegistryPlugin.Services;
using RegistryPlugin.SyscacheObjectTable;
using RegistryPlugin.Taskband;
using RegistryPlugin.TaskCache;
using RegistryPlugin.TaskFlowShellActivities;
using RegistryPlugin.TerminalServerClient;
using RegistryPlugin.TypedURLs;
using RegistryPlugin.USBSTOR;
using RegistryPlugin.UserAssist;
using RegistryPlugin.WordWheelQuery;
using ValuesOut = RegistryPlugin.USBSTOR.ValuesOut;

namespace RegistryPlugins.Test;

[TestFixture]
public class PluginTests
{
    [Test]
    public void TaskCache()
    {
        var r = new TaskCache();


        var reg = TestHives.Corpus(@"SOFTWARE_dblake");


        // var l = new List<string>();
        // l.Add(@"C:\Temp\tout\G\Users\fredr\ntuser.dat.LOG1");
        // l.Add(@"C:\Temp\tout\G\Users\fredr\ntuser.dat.LOG2");


        //      reg.ProcessTransactionLogs(l, true);


        reg.ParseHive();

        var key = reg.GetKey(@"Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks");

        key.Should().NotBeNull();

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);


        Debug.WriteLine(r.Values.Count);


        foreach (var rValue in r.Values)
        {
            Debug.WriteLine(rValue);
        }
    }


    [Test]
    public void TaskShellitem()
    {
        var r = new TaskFlowShellActivities();
        var reg = TestHives.Corpus(@"!Private\NTUSER_ERZ_Win10.DAT");

        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\CloudStore\Store\Cache\DefaultAccount\$$windows.data.taskflow.shellactivities\Current");

        key.Should().NotBeNull();

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);


        r.Values.Count.Should().Be(545);

        var ff = (RegistryPlugin.TaskFlowShellActivities.ValuesOut)r.Values[0];

        ff.Other.Should().NotBeNullOrEmpty();
        ff.Other.Should().Contain("Timestamp");

        Debug.WriteLine(ff.Other);

        foreach (var rValue in r.Values)
        {
            Debug.WriteLine(rValue);
        }
    }

    [Test]
    public void AppCompatFlags()
    {
        var r = new AppCompatFlags();

        var reg = TestHives.Corpus(@"SoftwareCID\Software");
        reg.ParseHive();

        var key = reg.GetKey(@"Microsoft\Windows NT\CurrentVersion\AppCompatFlags\CIT\System");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(37);
        r.Errors.Count.Should().Be(0);

        var ff = (RegistryPlugin.AppCompatFlags.ValuesOut)r.Values[0];

        ff.ValueName.Should().Be("746E9C8C08A390000004EC55A1F64D10");
        ff.Executable.Should().Contain("\\DEVICE");
    }


    [Test]
    public void Syscache()
    {
        var r = new SyscacheObjectTable();

        var reg = TestHives.Corpus(@"SoftwareCID\Syscache.hve");
        reg.ParseHive();

        var key = reg.GetKey(@"DefaultObjectStore\ObjectTable");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(1585);
        r.Errors.Count.Should().Be(0);

        var ff = (RegistryPlugin.SyscacheObjectTable.ValuesOut)r.Values[0];

        ff.Sha1.Should().Be("f34bbe523cf4b187b2c27da2bcd267412301745d");
        ff.MftEntryNumber.Should().Be(26442);
        ff.MftEntryNumber.Should().Be(26442);
        ff.KeyPath.Should().Be("DefaultObjectStore\\ObjectTable\\1");
    }

    [Test]
    public void AppCompatTest()
    {
        var r = new AppCompat();

        var reg = TestHives.Corpus(@"SYSTEM");
        reg.ParseHive();

        var key = reg.GetKey(@"ControlSet001\Control\Session Manager\AppCompatCache");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(1024);

        var ff = (RegistryPlugin.AppCompatCache.ValuesOut)r.Values[0];

        ff.CacheEntryPosition.Should().Be(0);
        ff.ProgramName.Should().Contain("java");
    }

    [Test]
    public void AppCompatTestCreators()
    {
        var r = new AppCompat();

        var reg = TestHives.Corpus(@"SYSTEM_Creators");
        reg.ParseHive();

        var key = reg.GetKey(@"ControlSet001\Control\Session Manager\AppCompatCache");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(506);

        var ff = (RegistryPlugin.AppCompatCache.ValuesOut)r.Values[0];

        ff.CacheEntryPosition.Should().Be(0);
        ff.ProgramName.Should().Contain("nvstreg.exe");
    }


    [Test]
    public void BlakeKnownNetworks()
    {
        var r = new KnownNetworks();

        var reg = TestHives.Corpus(@"SOFTWARE_dblake");
        reg.ParseHive();

        var key = reg.GetKey(@"Microsoft\Windows NT\CurrentVersion\NetworkList");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(27);
        r.Errors.Count.Should().Be(0);

        var ff = (KnownNetwork)r.Values[0];

        ff.NetworkName.Should().Be(@"gogoinflight");
        ff.DNSSuffix.Should().Be(@"<none>");
        ff.ProfileGUID.Should().Be("{167B2E5E-29EA-429E-8D43-E82043F0D3CF}");
        ff.FirstConnectLOCAL.Year.Should().Be(2013);
        ff.FirstConnectLOCAL.Day.Should().Be(3);
    }

    [Test]
    public void BlakeRecentDocs()
    {
        var r = new RecentDocs();

        var reg = TestHives.Corpus(@"NTUSER_dblake.DAT");
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\RecentDocs");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(192);
        r.Errors.Count.Should().Be(0);

        var ff = (RecentDoc)r.Values[0];

        ff.ValueName.Should().Be("83");
        ff.Extension.Should().Contain("RecentDocs");
    }

    [Test]
    public void BlakeServices()
    {
        var r = new Services();

        var reg = TestHives.Corpus(@"SYSTEM_dblake");
        reg.ParseHive();

        var key = reg.GetKey(@"ControlSet001\Services");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(553);
        r.Errors.Count.Should().Be(0);

        var ff = (Service)r.Values[0];

        ff.Name.Should().Be(".NET CLR Data");
        ff.Description.Should().Be("");
        ff.NameKeyLastWrite.Year.Should().Be(2013);

        ff = (Service)r.Values[8];

        ff.Name.Should().Be("3ware");
        ff.Description.Should().Be("");
        ff.NameKeyLastWrite.Year.Should().Be(2013);

        ff = (Service)r.Values[263];

        ff.Name.Should().Be("napagent");
        ff.Description.Should().Be(@"@%SystemRoot%\system32\qagentrt.dll,-7");
        ff.StartMode.Should().Be(Service.ServiceStartMode.Manual);
        ff.ServiceType.Should().Be(Service.ServiceTypeEnum.Win32ShareProcess);
        ff.ServiceDLL.Should().Be(@"%SystemRoot%\system32\qagentRT.dll");
        ff.NameKeyLastWrite.Year.Should().Be(2013);
    }

    [Test]
    public void BlakeTypedURLs()
    {
        var r = new TypedURLs();

        var reg = TestHives.Corpus(@"NTUSER_dblake.DAT");
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Internet Explorer\TypedURLs");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(18);
        r.Errors.Count.Should().Be(0);

        var ff = (TypedURL)r.Values[0];

        ff.Url.Should().Be("http://dropbox.com/");
        ff.Timestamp.Value.Year.Should().Be(2013);

        ff = (TypedURL)r.Values[5];

        ff.Url.Should().Be("https://asgardventurecapital-my.sharepoint.com/");
        ff.Timestamp.Value.Day.Should().Be(23);
    }

    [Test]
    public void BlakeWordWheel()
    {
        var r = new WordWheelQuery();

        var reg = TestHives.Corpus(@"NTUSER_dblake.DAT");
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\WordWheelQuery");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(6);
        r.Errors.Count.Should().Be(0);

        var ff = (RegistryPlugin.WordWheelQuery.ValuesOut)r.Values[0];

        ff.MruPosition.Should().Be(0);
        ff.SearchTerm.Should().Contain("defrag");

        ff = (RegistryPlugin.WordWheelQuery.ValuesOut)r.Values[1];

        ff.MruPosition.Should().Be(0);
        ff.SearchTerm.Should().Contain("cc");

        ff = (RegistryPlugin.WordWheelQuery.ValuesOut)r.Values[2];

        ff.MruPosition.Should().Be(1);
        ff.SearchTerm.Should().Contain("jboone");
    }

    [Test]
    public void BlakeUserAssist()
    {
        var r = new UserAssist();

        var reg = TestHives.Corpus(@"NTUSER_dblake.DAT");
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\UserAssist\{CEBFF5CD-ACE2-4F4F-9178-9926F41749EA}\Count");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(205);
        r.Errors.Count.Should().Be(0);

        var ff = (RegistryPlugin.UserAssist.ValuesOut)r.Values[1];

        ff.RunCounter.Should().Be(0);
        ff.ProgramName.Should().Be("Microsoft.Windows.Explorer");
        ff.FocusCount.Should().Be(619);
        ff.FocusTime.Should().Be("0d, 3h, 46m, 24s");
    }

    [Test]
    public void CidSizeTest()
    {
        var r = new CIDSizeMRU();

        var reg = TestHives.Repo("ntuser1.dat");
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\CIDSizeMRU");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(8);

        var ff = (CIDSizeInfo)r.Values[0];

        ff.MRUPosition.Should().Be(0);
        ff.Executable.Should().Contain("AcroRd32.exe");
    }

    [Test]
    public void DhcpNetworkHint()
    {
        var r = new DHCPNetworkHint();

        var reg = TestHives.Corpus(@"SYSTEM_dblake");
        reg.ParseHive();

        var key = reg.GetKey(@"ControlSet001\Services\Tcpip\Parameters\Interfaces");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(6);

        var ff = (RegistryPlugin.DHCPNetworkHint.ValuesOut)r.Values[1];

        ff.DHCPServer.Should().Be("10.17.0.7");
        ff.DHCPAddress.Should().Contain("10.17.14.218");
        ff.DefaultGateway.Should().Contain("10.17.0.1");
        ff.Interface.Should().Contain("{5185491C-401D-491E-8C6F-07F6AFFF1A64}");
        ff.InterfaceSubkey.Should().Contain("072776E2165627F6D266275656");
        ff.LeaseExpires.Year.Should().Be(2013);
        ff.LeaseObtained.Month.Should().Be(10);
        ff.NetworkHint.Should().Contain("prg.aero-fre");
    }


    //        [Test]
    //        public void LastVisitedPidlMruTest2()
    //        {
    //            var r = new LastVisitedPidlMRU();
    //            reg.ParseHive();
    //
    //            var key = reg.GetKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRU");
    //
    //            r.Values.Count.Should().Be(0);
    //
    //            r.ProcessValues(key);
    //
    //            r.Values.Count.Should().Be(6);
    //
    //        }


    [Test]
    public void FirstFolderTest()
    {
        var r = new FirstFolder();

        var reg = TestHives.Repo("ntuser1.dat");
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\FirstFolder");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(3);

        var ff = (FolderInfo)r.Values[0];

        ff.MRUPosition.Should().Be(0);
        ff.Executable.Should().Contain(@"C:\Program Files (x86)\Canon\MP Navigator EX 2.0\mpnex20.exe");
    }

    [Test]
    public void KnownNetwork()
    {
        var r = new KnownNetworks();
        var reg = TestHives.Corpus(@"SOFTWARE_HPSpectre_EST");
        reg.ParseHive();

        var key = reg.GetKey(@"Microsoft\Windows NT\CurrentVersion\NetworkList");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(23);
    }

    [Test]
    public void LastVisitedMRUTest()
    {
        var r = new LastVisitedMRU();
        var reg = TestHives.Corpus(@"ntuserNokiaShellBags.dat");
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedMRU");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(11);
    }

    [Test]
    public void LastVisitedPidlMruTest()
    {
        var r = new LastVisitedPidlMRU();
        var reg = TestHives.Repo("ntuser1.dat");
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRU");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(7);
    }

    [Test]
    public void OpenSaveMRUTest()
    {
        var r = new OpenSaveMRU();
        var reg = TestHives.Corpus(@"NTUSER_Loveall.DAT");
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSaveMRU");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(6);
    }

    [Test]
    public void OpenSavePidlMruTest()
    {
        var r = new OpenSavePidlMRU();
        var reg = TestHives.Repo("ntuser1.dat");
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(16);
    }

    [Test]
    public void SamDBlakeGroups()
    {
        var r = new UserAccounts();

        var reg = TestHives.Corpus(@"SAM_dblake");
        reg.RecoverDeleted = true;
        reg.ParseHive();

        var key = reg.GetKey(@"SAM\Domains\Account\Users");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().BeGreaterThan(0);
        r.Errors.Count.Should().Be(0);


        var users = (BindingList<UserOut>)r.Values;

        var user = users.Single(t => t.UserId == 1001);

        user.UserName.ToLowerInvariant().Should().Be("donald");
    }


    [Test]
    public void SamPluginPWHint()
    {
        var r = new UserAccounts();

        var reg = TestHives.Corpus(@"SAM_hasBigEndianDWord");
        reg.RecoverDeleted = true;
        reg.ParseHive();

        var key = reg.GetKey(@"SAM\Domains\Account\Users");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().BeGreaterThan(0);
        r.Errors.Count.Should().Be(0);

        var u = (UserOut)r.Values[2];

        u.PasswordHint.Should().Be("G");
    }

    [Test]
    public void RecentApps()
    {
        var r = new RecentApps();

        var reg = TestHives.Corpus(@"NTUSER_RecentAppsERZ.DAT");
        reg.RecoverDeleted = true;
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Windows\CurrentVersion\Search\RecentApps");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().BeGreaterThan(0);
        r.Errors.Count.Should().Be(0);

        var u = (RegistryPlugin.RecentApps.ValuesOut)r.Values[2];

        u.AppPath.Should().Contain("chrome.exe");
        u.RecentItems.Count.Should().Be(10);
        u.RecentDocs.Should().Contain("drivepr");
    }


    [Test]
    public void SamPluginShouldFindEricAccount()
    {
        var r = new UserAccounts();

        var reg = TestHives.Repo("SAM");
        reg.ParseHive();

        var key = reg.GetKey(@"SAM\Domains\Account\Users");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().BeGreaterThan(0);
        r.Values.Count.Should().Be(4);

        var users = (BindingList<UserOut>)r.Values;

        var user = users.Single(t => t.UserId == 1000);

        user.UserName.ToLowerInvariant().Should().Be("eric");

        var lastLogin = DateTimeOffset.Parse("3/25/2015 2:31:24 PM +00:00");

        user.LastLoginTime?.ToString("G").Should().Be(lastLogin.ToString("G"));

        user.LastLoginTime?.Year.Should().Be(lastLogin.Year);
        user.LastLoginTime?.Month.Should().Be(lastLogin.Month);
        user.LastLoginTime?.Day.Should().Be(lastLogin.Day);
        user.LastLoginTime?.Hour.Should().Be(lastLogin.Hour);
        user.LastLoginTime?.Minute.Should().Be(lastLogin.Minute);
        user.LastLoginTime?.Second.Should().Be(lastLogin.Second);

        var lastPwChange = DateTimeOffset.Parse("3/23/2015 9:15:55 PM +00:00");

        user.LastPasswordChange?.ToString("G").Should().Be(lastPwChange.ToString("G"));

        user.LastIncorrectPassword.Should().BeNull();
        user.ExpiresOn.Should().BeNull();

        user.TotalLoginCount.Should().Be(3);
        user.InvalidLoginCount.Should().Be(0);

        user = users.Single(t => t.UserId == 500);

        user.UserName.ToLowerInvariant().Should().Be("administrator");
    }

    [Test]
    public void TerminalServers()
    {
        var r = new TerminalServerClient();

        var reg = TestHives.Corpus(@"ALL\NTUSER.DAT");
        reg.ParseHive();

        var key = reg.GetKey(@"Software\Microsoft\Terminal Server Client");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(6);
        r.Errors.Count.Should().Be(0);

        var ff = (RegistryPlugin.TerminalServerClient.ValuesOut)r.Values[0];

        ff.MRUPosition.Should().Be(1);
        ff.HostName.Should().Contain("GOON");

        r = new TerminalServerClient();

        reg = TestHives.Corpus(@"ALL\NTUSER3.DAT");
        reg.ParseHive();

        key = reg.GetKey(@"Software\Microsoft\Terminal Server Client");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(7);
        r.Errors.Count.Should().Be(0);

        ff = (RegistryPlugin.TerminalServerClient.ValuesOut)r.Values[0];

        ff.MRUPosition.Should().Be(-1);
        ff.HostName.Should().Contain("GOON");
        ff.LastModified.Month.Should().Be(5);
        ff.LastModified.Minute.Should().Be(32);
        ff.LastModified.Second.Should().Be(8);
        ff.LastModified.Day.Should().Be(28);


        ff = (RegistryPlugin.TerminalServerClient.ValuesOut)r.Values[3];

        ff.MRUPosition.Should().Be(-1);
        ff.HostName.Should().Contain("SVR01");

        r = new TerminalServerClient();

        reg = TestHives.Corpus(@"NTUSER_dblake.DAT");
        reg.ParseHive();

        key = reg.GetKey(@"Software\Microsoft\Terminal Server Client");

        r.Values.Count.Should().Be(0);

        r.ProcessValues(key);

        r.Values.Count.Should().Be(0);
        r.Errors.Count.Should().Be(0);
    }

    [Test]
    public void SevenZipHistories()
    {
        var reg = TestHives.Corpus(@"NTUSER_MarkElliot_RecentApps.DAT");
        reg.ParseHive();

        var fm = reg.GetKey(@"SOFTWARE\7-Zip\FM");
        var extraction = reg.GetKey(@"SOFTWARE\7-Zip\Extraction");

        var copy = new RegistryPlugin._7_ZipCopyHistory.SevenZipCopyHistory();
        copy.ProcessValues(fm);
        copy.Values.Count.Should().Be(9);
        ((RegistryPlugin._7_ZipCopyHistory.ValuesOut)copy.Values[0]).CopyPath.Should().Be(@"D:\temp\");

        var folder = new RegistryPlugin._7_ZipFolderHistory.SevenZipFolderHistory();
        folder.ProcessValues(fm);
        folder.Values.Count.Should().Be(24);

        var path = new RegistryPlugin._7_ZipPathHistory.SevenZipPathHistory();
        path.ProcessValues(extraction);
        path.Values.Count.Should().Be(2);
        ((RegistryPlugin._7_ZipPathHistory.ValuesOut)path.Values[0]).ExtractionPath.Should().Be(@"D:\VirtualMachines\Kali-Linux-1.1\");

        copy.Errors.Should().BeEmpty();
        folder.Errors.Should().BeEmpty();
        path.Errors.Should().BeEmpty();
    }
}
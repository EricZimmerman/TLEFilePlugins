using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TLEFileEZTools;
using TLEFileMisc;

namespace TLEFile.Text
{
    public class TestMain
    {

        [Test]
    public void EzTools_Evtx()
    {
        var t = new TLEFileEZTools.EvtxECmd();
        t.ProcessFile(@"C:\temp\foo.csv");

//20210202154458_EvtxECmd_Output.csv

    }

    
    [Test]
    public void EzTools_Evtx2()
    {
        var t = new TLEFileEZTools.SbeCmd();
        t.ProcessFile(@"C:\Users\eric\Desktop\2021-07-21115457_ShellBagsExplorerExport.csv");

    }


    [Test]
    public void EzTools_Evtx3()
    {
        var t = new TLEFileEZTools.EvtxECmd();
        t.ProcessFile(@"C:\temp\20210202155341_EvtxECmd_Output.csv");

    }

    [Test]
    public void EzTools_Evtx4()
    {
        var t = new TLEFileEZTools.EvtxECmd();
        t.ProcessFile(@"C:\temp\foo2.csv");

    }


    [Test]
    public void EzTools_KapeTL()
    {
        var t = new TLEFileTimelines.KapeMiniTimeline();
        t.ProcessFile(@"C:\Temp\minitimeline.csv");

    }

    [Test]
    public void GenericTest()
    {
        var t = new TLEFileGenericCsv.GenericCsv();
        t.ProcessFile(@"C:\Temp\minitimeline.csv");

    }

    [Test]
    public void BrowserHistView()
    {
        var t = new J();
        t.ProcessFile(@"C:\temp\20210928133849_MFTECmd_$J_Output.csv");

    }

    
    }
}

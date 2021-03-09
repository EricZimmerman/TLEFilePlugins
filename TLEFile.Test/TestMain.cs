using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

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
        var t = new TLEFileEZTools.EvtxECmd();
        t.ProcessFile(@"C:\temp\20210204123845_EvtxECmd_Output.csv");

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

    
    }
}

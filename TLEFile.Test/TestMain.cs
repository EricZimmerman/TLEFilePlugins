﻿using System;
using System.Diagnostics;
using NUnit.Framework;
using Serilog;
using TLEFileEZTools;
using TLEFileMisc;
using TLEFileTimelines;
using TLEFileGenericCsv;

namespace TLEFile.Test
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
    public void EzTools_SbeCmd()
    {
        var t = new TLEFileEZTools.SbeCmd();
        t.ProcessFile(@"C:\temp\testdata\M__Forensics_TrainingImages_AliHadi_Challenge5_tout_E_Users_Joker_AppData_Local_Microsoft_Windows_UsrClass.dat.csv");
        
        Debug.WriteLine(t.DataList.Count);

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
        t.ProcessFile(@"C:\temp\Large_RecordNumber-EventRecordId.csv");

    }


    [Test]
    public void EzTools_KapeTL()
    {
        var t = new TLEFileTimelines.KapeMiniTimeline();
        t.ProcessFile(@"C:\Temp\minitimeline.csv");

    }
    
    [Test]
    public void HyabMin()
    {
        var t = new HayabusaMinimal();
        t.ProcessFile(@"C:\temp\hayabusa__minimal.csv");

    }
    
    [Test]
    public void HyabStd()
    {
        var t = new HayabusaStandard();
        t.ProcessFile(@"C:\temp\hayabusa_standard.csv");

    }
    
    [Test]
    public void HyabVer()
    {
        var t = new HayabusaVerbose();
        t.ProcessFile(@"C:\temp\hayabusa-results\hayabusa-results.csv");

    }
    
    [Test]
    public void HyabSuperVer()
    {
        var t = new HayabusaSuperVerbose();
        t.ProcessFile(@"C:\temp\hayabusa_super_verbose.csv");
        
        var t2 = new HayabusaSuperVerbose();
        t2.ProcessFile(@"C:\temp\sample-hayabusa-results.csv");

        

    }
    
    [Test]
    public void FTTest()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Debug()
            .WriteTo.Console()
                
            .CreateLogger();

        
        
        var t = new ForensicTimeline();
        t.ProcessFile(@"C:\temp\20250416_235546_forensic_timeliner.csv");
        Log.Information("Count: {C}",t.DataList.Count);

    }
    
    
    [Test]
    public void GenTest()
    {
        var t = new GenericCsv();
        t.ProcessFile(@"C:\temp\20240904144034_SumECmd_DETAIL_DnsInfo_Output.csv");
        

    }


        //PsortTimeline

        
        [Test]
        public void PsortTimelineTest()
        {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug()
                .WriteTo.Console()
                
                .CreateLogger();

            Console.WriteLine(1);
            Debug.WriteLine("Test 1");
            var t = new TLEFileTimelines.PsortTimeline();
            t.ProcessFile(@"C:\temp\Testing-TLE-for-608\SmallTest.csv");

            Console.WriteLine(2);
            t = new PsortTimeline();
            t.ProcessFile(@"C:\temp\Testing-TLE-for-608\base-av-log2timeline.csv");

            Console.WriteLine(3);
            t = new PsortTimeline();
            t.ProcessFile(@"C:\temp\Testing-TLE-for-608\base-rd-01-log2timeline.csv");

            
            
        }

    [Test]
    public void GenericTest()
    {
        var t = new TLEFileGenericCsv.GenericCsv();
        t.ProcessFile(@"C:\temp\Test.csv");

    }

    [Test]
    public void BrowserHistView()
    {
        var t = new J();
        t.ProcessFile(@"C:\temp\20210928133849_MFTECmd_$J_Output.csv");

    }

    
    }
}

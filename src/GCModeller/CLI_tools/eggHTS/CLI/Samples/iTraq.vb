#Region "Microsoft.VisualBasic::974ffde21fe6b08796f29ecd08a396c5, CLI_tools\eggHTS\CLI\Samples\iTraq.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:

' Module CLI
' 
'     Function: iTraqAnalysisMatrixSplit, iTraqBridge, iTraqRSDPvalueDensityPlot, iTraqSignReplacement, iTraqTtest
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.DATA
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.Office.Excel.Extensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports Xlsx = Microsoft.VisualBasic.MIME.Office.Excel.File

Partial Module CLI

    ''' <summary>
    ''' 处理iTraq实验搭桥结果数据
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/iTraq.Bridge.Matrix")>
    <Usage("/iTraq.Bridge.Matrix /A <A_iTraq.csv> /B <B_iTraq.csv> /C <bridge_symbol> [/symbols.A <symbols.csv> /symbols.B <symbols.csv> /out <matrix.csv>]")>
    <Group(CLIGroups.iTraqTool)>
    Public Function iTraqBridge(args As CommandLine) As Integer
        Dim A$ = args("/A")
        Dim B$ = args("/B")
        Dim C$ = args("/C")
        Dim out$ = args("/out") Or $"{A.TrimSuffix}_{B.BaseName},bridge={C}.csv"
        Dim symbolsA = args("/symbols.A").LoadCsv(Of iTraqSymbols).ToArray
        Dim symbolsB = args("/symbols.B").LoadCsv(Of iTraqSymbols)

        ' 首先合并为一个matrix，之后再做符号替换
        Dim ALL = iTraqSample.BridgeCombine(A.LoadCsv(Of iTraqReader), B.LoadCsv(Of iTraqReader), C)
        Dim symbols = symbolsA.TagWith("A") + symbolsB.TagWith("B")

        Call ALL.SaveTo(out)

        If symbols <> 0 Then
            With ALL
                Call .iTraqMatrix(symbols) _
                     .ToArray _
                     .SaveTo(out.TrimSuffix & ".matrix.csv")
                Call .SymbolReplace(symbols) _
                     .ToArray _
                     .SaveTo(out.TrimSuffix & $".sample.csv")
            End With
        End If

        Return 0
    End Function

    <ExportAPI("/iTraq.Symbol.Replacement")>
    <Description("* Using this CLI tool for processing the tag header of iTraq result at first.")>
    <Usage("/iTraq.Symbol.Replacement /in <iTraq.data.csv/xlsx> /symbols <symbols.csv/xlsx> [/sheet.name <Sheet1> /symbolSheet <sheetName> /out <out.DIR>]")>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.csv, *.xlsx",
              AcceptTypes:={GetType(iTraqReader)},
              Description:="")>
    <ArgumentAttribute("/symbols", False, CLITypes.File,
              Extensions:="*.csv, *.xlsx",
              AcceptTypes:={GetType(iTraqSymbols)},
              Description:="Using for replace the mass spectrum expeirment symbol to the user experiment tag.")>
    <Group(CLIGroups.iTraqTool)>
    Public Function iTraqSignReplacement(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].ParentPath)
        Dim symbols As iTraqSymbols()
        Dim input As Value(Of String) = ""

        With (args <= "/symbols")
            If .ExtensionSuffix.TextEquals("csv") Then
                symbols = .LoadCsv(Of iTraqSymbols)
            Else
                symbols = Xlsx.Open(.ByRef) _
                    .GetTable(args("/symbolSheet") Or Sheet1) _
                    .AsDataSource(Of iTraqSymbols) _
                    .ToArray
            End If
        End With

        If [in].ExtensionSuffix.TextEquals("csv") Then
            input = [in]
        Else
            Dim sheet$ = args <= "/sheet.Name"

            Call Xlsx.Open([in]) _
                .GetTable(sheet Or "Sheet1".AsDefault) _
                .Save(input = TempFileSystem.GetAppSysTempFile(, App.PID), encoding:=Encodings.UTF8)
        End If

        With [input].Value.LoadCsv(Of iTraqReader)
            Call .iTraqMatrix(symbols) _
                 .ToArray _
                 .SaveTo(out & "/matrix.csv")
            Call .SymbolReplace(symbols) _
                 .ToArray _
                 .SaveTo(out & $"/{[in].BaseName}.sample.csv")
        End With

        Return 0
    End Function

    ''' <summary>
    ''' 根据实验设计的信息进行矩阵的分组
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/iTraq.matrix.split")>
    <Description("Split the raw matrix into different compare group based on the experimental designer information.")>
    <Usage("/iTraq.matrix.split /in <matrix.csv> /sampleInfo <sampleInfo.csv> /designer <analysis.design.csv> [/allowed.swap /out <out.Dir>]")>
    <Group(CLIGroups.iTraqTool)>
    <ArgumentAttribute("/sampleInfo", False, CLITypes.File, AcceptTypes:={GetType(SampleInfo)})>
    <ArgumentAttribute("/designer", False, CLITypes.File, AcceptTypes:={GetType(AnalysisDesigner)},
              Description:="The analysis designer in csv file format for the DEPs calculation, should contains at least two column: ``<Controls>,<Experimental>``. 
              The analysis design: ``controls vs experimental`` means formula ``experimental/controls`` in the FoldChange calculation.")>
    Public Function iTraqAnalysisMatrixSplit(args As CommandLine) As Integer
        Dim sampleInfo = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim designer = (args <= "/designer").LoadCsv(Of AnalysisDesigner)
        Dim out$ = args.GetValue("/out", (args <= "/in").TrimSuffix & "-Groups/")
        Dim matrix As DataSet() = DataSet.LoadDataSet(args <= "/in").ToArray
        Dim allowedSwap As Boolean = args.IsTrue("/allowed.swap")

        Call $"Matrix have {matrix.Length} proteins".__INFO_ECHO

        For Each group In matrix.MatrixSplit(sampleInfo, designer, allowedSwap)
            Dim groupName$ = AnalysisDesigner.CreateTitle(group.name)
            Dim path$ = out & $"/{groupName.NormalizePathString(False)}.csv"
            Dim data As DataSet() = group.value

            Call $"{groupName} -> {data.Length} proteins...".__DEBUG_ECHO

            If Not data.All(Function(x) x.Properties.Count = 0) Then
                Call data _
                    .InvalidsAsRLangNA("NA") _
                    .ToArray _
                    .SaveTo(path)
            Else
                Call $"``{groupName}`` have no values, please check for the labels...".Warning
            End If
        Next

        Return 0
    End Function

    ''' <summary>
    ''' 可视化样本的一致重复性
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/iTraq.RSD-P.Density")>
    <Usage("/iTraq.RSD-P.Density /in <matrix.csv> [/out <out.png>]")>
    <Description("Visualize data QC analysis result.")>
    <ArgumentAttribute("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.csv",
              Description:="A data matrix which is comes from the ``/iTraq.matrix.split`` command.")>
    <ArgumentAttribute("/out", True, CLITypes.File,
              Extensions:="*.png, *.svg",
              Description:="The file path of the plot result image.")>
    <Group(CLIGroups.iTraqTool)>
    Public Function iTraqRSDPvalueDensityPlot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = (args <= "/out") Or $"{[in].TrimSuffix}.RSD-P.density.png".AsDefault
        Dim matrix As DataSet() = DataSet.LoadDataSet([in]).ToArray
        Dim n% = matrix.PropertyNames.Distinct.Count

        Return matrix _
            .RSDP(n) _
            .RSDPdensity(padding:="padding:100px 80px 150px 200px;") _
            .Save(out) _
            .CLICode
    End Function

    <ExportAPI("/iTraq.t.test")>
    <Usage("/iTraq.t.test /in <matrix.csv> [/level <default=1.5> /p.value <default=0.05> /FDR <default=0.05> /skip.significant.test /pairInfo <sampleTuple.csv> /out <out.csv>]")>
    <Description("Implements the screening for different expression proteins by using log2FC threshold and t.test pvalue threshold.")>
    <Group(CLIGroups.iTraqTool)>
    <ArgumentAttribute("/FDR", True, CLITypes.Double,
              Description:="do FDR adjust on the p.value result? If this argument value is set to 1, means no adjustment.")>
    <ArgumentAttribute("/skip.significant.test", True,
              CLITypes.Boolean,
              AcceptTypes:={GetType(Boolean)},
              Description:="If this option is presented in the CLI input, then the significant test from the p.value and FDR will be disabled.")>
    Public Function iTraqTtest(args As CommandLine) As Integer
        Dim data As DataSet() = DataSet.LoadDataSet(args <= "/in").ToArray
        Dim level# = args.GetValue("/level", 1.5)
        Dim pvalue# = args.GetValue("/p.value", 0.05)
        Dim FDR# = args.GetValue("/FDR", 0.05)
        Dim pairInfo$ = args <= "/pairInfo"
        Dim out$
        Dim sst As Boolean = args.IsTrue("/skip.significant.test")

        If pairInfo.FileExists Then
            out$ = (args <= "/out") Or $"{(args <= "/in").TrimSuffix}.log2FC.paired.t-test.csv".AsDefault
        Else
            If sst Then
                out$ = (args <= "/out") Or $"{(args <= "/in").TrimSuffix}.log2FC.csv".AsDefault
            Else
                out$ = (args <= "/out") Or $"{(args <= "/in").TrimSuffix}.log2FC.t.test.csv".AsDefault
            End If
        End If

        If Not sst Then
            Dim DEPs As DEP_iTraq() = data.logFCtest(
                level, pvalue, FDR,
                pairInfo:=pairInfo.LoadCsv(Of SampleTuple))

            Return DEPs _
                .Where(Function(x) x.log2FC <> 0R) _
                .ToArray _
                .SaveDataSet(out) _
                .CLICode
        Else
            Return data.log2Test(level) _
                .Where(Function(x) x.log2FC <> 0) _
                .ToArray _
                .SaveDataSet(out, Encodings.UTF8) _
                .CLICode
        End If
    End Function
End Module

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports Xlsx = Microsoft.VisualBasic.MIME.Office.Excel.File

Partial Module CLI

    <ExportAPI("/iTraq.Symbol.Replacement")>
    <Description("* Using this CLI tool for processing the tag header of iTraq result at first.")>
    <Usage("/iTraq.Symbol.Replacement /in <iTraq.data.csv/xlsx> /symbols <symbols.csv> [/sheet.name <Sheet1> /out <out.DIR>]")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.csv, *.xlsx",
              AcceptTypes:={GetType(iTraqReader)},
              Description:="")>
    <Argument("/symbols", False, CLITypes.File,
              AcceptTypes:={GetType(iTraqSymbols)},
              Description:="Using for replace the mass spectrum expeirment symbol to the user experiment tag.")>
    <Group(CLIGroups.iTraqTool)>
    Public Function iTraqSignReplacement(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].ParentPath)
        Dim symbols = (args <= "/symbols").LoadCsv(Of iTraqSymbols)
        Dim input$

        If [in].ExtensionSuffix.TextEquals("csv") Then
            input = [in]
        Else
            input = App.GetAppSysTempFile(".csv", App.PID)

            Dim sheet$ = args <= "/sheet.Name"

            Call Xlsx.Open([in]) _
                .GetTable(sheet Or "Sheet1".AsDefault) _
                .Save(input, encoding:=Encodings.UTF8)
        End If

        With [input].LoadCsv(Of iTraqReader)
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
    <Argument("/sampleInfo", False, CLITypes.File, AcceptTypes:={GetType(SampleInfo)})>
    <Argument("/designer", False, CLITypes.File, AcceptTypes:={GetType(AnalysisDesigner)},
              Description:="The analysis designer in csv file format for the DEPs calculation, should contains at least two column: ``<Controls>,<Experimental>``. 
              The analysis design: ``controls vs experimental`` means formula ``experimental/controls`` in the FoldChange calculation.")>
    Public Function iTraqAnalysisMatrixSplit(args As CommandLine) As Integer
        Dim sampleInfo = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim designer = (args <= "/designer").LoadCsv(Of AnalysisDesigner)
        Dim out$ = args.GetValue("/out", (args <= "/in").TrimSuffix & "-Groups/")
        Dim matrix As DataSet() = DataSet.LoadDataSet(args <= "/in").ToArray
        Dim allowedSwap As Boolean = args.IsTrue("/allowed.swap")

        For Each group In matrix.MatrixSplit(sampleInfo, designer, allowedSwap)
            Dim groupName$ = AnalysisDesigner.CreateTitle(group.Name)
            Dim path$ = out & $"/{groupName.NormalizePathString(False)}.csv"
            Dim data As DataSet() = group.Value

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
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              Extensions:="*.csv",
              Description:="A data matrix which is comes from the ``/iTraq.matrix.split`` command.")>
    <Group(CLIGroups.iTraqTool)>
    Public Function iTraqRSDPvalueDensityPlot(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = (args <= "/out") Or $"{[in].TrimSuffix}.RSD-P.density.png".AsDefault
        Dim matrix As DataSet() = DataSet.LoadDataSet([in]).ToArray
        Dim n% = matrix.PropertyNames.Distinct.Count

        Return matrix _
            .RSDP(n) _
            .RSDPdensity() _
            .Save(out) _
            .CLICode
    End Function

    <ExportAPI("/iTraq.t.test")>
    <Usage("/iTraq.t.test /in <matrix.csv> [/level <default=1.5> /p.value <default=0.05> /FDR <default=0.05> /skip.significant.test /pairInfo <sampleTuple.csv> /out <out.csv>]")>
    <Group(CLIGroups.iTraqTool)>
    <Argument("/FDR", True, CLITypes.Double,
              Description:="do FDR adjust on the p.value result? If this argument value is set to 1, means no adjustment.")>
    <Argument("/skip.significant.test", True,
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
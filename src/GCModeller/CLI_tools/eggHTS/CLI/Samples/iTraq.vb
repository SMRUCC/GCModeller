Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.HTS.Proteomics
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Partial Module CLI

    <ExportAPI("/iTraq.Symbol.Replacement")>
    <Usage("/iTraq.Symbol.Replacement /in <iTraq.data.csv> /symbols <symbols.csv> [/out <out.DIR>]")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(iTraqReader)},
              Description:="")>
    <Argument("/symbols", False, CLITypes.File,
              AcceptTypes:={GetType(iTraqSymbols)},
              Description:="Using for replace the mass spectrum expeirment symbol to the user experiment tag.")>
    Public Function iTraqSignReplacement(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args.GetValue("/out", [in].ParentPath)
        Dim symbols = (args <= "/symbols").LoadCsv(Of iTraqSymbols)

        With [in].LoadCsv(Of iTraqReader)
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
    <Usage("/iTraq.matrix.split /in <matrix.csv> /sampleInfo <sampleInfo.csv> /designer <analysis.design.csv> [/out <out.Dir>]")>
    Public Function iTraqAnalysisMatrixSplit(args As CommandLine) As Integer
        Dim sampleInfo = (args <= "/sampleInfo").LoadCsv(Of SampleInfo)
        Dim designer = (args <= "/designer").LoadCsv(Of ExperimentAnalysis)
        Dim out$ = args.GetValue("/out", (args <= "/in").TrimSuffix & "-Groups/")
        Dim matrix As DataSet() = DataSet.LoadDataSet(args <= "/in").ToArray

        With sampleInfo.DataAnalysisDesign(analysis:=designer)

            For Each group In .ref
                Dim groupName$ = group.Key
                Dim labels = group.Value
                Dim data = matrix _
                    .Select(Function(x)
                                Dim values As New List(Of KeyValuePair(Of String, Double))

                                For Each label In labels
                                    With label.ToString
                                        If x.HasProperty(.ref) Then
                                            Call values.Add(.ref, x(.ref))
                                        Else
                                            ' 可能是在进行质谱实验的时候将顺序颠倒了，在这里将标签颠倒一下试试
                                            With label.Swap.ToString
                                                If x.HasProperty(.ref) Then
                                                    ' 由于在取出值之后使用1除来进行翻转，所以在这里标签还是用原来的顺序，不需要进行颠倒了
                                                    values.Add(label.ToString, 1 / x(.ref))
                                                End If
                                            End With
                                        End If
                                    End With
                                Next

                                Return New DataSet With {
                                    .ID = x.ID,
                                    .Properties = values _
                                        .OrderBy(Function(d) d.Key) _
                                        .ToDictionary()
                                }
                            End Function) _
                    .ToArray
                Dim path$ = out & $"/{groupName.NormalizePathString(False)}.csv"

                If Not data.All(Function(x) x.Properties.Count = 0) Then
                    Call data.SaveTo(path)
                    Call StripNaN(path, replaceAs:="NA")
                End If
            Next
        End With

        Return 0
    End Function

    <ExportAPI("/iTraq.t.test")>
    <Usage("/iTraq.t.test /in <matrix.csv> [/level <default=1.5> /p.value <default=0.05> /FDR <default=0.05> /out <out.csv>]")>
    Public Function iTraqTtest(args As CommandLine) As Integer
        Dim data As DataSet() = DataSet.LoadDataSet(args <= "/in").ToArray
        Dim level# = args.GetValue("/level", 1.5)
        Dim pvalue# = args.GetValue("/p.value", 0.05)
        Dim FDR# = args.GetValue("/FDR", 0.05)
        Dim out$ = args.GetValue("/out", (args <= "/in").TrimSuffix & ".log2FC.t.test.csv")
        Dim DEPs As DEP_iTraq() = data.logFCtest(level, pvalue, FDR)

        Return DEPs _
            .Where(Function(x) x.log2FC <> 0R) _
            .ToArray _
            .SaveDataSet(out) _
            .CLICode
    End Function
End Module
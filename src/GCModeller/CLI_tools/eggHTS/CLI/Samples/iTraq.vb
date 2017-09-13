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
        Dim matrix As DataSet() = DataSet.LoadDataSet(args <= "/in")

        With sampleInfo.DataAnalysisDesign(analysis:=designer)

            For Each group In .ref
                Dim groupName = group.Key
                Dim labels$() = group _
                    .Value _
                    .Select(Function(x) x.ToString) _
                    .ToArray

                Dim data = matrix _
                    .Select(Function(x) x.SubSet(labels)) _
                    .ToArray
                Dim path$ = out & $"/{groupName.NormalizePathString(False)}.csv"

                Call data.SaveTo(path)
            Next
        End With

        Return 0
    End Function
End Module
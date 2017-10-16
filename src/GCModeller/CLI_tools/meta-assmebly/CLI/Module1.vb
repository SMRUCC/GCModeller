Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Partial Module CLI

    <ExportAPI("/significant.difference")>
    <Usage("/significant.difference /in <data.csv> /groups <sampleInfo.csv> [/out <out.csv.DIR>]")>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(DataSet)},
              Extensions:="*.csv",
              Description:="A matrix file that contains the sample data.")>
    <Argument("/groups", False, CLITypes.File,
              AcceptTypes:={GetType(SampleInfo)},
              Extensions:="*.csv",
              Description:="")>
    Public Function SignificantDifference(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim group$ = args <= "/groups"
        Dim out$ = (args <= "/out") Or $"{in$.TrimSuffix}_{group.BaseName}.significant.difference/".AsDefault
        Dim data As DataSet() = DataSet.LoadDataSet([in])
        Dim samples As NamedCollection(Of SampleInfo)() = group _
            .LoadCsv(Of SampleInfo) _
            .GroupBy(Function(s) s.sample_group) _
            .Select(Function(g)
                        Return New NamedCollection(Of SampleInfo) With {
                            .Name = g.Key,
                            .Value = g.ToArray
                        }
                    End Function) _
            .ToArray

        For Each ga In samples
            For Each gb In samples.Where(Function(g) g.Name <> ga.Name)

            Next
        Next

        Return 0
    End Function
End Module
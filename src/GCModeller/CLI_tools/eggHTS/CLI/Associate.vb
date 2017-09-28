Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS

Partial Module CLI

    <ExportAPI("/Network.PCC")>
    <Usage("/Network.PCC /in <matrix.csv> [/cut <default=0.45> /out <out.DIR>]")>
    Public Function PccNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim cut# = args.GetValue("/cut", 0.45)
        Dim out$ = (args <= "/out") Or ([in].TrimSuffix & ".PCC/").AsDefault
        Dim matrix As DataSet() = DataSet.LoadDataSet([in]).ToArray

        With CorrelationNetwork.BuildNetwork(matrix, cut)
            Call .matrix.SaveTo(out & "/matrix.csv")
            Return .net.Save(out).CLICode
        End With
    End Function

    <ExportAPI("/KOBAS.similarity")>
    <Usage("/KOBAS.Similarity /group1 <DIR> /group2 <DIR> [/fileName <default=output_run-Gene Ontology.csv> /out <out.DIR>]")>
    Public Function KOBASSimilarity(args As CommandLine) As Integer
        Dim group1$ = args <= "/group1"
        Dim group2$ = args <= "/group2"
        Dim fileName$ = args.GetValue("/fileName", "output_run-Gene Ontology.csv")
        Dim out$ = args.GetValue("/out", group1.TrimDIR & "-" & group2.BaseName & $".{fileName}/")

        Dim files1 = (ls - l - r - fileName <= group1) _
            .Select(Function(file)
                        Return file.LoadTerms(file.ParentPath.ParentDirName)
                    End Function) _
            .ToArray
        Dim files2 = (ls - l - r - fileName <= group2) _
            .Select(Function(file)
                        Return file.LoadTerms(file.ParentPath.ParentDirName)
                    End Function) _
            .ToArray

        Dim matrixA As New Dictionary(Of DataSet)
        Dim matrixB As New Dictionary(Of DataSet)
        Dim Sim As New List(Of DataSet)

        For Each a In files1
            Dim vector As New DataSet With {
                .ID = $"[{group1.BaseName}] " & a.Name,
                .Properties = New Dictionary(Of String, Double)
            }

            For Each b In files2
                With Measure.Similarity(a, b)

                    .A.ID = $"[{group1.BaseName}] " & .A.ID
                    .B.ID = $"[{group2.BaseName}] " & .B.ID

                    If Not matrixA.ContainsKey(.A.ID) Then
                        matrixA += .A
                    End If
                    If Not matrixB.ContainsKey(.B.ID) Then
                        matrixB += .B
                    End If

                    vector.Properties.Add($"[{group2.BaseName}] " & b.Name, .similarity)
                End With
            Next

            Sim += vector
        Next

        Call matrixA.SaveTo(out & $"/{group1.BaseName}.csv")
        Call matrixB.SaveTo(out & $"/{group2.BaseName}.csv")
        Call Sim.SaveTo(out & $"/SimOf-{fileName}")

        Return 0
    End Function
End Module
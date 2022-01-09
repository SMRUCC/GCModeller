Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSVA
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.genomics.Visualize
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports HTSMatrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("GSVA")>
Module GSVA

    <ExportAPI("gsva")>
    <RApiReturn(GetType(HTSMatrix))>
    Public Function gsva(expr As Object, geneSet As Object, Optional env As Environment = Nothing) As Object
        Dim mat As HTSMatrix
        Dim background As Background

        If TypeOf expr Is dataframe Then
            mat = New HTSMatrix With {
                .sampleID = DirectCast(expr, dataframe).colnames,
                .expression = DirectCast(expr, dataframe) _
                    .forEachRow _
                    .Select(Function(r)
                                Return New DataFrameRow With {
                                    .geneID = r.name,
                                    .experiments = REnv.asVector(Of Double)(r.ToArray)
                                }
                            End Function) _
                    .ToArray
            }
        Else
            mat = expr
        End If

        If TypeOf geneSet Is list Then
            background = New Background With {
                .clusters = DirectCast(geneSet, list).slots _
                    .Select(Function(c)
                                Return New Cluster With {
                                    .ID = c.Key,
                                    .description = c.Key,
                                    .names = c.Key,
                                    .members = DirectCast(REnv.asVector(Of String)(c.Value), String()) _
                                        .Select(AddressOf createGene) _
                                        .ToArray
                                }
                            End Function) _
                    .ToArray
            }
        Else
            background = geneSet
        End If

        Return mat.gsva(background)
    End Function

    <ExportAPI("diff")>
    Public Function diff(gsva As HTSMatrix, compares As DataAnalysis) As GSVADiff()
        If compares.size <> 2 Then
            Throw New InvalidProgramException
        End If

        Dim i1 As Integer() = gsva.IndexOf(compares.experiment)
        Dim i2 As Integer() = gsva.IndexOf(compares.control)

        Return gsva.expression _
            .Select(Function(expr)
                        Dim test As TwoSampleResult = t.Test(expr(i1), expr(i2))

                        Return New GSVADiff With {
                            .pathName = expr.geneID,
                            .t = test.TestValue,
                            .pvalue = test.Pvalue
                        }
                    End Function) _
            .ToArray
    End Function

    Private Function createGene(name As String) As BackgroundGene
        Return New BackgroundGene With {
            .accessionID = name,
            .name = name,
            .[alias] = {name},
            .locus_tag = New NamedValue With {
                .name = name,
                .text = name
            },
            .term_id = {name}
        }
    End Function
End Module

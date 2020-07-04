
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports Vec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

<Package("geneExpression")>
Module geneExpression

    Sub New()

    End Sub

    ''' <summary>
    ''' load an expressin matrix data
    ''' </summary>
    ''' <param name="file$"></param>
    ''' <param name="exclude_samples"></param>
    ''' <returns></returns>
    <ExportAPI("load.expr")>
    Public Function loadExpression(file$, Optional exclude_samples As String() = Nothing) As Matrix
        Return Matrix.LoadData(file, If(exclude_samples Is Nothing, Nothing, New Index(Of String)(exclude_samples)))
    End Function

    <ExportAPI("average")>
    Public Function average(matrix As Matrix, sampleinfo As SampleInfo()) As Matrix
        Return Matrix.MatrixAverage(matrix, sampleinfo)
    End Function

    <ExportAPI("relative")>
    Public Function relative(matrix As Matrix) As Matrix
        Return New Matrix With {
            .sampleID = matrix.sampleID,
            .expression = matrix.expression _
                .Select(Function(gene)
                            Return New DataFrameRow With {
                                .geneID = gene.geneID,
                                .experiments = New Vec(gene.experiments) / gene.experiments.Max
                            }
                        End Function) _
                .ToArray
        }
    End Function
End Module

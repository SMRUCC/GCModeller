Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Class Matrix

    ''' <summary>
    ''' sample id of <see cref="DataFrameRow.experiments"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property sampleID As String()

    ''' <summary>
    ''' gene list
    ''' </summary>
    ''' <returns></returns>
    Public Property expression As DataFrameRow()

    Public Shared Iterator Function TakeSamples(data As DataFrameRow(), sampleVector As Integer(), reversed As Boolean) As IEnumerable(Of DataFrameRow)
        Dim samples As Double()

        For Each x As DataFrameRow In data
            samples = x.experiments.Takes(sampleVector, reversed:=reversed)

            Yield New DataFrameRow With {
                .geneID = x.geneID,
                .experiments = samples
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadData(file As String, Optional excludes As Index(Of String) = Nothing) As Matrix
        Return Document.LoadMatrixDocument(file, excludes)
    End Function

    ''' <summary>
    ''' calculate average value of the gene expression for
    ''' each sample group.
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="sampleInfo"></param>
    ''' <returns></returns>
    Public Shared Function MatrixAverage(matrix As Matrix, sampleInfo As SampleInfo()) As Matrix
        Dim sampleIndex As Index(Of String) = matrix.sampleID
        Dim groups As NamedCollection(Of Integer)() = sampleInfo _
            .GroupBy(Function(a) a.sample_info) _
            .Select(Function(g)
                        Return New NamedCollection(Of Integer) With {
                            .name = g.Key,
                            .value = g _
                                .Select(Function(sample)
                                            Return sampleIndex.IndexOf(sample.sample_name)
                                        End Function) _
                                .ToArray
                        }
                    End Function) _
            .ToArray
        Dim genes As DataFrameRow() = matrix.expression _
            .Select(Function(g)
                        Dim mean As Double() = groups _
                            .Select(Function(group)
                                        Return Aggregate index As Integer
                                               In group
                                               Let x As Double = g.experiments(index)
                                               Into Average(x)
                                    End Function) _
                            .ToArray

                        Return New DataFrameRow With {
                            .geneID = g.geneID,
                            .experiments = mean
                        }
                    End Function) _
            .ToArray

        Return New Matrix With {
            .sampleID = groups.Keys,
            .expression = genes
        }
    End Function
End Class



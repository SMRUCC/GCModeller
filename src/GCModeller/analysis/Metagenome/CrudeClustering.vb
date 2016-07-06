Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Parallel.Linq

Public Module CrudeClustering

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="s"></param>
    ''' <param name="kmax"></param>
    ''' <param name="n">原始输入的序列的总数</param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function RandomClustering(s As IEnumerable(Of I3merVector), kmax As Integer, n As Integer) As IEnumerable(Of Cluster)
        For Each x As I3merVector() In TaskPartitions.SplitIterator(s, n / kmax)
            Yield New Cluster With {
                .members = New List(Of I3merVector)(x)
            }
        Next
    End Function


End Module

Public Class I3merVector : Inherits ClassObject
    Public Property Name As String
    Public Property Vector As Dictionary(Of I3Mers, Integer)
End Class

Public Class Cluster

    Public Property members As List(Of I3merVector)

    Public Function PartitionProbability() As Double
        Return members.PartitionProbability
    End Function
End Class
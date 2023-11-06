Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory.KdTree
Imports Microsoft.VisualBasic.DataMining.KMeans

Public Class KNNGraph

    ReadOnly tree As KdTree(Of ClusterEntity)

    Sub New(data As IEnumerable(Of ClusterEntity))

    End Sub

    Private Class NodeVisits : Inherits KdNodeAccessor(Of ClusterEntity)

        ReadOnly dims As Dictionary(Of String, Integer)

        ''' <summary>
        ''' construct a data accessor with the dimension mapping
        ''' </summary>
        ''' <param name="dims"></param>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <DebuggerStepThrough>
        Sub New(dims As Dictionary(Of String, Integer))
            Me.dims = dims
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Sub setByDimensin(x As ClusterEntity, dimName As String, value As Double)
            x.entityVector(dims(dimName)) = value
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function GetDimensions() As String()
            Return dims.Keys.ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function metric(a As ClusterEntity, b As ClusterEntity) As Double
            Return a.e
        End Function

        Public Overrides Function getByDimension(x As ClusterEntity, dimName As String) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function nodeIs(a As ClusterEntity, b As ClusterEntity) As Boolean
            Throw New NotImplementedException()
        End Function

        Public Overrides Function activate() As ClusterEntity
            Throw New NotImplementedException()
        End Function
    End Class

End Class

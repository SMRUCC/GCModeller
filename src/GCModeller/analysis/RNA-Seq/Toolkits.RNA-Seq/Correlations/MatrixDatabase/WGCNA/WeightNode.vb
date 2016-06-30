Imports Microsoft.VisualBasic.DataVisualization.Network.Abstract
Imports Microsoft.VisualBasic.DocumentFormat
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace WGCNA

    ''' <summary>
    ''' CytoscapeEdges
    ''' </summary>
    Public Class Weight : Implements IInteraction

        <Column("fromNode")>
        Public Property FromNode As String Implements IInteraction.source
        <Column("toNode")>
        Public Property ToNode As String Implements IInteraction.target
        <Column("weight")>
        Public Property Weight As Double
        Public Property direction As String
        Public Property fromAltName As String
        Public Property toAltName As String

        Public Overrides Function ToString() As String
            Return $"{FromNode} <--> {ToNode};  {Weight}"
        End Function

        ''' <summary>
        ''' Gets the opposite gene in this interaction relationship
        ''' </summary>
        ''' <param name="Regulator"></param>
        ''' <returns></returns>
        Public Function GetOpposite(Regulator As String) As String
            If String.Equals(Regulator, FromNode) Then
                Return ToNode
            ElseIf String.Equals(Regulator, ToNode) Then
                Return FromNode
            Else
                Return ""
            End If
        End Function

        Public Shared Function Find(Id1 As String, Id2 As String, source As Weight()) As Weight
            Dim LQuery = (From wNode As Weight In source.AsParallel
                          Where (String.Equals(Id1, wNode.FromNode) AndAlso
                          String.Equals(Id2, wNode.ToNode)) OrElse
                          (String.Equals(Id2, wNode.FromNode) AndAlso String.Equals(Id1, wNode.ToNode))
                          Select wNode).FirstOrDefault
            Return LQuery
        End Function

        Public Shared Function Find(Id As String, source As Weight()) As Weight()
            Dim LQuery = (From item As Weight
                          In source.AsParallel
                          Where String.Equals(item.FromNode, Id) OrElse
                          String.Equals(item.ToNode, Id)
                          Select item).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
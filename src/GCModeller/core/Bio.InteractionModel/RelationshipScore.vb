Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph

''' <summary>
''' 带有分值的互做关系
''' </summary>
Public Class RelationshipScore
    Implements IInteraction
    Implements INetworkEdge

    Public Property Type As String Implements INetworkEdge.Interaction

    ''' <summary>
    ''' 通常为Regulator
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property InteractorA As String Implements IInteraction.source
    ''' <summary>
    ''' 通常为目标调控对象
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property InteractorB As String Implements IInteraction.target
    Public Property Score As Double Implements INetworkEdge.value

    Public Function GetConnectedId(Id As String) As String
        If String.Equals(InteractorA, Id) Then
            Return InteractorB
        ElseIf String.Equals(InteractorB, Id) Then
            Return InteractorA
        Else
            Return ""
        End If
    End Function

    Public Overrides Function ToString() As String
        Return $"{InteractorA}  ({Type}, {Score})    {InteractorB}"
    End Function
End Class
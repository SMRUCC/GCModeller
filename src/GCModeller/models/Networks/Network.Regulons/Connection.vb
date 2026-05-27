Imports Microsoft.VisualBasic.Data.GraphTheory.Network
Imports Microsoft.VisualBasic.Data.GraphTheory.SparseGraph

''' <summary>
''' the correlation connection between two genes
''' </summary>
Public Class Connection : Implements IInteraction, INetworkEdge

    Public Property gene1 As String Implements IInteraction.source
    Public Property gene2 As String Implements IInteraction.target
    Public Property is_directly As Boolean
    Public Property cor As Double Implements INetworkEdge.value
    Public Property pval As Double
    Public Property interaction As String Implements INetworkEdge.Interaction

End Class
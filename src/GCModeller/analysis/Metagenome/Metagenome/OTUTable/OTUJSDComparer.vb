Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Correlations

Public Class OTUJSDComparer : Inherits ComparisonProvider

    ReadOnly OTUs As New Dictionary(Of String, OTUTable)
    ReadOnly sampleids As String()

    Public Sub New(OTUs As IEnumerable(Of OTUTable), equals As Double, gt As Double)
        MyBase.New(equals, gt)

        For Each otu As OTUTable In OTUs.SafeQuery
            Call Me.OTUs.Add(otu.ID, otu)
        Next

        sampleids = Me.OTUs.Values.PropertyNames
    End Sub

    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Dim otu1 As OTUTable = OTUs(x)
        Dim otu2 As OTUTable = OTUs(y)
        Dim P As Double() = otu1(sampleids)
        Dim Q As Double() = otu2(sampleids)

        Return Correlations.JSD(P, Q)
    End Function

    Public Overrides Function GetObject(id As String) As Object
        Return OTUs(id)
    End Function
End Class

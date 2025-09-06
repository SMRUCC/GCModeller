Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class NTTree : Inherits ComparisonProvider

    ReadOnly tree As New ClusterTree
    ReadOnly map As New Dictionary(Of String, NTCluster)

    Public Sub New(equals As Double, gt As Double)
        MyBase.New(equals, gt)
    End Sub

    Public Sub MakeTtree(seeds As IEnumerable(Of NTCluster))
        Dim args As New ClusterTree.Argument With {
            .threshold = 0.85,
            .diff = 0.05,
            .alignment = Me
        }
        Dim key As String

        For Each seed As NTCluster In seeds
            key = seed.GetHashCode.ToString

            Call map.Add(key, seed)
            Call args.SetTargetKey(key)
            Call ClusterTree.Add(tree, args)
        Next
    End Sub

    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Dim a = map(x)
        Dim b = map(y)
        Dim jac As Double = New Vector(a.fingerprint).Tanimoto(New Vector(b.fingerprint))
        Return jac
    End Function

    Public Overrides Function GetObject(id As String) As Object
        Return map(id)
    End Function
End Class

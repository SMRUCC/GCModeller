Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class NTTree : Inherits ComparisonProvider

    ReadOnly tree As New ClusterTree
    ReadOnly map As New Dictionary(Of String, NTCluster)

    Default Public ReadOnly Property Item(i As Integer) As NTCluster
        Get
            Return map(CInt(i))
        End Get
    End Property

    Public Sub New(equals As Double, gt As Double)
        MyBase.New(equals, gt)
    End Sub

    Public Sub MakeTtree(seeds As IEnumerable(Of NTCluster))
        Dim args As New ClusterTree.Argument With {
            .threshold = equalsDbl,
            .diff = 0.1,
            .alignment = Me
        }
        Dim key As String
        Dim i As Integer = 0

        For Each seed As NTCluster In seeds
            key = i.ToString
            i += 1

            Call map.Add(key, seed)
            Call args.SetTargetKey(key)
            Call ClusterTree.Add(tree, args)
        Next
    End Sub

    Public Iterator Function GetClusters() As IEnumerable(Of NTCluster)
        Dim class_id As Integer = 1

        For Each node In ClusterTree.GetClusters(tree)
            For Each key As String In node.Members
                Dim gene As NTCluster = map(key)
                gene.cluster = class_id.ToString
                Yield gene
            Next

            class_id += 1
        Next
    End Function

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

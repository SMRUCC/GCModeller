Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class GraphScan : Inherits SeedScanner

    Public Sub New(param As PopulatorParameter, debug As Boolean)
        MyBase.New(param, debug)
    End Sub

    Public Overrides Iterator Function GetSeeds(regions() As FastaSeq) As IEnumerable(Of HSP)
        Dim tree = SeedCluster.BuildAVLTreeCluster(regions.Select(Function(f) New NamedValue(Of String)(f.Title, f.SequenceData)), cutoff:=0.5)
        Dim groups = tree.PopulateNodes.ToArray

        For Each group As BinaryTree(Of String, String) In groups

        Next
    End Function
End Class

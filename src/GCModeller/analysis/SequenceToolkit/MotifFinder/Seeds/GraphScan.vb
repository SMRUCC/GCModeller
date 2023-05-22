Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class GraphScan : Inherits SeedScanner

    Public Sub New(param As PopulatorParameter, debug As Boolean)
        MyBase.New(param, debug)
    End Sub

    Private Function GetSequenceIndex(regions() As FastaSeq) As Dictionary(Of String, FastaSeq)
        Dim index As New Dictionary(Of String, FastaSeq)

        For Each seq As FastaSeq In regions
            index(seq.Title) = seq
        Next

        Return index
    End Function

    Public Overrides Iterator Function GetSeeds(regions() As FastaSeq) As IEnumerable(Of HSP)
        Dim source = GetSequenceIndex(regions)
        Dim tree = SeedCluster.BuildAVLTreeCluster(regions.Select(Function(f) New NamedValue(Of String)(f.Title, f.SequenceData)), cutoff:=0.7)
        Dim groups = tree.PopulateNodes.Select(Function(t, i) New NamedCollection(Of String)((i + 1).ToString, t.Members)).ToArray
        Dim full As New FullScan(param, debug)

        Call param.logText($"get total {groups.Length} sequence groups!")

        For Each seqs As NamedCollection(Of String) In groups
            Call param.logText($"> processing sequence group with {seqs.Length} sequence.")

            If seqs.Length >= 2 Then
                For Each seed As HSP In full.GetSeeds(seqs.Select(Function(i) source(i)).ToArray)
                    Yield seed
                Next
            End If
        Next
    End Function
End Class

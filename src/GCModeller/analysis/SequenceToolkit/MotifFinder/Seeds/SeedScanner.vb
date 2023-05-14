Imports SMRUCC.genomics.SequenceModel.FASTA

Public MustInherit Class SeedScanner

    Protected ReadOnly param As PopulatorParameter
    Protected ReadOnly debug As Boolean

    Sub New(param As PopulatorParameter, debug As Boolean)
        Me.param = param
        Me.debug = debug
    End Sub

    Public MustOverride Iterator Function GetSeeds(regions As FastaSeq()) As IEnumerable(Of HSP)


End Class

Imports SMRUCC.genomics.SequenceModel

Namespace DEG

    ''' <summary>
    ''' The sequence summary and reference data
    ''' </summary>
    Public Class SeqRef : Implements IPolymerSequenceModel

        Public Property ID As String
        Public Property Xref As String
        Public Property geneName As String
        Public Property isVirulence As Boolean
        Public Property fullName As String
        Public Property Organism As String
        Public Property Reference As String
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

    End Class
End Namespace
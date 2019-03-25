Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.iGEM

    Public Class PartSeq : Implements IPolymerSequenceModel, INamedValue

        Public Property PartName As String Implements IKeyedEntity(Of String).Key
        Public Property Status As String
        Public Property Id As String
        Public Property Type As String
        Public Property Description As String
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Shared Iterator Function Parse(file As String) As IEnumerable(Of PartSeq)
            For Each seq As FastaSeq In StreamIterator.SeqSource(file)
                Dim headers = CommandLine.GetTokens(seq.Title)

                If headers.Length = 5 Then
                    Yield New PartSeq With {
                        .PartName = headers(0),
                        .Status = headers(1),
                        .Id = headers(2),
                        .Type = headers(3),
                        .Description = headers(4),
                        .SequenceData = seq.SequenceData
                    }
                Else
                    Yield New PartSeq With {
                       .PartName = headers(0),
                       .Status = headers(1),
                       .Id = headers(2),
                       .Type = Nothing,
                       .Description = headers(3),
                       .SequenceData = seq.SequenceData
                   }
                End If
            Next
        End Function
    End Class
End Namespace
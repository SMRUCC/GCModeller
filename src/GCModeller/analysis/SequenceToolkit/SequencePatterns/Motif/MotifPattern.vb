Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Motif

    ''' <summary>
    ''' Regular expression model for the motifs
    ''' </summary>
    Public Class MotifPattern : Implements sIdEnumerable

        Public Property Id As String Implements sIdEnumerable.Identifier
        Public Property Expression As String
        Public Property Motif As String
        Public Property Width As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function Scan(scanner As Scanner) As SimpleSegment()
            Return scanner.Scan(Expression)
        End Function
    End Class
End Namespace
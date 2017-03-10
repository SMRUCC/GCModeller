Imports System.Runtime.CompilerServices

Namespace ProteinModel

    Public Module Extensions

        <Extension>
        Public Function AsMetaString(Of T As IMotifDomain)(domain As T) As String
            With domain.Location.Normalization()
                Return $"{domain.ID}({ .Left}|{ .Right})"
            End With
        End Function
    End Module
End Namespace
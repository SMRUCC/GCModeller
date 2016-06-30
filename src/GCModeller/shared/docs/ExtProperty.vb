Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Terminal

Namespace DocumentFormat

    Public Module ExtProperty

        <Extension> Public Function IsPossibleRNA(transcript As Transcript) As Boolean
            If transcript.Length < Math.Abs(transcript.ATG - transcript.TGA) Then
                Return True
            ElseIf transcript._5UTR >= 1000 Then
                Return True
            Else
                Return False
            End If
        End Function

        <Extension> Public Function IsRNA(transcript As Transcript) As Boolean
            Return transcript.TSSs <> 0 AndAlso transcript.ATG = 0
        End Function

        <Extension> Public Function TSSsOverlapsATG(tr As Transcript) As Boolean
            Return tr.TSSs = tr.ATG
        End Function

        <Extension> Public Function TTSsOverlapsTGA(tr As Transcript) As Boolean
            Return tr.TTSs = tr.TGA
        End Function

        ''' <summary>
        ''' 上下游都分别重合不能够延伸
        ''' </summary>
        ''' <returns></returns>
        <Extension> Public Function BoundaryOverlaps(tr As Transcript) As Boolean
            Return tr.TTSsOverlapsTGA AndAlso tr.TSSsOverlapsATG
        End Function

        <Extension> Public Function PredictedsiRNA(tr As Transcript) As Boolean
            Return tr.BoundaryOverlaps AndAlso tr.MappingLocation.FragmentSize <= 150
        End Function

        <Extension> Public Function AssignTSSsId(source As Generic.IEnumerable(Of Transcript), Optional prefix As String = "TSS_") As Transcript()
            Dim array As Transcript() = source.ToArray
            Dim i As Integer = 1

            For Each transcript As Transcript In array
                If transcript._5UTR <> 0 Then
                    transcript.TSS_ID = prefix & STDIO.ZeroFill(i.MoveNext, 4)
                Else
                    If transcript.TSSsShared >= 30 Then
                        transcript.TSS_ID = prefix & STDIO.ZeroFill(i.MoveNext, 4)
                    End If
                End If
            Next

            Return array
        End Function
    End Module
End Namespace
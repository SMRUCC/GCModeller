Imports System.Runtime.CompilerServices
Imports System.Text
Imports LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles
Imports LANS.SystemsBiology.Assembly.MetaCyc.Schema.Metabolism

Namespace Assembly.MetaCyc

    Public Module Extensions

        Public ReadOnly Property Directions As IReadOnlyDictionary(Of String, ReactionDirections) =
            New Dictionary(Of String, ReactionDirections) From {
 _
            {"LEFT-TO-RIGHT", ReactionDirections.LeftToRight},
            {"REVERSIBLE", ReactionDirections.Reversible},
            {"RIGHT-TO-LEFT", ReactionDirections.RightToLeft}
        }

        <Extension> Public Function GetAttributeList(Of T As Slots.Object)(data As DataFile(Of T)) As String()
            Return (From s As String
                    In data.AttributeList
                    Where Not s.IsBlank
                    Select s
                    Distinct
                    Order By s.Length Descending).ToArray
        End Function

        ''' <summary>
        ''' 不会保留PGDB中的断行
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function ContactLines(source As String()) As String
            Dim sbr As StringBuilder = New StringBuilder(1024)
            Dim breaks As StringBuilder = New StringBuilder(512)

            For ind As Integer = 0 To source.Length - 1
                If ind = source.Length - 1 Then
                    sbr.AppendLine(source(ind))
                    Exit For
                End If

                If String.Compare(source(ind + 1).Chars(0), "/") <> 0 Then
                    sbr.AppendLine(source(ind))
                    Continue For
                End If

                breaks.Clear()
                breaks.Append(source(ind))
                ind += 1

                Do While String.Compare(source(ind).Chars(0), "/") = 0
                    breaks.Append(" ")
                    breaks.Append(source(ind).Substring(1))
                    ind += 1

                    If ind >= source.Length - 1 Then
                        Exit Do
                    End If
                Loop

                sbr.AppendLine(breaks.ToString)
            Next

            Call sbr.Replace("  ", " ")

            Return sbr.ToString
        End Function
    End Module
End Namespace
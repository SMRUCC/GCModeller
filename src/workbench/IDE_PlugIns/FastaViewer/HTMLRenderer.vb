Imports System.Runtime.CompilerServices
Imports System.Text
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Imaging

Public Module HTMLRenderer

    Public Const A As String = "<span style=""color:red"">A</span>"
    Public Const T As String = "<span style=""color:green"">T</span>"
    Public Const G As String = "<span style=""color:orange"">G</span>"
    Public Const C As String = "<span style=""color:blue"">C</span>"

    Public ReadOnly Property Nt As Dictionary(Of Char, String) =
        New Dictionary(Of Char, String) From {
 _
        {"A"c, A}, {"T"c, T}, {"G"c, G}, {"C"c, C}
    }

    <Extension>
    Public Function VisualNT(nt As FastaToken) As String
        Dim sb As New StringBuilder(4096)

        Call sb.Append($"<td>&gt;{nt.Title}</td>")
        Call sb.Append($"<td>{nt.Length}</td>")
        Call sb.Append("<td>")
        For Each r As Char In nt.SequenceData.ToUpper
            If HTMLRenderer.Nt.ContainsKey(r) Then
                sb.Append(HTMLRenderer.Nt(r))
            Else
                sb.Append(r)
            End If
        Next
        Call sb.Append("</td>")

        Return sb.ToString
    End Function

    Public Function VisualNts(file As FastaFile) As String
        Dim sb As New StringBuilder

        Call sb.AppendLine($"<font face=""{FontFace.Consolas}""><strong>")
        Call sb.AppendLine("<table>")
        Call sb.AppendLine("<tr><td>Title</td><td>Length</td><td>Sequence</td></tr>")

        For Each Nt As FastaToken In file
            Call sb.AppendLine($"<tr>{Nt.VisualNT}</tr>")
        Next

        Call sb.AppendLine("</table>")
        Call sb.AppendLine("</strong></font>")

        Return sb.ToString
    End Function
End Module

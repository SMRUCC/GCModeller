Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.ComponentModel
Imports r = System.Text.RegularExpressions.Regex

Namespace Regprecise

    Public Class RegulomeQuery : Inherits WebQuery(Of String)

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False
                   )

            MyBase.New(url:=AddressOf RegulomeQuery.url,
                       contextGuid:=AddressOf GetsId,
                       parser:=AddressOf ParseRegulon,
                       prefix:=Nothing,
                       cache:=cache,
                       interval:=interval,
                       offline:=offline
                   )
        End Sub

        Private Shared Function url(entryHref As String) As String
            Dim str$ = r.Match(entryHref, "href="".+?"">.+?</a>").Value
            Dim strUrl$ = "http://regprecise.lbl.gov/RegPrecise/" & str.href

            Return strUrl
        End Function

        <ExportAPI("Get.sId")>
        Public Shared Function GetsId(strData As String) As String
            Dim Id As String = Mid(strData, InStr(strData, """>") + 2)

            If String.IsNullOrEmpty(Trim(Id)) Then
                Return ""
            Else
                Id = Mid(Id, 1, Len(Id) - 4)
                Return Id
            End If
        End Function

        ''' <summary>
        ''' 下载某一个基因组内所有预测的调控元的数据
        ''' </summary>
        ''' <returns></returns>
        <ExportAPI("Regulon.Downloads")>
        Private Shared Function ParseRegulon(html As String, null As Type) As Object
            Dim list$()
            Dim regulators As New List(Of Regulator)
            Dim str$

            html$ = r _
                .Match(html, "<table class=""stattbl"".+?</table>", RegexOptions.Singleline) _
                .Match("<tbody>.+</tbody>", RegexOptions.Singleline)
            list = r _
                .Matches(html, "<tr .+?</tr>", RegexICSng) _
                .ToArray

            For i As Integer = 0 To list.Length - 1
                str = list(i)
                regulators += Regulator.CreateObject(str)
                Thread.Sleep(2000)
            Next

            Return New Regulon With {
                .regulators = regulators
            }
        End Function
    End Class
End Namespace
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.ComponentModel
Imports r = System.Text.RegularExpressions.Regex

Namespace Regprecise

    Public Class RegulomeQuery : Inherits WebQueryModule(Of String)

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            MyBase.New(cache:=cache, interval:=interval, offline:=offline)
        End Sub

        Protected Overrides Function doParseUrl(context As String) As String
            Dim str$ = r.Match(context, "href="".+?"">.+?</a>").Value
            Dim strUrl$ = "http://regprecise.lbl.gov/RegPrecise/" & str.href

            Return strUrl
        End Function

        ''' <summary>
        ''' 下载某一个基因组内所有预测的调控元的数据
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Function doParseObject(html As String, schema As Type) As Object
            Dim list$()
            Dim regulators As New List(Of Regulator)
            Dim str$
            Dim regulatorQuery As New RegulatorQuery($"{cache}/regulators/", sleepInterval, offlineMode)

            html$ = r _
                .Match(html, "<table class=""stattbl"".+?</table>", RegexOptions.Singleline) _
                .Match("<tbody>.+</tbody>", RegexOptions.Singleline)
            list = r _
                .Matches(html, "<tr .+?</tr>", RegexICSng) _
                .ToArray

            For i As Integer = 0 To list.Length - 1
                str = list(i)
                regulators += regulatorQuery.Query(Of Regulator)(str, ".txt")
            Next

            Return New Regulon With {
                .regulators = regulators
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function doParseGuid(context As String) As String
            Return GetsId(context)
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
    End Class
End Namespace
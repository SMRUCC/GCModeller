#Region "Microsoft.VisualBasic::7854cb755e85d9ecae20d206e8fbff12, data\RegulonDatabase\Regprecise\WebServices\WebParser\RegulomeQuery .vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 82
    '    Code Lines: 64 (78.05%)
    ' Comment Lines: 4 (4.88%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 14 (17.07%)
    '     File Size: 3.19 KB


    '     Class RegulomeQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: doParseGuid, doParseObject, doParseUrl, GetsId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
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
            Dim strUrl$ = "https://regprecise.lbl.gov/" & str.href

            Return strUrl
        End Function

        ''' <summary>
        ''' 下载某一个基因组内所有预测的调控元的数据
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Function doParseObject(html As String, schema As Type) As Object
            Dim list$()
            Dim regulators As New List(Of Regulator)
            Dim regulator As Regulator
            Dim str$
            Dim genomeName$ = r.Match(html, "<i>.*?</i>", RegexICSng) _
                .Value _
                .GetValue _
                .NormalizePathString(False)
            Dim regulatorQuery As New RegulatorQuery($"{cache}/regulators/{genomeName}", sleepInterval, offlineMode)

            html$ = r _
                .Match(html, "<table class=""stattbl"".+?</table>", RegexOptions.Singleline) _
                .Match("<tbody>.+</tbody>", RegexOptions.Singleline)
            list = r _
                .Matches(html, "<tr .+?</tr>", RegexICSng) _
                .ToArray

            For i As Integer = 0 To list.Length - 1
                str = list(i)
                regulator = regulatorQuery.Query(Of Regulator)(str, ".txt")
                regulators += RegulatorQuery.basicParser(str, regulator)
            Next

            Return New Regulome With {
                .regulators = regulators
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function doParseGuid(context As String) As String
            Dim str$ = r.Match(context, "href="".+?"">.+?</a>").Value
            Dim genomeName As String = GetsId(str).NormalizePathString(False)

            Return genomeName
        End Function

        <ExportAPI("Get.sId")>
        Public Shared Function GetsId(strData As String) As String
            Dim left As Integer = InStr(strData, """>") + 2
            Dim Id As String = Mid(strData, left)

            If String.IsNullOrEmpty(Strings.Trim(Id)) Then
                Return ""
            Else
                Id = Mid(Id, 1, Len(Id) - 4)
                Return Id
            End If
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::6cb995eb22e98d62cfe0dfaa020bed1c, core\Bio.Assembly\Assembly\KEGG\Web\FetchSequence.vb"

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

    '     Class FetchSequence
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: DbgetUrl, parseSeqHtml, seqUid, urlCreator
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.WebServices

    Public Class FetchSequence : Inherits WebQuery(Of QueryEntry)

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub New(isNucl As Boolean,
                       <CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            MyBase.New(urlCreator(isNucl),
                       AddressOf seqUid,
                       AddressOf parseSeqHtml,,
                       cache:=cache,
                       interval:=interval,
                       offline:=offline
            )
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function seqUid(entry As QueryEntry) As String
            Return $"{entry.speciesID}/{entry.locusID}"
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function urlCreator(isNucl As Boolean) As Func(Of QueryEntry, String)
            Return Function(entry) DbgetUrl(entry, isNucl)
        End Function

        Const KEGG_DBGET_QUERY_NT As String = "http://www.genome.jp/dbget-bin/www_bget?-f+-n+n+{0}:{1}"
        Const KEGG_DBGET_QUERY_PROTEIN As String = "http://www.genome.jp/dbget-bin/www_bget?-f+-n+a+{0}:{1}"

        Public Shared Function DbgetUrl(query As QueryEntry, isNucl As Boolean) As String
            Dim templateUrl$

            If isNucl Then
                templateUrl = KEGG_DBGET_QUERY_NT
            Else
                templateUrl = KEGG_DBGET_QUERY_PROTEIN
            End If

            Return String.Format(templateUrl, query.speciesID, query.locusID)
        End Function

        Const PAGE_CONTENT_FASTA_SEQUENCE As String = "[<]pre[>].+[<][/]pre[>]"

        ''' <summary>
        ''' 解析从KEGG数据库下载基因或者蛋白序列数据
        ''' </summary>
        ''' <returns></returns>
        Private Shared Function parseSeqHtml(html As String, null As Type) As FastaSeq
            If String.IsNullOrEmpty(html) OrElse InStr(html, "No such data was found.", CompareMethod.Text) > 0 Then
                Return Nothing
            End If

            Dim text$ = r.Match(html, PAGE_CONTENT_FASTA_SEQUENCE, RegexOptions.Singleline) _
                .Value _
                .RemoveXmlComments

            ' fasta text
            text = text.StringReplace("[<][/]?pre[>]", "", RegexICSng) _
                .Trim(ASCII.CR, ASCII.LF) _
                .UnescapeHTML

            Dim tokens As String() = text.LineTokens
            Dim fa As New FastaSeq With {
                .Headers = {tokens(Scan0).Trim(">"c)},
                .SequenceData = tokens.Skip(1).JoinBy("")
            }

            If String.IsNullOrEmpty(fa.SequenceData) Then
                Return Nothing
            Else
                Return fa
            End If
        End Function
    End Class
End Namespace

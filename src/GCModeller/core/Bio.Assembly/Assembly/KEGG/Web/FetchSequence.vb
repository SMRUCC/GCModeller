Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Text.Xml
Imports SMRUCC.genomics.ComponentModel
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
            If String.IsNullOrEmpty(html) OrElse InStr(html, ": No such data.", CompareMethod.Text) > 0 Then
                Return Nothing
            End If

            Dim text$ = r.Match(html, PAGE_CONTENT_FASTA_SEQUENCE, RegexOptions.Singleline).Value
            Dim previewLen% '= Len(String.Format("<pre>" & vbCrLf & "<!-- bget:db:genes --><!-- {0}:{1} -->", specieId, accessionId))

            text = Mid(text, previewLen + 1, Len(text) - previewLen - 6)

            Dim tokens As String() = text.LineTokens
            Dim fa As New FastaSeq With {
                .Headers = {
                    XmlEntity.UnescapeHTML(tokens.First).Trim(">")
                },
                .SequenceData = Mid(text, Len(tokens.First) + 1).TrimNewLine("")
            }

            If String.IsNullOrEmpty(fa.SequenceData) Then
                Return Nothing
            Else
                Return fa
            End If
        End Function
    End Class
End Namespace
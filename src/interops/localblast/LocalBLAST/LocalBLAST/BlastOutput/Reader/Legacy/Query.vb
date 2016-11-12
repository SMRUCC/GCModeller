#Region "Microsoft.VisualBasic::9c2af8913bf715d5f14cab538c003233, ..\interops\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Standard\Query.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text

Namespace LocalBLAST.BLASTOutput.Legacy

    Public Class Query

        Public Const BLAST_SECTION_HEADER As String = "BLAST. \d+[.]\d+[.]\d+[+]?( [[]Sep[-]21[-]2011[]])?"
        Public Const HITS_NOT_FOUND As String = "***** No hits found *****"
        Public Const SEPERATOR As String = "Searching..................................................done"

        ''' <summary>
        ''' 目标查询序列的名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property QueryName As String
        ''' <summary>
        ''' 目标查询序列的长度
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Length As Integer

        ''' <summary>
        ''' 目标查询序列的匹配序列列表
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property Hits As Hit()

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return String.IsNullOrEmpty(QueryName) AndAlso Me.Hits.IsNullOrEmpty
            End Get
        End Property

        Public Function GrepQuery(method As TextGrepMethod) As Integer
            If Not QueryName.IsNullOrEmpty Then
                QueryName = method(QueryName)
                '       QueryName = QueryName.Split(vbCrLf).First.Split(vbCrLf).First.Split(vbCr).First
            Else
                QueryName = "Unknown"
            End If
            Return 0
        End Function

        Public Function GrepHits(method As TextGrepMethod) As Integer
            If Hits Is Nothing OrElse Hits.Count = 0 Then
                Return 0
            Else
                Dim Query = From Hit As Hit In Hits.AsParallel
                            Select Hit.Grep(method) '
                Return Query.ToArray.Count
            End If
        End Function

        Public Overrides Function ToString() As String
            If Hits Is Nothing OrElse Hits.Count = 0 Then
                Return Query.HITS_NOT_FOUND
            Else
                Return String.Format("{0}; ({1} hits)", QueryName, Hits.Count)
            End If
        End Function

        Public Const QUERY_LENGTH As String = "         \(\d+ letters\)"
        Public Const QUERY_BLOCK_SECTION As String = "Query= .+" & QUERY_LENGTH

        Public Shared Function TryParse(Text As String) As Query
            Dim QueryBlock As String = Regex.Match(
                    Text, Legacy.Query.QUERY_BLOCK_SECTION,
                    RegexOptions.Singleline).Value

            Dim Tokens = QueryBlock.Split(Chr(10))
            Dim NameBuilder As StringBuilder = New StringBuilder(128)
            Dim QueryLength = Val(Regex.Match(Tokens.Last, "\d+").Value)

            For i As Integer = 0 To Tokens.Count - 2
                Call NameBuilder.Append(Tokens(i))
            Next
            Call NameBuilder.Replace(vbCrLf, " ")
            Call NameBuilder.Replace(vbCr, " ")
            Call NameBuilder.Replace(vbLf, " ")
            Dim Query As Query = New Query With {.Length = QueryLength, .QueryName = NameBuilder.Replace("Query= ", "").ToString}

            If InStr(Text, Query.HITS_NOT_FOUND) Then
                Query.Hits = New Hit() {}
            Else
                Dim HitsSectionText As String = Strings.Split(Text, Query.SEPERATOR).Last
                Dim HitTexts As String() = Regex.Split(HitsSectionText, "^>", RegexOptions.Multiline).Skip(1).ToArray
                Dim Hits = From s As String In HitTexts Select Hit.TryParse(s) '

                Query.Hits = Hits.ToArray
            End If

            Return Query
        End Function
    End Class
End Namespace

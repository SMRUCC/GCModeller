#Region "Microsoft.VisualBasic::693c03baa5cc25e9e841c959b15de969, data\RCSB PDB\PDB\Keywords\Headers\Header.vb"

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

    '   Total Lines: 228
    '    Code Lines: 170 (74.56%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 58 (25.44%)
    '     File Size: 6.50 KB


    '     Class Header
    ' 
    '         Properties: [Date], Keyword, pdbID, Title
    ' 
    '         Function: Parse, ToString
    ' 
    '     Class Title
    ' 
    '         Properties: Keyword, Title
    ' 
    '         Function: Append, ToString
    ' 
    '     Class Keywords
    ' 
    '         Properties: Keyword, keywords
    ' 
    '         Function: Parse, ToString
    ' 
    '     Class ExperimentData
    ' 
    '         Properties: Experiment, Keyword
    ' 
    '         Function: Parse, ToString
    ' 
    '     Class Author
    ' 
    '         Properties: Keyword, Name
    ' 
    '         Function: Parse, ToString
    ' 
    '     Class DbReference
    ' 
    '         Properties: db_xrefs, Keyword, XrefIndex
    ' 
    '         Function: Append, ToString
    ' 
    '         Sub: Flush
    ' 
    '     Class Site
    ' 
    '         Properties: Keyword
    ' 
    '         Function: Append, ToString
    ' 
    '     Class Master
    ' 
    '         Properties: Keyword, line
    ' 
    '         Function: Parse, ToString
    ' 
    '     Class CRYST1
    ' 
    '         Properties: Keyword
    ' 
    '         Function: Append
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Keywords

    ''' <summary>
    ''' the pdb file header description information
    ''' </summary>
    Public Class Header : Inherits Keyword

        Public Property pdbID As String
        Public Property [Date] As String
        Public Property Title As String

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_HEADER
            End Get
        End Property

        Public ReadOnly Property EmptyContent As Boolean
            Get
                Return pdbID.StringEmpty(, True) AndAlso Title.StringEmpty(, True)
            End Get
        End Property

        Friend Shared Function Parse(line As String) As Header
            Dim str = line.StringSplit("\s+")
            Dim header As New Header With {
                .pdbID = str(str.Length - 1),
                .[Date] = str(str.Length - 2),
                .Title = str.Take(str.Length - 2).JoinBy(" ")
            }

            Return header
        End Function

        Public Overrides Function ToString() As String
            Return $"({pdbID}) {Title}"
        End Function

        ''' <summary>
        ''' 生成符合PDB格式的HEADER行字符串
        ''' </summary>
        ''' <returns>格式化的HEADER行字符串</returns>
        Public Function ToPdbString() As String
            ' 基础格式: "HEADER    {TITLE} {DATE}   {PDBID}"
            ' 确保PDBID为4字符（PDB标准）
            Dim formattedPdbId As String = If(pdbID IsNot Nothing AndAlso pdbID.Length >= 4, pdbID.Substring(0, 4), pdbID.PadRight(4))

            ' 格式化日期（如果提供）
            Dim formattedDate As String = If(String.IsNullOrEmpty([Date]), DateTime.Now.ToString("dd-MMM-yy").ToUpper(), [Date])

            ' 清理标题中的多余空格
            Dim cleanTitle As String = If(Title IsNot Nothing, Title.Trim().Replace("  ", " "), "")

            ' 组合成PDB格式行
            Return $"HEADER    {cleanTitle,-40} {formattedDate,-9}   {formattedPdbId}"
        End Function

    End Class

    ''' <summary>
    ''' pdb file title description information
    ''' </summary>
    Public Class Title : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_TITLE
            End Get
        End Property

        Public Property Title As String

        Public Overrides Function ToString() As String
            Return Title
        End Function

        Friend Shared Function Append(ByRef title As Title, str As String) As Title
            If title Is Nothing Then
                title = New Title With {.Title = str}
            Else
                ' join multiple lines of the title text data
                title.Title = title.Title & " " & str.GetTagValue(" ", trim:=True).Value
            End If

            Return title
        End Function

        ''' <summary>
        ''' 生成 PDB 文件格式的 TITLE 部分内容
        ''' </summary>
        ''' <param name="titleObj">Title 类对象，包含标题文本</param>
        ''' <param name="maxLineLength">单行最大长度（默认为标准的80字符，PDB记录行通常为80字符）</param>
        ''' <returns>格式化后的 PDB TITLE 行列表，每行都是一个字符串</returns>
        Public Shared Iterator Function GeneratePdbTitleLines(titleObj As Title, Optional maxLineLength As Integer = 80) As IEnumerable(Of String)
            If titleObj Is Nothing OrElse String.IsNullOrEmpty(titleObj.Title) Then
                Yield "TITLE"
                Return
            End If

            ' PDB 记录类型标识占6列（如"TITLE "），行号占2列（如" 1"），空格占1列，因此描述文本可用长度为 80 - 6 - 2 - 1 = 71
            Dim maxTextLength As Integer = maxLineLength - 9 ' 扣除 "TITLE X " 的固定字符占用（6+2+1）
            Dim titleText As String = titleObj.Title.Trim()
            Dim currentIndex As Integer = 0
            Dim lineCount As Integer = 0

            While currentIndex < titleText.Length
                lineCount += 1
                ' 计算本次可截取的长度
                Dim partLength As Integer = Math.Min(maxTextLength, titleText.Length - currentIndex)
                ' 截取部分文本，并确保不会在单词中间切断（简单实现，可按需增强）
                Dim part As String = titleText.Substring(currentIndex, partLength)

                ' 如果是第一行，格式为 "TITLE     [描述]"
                If lineCount = 1 Then
                    Yield $"TITLE    {part}"
                Else
                    ' 续行格式为 "TITLE [行号] [描述]"，行号从2开始
                    Yield $"TITLE{lineCount.ToString().PadLeft(2)}{part}"
                End If

                currentIndex += partLength
            End While
        End Function
    End Class

    Public Class Keywords : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return RCSB.PDB.Keywords.Keyword.KEYWORD_KEYWDS
            End Get
        End Property

        Public Property keywords As String()

        Public Overrides Function ToString() As String
            Return keywords.JoinBy("; ")
        End Function

        Friend Shared Function Parse(line As String) As Keywords
            Return New Keywords With {.keywords = line.StringSplit(",\s+")}
        End Function

    End Class

    Public Class ExperimentData : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_EXPDTA
            End Get
        End Property

        Public Property Experiment As String

        Public Overrides Function ToString() As String
            Return Experiment
        End Function

        Friend Shared Function Parse(line As String) As ExperimentData
            Return New ExperimentData With {.Experiment = line}
        End Function

    End Class

    ''' <summary>
    ''' the author list of the pdb file
    ''' </summary>
    Public Class Author : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_AUTHOR
            End Get
        End Property

        Public Property Name As String()

        Public Overrides Function ToString() As String
            Return Name.JoinBy("; ")
        End Function

        Friend Shared Function Parse(line As String) As Author
            Return New Author With {
                .Name = line.StringSplit("\s*,\s*")
            }
        End Function

        ''' <summary>
        ''' 生成符合PDB文件格式的AUTHOR字段文本。
        ''' </summary>
        ''' <returns>格式化后的AUTHOR字段文本，每行以"AUTHOR"关键字开头</returns>
        Public Function ToPDBAuthorFieldText() As String
            If Name.IsNullOrEmpty Then
                Return $"{Keywords.KEYWORD_AUTHOR}{vbCrLf}"
            End If

            ' 将作者名用分号和空格连接成一个长字符串
            Dim allAuthors As String = String.Join("; ", Name)
            ' PDB行标准长度（包括关键字）
            Const maxLineLength As Integer = 80
            ' 关键字长度 "AUTHOR " (注意包含空格)
            Dim keywordPrefix As String = Keywords.KEYWORD_AUTHOR & " "
            Dim prefixLength As Integer = keywordPrefix.Length

            ' 如果整个作者字符串（加上关键字）能在一行内放下
            If prefixLength + allAuthors.Length <= maxLineLength Then
                Return keywordPrefix & allAuthors
            End If

            ' 多行处理
            Dim lines As New List(Of String)
            Dim currentLine As String = keywordPrefix ' 当前行内容，以关键字开头
            Dim currentLength As Integer = currentLine.Length

            ' 按作者名分割（假设以分号分隔的每个作者名是一个整体，不应被截断）
            ' 注意：原始Author类的ToString是用分号空格连接，这里按分号加空格分割回来
            Dim authorNames As String() = allAuthors.Split(New String() {"; "}, StringSplitOptions.RemoveEmptyEntries)

            For i As Integer = 0 To authorNames.Length - 1
                Dim author As String = authorNames(i)
                ' 如果当前不是最后一个作者，则需要在名字后加分隔符
                Dim authorWithSeparator As String = If(i < authorNames.Length - 1, author & "; ", author)

                ' 如果当前行加上下一个作者名（及分隔符）会超出行长限制
                If currentLength + authorWithSeparator.Length > maxLineLength Then
                    ' 先保存当前行（不加这个作者）
                    lines.Add(currentLine)
                    ' 新行开始（续行行首不需要关键字，但需留出与关键字等长的空格）
                    currentLine = New String(" "c, prefixLength) & authorWithSeparator
                    currentLength = currentLine.Length
                Else
                    ' 当前行可以容纳这个作者（及分隔符）
                    currentLine &= authorWithSeparator
                    currentLength += authorWithSeparator.Length
                End If
            Next

            ' 添加最后一行
            If Not String.IsNullOrEmpty(currentLine) Then
                lines.Add(currentLine)
            End If

            ' 将每行用换行符连接，并确保最后以换行符结束
            Return String.Join(vbCrLf, lines)
        End Function
    End Class

    Public Class DbReference : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_DBREF
            End Get
        End Property

        Dim cache As New List(Of String)

        Public Property db_xrefs As NamedValue(Of String)()
        Public ReadOnly Property XrefIndex As Dictionary(Of String, String())
            Get
                Return db_xrefs _
                    .SafeQuery _
                    .GroupBy(Function(a) a.Name) _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.Values
                                  End Function)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return XrefIndex.GetJson
        End Function

        Friend Shared Function Append(ByRef ref As DbReference, str As String) As DbReference
            If ref Is Nothing Then
                ref = New DbReference
            End If
            ref.cache.Add(str)
            Return ref
        End Function

        Friend Overrides Sub Flush()

        End Sub

    End Class

    Public Class Site : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_SITE
            End Get
        End Property

        Dim str As New List(Of String)

        Public Overrides Function ToString() As String
            Return str.JoinBy(" ")
        End Function

        Friend Shared Function Append(ByRef site As Site, str As String) As Site
            If site Is Nothing Then
                site = New Site
            End If
            site.str.Append(str)
            Return site
        End Function
    End Class

    Public Class Master : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_MASTER
            End Get
        End Property

        Public Property line As String

        Public Overrides Function ToString() As String
            Return line
        End Function

        Public Shared Function Parse(str As String) As Master
            Return New Master With {
                .line = str
            }
        End Function

    End Class

    Public Class CRYST1 : Inherits Keyword

        Public Overrides ReadOnly Property Keyword As String
            Get
                Return Keywords.KEYWORD_CRYST1
            End Get
        End Property

        Dim cache As New List(Of String)

        Friend Shared Function Append(ByRef res As CRYST1, str As String) As CRYST1
            If res Is Nothing Then
                res = New CRYST1
            End If
            res.cache.Add(str)
            Return res
        End Function

    End Class
End Namespace

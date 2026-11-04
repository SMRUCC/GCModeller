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

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

Imports Microsoft.VisualBasic.Linq

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' 将数据写入现有的genbank文件或者创建新的genbank文件
    ''' </summary>
    ''' <remarks>
    ''' 这个版本的写出模块是按照NCBI的标准列格式重新实现的:
    ''' 
    ''' 1. **流式写出**: 直接写入<see cref="TextWriter"/>，不会像旧版本那样先把整个文档
    '''    拼接为一个巨大的字符串(对于人基因组的染色体记录，单个记录的文本就具有数百Mb)。
    ''' 2. **标准列格式**: ``LOCUS``/``DEFINITION``/``SOURCE``/``FEATURES``的折行位置以及
    '''    缩进都是按照NCBI的平文件规范生成的。
    ''' 3. **序列分块写出**: ``ORIGIN``段直接按照60个碱基一行写出，不会对整条序列进行
    '''    额外的字符串复制。
    ''' </remarks>
    <Package("NCBI.Genbank.WriteStream", Publisher:="GCModeller", Url:="http://gcmodeller.org")>
    Public Module GbkWriter

        ''' <summary>
        ''' 头部字段名所占用的列宽度
        ''' </summary>
        Const FIELD_WIDTH As Integer = 12
        ''' <summary>
        ''' feature 字段名所占用的列宽度(第6列到第21列)
        ''' </summary>
        Const FEATURE_FIELD_WIDTH As Integer = 16

        ''' <summary>
        ''' 不需要使用引号包裹的数值类型的qualifier
        ''' </summary>
        ReadOnly unquotedQualifiers As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            "codon_start",
            "transl_table",
            "number",
            "mod_base",
            "rpt_unit_range"
        }

        ''' <summary>
        ''' 将genbank对象模型转换为文本文档数据以进行数据保存
        ''' </summary>
        ''' <param name="gb"></param>
        ''' <returns></returns>
        <ExportAPI("Doc.Create"), Extension>
        Public Function CreateDoc(gb As GenBank.GBFF.File) As String
            Using writer As New StringWriter(New StringBuilder(1024))
                Call Save(gb, writer)
                Return writer.ToString()
            End Using
        End Function

        ''' <summary>
        ''' 将一条genbank记录流式的写出到目标文本写入器
        ''' </summary>
        ''' <param name="gb">要写出的genbank记录</param>
        ''' <param name="writer">目标文本写入器</param>
        Public Function Save(gb As GenBank.GBFF.File, writer As TextWriter) As Boolean
            If gb Is Nothing Then
                Return False
            End If

            Call __writeLocus(writer, gb.Locus)
            Call __writeField(writer, "DEFINITION", If(gb.Definition Is Nothing, "", gb.Definition.Value))
            Call __writeAccession(writer, gb.Accession, gb.Version)

            If gb.DbLinks IsNot Nothing AndAlso Not gb.DbLinks.Links.IsNullOrEmpty Then
                Call __writeDblink(writer, gb.DbLinks)
            End If

            Call __writeField(writer, "KEYWORDS", __keywords(gb.Keywords))

            If gb.Source IsNot Nothing Then
                Call __writeSource(writer, gb.Source)
            End If

            If gb.Reference IsNot Nothing Then
                Call __writeReference(writer, gb.Reference)
            End If

            If gb.Comment IsNot Nothing Then
                Call __writeComment(writer, gb.Comment)
            End If

            If Not String.IsNullOrEmpty(gb.Project) Then
                Call __writeField(writer, "PROJECT", gb.Project)
            End If
            If Not String.IsNullOrEmpty(gb.Segment) Then
                Call __writeField(writer, "SEGMENT", gb.Segment)
            End If
            If Not String.IsNullOrEmpty(gb.Wgs) Then
                Call __writeField(writer, "WGS", gb.Wgs)
            End If
            If gb.Primary IsNot Nothing Then
                Call __writeRawLines(writer, gb.Primary.Lines)
            End If

            If gb.Features IsNot Nothing Then
                Call __writeFeatures(writer, gb.Features)
            End If

            If gb.Contig IsNot Nothing Then
                Call __writeContig(writer, gb.Contig)
            End If

            ' 未被解析的字段按照原始文本原样写出
            If gb.RawSections IsNot Nothing Then
                For Each key As String In gb.RawSections.Keys
                    Call __writeRawLines(writer, gb.RawSections(key))
                Next
            End If

            If gb.Origin IsNot Nothing Then
                Call __writeOrigin(writer, gb.Origin)
            End If

            Call writer.WriteLine(GbkFormat.RECORD_DELIMITER)
            Call writer.Flush()

            Return True
        End Function

        ''' <summary>
        ''' 将genbank记录写出到指定的文件
        ''' </summary>
        ''' <param name="gb"></param>
        ''' <param name="path">``*.gb``，所需要进行保存的genbank数据库的文件路径</param>
        ''' <param name="encoding">文本编码</param>
        <ExportAPI("Write.GBK")>
        <Extension>
        Public Function WriteGenbank(gb As GenBank.GBFF.File, path$, Optional encoding As Encodings = Encodings.ASCII) As Boolean
            Return gb.Save(path, encoding.CodePage)
        End Function

        ''' <summary>
        ''' 将多条genbank记录写出到指定的文件
        ''' </summary>
        <ExportAPI("Write.GBK")>
        <Extension>
        Public Function WriteGenbank(records As IEnumerable(Of GenBank.GBFF.File), path$, Optional encoding As Encodings = Encodings.ASCII) As Boolean
            Using writer As New StreamWriter(New FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 1024 * 64), GetEncoding(encoding))
                For Each gb As GenBank.GBFF.File In records
                    Call Save(gb, writer)
                Next
            End Using

            Return True
        End Function

        ''' <summary>
        ''' 将<see cref="Encodings"/>枚举转换为文本编码对象
        ''' </summary>
        Public Function GetEncoding(encoding As Encodings) As System.Text.Encoding
            Select Case encoding
                Case Encodings.ASCII
                    Return System.Text.Encoding.ASCII
                Case Encodings.UTF8
                    Return New System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier:=True)
                Case Encodings.UTF8WithoutBOM
                    Return New System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)
                Case Encodings.Unicode, Encodings.UTF16
                    Return System.Text.Encoding.Unicode
                Case Else
                    Return System.Text.Encoding.Default
            End Select
        End Function

#Region "LOCUS / head fields"

        Private Sub __writeLocus(writer As TextWriter, locus As LOCUS)
            If locus Is Nothing Then
                Return
            End If

            Dim name As String = If(locus.AccessionID, "n/a")
            Dim length As String = locus.Length.ToString()
            Dim mol As String = If(locus.Type, "DNA")
            Dim topology As String = If(locus.Molecular, "linear")
            Dim division As String = If(locus.Division, "")
            Dim dateText As String = If(String.IsNullOrEmpty(locus.UpdateTime), Now.ToString("dd-MMM-yyyy").ToUpperInvariant(), locus.UpdateTime)

            ' 列布局: 名称(第13列到第28列) 长度(右对齐到第40列) " bp" 分子类型 拓扑结构 分类代码 日期
            Call writer.WriteLine(
                $"LOCUS       {name.PadRight(FEATURE_FIELD_WIDTH)}{length.PadLeft(12)} bp    {mol.PadRight(8)}{topology.PadRight(9)}{division.PadRight(3)} {dateText}")
        End Sub

        Private Sub __writeAccession(writer As TextWriter, accession As ACCESSION, version As VERSION)
            If accession IsNot Nothing AndAlso Not String.IsNullOrEmpty(accession.AccessionId) Then
                Call writer.WriteLine($"{"ACCESSION".PadRight(FIELD_WIDTH)}{accession.AccessionId}")
            End If

            If version IsNot Nothing AndAlso Not version.IsEmpty Then
                Dim text As New StringBuilder($"{"VERSION".PadRight(FIELD_WIDTH)}{version.VersionString}")

                If Not String.IsNullOrEmpty(version.GI) Then
                    Call text.Append($"  GI:{version.GI}")
                End If

                Call writer.WriteLine(text.ToString())
            End If
        End Sub

        Private Sub __writeDblink(writer As TextWriter, links As DBLINK)
            Dim first As Boolean = True

            For Each link As NamedValue(Of String) In links.Links
                Dim text As String = If(String.IsNullOrEmpty(link.Value), link.Name, $"{link.Name}: {link.Value}")

                If first Then
                    Call writer.WriteLine($"{"DBLINK".PadRight(FIELD_WIDTH)}{text}")
                    first = False
                Else
                    Call writer.WriteLine($"{New String(" "c, FIELD_WIDTH)}{text}")
                End If
            Next
        End Sub

        Private Function __keywords(keywords As Keywords.KEYWORDS) As String
            If keywords Is Nothing OrElse keywords.KeyWordList.IsNullOrEmpty Then
                Return "."
            Else
                Return String.Join("; ", keywords.KeyWordList)
            End If
        End Function

        ''' <summary>
        ''' 写出一个头部字段: 字段名占用12列，字段的值在第13列之后，超过行宽的时候自动折行
        ''' </summary>
        Private Sub __writeField(writer As TextWriter, name As String, value As String)
            Dim lines As String() = __wrap(value, GbkFormat.LINE_WIDTH - FIELD_WIDTH, GbkFormat.LINE_WIDTH - FIELD_WIDTH)
            Dim prefix As String = name.PadRight(FIELD_WIDTH)

            Call writer.WriteLine(prefix & lines(Scan0))

            For i As Integer = 1 To lines.Length - 1
                Call writer.WriteLine($"{New String(" "c, FIELD_WIDTH)}{lines(i)}")
            Next
        End Sub

        Private Sub __writeRawLines(writer As TextWriter, lines As String())
            For Each line As String In lines.SafeQuery
                Call writer.WriteLine(line)
            Next
        End Sub

        Private Sub __writeSource(writer As TextWriter, source As SOURCE)
            Call __writeField(writer, "SOURCE", If(source.SpeciesName, ""))

            Dim organism As ORGANISM = source.OrganismHierarchy

            If organism Is Nothing Then
                Return
            End If

            ' ORGANISM字段的名称占用第3列到第12列
            Dim lines As String() = __wrap(If(organism.SpeciesName, ""), GbkFormat.LINE_WIDTH - FIELD_WIDTH, GbkFormat.LINE_WIDTH - FIELD_WIDTH)

            Call writer.WriteLine($"{"  ORGANISM".PadRight(FIELD_WIDTH)}{lines(Scan0)}")

            For i As Integer = 1 To lines.Length - 1
                Call writer.WriteLine($"{New String(" "c, FIELD_WIDTH)}{lines(i)}")
            Next

            If Not organism.Lineage.IsNullOrEmpty Then
                Dim lineage As String = String.Join("; ", organism.Lineage)

                If Not lineage.EndsWith(".") Then
                    lineage &= "."
                End If

                For Each line As String In __wrap(lineage, GbkFormat.LINE_WIDTH - FIELD_WIDTH, GbkFormat.LINE_WIDTH - FIELD_WIDTH)
                    Call writer.WriteLine($"{New String(" "c, FIELD_WIDTH)}{line}")
                Next
            End If
        End Sub

        Private Sub __writeComment(writer As TextWriter, comment As COMMENT)
            If Not comment.Lines.IsNullOrEmpty Then
                ' 保留原始的换行位置
                Call __writeRawLines(writer, comment.Lines)
            ElseIf Not String.IsNullOrEmpty(comment.Comment) Then
                Call __writeField(writer, "COMMENT", comment.Comment)
            End If
        End Sub

#End Region

#Region "REFERENCE"

        Private Sub __writeReference(writer As TextWriter, reference As REFERENCE)
            For i As Integer = 0 To If(reference.ReferenceList Is Nothing, 0, reference.ReferenceList.Length) - 1
                Dim info As RefInfo = reference.ReferenceList(i)
                Dim head As New StringBuilder($"{"REFERENCE".PadRight(FIELD_WIDTH)}{If(info.Id > 0, info.Id, i + 1)}")

                If info.BaseLocation IsNot Nothing AndAlso info.BaseLocation.right > 0 Then
                    Call head.Append($"  (bases {info.BaseLocation.left} to {info.BaseLocation.right})")
                End If

                Call writer.WriteLine(head.ToString())

                Dim authors As String = String.Join(", ", info.Authors.SafeQuery)

                If Not String.IsNullOrEmpty(authors) Then
                    Call __writeSubField(writer, "AUTHORS", authors)
                End If
                If Not String.IsNullOrEmpty(info.Consrtm) Then
                    Call __writeSubField(writer, "CONSRTM", info.Consrtm)
                End If
                If Not String.IsNullOrEmpty(info.Title) Then
                    Call __writeSubField(writer, "TITLE", info.Title)
                End If
                If Not String.IsNullOrEmpty(info.Journal) Then
                    Call __writeSubField(writer, "JOURNAL", info.Journal)
                End If
                If Not String.IsNullOrEmpty(info.Pubmed) Then
                    ' PUBMED 字段的名称是从第4列开始的
                    Call __writeSubField(writer, "PUBMED", info.Pubmed, prefixSpaces:=3)
                End If
                If Not String.IsNullOrEmpty(info.Remark) Then
                    Call __writeSubField(writer, "REMARK", info.Remark)
                End If
            Next
        End Sub

        Private Sub __writeSubField(writer As TextWriter, name As String, value As String, Optional prefixSpaces As Integer = 2)
            Dim head As String = New String(" "c, prefixSpaces) & name

            Call __writeField(writer, head, value)
        End Sub

#End Region

#Region "FEATURES"

        Private Sub __writeFeatures(writer As TextWriter, features As FEATURES)
            Call writer.WriteLine(GbkFormat.FEATURES_HEADER)

            For Each feature As Feature In features
                Call __writeFeature(writer, feature)
            Next
        End Sub

        Private Sub __writeFeature(writer As TextWriter, feature As Feature)
            Dim key As String = If(feature.KeyName, "")
            Dim location As String = If(feature.Location Is Nothing, "", feature.Location.ToString())
            Dim prefix As String = "     " & key.PadRight(FEATURE_FIELD_WIDTH)
            Dim locLines As String() = __wrapLocation(location, GbkFormat.LINE_WIDTH - prefix.Length)

            Call writer.WriteLine(prefix & locLines(Scan0))

            For i As Integer = 1 To locLines.Length - 1
                Call writer.WriteLine($"{New String(" "c, GbkFormat.QUALIFIER_COLUMN)}{locLines(i)}")
            Next

            For Each attr As NamedValue(Of String) In feature.PairedValues
                Call __writeQualifier(writer, attr.Name, attr.Value)
            Next
        End Sub

        Private Sub __writeQualifier(writer As TextWriter, name As String, value As String)
            Dim text As String = __qualifierValue(name, value)
            Dim prefix As String = New String(" "c, GbkFormat.QUALIFIER_COLUMN) & "/" & name
            Dim lines As String()

            If String.IsNullOrEmpty(text) Then
                Call writer.WriteLine(prefix)
                Return
            End If

            prefix = prefix & "="
            lines = __wrap(text, GbkFormat.LINE_WIDTH - prefix.Length, GbkFormat.LINE_WIDTH - GbkFormat.QUALIFIER_COLUMN)


            Call writer.WriteLine(prefix & lines(Scan0))

            For i As Integer = 1 To lines.Length - 1
                Call writer.WriteLine($"{New String(" "c, GbkFormat.QUALIFIER_COLUMN)}{lines(i)}")
            Next
        End Sub

        Private Function __qualifierValue(name As String, value As String) As String
            If String.IsNullOrEmpty(value) Then
                Return ""
            End If

            If unquotedQualifiers.Contains(name) AndAlso __isNumber(value) Then
                Return value
            End If

            Return """" & value.Replace("""", """""") & """"
        End Function

        Private Function __isNumber(s As String) As Boolean
            For i As Integer = 0 To s.Length - 1
                If s(i) < "0"c OrElse s(i) > "9"c Then
                    Return False
                End If
            Next

            Return s.Length > 0
        End Function

#End Region

#Region "CONTIG / ORIGIN"

        Private Sub __writeContig(writer As TextWriter, contig As CONTIG)
            Dim text As String = contig.ToString()
            Dim lines As String() = __wrapLocation(text, GbkFormat.LINE_WIDTH - FIELD_WIDTH)

            Call writer.WriteLine($"{"CONTIG".PadRight(FIELD_WIDTH)}{lines(Scan0)}")

            For i As Integer = 1 To lines.Length - 1
                Call writer.WriteLine($"{New String(" "c, FIELD_WIDTH)}{lines(i)}")
            Next
        End Sub

        ''' <summary>
        ''' 保存基因组序列数据
        ''' </summary>
        ''' <param name="writer"></param>
        ''' <param name="origin"></param>
        Private Sub __writeOrigin(writer As TextWriter, origin As ORIGIN)
            If origin.Size <= 0 Then
                Return
            End If

            Call writer.WriteLine("ORIGIN")

            Dim seq As String = origin.SequenceData
            Dim buffer As Char() = New Char(GbkFormat.LINE_WIDTH - 1) {}
            Dim position As Integer = 1
            Dim offset As Integer = 0

            Do While offset < seq.Length
                Dim len As Integer = Math.Min(GbkFormat.SEQUENCE_PER_LINE, seq.Length - offset)
                Dim pos As String = position.ToString()
                Dim width As Integer = Math.Max(GbkFormat.POSITION_WIDTH, pos.Length)

                ' 位置号从第1列开始右对齐
                For i As Integer = 0 To width - 1
                    buffer(i) = " "c
                Next

                For i As Integer = 0 To pos.Length - 1
                    buffer(width - pos.Length + i) = pos(i)
                Next

                buffer(width) = " "c

                Dim j As Integer = width + 1

                For i As Integer = 0 To len - 1
                    If i > 0 AndAlso (i Mod GbkFormat.SEQUENCE_GROUP_SIZE) = 0 Then
                        buffer(j) = " "c
                        j += 1
                    End If

                    buffer(j) = seq(offset + i)
                    j += 1
                Next

                Call writer.Write(buffer, 0, j)
                Call writer.WriteLine()

                offset += len
                position += len
            Loop
        End Sub

#End Region

#Region "text wrapping"

        ''' <summary>
        ''' 按照标准的行宽进行折行: 优先在空格处折行
        ''' </summary>
        ''' <param name="text">目标文本</param>
        ''' <param name="firstWidth">第一行可用的列宽度</param>
        ''' <param name="width">后续行可用的列宽度</param>
        Private Function __wrap(text As String, firstWidth As Integer, width As Integer) As String()
            If firstWidth < 8 Then
                firstWidth = 8
            End If
            If width < 8 Then
                width = 8
            End If
            If String.IsNullOrEmpty(text) Then
                Return New String() {""}
            End If
            If text.Length <= firstWidth Then
                Return New String() {text}
            End If

            Dim lines As New List(Of String)
            Dim sb As New StringBuilder()
            Dim limit As Integer = firstWidth
            Dim i As Integer = 0

            Do While i < text.Length
                Dim space As Integer = text.IndexOf(" "c, i)

                If space < 0 Then
                    space = text.Length
                End If

                Dim token As String = text.Substring(i, space - i)
                Dim nextLength As Integer = If(sb.Length = 0, token.Length, sb.Length + 1 + token.Length)

                If nextLength > limit AndAlso sb.Length > 0 Then
                    Call lines.Add(sb.ToString())
                    Call sb.Clear()
                    limit = width
                ElseIf sb.Length > 0 Then
                    Call sb.Append(" "c)
                End If

                If token.Length > limit Then
                    ' 超长的单词(例如没有空白字符分隔的长编号)只能够硬切
                    Dim rest As String = token

                    Do While rest.Length > limit
                        If sb.Length > 0 Then
                            Call lines.Add(sb.ToString())
                            Call sb.Clear()
                            limit = width
                        End If

                        Call lines.Add(rest.Substring(0, limit))
                        rest = rest.Substring(limit)
                    Loop

                    Call sb.Append(rest)
                    limit = width
                Else
                    Call sb.Append(token)
                End If

                i = space + 1
            Loop

            If sb.Length > 0 Then
                Call lines.Add(sb.ToString())
            End If

            Return lines.ToArray()
        End Function

        ''' <summary>
        ''' 按照标准的行宽进行折行: 优先在逗号处折行(location或者contig字段)
        ''' </summary>
        Private Function __wrapLocation(text As String, firstWidth As Integer) As String()
            If firstWidth < 8 Then
                firstWidth = 8
            End If
            If String.IsNullOrEmpty(text) Then
                Return New String() {""}
            End If
            If text.Length <= firstWidth Then
                Return New String() {text}
            End If

            Dim lines As New List(Of String)
            Dim sb As New StringBuilder()
            Dim limit As Integer = firstWidth
            Dim i As Integer = 0

            Do While i < text.Length
                Dim comma As Integer = text.IndexOf(","c, i)

                If comma < 0 Then
                    comma = text.Length - 1
                End If

                Dim token As String = text.Substring(i, comma - i + 1)
                Dim nextLength As Integer = sb.Length + token.Length

                If sb.Length > 0 AndAlso nextLength > limit Then
                    Call lines.Add(sb.ToString())
                    Call sb.Clear()
                    limit = GbkFormat.LINE_WIDTH - GbkFormat.QUALIFIER_COLUMN
                End If

                Call sb.Append(token)

                If sb.Length > limit Then
                    ' 单个片段过长的时候只能够硬切
                    Call lines.Add(sb.ToString())
                    Call sb.Clear()
                End If

                i = comma + 1
            Loop

            If sb.Length > 0 Then
                Call lines.Add(sb.ToString())
            End If

            Return lines.ToArray()
        End Function

#End Region

#Region "兼容旧版本的ToString接口"

        <ExportAPI("ToString")>
        Public Function ToString(link As Keywords.DBLINK) As String
            If link Is Nothing Then
                Return ""
            End If

            Return link.ToString()
        End Function

        ''' <summary>
        ''' 保存基因组序列数据
        ''' </summary>
        <ExportAPI("ToString")>
        Public Function ToString(origin As Keywords.ORIGIN) As String
            If origin Is Nothing Then
                Return ""
            End If

            Using writer As New StringWriter()
                Call __writeOrigin(writer, origin)
                Return writer.ToString()
            End Using
        End Function

        ''' <summary>
        ''' 生成基因组位点注释数据
        ''' </summary>
        <ExportAPI("ToString")>
        Public Function ToString(features As Keywords.FEATURES.FEATURES) As String
            If features Is Nothing Then
                Return ""
            End If

            Using writer As New StringWriter()
                Call __writeFeatures(writer, features)
                Return writer.ToString()
            End Using
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(feature As Feature) As String
            If feature Is Nothing Then
                Return ""
            End If

            Using writer As New StringWriter()
                Call __writeFeature(writer, feature)
                Return writer.ToString()
            End Using
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(acc As Keywords.ACCESSION) As String
            If acc Is Nothing Then
                Return ""
            Else
                Return $"ACCESSION   {acc.AccessionId}"
            End If
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(version As Keywords.VERSION) As String
            If version Is Nothing OrElse version.IsEmpty Then
                Return ""
            End If
            If String.IsNullOrEmpty(version.GI) Then
                Return $"VERSION     {version.VersionString}"
            Else
                Return $"VERSION     {version.VersionString}  GI:{version.GI}"
            End If
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(keyword As Keywords.KEYWORDS) As String
            Return $"KEYWORDS    " & __keywords(keyword)
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(source As Keywords.SOURCE) As String
            If source Is Nothing Then
                Return ""
            End If

            Using writer As New StringWriter()
                Call __writeSource(writer, source)
                Return writer.ToString()
            End Using
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(locus As GBFF.Keywords.LOCUS) As String
            If locus Is Nothing Then
                Return ""
            End If

            Using writer As New StringWriter()
                Call __writeLocus(writer, locus)
                Return writer.ToString()
            End Using
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(def As GBFF.Keywords.DEFINITION) As String
            If def Is Nothing Then
                Return ""
            End If

            Using writer As New StringWriter()
                Call __writeField(writer, "DEFINITION", def.Value)
                Return writer.ToString()
            End Using
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(i As Integer, ref As Keywords.RefInfo) As String
            Dim reference As New Keywords.REFERENCE With {.ReferenceList = {ref}}

            Using writer As New StringWriter()
                Call __writeReference(writer, reference)
                Return writer.ToString()
            End Using
        End Function

        <ExportAPI("ToString")>
        Public Function ToString(ref As Keywords.REFERENCE) As String
            If ref Is Nothing Then
                Return ""
            End If

            Using writer As New StringWriter()
                Call __writeReference(writer, ref)
                Return writer.ToString()
            End Using
        End Function

#End Region
    End Module
End Namespace

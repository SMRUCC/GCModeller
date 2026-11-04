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
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' NCBI GenBank(GBFF) 平文件的解析模块。
    ''' </summary>
    ''' <remarks>
    ''' 这个版本的解析器是针对大型基因组（例如人基因组，单个gbff文件可以达到数Gb）重新设计的:
    ''' 
    ''' 1. **单遍流式解析**: 逐行读取，按照字段名分派到对应的解析器，不再需要先把整个文件
    '''    加载为一个字符串数组(旧版本使用<c>IO.File.ReadAllLines</c>，对于4.8Gb的人基因组文件
    '''    会产生6550万个字符串对象)。
    ''' 2. **增量构建**: ``FEATURES``段与``ORIGIN``段在读取的过程中直接构建对象，不会缓存
    '''    原始的文本行。
    ''' 3. **低内存模式**: 通过<see cref="GbkReadOptions.LoadSequence"/>可以关闭序列数据的内存加载，
    '''    此时序列数据保留在文件之中按需回读。
    ''' 4. **随机访问**: 通过<see cref="GbkRecordIndex"/>可以先建立记录的偏移量索引，
    '''    然后通过<see cref="File.LoadRecord(String, String, GbkReadOptions)"/>直接读取指定的记录。
    ''' </remarks>
    Public Module GbkParser

        ''' <summary>
        ''' 将gbff文件之中的第一条记录读取出来
        ''' </summary>
        ''' <param name="path">目标数据库文件的路径</param>
        Public Function Read(path As String) As File
            Return Read(path, GbkReadOptions.Default)
        End Function

        ''' <summary>
        ''' 将gbff文件之中的第一条记录读取出来
        ''' </summary>
        Public Function Read(path As String, options As GbkReadOptions) As File
            Using records As New GbkRecordReader(path, If(options, GbkReadOptions.Default), stopAfterFirst:=True)
                For Each gb As File In records
                    Return gb
                Next
            End Using

            Return Nothing
        End Function

        ''' <summary>
        ''' 从一个数据流之中读取第一条genbank记录
        ''' </summary>
        Public Function Read(stream As Stream, Optional defaultAccession As String = Nothing, Optional options As GbkReadOptions = Nothing) As File
            Using records As New GbkRecordReader(stream, defaultAccession, If(options, GbkReadOptions.Default), stopAfterFirst:=True)
                For Each gb As File In records
                    Return gb
                Next
            End Using

            Return Nothing
        End Function

        ''' <summary>
        ''' 流式读取数据库文件之中的全部genbank记录。
        ''' 
        ''' 由于这是一个流式的迭代器，每一条记录在被使用完毕之后就可以被垃圾回收，
        ''' 因此即使整个文件具有数Gb的大小，内存的峰值也只会是一个记录的大小。
        ''' </summary>
        ''' <param name="path">目标数据库文件的路径</param>
        ''' <param name="options">读取选项</param>
        Public Iterator Function LoadDatabase(path As String, Optional options As GbkReadOptions = Nothing) As IEnumerable(Of File)
            Using records As New GbkRecordReader(path, If(options, GbkReadOptions.Default))
                For Each gb As File In records
                    Yield gb
                Next
            End Using
        End Function

        ''' <summary>
        ''' 流式读取数据流之中的全部genbank记录(可以用于读取解压之后的gzip数据流)
        ''' </summary>
        ''' <param name="stream">数据流</param>
        ''' <param name="defaultAccession">当记录之中缺失编号的时候所使用默认编号</param>
        ''' <param name="options">读取选项</param>
        Public Iterator Function LoadDatabase(stream As Stream,
                                             Optional defaultAccession As String = Nothing,
                                             Optional options As GbkReadOptions = Nothing) As IEnumerable(Of File)

            Using records As New GbkRecordReader(stream, defaultAccession, If(options, GbkReadOptions.Default))
                For Each gb As File In records
                    Yield gb
                Next
            End Using
        End Function

        ''' <summary>
        ''' 按照记录的编号从数据库文件之中读取单条记录(随机访问)
        ''' </summary>
        ''' <param name="path">目标数据库文件的路径</param>
        ''' <param name="accession">记录的编号，即``LOCUS``行的第一个字段</param>
        ''' <param name="options">读取选项</param>
        ''' <returns>当目标记录不存在的时候返回空值</returns>
        Public Function LoadRecord(path As String, accession As String, Optional options As GbkReadOptions = Nothing) As File
            options = If(options, GbkReadOptions.Default)

            Dim index As GbkRecordIndex = GbkRecordIndex.Open(path, options)
            Dim record As GbkRecordInfo = Nothing

            If Not index.TryGetRecord(accession, record) Then
                Return Nothing
            End If

            Return LoadRecord(path, record, options)
        End Function

        ''' <summary>
        ''' 按照记录的偏移量信息读取单条记录
        ''' </summary>
        Public Function LoadRecord(path As String, record As GbkRecordInfo, Optional options As GbkReadOptions = Nothing) As File
            Using records As New GbkRecordReader(path, If(options, GbkReadOptions.Default), startOffset:=record.LocusOffset, stopAfterFirst:=True)
                For Each gb As File In records
                    Return gb
                Next
            End Using

            Return Nothing
        End Function
    End Module

    ''' <summary>
    ''' genbank记录的流式读取器
    ''' </summary>
    Friend NotInheritable Class GbkRecordReader : Implements IEnumerable(Of File), IDisposable

        Private ReadOnly reader As GbkRawReader
        Private ReadOnly options As GbkReadOptions
        Private ReadOnly defaultAccession As String
        Private ReadOnly path As String
        Private ReadOnly stopAfterFirst As Boolean

        ' ============ 当前记录的解析状态 ============
        Private gb As File
        Private section As String
        Private ReadOnly sectionLines As New List(Of String)
        Private features As FeatureBuilder
        Private rawSections As Dictionary(Of String, String())

        Private seq As Char()
        Private seqLen As Integer
        Private totalLetters As Long
        Private perLine As Integer
        Private originLineStart As Long
        Private originLines As Long
        Private anchorLetters As List(Of Long)
        Private anchorOffsets As List(Of Long)

        Sub New(path As String, options As GbkReadOptions, Optional startOffset As Long = 0, Optional stopAfterFirst As Boolean = False)
            Me.path = path
            Me.options = If(options, GbkReadOptions.Default)
            Me.defaultAccession = IO.Path.GetFileNameWithoutExtension(path)
            Me.stopAfterFirst = stopAfterFirst
            Me.reader = New GbkRawReader(path, Me.options.BufferSize)

            If startOffset > 0 Then
                Call Me.reader.Seek(startOffset)
            End If
        End Sub

        Sub New(stream As Stream, defaultAccession As String, options As GbkReadOptions, Optional stopAfterFirst As Boolean = False)
            Me.path = Nothing
            Me.options = If(options, GbkReadOptions.Default)
            Me.defaultAccession = If(String.IsNullOrEmpty(defaultAccession), "unknown", defaultAccession)
            Me.stopAfterFirst = stopAfterFirst
            Me.reader = New GbkRawReader(stream, Me.options.BufferSize)
        End Sub

        Public Iterator Function GetEnumerator() As IEnumerator(Of File) Implements IEnumerable(Of File).GetEnumerator
            Do While reader.ReadLine()
                Dim line As String = reader.Line

                If line.StartsWith("LOCUS", StringComparison.Ordinal) AndAlso GbkFormat.IsSectionHeader(line) Then
                    Dim record As File

                    Try
                        record = __readRecord()
                    Catch ex As Exception
                        Throw New GbkFormatException("failed to parse the genbank record!", record:=line, lineNumber:=reader.LineNumber, lineText:=line, innerException:=ex)
                    End Try

                    If record IsNot Nothing Then
                        Yield record

                        If stopAfterFirst Then
                            Exit Do
                        End If
                    End If
                End If
            Loop
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        ''' <summary>
        ''' 解析一条记录，调用的时候读取器要正好定位在``LOCUS``行上面
        ''' </summary>
        Private Function __readRecord() As File
            Call __reset()

            Dim line As String = reader.Line
            Dim keyword As String = Nothing
            Dim value As String = Nothing

            Call GbkFormat.SplitKeyword(line, keyword, value)

            gb.Locus = LOCUS.InternalParser(line)
            section = "LOCUS"

            Do While reader.ReadLine()
                line = reader.Line

                If line = GbkFormat.RECORD_DELIMITER Then
                    Exit Do
                End If

                If GbkFormat.IsSectionHeader(line) Then
                    Call __flushSection()
                    Call GbkFormat.SplitKeyword(line, keyword, value)

                    section = keyword.ToUpperInvariant()

                    Select Case section
                        Case "FEATURES"
                            features = New FeatureBuilder(__accession())
                        Case "ORIGIN"
                            Call __beginOrigin()
                        Case "LOCUS"
                            ' 记录之间缺失了 // 分隔符
                            Exit Do
                        Case Else
                            Call sectionLines.Add(line)
                    End Select
                Else
                    Select Case section
                        Case "FEATURES"
                            If features IsNot Nothing Then
                                Call features.Feed(line)
                            End If
                        Case "ORIGIN"
                            Call __addOriginLine(line)
                        Case Else
                            Call sectionLines.Add(line)
                    End Select
                End If
            Loop

            Call __flushSection()

            Return __complete()
        End Function

        Private Sub __reset()
            gb = New File
            section = Nothing
            features = Nothing

            Call sectionLines.Clear()

            rawSections = New Dictionary(Of String, String())(StringComparer.OrdinalIgnoreCase)

            seq = Nothing
            seqLen = 0
            totalLetters = 0
            perLine = 0
            originLineStart = -1
            originLines = 0
            anchorLetters = Nothing
            anchorOffsets = Nothing
        End Sub

        Private Function __accession() As String
            If gb.Locus IsNot Nothing AndAlso Not String.IsNullOrEmpty(gb.Locus.AccessionID) Then
                Return gb.Locus.AccessionID
            Else
                Return defaultAccession
            End If
        End Function

        ''' <summary>
        ''' 将当前累积的字段文本行解析为对应的对象
        ''' </summary>
        Private Sub __flushSection()
            If sectionLines.Count = 0 Then
                Return
            End If

            Dim name As String = section
            Dim lines As String() = sectionLines.ToArray()

            Call sectionLines.Clear()

            Select Case name
                Case "DEFINITION"
                    gb.Definition = lines
                Case "ACCESSION"
                    gb.Accession = ACCESSION.CreateObject(lines, defaultAccession)
                Case "VERSION"
                    gb.Version = lines
                Case "DBLINK"
                    gb.DbLinks = DBLINK.Parser(lines)
                Case "KEYWORDS"
                    gb.Keywords = Keywords.KEYWORDS.__innerParser(lines)
                Case "SOURCE"
                    gb.Source = lines
                Case "REFERENCE"
                    gb.Reference = REFERENCE.InternalParser(lines)
                Case "COMMENT"
                    gb.Comment = lines
                Case "CONTIG"
                    gb.Contig = CONTIG.Parser(lines)
                Case "PRIMARY"
                    gb.Primary = PRIMARY.Parser(lines)
                Case "SEGMENT"
                    gb.Segment = __parseSegment(lines)
                Case "PROJECT"
                    gb.Project = ACCESSION.__fieldValue(lines(Scan0))
                Case "WGS", "WGS_SCAFLD"
                    gb.Wgs = ACCESSION.__fieldValue(lines(Scan0))
                Case Else
                    rawSections(name) = lines
            End Select
        End Sub

        Private Shared Function __parseSegment(lines As String()) As String
            Dim value As String = If(lines.IsNullOrEmpty, "", ACCESSION.__fieldValue(lines(Scan0)))

            Return value
        End Function

        Private Sub __beginOrigin()
            seq = Nothing
            seqLen = 0
            totalLetters = 0
            perLine = 0
            originLines = 0
            originLineStart = -1
            anchorLetters = New List(Of Long)
            anchorOffsets = New List(Of Long)

            If options.LoadSequence Then
                Dim hint As Integer = 0

                If gb.Locus IsNot Nothing Then
                    hint = gb.Locus.Length
                End If

                If hint <= 0 Then
                    hint = 1024
                End If

                seq = New Char(hint - 1) {}
            End If
        End Sub

        ''' <summary>
        ''' 处理一行``ORIGIN``段的序列数据。
        ''' 
        ''' 这里不按照固定的列位置来截取序列，因为当序列的位置大于1亿的时候，
        ''' 行首的位置号会占据9列，从而使得序列数据的起始列位置发生变化。
        ''' </summary>
        Private Sub __addOriginLine(line As String)
            If originLineStart < 0 Then
                originLineStart = reader.LineStart
                Call anchorLetters.Add(0)
                Call anchorOffsets.Add(originLineStart)
            End If

            Dim letters As Integer = NucleotideSlice.CountLetters(line)

            If letters > perLine Then
                perLine = letters
            End If

            If options.LoadSequence Then
                If seqLen + letters > seq.Length Then
                    Call __growSequence(seqLen + letters)
                End If

                seqLen += NucleotideSlice.ExtractLetters(line, seq, seqLen)
            End If

            totalLetters += letters
            originLines += 1

            Dim interval As Integer = options.SequenceAnchorInterval

            If interval > 0 AndAlso (originLines Mod interval) = 0 Then
                Call anchorLetters.Add(totalLetters)
                Call anchorOffsets.Add(reader.LineEnd)
            End If
        End Sub

        Private Sub __growSequence(needed As Integer)
            Dim size As Integer = If(seq IsNot Nothing AndAlso seq.Length > 0, seq.Length * 2, 1024)

            If size < needed Then
                size = needed
            End If

            Dim tmp As Char() = New Char(size - 1) {}

            If seqLen > 0 Then
                Call Array.Copy(seq, tmp, seqLen)
            End If

            seq = tmp
        End Sub

        ''' <summary>
        ''' 完成一条记录的构建: 填充缺失字段的默认值，并且建立对象之间的引用关系
        ''' </summary>
        Private Function __complete() As File
            If features IsNot Nothing Then
                gb.Features = features.Complete()
            ElseIf options.LoadFeatures Then
                gb.Features = New FEATURES
            End If

            If gb.Features Is Nothing Then
                gb.Features = New FEATURES
            End If

            If gb.Comment Is Nothing Then
                gb.Comment = New COMMENT With {.Comment = ""}
            End If
            If gb.Definition Is Nothing Then
                gb.Definition = New DEFINITION With {.Value = ""}
            End If
            If gb.Accession Is Nothing Then
                gb.Accession = ACCESSION.CreateObject(Nothing, defaultAccession)
            End If
            If gb.Version Is Nothing Then
                gb.Version = New VERSION
            End If
            If gb.Keywords Is Nothing Then
                gb.Keywords = New Keywords.KEYWORDS With {.KeyWordList = New List(Of String)}
            End If
            If gb.DbLinks Is Nothing Then
                gb.DbLinks = New DBLINK With {.Links = New NamedValue(Of String)() {}}
            End If
            If gb.Reference Is Nothing Then
                gb.Reference = New REFERENCE With {.ReferenceList = New RefInfo() {}}
            End If
            If gb.Source Is Nothing Then
                gb.Source = New SOURCE
            End If
            If gb.Locus Is Nothing Then
                gb.Locus = New LOCUS With {.AccessionID = defaultAccession}
            End If
            If rawSections.Count > 0 Then
                gb.RawSections = rawSections
            End If

            Dim origin As New ORIGIN

            If options.LoadSequence Then
                If seq Is Nothing OrElse seqLen = 0 Then
                    origin.SequenceData = ""
                Else
                    origin.SequenceData = New String(seq, 0, seqLen)
                End If
            ElseIf Not String.IsNullOrEmpty(path) AndAlso originLineStart >= 0 Then
                origin.Provider = New FileBackedSequenceProvider(
                    path,
                    originLineStart,
                    totalLetters,
                    perLine,
                    anchorLetters.ToArray(),
                    anchorOffsets.ToArray())
            Else
                origin.Provider = New InMemorySequenceProvider("")
            End If

            gb.Origin = origin

            gb.Accession.gb = gb
            gb.Comment.gb = gb
            gb.Definition.gb = gb
            gb.Features.gb = gb
            gb.Keywords.gb = gb
            gb.Locus.gb = gb
            gb.Reference.gb = gb
            gb.Source.gb = gb
            gb.Version.gb = gb
            gb.DbLinks.gb = gb
            gb.Origin.gb = gb

            Call gb.Features.LinkEntry()

            Return gb
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Call reader.Dispose()
        End Sub
    End Class
End Namespace

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

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' 数据库文件之中一条genbank记录的位置信息
    ''' </summary>
    Public Class GbkRecordInfo

        ''' <summary>
        ''' 记录的编号(``LOCUS``行的第一个字段)
        ''' </summary>
        Public Property Accession As String
        ''' <summary>
        ''' ``LOCUS``行起始字节在文件之中的偏移量
        ''' </summary>
        Public Property LocusOffset As Long
        ''' <summary>
        ''' 记录结束符(``//``)之后的字节偏移量
        ''' </summary>
        Public Property EndOffset As Long

        ''' <summary>
        ''' 记录所占用的字节长度
        ''' </summary>
        Public ReadOnly Property Length As Long
            Get
                If EndOffset > LocusOffset Then
                    Return EndOffset - LocusOffset
                Else
                    Return 0
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Accession} @{LocusOffset} (+{Length} bytes)"
        End Function
    End Class

    ''' <summary>
    ''' Genbank 数据库文件的记录偏移量索引。
    ''' </summary>
    ''' <remarks>
    ''' 对于人基因组这种具有数Gb大小的gbff文件而言，如果需要读取其中某一条染色体记录，
    ''' 从头扫描一遍整个文件是不可接受的。这个类型会通过一次顺序扫描建立``LOCUS``行与
    ''' ``//``结束符的字节偏移量索引，之后<see cref="GbkParser.LoadRecord(String, String, GbkReadOptions)"/>
    ''' 就可以直接``Seek``到目标记录，实现秒级的随机访问。
    ''' 
    ''' 索引会缓存到磁盘之中(<c>{gbff路径}.idx</c>)，缓存的有效性通过文件的大小以及
    ''' 最后修改时间来进行校验。
    ''' </remarks>
    Public NotInheritable Class GbkRecordIndex

        Private Const CACHE_VERSION As String = "gbff-record-index-v1"

        Private ReadOnly records As New Dictionary(Of String, GbkRecordInfo)(StringComparer.OrdinalIgnoreCase)
        Private ReadOnly ordered As New List(Of GbkRecordInfo)

        Private _cachePath As String

        ''' <summary>
        ''' 索引缓存文件的路径
        ''' </summary>
        Public ReadOnly Property CachePath As String
            Get
                Return _cachePath
            End Get
        End Property

        ''' <summary>
        ''' 记录的数量
        ''' </summary>
        Public ReadOnly Property Count As Integer
            Get
                Return ordered.Count
            End Get
        End Property

        ''' <summary>
        ''' 按照记录在文件之中的先后顺序返回全部记录的位置信息
        ''' </summary>
        Public ReadOnly Property Items As GbkRecordInfo()
            Get
                Return ordered.ToArray
            End Get
        End Property

        ''' <summary>
        ''' 按照记录的编号获取位置信息，记录不存在的时候返回空值
        ''' </summary>
        Default Public ReadOnly Property Item(accession As String) As GbkRecordInfo
            Get
                Return GetRecord(accession)
            End Get
        End Property

        ''' <summary>
        ''' 按照记录的编号获取位置信息，记录不存在的时候返回空值
        ''' </summary>
        Public Function GetRecord(accession As String) As GbkRecordInfo
            Dim record As GbkRecordInfo = Nothing

            If TryGetRecord(accession, record) Then
                Return record
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 按照记录的编号获取位置信息
        ''' </summary>
        ''' <param name="accession">记录的编号</param>
        ''' <param name="record">目标记录的位置信息</param>
        ''' <returns>目标记录是否存在</returns>
        Public Function TryGetRecord(accession As String, ByRef record As GbkRecordInfo) As Boolean
            If String.IsNullOrEmpty(accession) Then
                record = Nothing
                Return False
            End If

            Return records.TryGetValue(accession.Trim(), record)
        End Function

        Private Sub __add(record As GbkRecordInfo)
            If record Is Nothing OrElse String.IsNullOrEmpty(record.Accession) Then
                Return
            End If

            If records.ContainsKey(record.Accession) Then
                Return
            End If

            records(record.Accession) = record
            Call ordered.Add(record)
        End Sub

        ''' <summary>
        ''' 打开(或者建立)目标数据库文件的记录偏移量索引
        ''' </summary>
        ''' <param name="path">数据库文件的路径</param>
        ''' <param name="options">读取选项</param>
        Public Shared Function Open(path As String, Optional options As GbkReadOptions = Nothing) As GbkRecordIndex
            options = If(options, GbkReadOptions.Default)

            If Not IO.File.Exists(path) Then
                Throw New IO.FileNotFoundException($"the target genbank database file '{path}' does not exist!", path)
            End If

            Dim cache As String = __cachePath(path)
            Dim info As New IO.FileInfo(path)

            If options.BuildRecordIndexCache AndAlso IO.File.Exists(cache) Then
                Dim cached As GbkRecordIndex = __loadCache(cache, info)

                If cached IsNot Nothing Then
                    Return cached
                End If
            End If

            Dim index As GbkRecordIndex = Scan(path)

            index._cachePath = cache
            Call index.SetSource(path)

            If options.BuildRecordIndexCache Then
                Try
                    Call index.Save(cache)
                Catch ex As Exception
                    Call App.LogException(New GbkFormatException($"failed to save the record index cache of '{path}'!", innerException:=ex))
                End Try
            End If

            Return index
        End Function

        ''' <summary>
        ''' 顺序扫描整个数据库文件，建立记录偏移量索引
        ''' </summary>
        ''' <param name="path">数据库文件的路径</param>
        Public Shared Function Scan(path As String) As GbkRecordIndex
            Dim index As New GbkRecordIndex()
            Dim current As GbkRecordInfo = Nothing

            Using reader As New GbkRawReader(path)
                Do While reader.ReadLine()
                    Dim line As String = reader.Line

                    If line.StartsWith("LOCUS", StringComparison.Ordinal) AndAlso GbkFormat.IsSectionHeader(line) Then
                        ' 上一条记录缺失了 // 结束符
                        If current IsNot Nothing Then
                            current.EndOffset = reader.LineStart
                            Call index.__add(current)
                            current = Nothing
                        End If

                        current = New GbkRecordInfo With {
                            .Accession = __accession(line),
                            .LocusOffset = reader.LineStart,
                            .EndOffset = 0
                        }
                    ElseIf current IsNot Nothing AndAlso String.Equals(line, GbkFormat.RECORD_DELIMITER, StringComparison.Ordinal) Then
                        current.EndOffset = reader.LineEnd
                        Call index.__add(current)
                        current = Nothing
                    End If
                Loop

                If current IsNot Nothing Then
                    current.EndOffset = reader.LineEnd
                    Call index.__add(current)
                End If
            End Using

            index._cachePath = __cachePath(path)
            Call index.SetSource(path)

            Return index
        End Function

        Private Shared Function __accession(locusLine As String) As String
            Dim tokens As String() = locusLine.Split({" "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)

            If tokens.Length >= 2 Then
                Return tokens(1)
            Else
                Return Nothing
            End If
        End Function

        Private Shared Function __cachePath(path As String) As String
            Return path & ".idx"
        End Function

        ''' <summary>
        ''' 将索引写出到缓存文件之中
        ''' </summary>
        Public Function Save(cachePath As String) As Boolean
            If String.IsNullOrEmpty(cachePath) Then
                Return False
            End If

            Dim info As New IO.FileInfo(cachePath)

            If Not String.IsNullOrEmpty(info.DirectoryName) AndAlso Not info.Directory.Exists Then
                Call info.Directory.Create()
            End If

            Using writer As New IO.StreamWriter(cachePath, append:=False, encoding:=Text.Encoding.UTF8)
                ' 缓存头: 版本号 + 源文件的大小 + 源文件的最后修改时间
                Call writer.WriteLine(String.Join(vbTab, {
                    CACHE_VERSION,
                    _sourceLength.ToString(),
                    _sourceUpdateTime.ToString()
                }))

                For Each record As GbkRecordInfo In ordered
                    Call writer.WriteLine(String.Join(vbTab, {
                        record.Accession,
                        record.LocusOffset.ToString(),
                        record.EndOffset.ToString()
                    }))
                Next
            End Using

            _cachePath = cachePath

            Return True
        End Function

        Private _sourceLength As Long
        Private _sourceUpdateTime As Long

        ''' <summary>
        ''' 设置索引所对应的源文件的校验信息(文件大小以及最后修改时间)
        ''' </summary>
        Public Sub SetSource(path As String)
            Dim info As New IO.FileInfo(path)

            If info.Exists Then
                _sourceLength = info.Length
                _sourceUpdateTime = info.LastWriteTimeUtc.Ticks
            End If
        End Sub

        Private Shared Function __loadCache(cachePath As String, source As IO.FileInfo) As GbkRecordIndex
            Try
                Using reader As New IO.StreamReader(cachePath, Text.Encoding.UTF8)
                    Dim header As String = reader.ReadLine()

                    If String.IsNullOrEmpty(header) Then
                        Return Nothing
                    End If

                    Dim tokens As String() = header.Split(ControlChars.Tab)

                    If tokens.Length < 3 OrElse Not String.Equals(tokens(0), CACHE_VERSION, StringComparison.Ordinal) Then
                        Return Nothing
                    End If

                    Dim size As Long
                    Dim updateTime As Long

                    If Not Long.TryParse(tokens(1), size) OrElse Not Long.TryParse(tokens(2), updateTime) Then
                        Return Nothing
                    End If

                    ' 缓存有效性校验: 文件的大小以及最后修改时间都需要一致
                    If size <> source.Length OrElse updateTime <> source.LastWriteTimeUtc.Ticks Then
                        Return Nothing
                    End If

                    Dim index As New GbkRecordIndex()

                    index._cachePath = cachePath
                    index._sourceLength = size
                    index._sourceUpdateTime = updateTime

                    Do While True
                        Dim line As String = reader.ReadLine()

                        If line Is Nothing Then
                            Exit Do
                        End If
                        If line.Length = 0 Then
                            Continue Do
                        End If

                        Dim cols As String() = line.Split(ControlChars.Tab)

                        If cols.Length < 3 Then
                            Continue Do
                        End If

                        Dim record As New GbkRecordInfo With {.Accession = cols(0)}
                        Dim locusOffset As Long
                        Dim endOffset As Long

                        If Not Long.TryParse(cols(1), locusOffset) Then
                            Continue Do
                        End If
                        If Not Long.TryParse(cols(2), endOffset) Then
                            Continue Do
                        End If

                        record.LocusOffset = locusOffset
                        record.EndOffset = endOffset

                        Call index.__add(record)
                    Loop

                    Return index
                End Using
            Catch ex As Exception
                Call App.LogException(New GbkFormatException($"failed to load the record index cache '{cachePath}'!", innerException:=ex))
                Return Nothing
            End Try
        End Function
    End Class
End Namespace

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
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' NCBI GenBank database file.
    ''' </summary>
    ''' <remarks>
    ''' The National Center for Biotechnology Information (NCBI) GenBank® database is a comprehensive, 
    ''' annotated collection of all publicly available nucleotide sequences and their protein translations. 
    ''' 
    ''' 文件结构说明: 
    ''' 
    ''' - ``LOCUS``: 记录的摘要信息(编号/长度/分子类型/拓扑结构/分类代码/更新日期)
    ''' - ``DEFINITION``: 记录的描述信息
    ''' - ``ACCESSION``/``VERSION``: 记录的编号以及版本号
    ''' - ``DBLINK``: 关联的外部数据库编号
    ''' - ``KEYWORDS``: 关键词
    ''' - ``SOURCE``/``ORGANISM``: 物种信息以及分类学谱系
    ''' - ``REFERENCE``: 引用文献
    ''' - ``COMMENT``: 附加的注释说明
    ''' - ``FEATURES``: 位点注释数据表
    ''' - ``CONTIG``: ``CON``类型的记录之中用于拼接序列的片段列表
    ''' - ``ORIGIN``: 序列数据
    ''' </remarks>
    <Package("NCBI.Genbank.GBFF")>
    Public Class File : Implements ISaveHandle

        Public Property Comment As Keywords.COMMENT
        ''' <summary>
        ''' This GenBank keyword section stores the sequence data for this database.
        ''' </summary>
        ''' <returns></returns>
        Public Property Origin As Keywords.ORIGIN
        Public Property Features As Keywords.FEATURES.FEATURES
        ''' <summary>
        ''' LocusID, GI or AccessionID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Accession As Keywords.ACCESSION
        Public Property Reference As Keywords.REFERENCE
        ''' <summary>
        ''' The definition value for this organism's GenBank data.
        ''' </summary>
        Public Property Definition As Keywords.DEFINITION
        Public Property Version As Keywords.VERSION

        ''' <summary>
        ''' 物种信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Source As Keywords.SOURCE
        ''' <summary>
        ''' The brief entry information of this genbank data.
        ''' </summary>
        Public Property Locus As Keywords.LOCUS
        Public Property Keywords As Keywords.KEYWORDS
        Public Property DbLinks As DBLINK

        ''' <summary>
        ''' ``CONTIG``段: 对于``CON``类型的记录(例如人基因组的染色体)，这个字段描述了
        ''' 序列是由哪些片段拼接而成的
        ''' </summary>
        Public Property Contig As Keywords.CONTIG
        ''' <summary>
        ''' ``PRIMARY``段
        ''' </summary>
        Public Property Primary As Keywords.PRIMARY
        ''' <summary>
        ''' ``SEGMENT``段的值(形如``1 of 10``)
        ''' </summary>
        Public Property Segment As String
        ''' <summary>
        ''' ``PROJECT``段的值
        ''' </summary>
        Public Property Project As String
        ''' <summary>
        ''' ``WGS``/``WGS_SCAFLD``段的值
        ''' </summary>
        Public Property Wgs As String
        ''' <summary>
        ''' 未被显式解析的其它字段的原始文本行: 保留这些数据用于保证解析之后再写出的往返保真性
        ''' </summary>
        Public Property RawSections As Dictionary(Of String, String())

        ''' <summary>
        ''' 这个记录是否为``CON``(constructed)类型
        ''' </summary>
        Public ReadOnly Property IsCON As Boolean
            Get
                Return Locus IsNot Nothing AndAlso Locus.IsCON
            End Get
        End Property

        ''' <summary>
        ''' 分子的拓扑结构
        ''' </summary>
        Public ReadOnly Property Topology As String
            Get
                If Locus Is Nothing Then
                    Return Nothing
                Else
                    Return Locus.Topology
                End If
            End Get
        End Property

        ''' <summary>
        ''' Is plasmid source?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>(这个Genbank对象是否为一个质粒的基因组数据)</remarks>
        Public ReadOnly Property isPlasmid As Boolean
            Get
                Return Not String.IsNullOrEmpty(Features.source.Query("plasmid"))
            End Get
        End Property

        ''' <summary>
        ''' the organism species name and strain information of this genbank data, 
        ''' this property is a combination of the source feature's species name and 
        ''' the source feature's strain qualifier value.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Species As String
            Get
                Dim name As String = ""

                If Source IsNot Nothing Then
                    name = Source.SpeciesName
                End If
                If Features IsNot Nothing AndAlso Features.source IsNot Nothing Then
                    name = name & " " & Features.source.Query("strain")
                End If

                Return name
            End Get
        End Property

        ''' <summary>
        ''' get ncbi taxonomy id
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 物种数据
        ''' </remarks>
        Public ReadOnly Property Taxon As String
            Get
                If Features Is Nothing OrElse Features.source Is Nothing Then
                    Return Nothing
                End If

                For Each s As String In Features.source.QueryDuplicated("db_xref")
                    Dim i As Integer = s.IndexOf(":"c)

                    If i > 0 AndAlso String.Equals(s.Substring(0, i), "taxon", StringComparison.OrdinalIgnoreCase) Then
                        Return s.Substring(i + 1).Trim()
                    End If
                Next

                Return Nothing
            End Get
        End Property

        ''' <summary>
        ''' 这个Genbank对象是否具有序列数据
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' 在低内存模式之下，这个属性不会导致序列数据被加载到内存之中。
        ''' </remarks>
        Public ReadOnly Property HasSequenceData As Boolean
            Get
                If Origin Is Nothing Then
                    Return False
                Else
                    Return Origin.Size > 0
                End If
            End Get
        End Property

        ''' <summary>
        ''' This GenBank data is the WGS(Whole genome shotgun) type data.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsWGS As Boolean
            Get
                If Definition IsNot Nothing AndAlso Not String.IsNullOrEmpty(Definition.Value) Then
                    For Each KeyWord As String In GenBank.GBFF.Keywords.DEFINITION.WGSKeywords
                        If Definition.Value.IndexOf(KeyWord, StringComparison.OrdinalIgnoreCase) >= 0 Then
                            Return True
                        End If
                    Next
                End If

                If Keywords IsNot Nothing Then
                    For Each s As String In Keywords
                        For Each KeyWord As String In GenBank.GBFF.Keywords.DEFINITION.WGSKeywords
                            If s.StartsWith(KeyWord, StringComparison.OrdinalIgnoreCase) Then
                                Return True
                            End If
                        Next
                    Next
                End If

                Return False
            End Get
        End Property

        ''' <summary>
        ''' Gets the original source brief entry information of this genome.(获取这个基因组的摘要信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SourceFeature As Feature
            Get
                Return Features.source
            End Get
        End Property

        ''' <summary>
        ''' 提取一个feature所对应的核酸序列: 多段位点会按照顺序拼接，并且会在需要的时候反向互补
        ''' </summary>
        ''' <param name="feature">The target feature site on the genome DNA sequence.</param>
        Public Function ReadSequence(feature As Feature) As String
            If Origin Is Nothing OrElse feature Is Nothing Then
                Return ""
            Else
                Return Origin.GetSequence(feature)
            End If
        End Function

        ''' <summary>
        ''' Read the gene nucleic acid sequence of a gene feature and then returns a fasta sequence object.
        ''' (读取一个基因特性的核酸序列，该Feature对象可以为任意形式的Qualifier的值，但是必需要具有Location属性)
        ''' </summary>
        ''' <param name="feature">The target feature site on the genome DNA sequence.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Function Read(feature As Feature) As FASTA.FastaSeq
            Dim sequence As String = ReadSequence(feature)

            Dim fa As New FASTA.FastaSeq With {
                .SequenceData = sequence,
                .Headers = New String() {
                    "Feature",
                    If(feature.Location Is Nothing, "", feature.Location.ToString),
                    feature.KeyName
                }
            }

            Return fa
        End Function

        ''' <summary>
        ''' Read a specific GenBank database text file.
        ''' (读取一个特定的GenBank数据库文件)
        ''' </summary>
        ''' <param name="Path">The target database text file to read.(所要读取的目标数据库文件)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Shadows Widening Operator CType(Path As String) As NCBI.GenBank.GBFF.File
            Return GbkParser.Read(Path)
        End Operator

        ''' <summary>
        ''' 读取目标genbank文件之中的第一条记录
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Load")>
        Public Shared Function Load(path As String) As NCBI.GenBank.GBFF.File
            Return GbkParser.Read(path)
        End Function

        ''' <summary>
        ''' 读取目标genbank文件之中的第一条记录
        ''' </summary>
        Public Shared Function Load(path As String, options As GbkReadOptions) As NCBI.GenBank.GBFF.File
            Return GbkParser.Read(path, If(options, GbkReadOptions.Default))
        End Function

        ''' <summary>
        ''' 按照记录的编号从数据库文件之中读取单条记录(随机访问)。
        ''' 
        ''' 这个函数会首先使用<see cref="GbkRecordIndex"/>定位到目标记录，因此对于具有
        ''' 数Gb大小的人基因组数据库文件，也可以以秒级的时间读取到指定的染色体记录。
        ''' </summary>
        ''' <param name="path">数据库文件</param>
        ''' <param name="accession">记录的编号</param>
        ''' <param name="options">读取选项</param>
        Public Shared Function LoadRecord(path As String,
                                          accession As String,
                                          Optional options As GbkReadOptions = Nothing) As NCBI.GenBank.GBFF.File

            Return GbkParser.LoadRecord(path, accession, If(options, GbkReadOptions.Default))
        End Function

        ''' <summary>
        ''' 按照记录的编号从数据库文件之中读取单条记录，记录不存在的时候返回<c>False</c>
        ''' </summary>
        Public Shared Function TryLoadRecord(path As String,
                                             accession As String,
                                             ByRef gb As NCBI.GenBank.GBFF.File,
                                             Optional options As GbkReadOptions = Nothing) As Boolean

            gb = LoadRecord(path, accession, options)

            Return gb IsNot Nothing
        End Function

        ''' <summary>
        ''' 检查目标文件是否为Genbank文件格式
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function IsValidGenbankFormat(path As String) As Boolean
            Static samplingHeaders As Index(Of String) = {
                "LOCUS", "DEFINITION", "ACCESSION", "VERSION", "DBLINK"
            }

            If Not IO.File.Exists(path) Then
                Return False
            End If

            Dim hits% = 0

            Using reader As New GbkRawReader(path, 8192)
                Do While hits < samplingHeaders.Count AndAlso reader.ReadLine()
                    Dim keyword As String = Nothing
                    Dim value As String = Nothing

                    Call GbkFormat.SplitKeyword(reader.Line, keyword, value)

                    If keyword Like samplingHeaders Then
                        hits += 1
                    End If
                Loop
            End Using

            Return hits / samplingHeaders.Count >= 0.5
        End Function

        Public Const GenbankMultipleRecordDelimiterRegexp$ = "^//$"
        Public Const GenbankMultipleRecordDelimiter$ = "//"

        ''' <summary>
        ''' Using this function to load the ncbi genbank database file if the database file 
        ''' contains more than one genome.
        ''' </summary>
        ''' <param name="filePath">The file path of the genbank database file, this gb file may contains sevral gb sections</param>
        ''' <returns></returns>
        ''' <remarks>      
        ''' (假若一个gbk文件之中包含有多个记录的话，可以使用这个函数进行数据的加载，多个genebank记录在一个文件之中
        ''' 一般出现在细菌具有染色体基因组和质粒基因组这种多个复制子的情况)</remarks>
        '''
        Public Shared Function LoadDatabase(filePath As String, Optional suppressError As Boolean = False) As IEnumerable(Of File)
            Return LoopLoad(filePath, GbkReadOptions.Default, suppressError)
        End Function

        ''' <summary>
        ''' 使用指定的读取选项加载数据库文件之中的全部记录
        ''' </summary>
        Public Shared Function LoadDatabase(filePath As String, options As GbkReadOptions, Optional suppressError As Boolean = False) As IEnumerable(Of File)
            Return LoopLoad(filePath, If(options, GbkReadOptions.Default), suppressError)
        End Function

        Private Shared Iterator Function LoopLoad(filePath As String, options As GbkReadOptions, suppressError As Boolean) As IEnumerable(Of File)
            If Not IO.File.Exists(filePath) Then
                Throw New FileNotFoundException($"the target genbank database file '{filePath}' does not exist!", filePath)
            End If

            If Not suppressError Then
                For Each gb As File In GbkParser.LoadDatabase(filePath, options)
                    Yield gb
                Next

                Return
            End If

            ' 单个记录的解析错误不会打断整个数据库文件的读取
            Using records As New GbkRecordReader(filePath, options)
                Dim enumerator As IEnumerator(Of File) = records.GetEnumerator()
                Dim gb As File = Nothing

                Do While True
                    Try
                        If Not enumerator.MoveNext() Then
                            Exit Do
                        End If

                        gb = enumerator.Current
                    Catch ex As Exception
                        Call App.LogException(New GbkFormatException($"error while parsing a record in '{filePath}'!", innerException:=ex))
                        Exit Do
                    End Try

                    Yield gb
                Loop
            End Using
        End Function

        ''' <summary>
        ''' Load multiple genbank assembly data that parsed from a specific data file
        ''' </summary>
        ''' <param name="file"></param>
        ''' <param name="defaultAccession"></param>
        ''' <param name="suppressError"></param>
        ''' <returns></returns>
        Public Shared Iterator Function LoadDatabase(file As Stream,
                                                     Optional defaultAccession$ = Nothing,
                                                     Optional suppressError As Boolean = False) As IEnumerable(Of File)
            If defaultAccession.StringEmpty Then
                If TypeOf file Is FileStream Then
                    defaultAccession = DirectCast(file, FileStream).Name.BaseName
                Else
                    defaultAccession = "unknown"
                End If
            End If

            Using records As New GbkRecordReader(file, defaultAccession, GbkReadOptions.Default)
                Dim enumerator As IEnumerator(Of File) = records.GetEnumerator()

                Do While True
                    Dim gb As File = Nothing

                    Try
                        If Not enumerator.MoveNext() Then
                            Exit Do
                        End If

                        gb = enumerator.Current
                    Catch ex As Exception
                        If suppressError Then
                            Exit Do
                        Else
                            Throw New Exception(defaultAccession, ex)
                        End If
                    End Try

                    Yield gb
                Loop
            End Using
        End Function

        Public Function Save(FilePath As String, Encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Using file As Stream = FilePath.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Return Save(file, Encoding)
            End Using
        End Function

        Public Function Save(s As Stream, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            ' 目标数据流由调用者管理，这里不会在写出完成之后关闭数据流
            Using wr As New StreamWriter(s, encoding, bufferSize:=1024 * 64, leaveOpen:=True)
                Return GbkWriter.Save(Me, wr)
            End Using
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, __getEncoding(encoding))
        End Function

        ''' <summary>
        ''' 将genbank记录以指定的代码页写出到文件之中
        ''' </summary>
        ''' <param name="path">目标文件的路径</param>
        ''' <param name="codePage">
        ''' 文本的代码页。GenBank是ASCII文本，所以一般使用<c>20127</c>(<see cref="Encoding.ASCII"/>的代码页)
        ''' </param>
        Public Function Save(path As String, codePage As Integer) As Boolean
            Return Save(path, __getEncoding(codePage))
        End Function

        Private Shared Function __getEncoding(codePage As Integer) As Encoding
            If codePage <= 0 Then
                Return System.Text.Encoding.Default
            End If

            Try
                Return System.Text.Encoding.GetEncoding(codePage)
            Catch ex As Exception
                Call App.LogException(New ArgumentException($"invalid code page '{codePage}'!", NameOf(codePage), ex))
                Return System.Text.Encoding.UTF8
            End Try
        End Function

        Private Shared Function __getEncoding(encoding As Encodings) As Encoding
            Return GbkWriter.GetEncoding(encoding)
        End Function
    End Class
End Namespace

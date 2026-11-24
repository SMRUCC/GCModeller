#Region "Microsoft.VisualBasic::c534f22d69f44f52f9ac1aa5e560611f, analysis\HTS_matrix\File\Document.vb"

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

'   Total Lines: 86
'    Code Lines: 52 (60.47%)
' Comment Lines: 22 (25.58%)
'    - Xml Docs: 95.45%
' 
'   Blank Lines: 12 (13.95%)
'     File Size: 2.96 KB


' Module Document
' 
'     Function: LoadMatrixDocument, ParseHeaders, SaveMatrix
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports BinaryMatrixCodec = Microsoft.VisualBasic.Serialization.BinaryDumping.NetworkByteOrderBuffer.Compression

Public Module Document

    <Extension>
    Public Function ParseHeaders(line As String) As String()
        Return line.Split(ASCII.TAB, ","c) _
           .Skip(1) _
           .Select(Function(s) s.Trim(""""c, " "c)) _
           .ToArray
    End Function

    Public Iterator Function ReadTable(path As String, deli As Char, Optional encoding As Encodings = Encodings.UTF8) As IEnumerable(Of String())
        Dim result As New Dictionary(Of String, Dictionary(Of String, String))(StringComparer.Ordinal)

        Using parser As New TextFieldParser(path, encoding.CodePage) With {
            .TextFieldType = FieldType.Delimited,
            .Delimiters = {deli},
            .HasFieldsEnclosedInQuotes = True
        }
            While Not parser.EndOfData
                Yield parser.ReadFields()
            End While
        End Using
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="file">
    ''' table file could be csv table or tsv table file.
    ''' </param>
    ''' <param name="excludes">
    ''' excludes some sample columns data which is speicifed 
    ''' by this parameter
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 因为矩阵文档是由数字构成的，所以在这里不再使用csv文件解析器来完成，
    ''' 直接通过分隔符进行解析来获取最好的解析性能
    ''' </remarks>
    Public Function LoadMatrixDocument(file As String, excludes As Index(Of String), Optional tqdm As Boolean = False) As Matrix
        Dim sampleIds As String() = file.ReadFirstLine.ParseHeaders
        Dim takeIndex As Integer() = parseTakeIndex(sampleIds, excludes)

        ' parse matrix data in stream model, the rows are parsed in parallel
        Dim matrix As DataFrameRow() = file _
            .LineIterators(strictFile:=True, tqdm:=tqdm) _
            .Skip(1) _
            .loadGeneMatrixParallel(takeIndex)

        Return New Matrix With {
            .expression = matrix,
            .sampleID = takeIndex _
                .Select(Function(i) sampleIds(i)) _
                .ToArray,
            .tag = file.FileName
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="samples">
    ''' make matrix subset by the sample id
    ''' </param>
    ''' <param name="tqdm"></param>
    ''' <returns></returns>
    Public Function LoadMatrixDocument(file As String, samples As IEnumerable(Of SampleInfo), Optional tqdm As Boolean = False) As Matrix
        Dim sampleIds As String() = file.ReadFirstLine.ParseHeaders
        Dim subset As Index(Of String) = samples.Keys.Indexing
        Dim takeIndex As Integer() = sampleIds _
            .Select(Function(name, i) (i, flag:=name Like subset)) _
            .Where(Function(a) a.flag = True) _
            .Select(Function(a) a.i) _
            .ToArray
        Dim excludes As String() = sampleIds.Where(Function(id) Not id Like subset).ToArray

        ' parse matrix data in stream model, the rows are parsed in parallel
        Dim matrix As DataFrameRow() = file _
            .LineIterators(strictFile:=True, tqdm:=tqdm) _
            .Skip(1) _
            .loadGeneMatrixParallel(takeIndex)

        Return New Matrix With {
            .expression = matrix,
            .sampleID = takeIndex _
                .Select(Function(i) sampleIds(i)) _
                .ToArray,
            .tag = file.FileName
        }
    End Function

    ''' <summary>
    ''' save matrix as csv file
    ''' </summary>
    ''' <param name="mat"></param>
    ''' <param name="file"></param>
    ''' <param name="idcolName"></param>
    ''' <returns></returns>
    <Extension>
    Public Function SaveMatrix(mat As Matrix, file As String, Optional idcolName As String = "geneID") As Boolean
        Using table As StreamWriter = file.OpenWriter(Encodings.UTF8WithoutBOM)
            Call WriteCsvHeader(table, mat.sampleID, idcolName)
            Call WriteCsvRows(table, mat.expression)
        End Using

        Return True
    End Function

    ''' <summary>
    ''' 写出矩阵文本文件的表头行
    ''' </summary>
    ''' <param name="table">输出流</param>
    ''' <param name="sampleIDs">样本（列）标签</param>
    ''' <param name="idcolName">第一列（行标签）的列名</param>
    <Extension>
    Public Sub WriteCsvHeader(table As TextWriter, sampleIDs As String(), Optional idcolName As String = "geneID")
        Call table.WriteLine({idcolName}.Join(sampleIDs).JoinBy(","))
    End Sub

    ''' <summary>
    ''' 逐行写出矩阵文本数据（流式，不要求整表驻留内存）
    ''' </summary>
    ''' <param name="table">输出流</param>
    ''' <param name="rows">惰性的矩阵行序列</param>
    <Extension>
    Public Sub WriteCsvRows(table As TextWriter, rows As IEnumerable(Of DataFrameRow))
        If rows Is Nothing Then
            Return
        End If

        For Each gene As DataFrameRow In rows
            Call table.WriteLine(formatRow(gene))
        Next
    End Sub

    <Extension>
    Private Function formatRow(gene As DataFrameRow) As String
        Return New String() {$"""{gene.geneID}"""} _
            .Join(gene.experiments.Select(Function(d) d.ToString)) _
            .JoinBy(",")
    End Function

    ''' <summary>
    ''' 惰性枚举 CSV/TSV 矩阵文件之中的数据行（自动跳过头部行）
    ''' </summary>
    ''' <param name="csvPath">矩阵文本文件</param>
    ''' <param name="excludes">需要排除的样本列</param>
    ''' <param name="tqdm">是否显示进度条</param>
    ''' <param name="degreeOfParallelism">解析并行度，0 = 全部逻辑核心</param>
    ''' <returns>按文件顺序惰性输出的行</returns>
    ''' <remarks>
    ''' 与 <see cref="LoadMatrixDocument(String, Index(Of String), Boolean)"/> 的区别是**不会**
    ''' 把整张矩阵读进内存：每批只解析 128 行，消费完即可回收。
    ''' </remarks>
    <Extension>
    Public Iterator Function StreamRows(csvPath As String,
                                        Optional excludes As Index(Of String) = Nothing,
                                        Optional tqdm As Boolean = False,
                                        Optional degreeOfParallelism As Integer = 0) As IEnumerable(Of DataFrameRow)

        Dim sampleIds As String() = csvPath.ReadFirstLine.ParseHeaders
        Dim takeIndex As Integer() = parseTakeIndex(sampleIds, excludes)

        For Each row As DataFrameRow In csvPath _
            .LineIterators(strictFile:=True, tqdm:=tqdm) _
            .Skip(1) _
            .ReadGeneRows(takeIndex, degreeOfParallelism)

            Yield row
        Next
    End Function

    ''' <summary>
    ''' CSV/TSV → 二进制 的流式单遍转换
    ''' </summary>
    ''' <param name="csvPath">源矩阵文本文件</param>
    ''' <param name="binPath">目标二进制文件</param>
    ''' <param name="excludes">需要排除的样本列</param>
    ''' <param name="codec">压缩方式</param>
    ''' <param name="blockRows">压缩分块行数，0 = 默认值</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 峰值内存 = 一个解析批次（约 128 行） + 一个写缓冲（不超过 4 MB），与矩阵总行数无关。
    ''' 转换中途异常会删除临时文件，不会留下被误认为合法的半成品。
    ''' </remarks>
    <Extension>
    Public Function ToBinary(csvPath As String,
                             binPath As String,
                             Optional excludes As Index(Of String) = Nothing,
                             Optional codec As BinaryMatrixCodec = BinaryMatrixCodec.none,
                             Optional blockRows As Integer = 0) As Boolean

        Dim sampleIds As String() = csvPath.ReadFirstLine.ParseHeaders
        Dim takeIndex As Integer() = parseTakeIndex(sampleIds, excludes)
        Dim samples As String() = takeIndex.Select(Function(i) sampleIds(i)).ToArray

        Using writer As HTSMatrixWriter = HTSMatrixWriter.CreateFile(
            path:=binPath,
            sampleIDs:=samples,
            featureIDs:=Nothing,
            tag:=csvPath.FileName,
            codec:=codec,
            blockRows:=blockRows
        )

            Call writer.WriteRows(StreamRows(csvPath, excludes))
            Call writer.Complete()
        End Using

        Return True
    End Function

    ''' <summary>
    ''' 二进制 → CSV 的流式导出
    ''' </summary>
    ''' <param name="binPath">源二进制矩阵文件</param>
    ''' <param name="csvPath">目标矩阵文本文件</param>
    ''' <param name="idcolName">第一列的列名</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToCsv(binPath As String, csvPath As String, Optional idcolName As String = "geneID") As Boolean
        Using reader As HTSMatrixReader = BinaryMatrix.Open(binPath)
            Using table As StreamWriter = csvPath.OpenWriter(Encodings.UTF8WithoutBOM)
                Call WriteCsvHeader(table, reader.SampleIDs.ToArray, idcolName)
                Call WriteCsvRows(table, reader.Rows())
            End Using
        End Using

        Return True
    End Function

    ''' <summary>
    ''' 计算需要保留的数值列下标（与 <see cref="LoadMatrixDocument(String, Index(Of String), Boolean)"/> 一致）
    ''' </summary>
    Private Function parseTakeIndex(sampleIds As String(), excludes As Index(Of String)) As Integer()
        If excludes Is Nothing Then
            Return sampleIds.Sequence.ToArray
        Else
            Return sampleIds _
                .Select(Function(name, i) (i, Not name Like excludes)) _
                .Where(Function(a) a.Item2 = True) _
                .Select(Function(a) a.i) _
                .ToArray
        End If
    End Function

    ''' <summary>
    ''' 从外部转录组向量文件读取单样本（或一组均值）的基因表达水平
    ''' 文件格式：两列基因名与表达值，支持 CSV/TSV，首行可为表头（gene / expression，大小写不敏感）
    ''' 例如：
    '''   gene,expression
    '''   codY,12.3
    '''   comK,4.5
    ''' 返回 基因名 → 表达值 的键值对字典。基因名按 OrdinalIgnoreCase 比较去重（后者覆盖前者）。
    ''' 若文件不存在或解析不到任何记录则抛出友好异常。
    ''' </summary>
    <Extension>
    Public Function ReadExpressionVector(path As String) As Dictionary(Of String, Double)
        If Not File.Exists(path) Then
            Throw New Exception(String.Format("外部转录组向量文件不存在: {0}", path))
        End If

        Dim lines As String() = File.ReadAllLines(path)
        If lines.Length = 0 Then
            Throw New Exception(String.Format("外部转录组向量文件为空: {0}", path))
        End If

        Dim delim As Char() = New Char() {","c, vbTab, ";"c}
        Dim result As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

        ' 判断首行是否为表头
        Dim startRow As Integer = 0
        Dim firstTokens As String() = lines(0).Split(delim, StringSplitOptions.RemoveEmptyEntries)
        If firstTokens.Length >= 2 Then
            Dim col0 As String = firstTokens(0).Trim()
            Dim col1 As String = firstTokens(1).Trim()
            If col0.Equals("gene", StringComparison.OrdinalIgnoreCase) OrElse
               col0.Equals("genes", StringComparison.OrdinalIgnoreCase) Then
                startRow = 1
            End If
        End If

        For i = startRow To lines.Length - 1
            Dim line As String = lines(i).Trim()
            If String.IsNullOrEmpty(line) Then Continue For

            Dim tokens As String() = line.Split(delim, StringSplitOptions.RemoveEmptyEntries)
            If tokens.Length < 2 Then Continue For

            Dim gene As String = tokens(0).Trim()
            Dim value As Double
            If Not Double.TryParse(tokens(1).Trim(), value) Then Continue For

            ' 按大小写不敏感去重：后者覆盖前者
            result(gene) = value
        Next

        If result.Count = 0 Then
            Throw New Exception(String.Format("外部转录组向量文件未解析到任何有效记录: {0}", path))
        End If

        Return result
    End Function
End Module

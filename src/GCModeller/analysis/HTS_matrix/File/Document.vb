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
        Dim takeIndex As Integer()

        If excludes Is Nothing Then
            takeIndex = sampleIds.Sequence.ToArray
        Else
            takeIndex = sampleIds _
                .Select(Function(name, i) (i, Not name Like excludes)) _
                .Where(Function(a) a.Item2 = True) _
                .Select(Function(a) a.i) _
                .ToArray
        End If

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
            Dim line As String = {idcolName}.Join(mat.sampleID).JoinBy(",")

            Call table.WriteLine(line)

            For Each gene As DataFrameRow In mat.expression
                line = New String() {$"""{gene.geneID}"""} _
                    .Join(gene.experiments.Select(Function(d) d.ToString)) _
                    .JoinBy(",")

                Call table.WriteLine(line)
            Next
        End Using

        Return True
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

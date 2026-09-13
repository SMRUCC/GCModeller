#Region "Microsoft.VisualBasic::7069787f65f1768d2e2c8e4c2987e0f3, localblast\PanGenome\Output\PanGenomeResult.vb"

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

'   Total Lines: 77
'    Code Lines: 17 (22.08%)
' Comment Lines: 50 (64.94%)
'    - Xml Docs: 90.00%
' 
'   Blank Lines: 10 (12.99%)
'     File Size: 2.66 KB


' Class PanGenomeResult
' 
'     Properties: CloudGeneFamilies, CollinearBlocks, CoreGeneFamilies, DispensableGeneFamilies, GeneFamilies
'                 GeneticDistanceMatrix, PangenomeCurveData, PAVMatrix, ShellGeneFamilies, SingleCopyOrthologFamilies
'                 SoftCoreGeneFamilies, SpecificGeneFamilies, StructuralVariations, TotalGenesInGenomes
' 
' /********************************************************************************/

#End Region

Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.Serialization
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.PanGenome.ReportJSON

''' <summary>
''' 分析结果存储结构（修改为支持多基因组）
''' </summary>
''' 
<DataContract> Public Class PanGenomeResult
    ''' <summary>
    ''' Key为基因家族ID，Value为该家族包含的所有基因ID列表
    ''' </summary>
    ''' <returns></returns>
    Public Property GeneFamilies As New Dictionary(Of String, String())()

    ''' <summary>
    ''' 核心基因家族（所有品种都有）
    ''' </summary>
    ''' <returns></returns>
    Public Property CoreGeneFamilies As String()
    ''' <summary>
    ''' 附属基因家族（部分品种有，但不是全部）
    ''' </summary>
    ''' <returns></returns>
    Public Property DispensableGeneFamilies As String()
    ''' <summary>
    ''' 特异性基因家族（仅1个品种有）
    ''' </summary>
    ''' <returns></returns>
    Public Property SpecificGeneFamilies As String()
    ''' <summary>
    ''' 单拷贝直系同源基因家族（每个品种仅1个拷贝）
    ''' </summary>
    ''' <returns></returns>
    Public Property SingleCopyOrthologFamilies As String()

    ''' <summary>
    ''' 统计数据（修改为字典，Key为基因组名称，Value为该基因组基因总数）
    ''' </summary>
    ''' <returns></returns>
    Public Property TotalGenesInGenomes As New Dictionary(Of String, Integer)()

    ''' <summary>
    ''' 1. PAV 矩阵
    ''' 
    ''' Key为基因家族ID，Value为字典(Key为基因组名，Value为拷贝数/存在与否)
    ''' </summary>
    ''' <returns></returns>
    Public Property PAVMatrix As New Dictionary(Of String, Dictionary(Of String, Integer))()

    ''' <summary>
    ''' 2. 泛基因组曲线数据
    ''' 列表项为：加入的第N个基因组，总基因数，核心基因数
    ''' </summary>
    ''' <returns></returns>
    Public Property PangenomeCurveData As PangenomeCurveData()

    ''' <summary>
    ''' 3. 共线性区块
    ''' </summary>
    ''' <returns></returns>
    Public Property CollinearBlocks As CollinearBlock()

    ''' <summary>
    ''' 结构变异列表
    ''' </summary>
    ''' <returns></returns>
    Public Property StructuralVariations As StructuralVariation()

    ' 新增：扩展分类结果
    Public Property SoftCoreGeneFamilies As String()
    Public Property ShellGeneFamilies As String()
    Public Property CloudGeneFamilies As String()

    ' 新增：遗传距离矩阵
    ' Key为 "GenomeA_vs_GenomeB"，Value为平均遗传距离
    Public Property GeneticDistanceMatrix As New Dictionary(Of String, Double)()

    ' ==========================================
    ' 新增：在分析阶段就预先计算好、供报告与外部脚本复用的统计数据缓存。
    ' 这些属性为 Nothing 的时候表示还没有计算过，由对应的取数方法惰性计算之后回填；
    ' 这样可以避免"生成HTML报告"与"外部脚本再次获取散点数据"这两个流程重复做同样的计算。
    ' ==========================================

    ''' <summary>
    ''' 基因组基本信息统计（基因总数/特有基因数/核心基因占比）
    ''' </summary>
    ''' <returns></returns>
    Public Property GenomeStats As GenomeStatRow()
    ''' <summary>
    ''' PAV矩阵的PCA降维散点数据
    ''' </summary>
    ''' <returns></returns>
    Public Property PCAData As PCAScatterDataset
    ''' <summary>
    ''' 基因组存在/缺失均衡度的香农信息熵散点数据
    ''' </summary>
    ''' <returns></returns>
    Public Property GenomeEntropyData As GenomeEntropyDataset

    ''' <summary>
    ''' 基因家族分布比例统计（按基因数与按家族个数两种口径）
    ''' </summary>
    ''' <returns></returns>
    Public Property CategoryPercent As CategoryPercentDataset

    ''' <summary>
    ''' SV结构变异的信息熵散点图与KMeans聚类结果
    ''' </summary>
    ''' <returns></returns>
    Public Property SVEntropy As SVDomainEntropyDataset

    ''' <summary>
    ''' SV结构变异的 CopyNumber / Median 矩阵缓存
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' 这两个矩阵可以完全由 <see cref="StructuralVariations"/> 与 <see cref="TotalGenesInGenomes"/>
    ''' 确定性地重建出来，因此不写入归档文件之中
    ''' （在基因组数量非常多的时候，这两个矩阵的文本量会达到数百MB），
    ''' 而是由 <see cref="SVMatrixPair"/> 在首次被访问的时候惰性重建之后缓存下来。
    ''' </remarks>
    Friend Property SVMatrices As SVMatrixPair

    Public Function GetPAVMatrix() As DataFrame
        Dim df As New DataFrame With {.rownames = PAVMatrix.Keys.ToArray}
        Dim counter = PAVMatrix

        For Each genome_id As String In TotalGenesInGenomes.Keys
            Call df.add(genome_id, From family_id As String
                                   In df.rownames
                                   Let count = counter(family_id)
                                   Select count.TryGetValue(genome_id))
        Next

        Return df
    End Function

    Public Function GetGeneticDistance() As DataFrame
        Dim df As New DataFrame With {.rownames = TotalGenesInGenomes.Keys.ToArray}
        ' 距离矩阵是对称的：GenomeA_vs_GenomeB 与 GenomeB_vs_GenomeA 的距离值是一样的，
        ' 这里需要把两个方向都建立索引，否则导出来的矩阵会只有一半的数据，另一半全都是0
        Dim tuples As New Dictionary(Of String, Dictionary(Of String, Double))()

        For Each d As KeyValuePair(Of String, Double) In GeneticDistanceMatrix
            Dim vs As String() = d.Key.Split({"_vs_"}, StringSplitOptions.None)

            If vs.Length <> 2 Then
                Continue For
            End If
            If Not tuples.ContainsKey(vs(0)) Then
                tuples.Add(vs(0), New Dictionary(Of String, Double)())
            End If
            If Not tuples.ContainsKey(vs(1)) Then
                tuples.Add(vs(1), New Dictionary(Of String, Double)())
            End If

            tuples(vs(0))(vs(1)) = d.Value
            tuples(vs(1))(vs(0)) = d.Value
        Next

        For Each genome_id As String In df.rownames
            Dim sin As Dictionary(Of String, Double) = tuples.TryGetValue(genome_id)

            If sin Is Nothing Then
                sin = New Dictionary(Of String, Double)()
            End If

            Dim vec As Double() = sin.Takes(df.rownames).ToArray

            Call df.add(genome_id, vec)
        Next

        Return df
    End Function

    ''' <summary>
    ''' SV结构变异的 CopyNumber 矩阵（行=存在SV事件的基因家族，列=全部基因组，没有SV事件的单元格为0）
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSVCopyNumberMatrix() As DataFrame
        Return GetSVMatrix(useMedian:=False)
    End Function

    ''' <summary>
    ''' SV结构变异的 Median 矩阵（行=存在SV事件的基因家族，列=全部基因组，没有SV事件的单元格为0）
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSVMedianMatrix() As DataFrame
        Return GetSVMatrix(useMedian:=True)
    End Function

    ''' <summary>
    ''' 把SV矩阵转换为 <see cref="DataFrame"/>，方便R#脚本通过 write.csv 保存为矩阵文件
    ''' </summary>
    ''' <param name="useMedian">True取Median矩阵，False取CopyNumber矩阵</param>
    Private Function GetSVMatrix(useMedian As Boolean) As DataFrame
        Dim pair As SVMatrixPair = GetSVMatrices()
        Dim df As New DataFrame()

        If pair Is Nothing OrElse pair.Families Is Nothing OrElse pair.Families.Length = 0 Then
            Return df
        End If

        df.rownames = pair.Families

        Dim source As Double()() = If(useMedian, pair.Median, pair.CopyNumber)

        For g As Integer = 0 To pair.Genomes.Length - 1
            Dim column As Integer = g

            Call df.add(pair.Genomes(column),
                        source _
                            .Select(Function(row) If(row Is Nothing, 0.0, row(column))) _
                            .ToArray)
        Next

        Return df
    End Function

    ''' <summary>
    ''' 比例明细矩阵之中代表"全部基因组平均值"的那一行的行名
    ''' </summary>
    Const CategoryPercentMeanRow As String = "average"

    ''' <summary>
    ''' 基因家族分布比例的明细矩阵（行=每个基因组 + 一行平均值，列=四个家族类别）
    ''' </summary>
    ''' <param name="byFamilies">
    ''' True 使用"按家族个数"口径（该类别在基因组内出现的家族数 ÷ 出现的家族总数），
    ''' False 使用"按基因数"口径（该类别的拷贝数之和 ÷ 该基因组的基因总数）
    ''' </param>
    ''' <returns>单元格数值为百分比（0-100）</returns>
    Public Function GetCategoryPercentMatrix(Optional byFamilies As Boolean = False) As DataFrame
        Dim data As CategoryPercentDataset = Me.CategoryPercent
        Dim df As New DataFrame()

        If data Is Nothing OrElse data.categories Is Nothing OrElse data.categories.Length = 0 Then
            Return df
        End If

        Dim matrix As Double()() = If(byFamilies, data.genomeByFamilyCount, data.genomeByGeneCount)
        Dim means As Double() = If(byFamilies, data.byFamilyCount, data.byGeneCount)
        Dim rownames As New List(Of String)()
        Dim rows As New List(Of Double())()

        For g As Integer = 0 To data.genomes.TryCount - 1
            Dim column As Integer = g
            Dim row As Double() = New Double(data.categories.Length - 1) {}

            For i As Integer = 0 To row.Length - 1
                If matrix IsNot Nothing AndAlso i < matrix.Length AndAlso
                   matrix(i) IsNot Nothing AndAlso column < matrix(i).Length Then

                    row(i) = matrix(i)(column)
                End If
            Next

            Call rownames.Add(data.genomes(column))
            Call rows.Add(row)
        Next

        ' 追加一行全部基因组的平均值，方便直接与报告页面上的比例图对照
        Dim meanRow As Double() = New Double(data.categories.Length - 1) {}

        For i As Integer = 0 To meanRow.Length - 1
            If means IsNot Nothing AndAlso i < means.Length Then
                meanRow(i) = means(i)
            End If
        Next

        Call rownames.Add(CategoryPercentMeanRow)
        Call rows.Add(meanRow)

        df.rownames = rownames.ToArray

        For i As Integer = 0 To data.categories.Length - 1
            Dim column As Integer = i

            Call df.add(data.categories(column), rows.Select(Function(row) row(column)).ToArray)
        Next

        Return df
    End Function

    ''' <summary>
    ''' save current pan-genome analysis result object as zip archive file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <remarks>
    ''' 分析结果以zip归档文件的形式保存：归档之内的每一个条目都是一个制表符分隔的文本表，
    ''' 这样保存出来的结果既压缩得很小，也可以直接解压出来用文本编辑器查看。
    ''' 
    ''' 归档之内的条目：
    ''' 
    ''' + manifest:          格式版本号以及各个数据集合的规模
    ''' + genomes:           GenomeName / 基因总数
    ''' + families:          家族ID / 该家族的基因列表
    ''' + categories:        家族分类 / 家族ID
    ''' + pav:               家族ID / 稀疏的拷贝数（只保存非零值）
    ''' + curve:             泛基因组曲线
    ''' + distance:          遗传距离矩阵
    ''' + collinear:         共线性区块统计（包含区块在两个基因组之上的起止坐标）
    ''' + collinear.links:   共线性区块之内的逐基因同源配对（仅当保留了配对数据时存在）
    ''' + sv:                结构变异事件
    ''' + genome.stats:      基因组基本信息统计
    ''' + pca:               PAV矩阵的PCA降维散点数据
    ''' + entropy:           基因组存在/缺失均衡度熵散点数据
    ''' + category.percent:  基因家族分布比例（两种口径，含逐基因组明细）
    ''' + sv.entropy:        SV结构变异的信息熵散点图与KMeans聚类结果
    ''' 
    ''' 注意：SV的 CopyNumber / Median 矩阵没有写入归档，因为它们可以由 sv 条目
    ''' 与基因组列表确定性地重建出来；在基因组数量非常多的时候这两个矩阵的文本量会达到数百MB。
    ''' </remarks>
    Public Sub Save(file As Stream)
        Using zip As New ZipArchive(file, ZipArchiveMode.Create, leaveOpen:=True)
            Call WriteSection(zip, EntryManifest, AddressOf WriteManifest)
            Call WriteSection(zip, EntryGenomes, AddressOf WriteGenomeSizes)
            Call WriteSection(zip, EntryFamilies, AddressOf WriteGeneFamilies)
            Call WriteSection(zip, EntryCategories, AddressOf WriteCategories)
            Call WriteSection(zip, EntryPAVMatrix, AddressOf WritePAVMatrix)
            Call WriteSection(zip, EntryCurve, AddressOf WritePangenomeCurve)
            Call WriteSection(zip, EntryDistance, AddressOf WriteGeneticDistance)
            Call WriteSection(zip, EntryCollinear, AddressOf WriteCollinearBlocks)
            Call WriteSection(zip, EntryCollinearLinks, AddressOf WriteCollinearLinks)
            Call WriteSection(zip, EntrySV, AddressOf WriteStructuralVariations)
            Call WriteSection(zip, EntryGenomeStats, AddressOf WriteGenomeStats)
            Call WriteSection(zip, EntryPCA, AddressOf WritePCAData)
            Call WriteSection(zip, EntryEntropy, AddressOf WriteGenomeEntropyData)
            Call WriteSection(zip, EntryCategoryPercent, AddressOf WriteCategoryPercent)
            Call WriteSection(zip, EntrySVEntropy, AddressOf WriteSVEntropy)
        End Using

        Call file.Flush()
    End Sub

    ''' <summary>
    ''' load pan-genome analysis result data from a given zip archive file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function LoadStream(file As Stream) As PanGenomeResult
        Dim result As New PanGenomeResult()

        Using zip As New ZipArchive(file, ZipArchiveMode.Read, leaveOpen:=True)
            Dim manifest As Dictionary(Of String, String) = ReadManifest(zip)
            Dim version As String = Nothing

            If Not manifest.TryGetValue("version", version) OrElse Not IsSupportedVersion(version) Then
                Throw New InvalidDataException($"invalid pan-genome result archive version: '{version}', expected: '{ArchiveVersion}'!")
            End If

            ' 基因组列表需要在PAV矩阵之前加载：
            ' PAV的每一行都需要包含全部的基因组键（缺失的拷贝数用0补齐），
            ' 否则在R#脚本之中导出表格的时候会得到空值(NA)而不是0
            Call ReadGenomeSizes(zip, result)
            Call ReadGeneFamilies(zip, result)
            Call ReadCategories(zip, result)
            Call ReadPAVMatrix(zip, result)
            Call ReadPangenomeCurve(zip, result)
            Call ReadGeneticDistance(zip, result)
            Call ReadCollinearBlocks(zip, result)
            Call ReadStructuralVariations(zip, result)
            Call ReadGenomeStats(zip, result)
            Call ReadPCAData(zip, result)
            Call ReadGenomeEntropyData(zip, result)
            Call ReadCategoryPercent(zip, result)
            Call ReadSVEntropy(zip, result)
        End Using

        Return result
    End Function

    ''' <summary>
    ''' 判断归档文件的格式版本是否可以被当前版本的代码读取
    ''' </summary>
    ''' <param name="version">归档文件的 manifest 条目之中记录的版本号</param>
    ''' <remarks>
    ''' 新版本的归档增加了统计数据缓存、类别占比、共线性区块坐标以及SV信息熵等内容；
    ''' 为了保持对历史分析结果的兼容性，旧版本的归档依然可以被读取，
    ''' 只不过缺失的条目会以空值的形式保留下来（需要的时候由对应的取数方法重新计算）。
    ''' </remarks>
    Private Shared Function IsSupportedVersion(version As String) As Boolean
        If String.IsNullOrEmpty(version) Then
            Return False
        End If

        Return version = ArchiveVersion OrElse version = ArchiveVersionV1
    End Function

#Region "archive helpers"

    Const ArchiveVersion As String = "pangenome-result/2.0"
    ''' <summary>
    ''' 旧版本的归档格式：这个版本之中没有统计数据缓存、类别占比、共线性区块坐标以及SV信息熵这些条目
    ''' </summary>
    Const ArchiveVersionV1 As String = "pangenome-result/1.0"

    Const EntryManifest As String = "manifest"
    Const EntryGenomes As String = "genomes"
    Const EntryFamilies As String = "families"
    Const EntryCategories As String = "categories"
    Const EntryPAVMatrix As String = "pav"
    Const EntryCurve As String = "curve"
    Const EntryDistance As String = "distance"
    Const EntryCollinear As String = "collinear"
    Const EntryCollinearLinks As String = "collinear.links"
    Const EntrySV As String = "sv"
    ''' <summary>基因组基本信息统计</summary>
    Const EntryGenomeStats As String = "genome.stats"
    ''' <summary>PAV矩阵的PCA降维散点数据</summary>
    Const EntryPCA As String = "pca"
    ''' <summary>基因组存在/缺失均衡度熵散点数据</summary>
    Const EntryEntropy As String = "entropy"
    ''' <summary>基因家族分布比例（两种口径）</summary>
    Const EntryCategoryPercent As String = "category.percent"
    ''' <summary>SV结构变异的信息熵散点图与聚类结果</summary>
    Const EntrySVEntropy As String = "sv.entropy"

    ''' <summary>
    ''' 元数据行的行首标记：条目之内以这个标记开头的行不是数据行，而是 Key/Value 形式的元数据
    ''' </summary>
    Const MetaMark As String = "#"

    Shared ReadOnly Utf8NoBom As New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False)
    Shared ReadOnly SepChar As Char = ChrW(9)
    Shared ReadOnly CrChar As Char = ChrW(13)
    Shared ReadOnly LfChar As Char = ChrW(10)
    Shared ReadOnly BackslashChar As Char = "\"c
    Shared ReadOnly EscapeChars As Char() = {SepChar, CrChar, LfChar, BackslashChar}

    Private Shared Sub WriteSection(zip As ZipArchive, name As String, write As Action(Of StreamWriter))
        Dim entry As ZipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal)

        Using stream As Stream = entry.Open()
            Using writer As New StreamWriter(stream, Utf8NoBom, 65536)
                Call write(writer)
                Call writer.Flush()
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' 逐行读取归档之内的一个文本表条目（如果条目不存在则返回空集合）
    ''' </summary>
    Private Shared Iterator Function ReadSection(zip As ZipArchive, name As String) As IEnumerable(Of String())
        Dim entry As ZipArchiveEntry = zip.GetEntry(name)

        If entry Is Nothing Then
            Return
        End If

        Using stream As Stream = entry.Open()
            Using reader As New StreamReader(stream, Encoding.UTF8)
                Do
                    Dim line As String = reader.ReadLine()

                    If line Is Nothing Then
                        Exit Do
                    ElseIf line.Length = 0 Then
                        Continue Do
                    End If

                    Yield SplitRow(line)
                Loop
            End Using
        End Using
    End Function

    Private Shared Function ReadManifest(zip As ZipArchive) As Dictionary(Of String, String)
        Dim manifest As New Dictionary(Of String, String)()

        For Each row As String() In ReadSection(zip, EntryManifest)
            If row.Length >= 2 Then
                manifest(row(0)) = row(1)
            End If
        Next

        Return manifest
    End Function

    Private Shared Function JoinRow(ParamArray fields As String()) As String
        Dim values As String() = New String(fields.Length - 1) {}

        For i As Integer = 0 To fields.Length - 1
            values(i) = Escape(fields(i))
        Next

        Return String.Join(SepChar, values)
    End Function

    Private Shared Function SplitRow(line As String) As String()
        Dim parts As String() = line.Split(SepChar)

        For i As Integer = 0 To parts.Length - 1
            parts(i) = Unescape(parts(i))
        Next

        Return parts
    End Function

    ''' <summary>
    ''' 转义制表符/换行符/反斜杠，避免它们破坏文本表的结构
    ''' </summary>
    Private Shared Function Escape(s As String) As String
        If String.IsNullOrEmpty(s) Then
            Return ""
        ElseIf s.IndexOfAny(EscapeChars) < 0 Then
            Return s
        End If

        Dim sb As New StringBuilder(s.Length + 8)

        For Each c As Char In s
            Select Case c
                Case SepChar : Call sb.Append("\t")
                Case CrChar : Call sb.Append("\r")
                Case LfChar : Call sb.Append("\n")
                Case BackslashChar : Call sb.Append("\\")
                Case Else : Call sb.Append(c)
            End Select
        Next

        Return sb.ToString()
    End Function

    Private Shared Function Unescape(s As String) As String
        If String.IsNullOrEmpty(s) OrElse s.IndexOf(BackslashChar) < 0 Then
            Return s
        End If

        Dim sb As New StringBuilder(s.Length)
        Dim i As Integer = 0

        While i < s.Length
            Dim c As Char = s(i)

            If c = BackslashChar AndAlso i < s.Length - 1 Then
                i += 1
                Select Case s(i)
                    Case "t"c : Call sb.Append(SepChar)
                    Case "r"c : Call sb.Append(CrChar)
                    Case "n"c : Call sb.Append(LfChar)
                    Case Else : Call sb.Append(s(i))
                End Select
            Else
                Call sb.Append(c)
            End If

            i += 1
        End While

        Return sb.ToString()
    End Function

    Private Shared Function formatNumber(x As Double) As String
        Return x.ToString("R", CultureInfo.InvariantCulture)
    End Function

    Private Shared Function parseNumber(s As String) As Double
        Dim x As Double

        If Double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, x) Then
            Return x
        Else
            Return 0
        End If
    End Function

    Private Shared Function parseInt(s As String) As Integer
        Dim x As Integer

        If Integer.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, x) Then
            Return x
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' 空字符串转换为Nothing，保持与内存对象一致的语义
    ''' </summary>
    Private Shared Function nil(s As String) As String
        If String.IsNullOrEmpty(s) Then
            Return Nothing
        Else
            Return s
        End If
    End Function

#End Region

#Region "archive writers"

    Private Sub WriteManifest(w As StreamWriter)
        Call w.WriteLine(JoinRow("version", ArchiveVersion))
        Call w.WriteLine(JoinRow("genomes", TotalGenesInGenomes.Count.ToString(CultureInfo.InvariantCulture)))
        Call w.WriteLine(JoinRow("families", GeneFamilies.Count.ToString(CultureInfo.InvariantCulture)))
        Call w.WriteLine(JoinRow("pav", PAVMatrix.Count.ToString(CultureInfo.InvariantCulture)))
        Call w.WriteLine(JoinRow("collinear", CollinearBlocks.TryCount.ToString(CultureInfo.InvariantCulture)))
        Call w.WriteLine(JoinRow("sv", StructuralVariations.TryCount.ToString(CultureInfo.InvariantCulture)))
        Call w.WriteLine(JoinRow("distance", GeneticDistanceMatrix.Count.ToString(CultureInfo.InvariantCulture)))
        Call w.WriteLine(JoinRow("curve", PangenomeCurveData.TryCount.ToString(CultureInfo.InvariantCulture)))
        Call w.WriteLine(JoinRow("stats", GenomeStats.TryCount.ToString(CultureInfo.InvariantCulture)))
        Call w.WriteLine(JoinRow("pca", If(PCAData Is Nothing, 0, PCAData.points.TryCount).ToString(CultureInfo.InvariantCulture)))
        Call w.WriteLine(JoinRow("entropy", If(GenomeEntropyData Is Nothing, 0, GenomeEntropyData.points.TryCount).ToString(CultureInfo.InvariantCulture)))
        Call w.WriteLine(JoinRow("percent", If(CategoryPercent Is Nothing, 0, CategoryPercent.categories.TryCount).ToString(CultureInfo.InvariantCulture)))
        Call w.WriteLine(JoinRow("sv.entropy", If(SVEntropy Is Nothing, 0, SVEntropy.points.TryCount).ToString(CultureInfo.InvariantCulture)))
    End Sub

    Private Sub WriteGenomeSizes(w As StreamWriter)
        For Each genome As KeyValuePair(Of String, Integer) In TotalGenesInGenomes
            Call w.WriteLine(JoinRow(genome.Key, genome.Value.ToString(CultureInfo.InvariantCulture)))
        Next
    End Sub

    Private Sub WriteGeneFamilies(w As StreamWriter)
        For Each family As KeyValuePair(Of String, String()) In GeneFamilies
            Call w.WriteLine(JoinRow(family.Key, joinGenes(family.Value)))
        Next
    End Sub

    Private Sub WriteCategories(w As StreamWriter)
        Call writeCategory(w, "core", CoreGeneFamilies)
        Call writeCategory(w, "softcore", SoftCoreGeneFamilies)
        Call writeCategory(w, "shell", ShellGeneFamilies)
        Call writeCategory(w, "cloud", CloudGeneFamilies)
        Call writeCategory(w, "specific", SpecificGeneFamilies)
        Call writeCategory(w, "dispensable", DispensableGeneFamilies)
        Call writeCategory(w, "singlecopy", SingleCopyOrthologFamilies)
    End Sub

    Private Shared Sub writeCategory(w As StreamWriter, category As String, families As String())
        If families Is Nothing Then
            Return
        End If

        For Each familyId As String In families
            Call w.WriteLine(JoinRow(category, familyId))
        Next
    End Sub

    ''' <summary>
    ''' PAV矩阵使用稀疏方式保存，只保存非零的拷贝数
    ''' </summary>
    Private Sub WritePAVMatrix(w As StreamWriter)
        Dim genomes As Dictionary(Of String, String) = escapedNames(TotalGenesInGenomes.Keys)

        For Each family As KeyValuePair(Of String, Dictionary(Of String, Integer)) In PAVMatrix
            Dim sb As New StringBuilder(Escape(family.Key))
            Dim row As Dictionary(Of String, Integer) = family.Value

            If row IsNot Nothing Then
                For Each count As KeyValuePair(Of String, Integer) In row
                    If count.Value > 0 Then
                        Dim name As String = Nothing

                        If Not genomes.TryGetValue(count.Key, name) Then
                            name = Escape(count.Key)
                        End If

                        Call sb.Append(SepChar).Append(name).Append(":"c).Append(count.Value.ToString(CultureInfo.InvariantCulture))
                    End If
                Next
            End If

            Call w.WriteLine(sb.ToString())
        Next
    End Sub

    Private Shared Function escapedNames(names As IEnumerable(Of String)) As Dictionary(Of String, String)
        Dim escaped As New Dictionary(Of String, String)()

        For Each name As String In names
            If Not escaped.ContainsKey(name) Then
                escaped.Add(name, Escape(name))
            End If
        Next

        Return escaped
    End Function

    Private Sub WritePangenomeCurve(w As StreamWriter)
        For Each point As PangenomeCurveData In PangenomeCurveData.SafeQuery
            Call w.WriteLine(JoinRow(point.GenomeCount.ToString(CultureInfo.InvariantCulture),
                                     point.TotalGenes.ToString(CultureInfo.InvariantCulture),
                                     point.CoreGenes.ToString(CultureInfo.InvariantCulture),
                                     point.SoftCoreGenes.ToString(CultureInfo.InvariantCulture)))
        Next
    End Sub

    Private Sub WriteGeneticDistance(w As StreamWriter)
        For Each distance As KeyValuePair(Of String, Double) In GeneticDistanceMatrix
            Call w.WriteLine(JoinRow(distance.Key, formatNumber(distance.Value)))
        Next
    End Sub

    Private Sub WriteCollinearBlocks(w As StreamWriter)
        For Each block As CollinearBlock In CollinearBlocks.SafeQuery
            Call w.WriteLine(JoinRow(nil(block.Genome1),
                                     nil(block.Genome2),
                                     nil(block.Chr1),
                                     nil(block.Chr2),
                                     block.GenePairCount.ToString(CultureInfo.InvariantCulture),
                                     formatNumber(block.Score),
                                     block.Start1.ToString(CultureInfo.InvariantCulture),
                                     block.End1.ToString(CultureInfo.InvariantCulture),
                                     block.Start2.ToString(CultureInfo.InvariantCulture),
                                     block.End2.ToString(CultureInfo.InvariantCulture)))
        Next
    End Sub

    Private Sub WriteCollinearLinks(w As StreamWriter)
        Dim index As Integer = 0

        For Each block As CollinearBlock In CollinearBlocks.SafeQuery
            If block.OrthologyLinks IsNot Nothing Then
                For Each link As OrthologyLink In block.OrthologyLinks
                    Call w.WriteLine(JoinRow(index.ToString(CultureInfo.InvariantCulture),
                                             nil(link.Tuple(0)),
                                             nil(link.Tuple(1))))
                Next
            End If

            index += 1
        Next
    End Sub

    Private Sub WriteStructuralVariations(w As StreamWriter)
        For Each sv As StructuralVariation In StructuralVariations.SafeQuery
            Call w.WriteLine(JoinRow(
                nil(sv.SV_ID),
                sv.Type.ToString,
                nil(sv.GenomeName),
                nil(sv.FamilyID),
                sv.CopyNumber.ToString(CultureInfo.InvariantCulture),
                formatNumber(sv.Median),
                nil(sv.Breakpoint_Chromosome),
                sv.Breakpoint_Position.ToString(CultureInfo.InvariantCulture),
                joinGenes(sv.RelatedGenes),
                nil(sv.Description)))
        Next
    End Sub

    Private Shared Function joinGenes(genes As String()) As String
        If genes Is Nothing OrElse genes.Length = 0 Then
            Return ""
        Else
            Return String.Join(";"c, genes)
        End If
    End Function

    ''' <summary>
    ''' 写出基因组基本信息统计
    ''' </summary>
    Private Sub WriteGenomeStats(w As StreamWriter)
        Dim stats As GenomeStatRow() = Me.GenomeStats

        Call WriteMeta(w, "count", stats.TryCount.ToString(CultureInfo.InvariantCulture))

        For Each stat As GenomeStatRow In stats.SafeQuery
            Call w.WriteLine(JoinRow(nil(stat.name),
                                     stat.geneCount.ToString(CultureInfo.InvariantCulture),
                                     stat.specificCount.ToString(CultureInfo.InvariantCulture),
                                     formatNumber(stat.coreRatio)))
        Next
    End Sub

    ''' <summary>
    ''' 写出PAV矩阵的PCA降维散点数据
    ''' </summary>
    Private Sub WritePCAData(w As StreamWriter)
        Dim data As PCAScatterDataset = Me.PCAData

        If data Is Nothing Then
            Call WriteMeta(w, "count", "0")
            Return
        End If

        Call WriteMeta(w, "pc1Label", nil(data.pc1Label))
        Call WriteMeta(w, "pc2Label", nil(data.pc2Label))
        Call WriteMeta(w, "pc3Label", nil(data.pc3Label))
        Call WriteMeta(w, "colorLabel", nil(data.colorLabel))
        Call WriteMeta(w, "familyCount", data.familyCount.ToString(CultureInfo.InvariantCulture))
        Call WriteMeta(w, "explained", joinNumbers(data.explained))
        Call WriteMeta(w, "count", data.points.TryCount.ToString(CultureInfo.InvariantCulture))

        For Each point As PCAPoint In data.points.SafeQuery
            Call w.WriteLine(JoinRow(nil(point.name),
                                     formatNumber(point.pc1),
                                     formatNumber(point.pc2),
                                     formatNumber(point.pc3),
                                     formatNumber(point.coreRatio),
                                     point.geneCount.ToString(CultureInfo.InvariantCulture)))
        Next
    End Sub

    ''' <summary>
    ''' 写出基因组存在/缺失均衡度的香农信息熵散点数据
    ''' </summary>
    Private Sub WriteGenomeEntropyData(w As StreamWriter)
        Dim data As GenomeEntropyDataset = Me.GenomeEntropyData

        If data Is Nothing Then
            Call WriteMeta(w, "count", "0")
            Return
        End If

        Call WriteMeta(w, "entropyLabel", nil(data.entropyLabel))
        Call WriteMeta(w, "specificRatioLabel", nil(data.specificRatioLabel))
        Call WriteMeta(w, "coreRatioLabel", nil(data.coreRatioLabel))
        Call WriteMeta(w, "familyCount", data.familyCount.ToString(CultureInfo.InvariantCulture))
        Call WriteMeta(w, "count", data.points.TryCount.ToString(CultureInfo.InvariantCulture))

        For Each point As GenomeEntropyPoint In data.points.SafeQuery
            Call w.WriteLine(JoinRow(nil(point.name),
                                     formatNumber(point.entropy),
                                     point.presentFamilies.ToString(CultureInfo.InvariantCulture),
                                     point.absentFamilies.ToString(CultureInfo.InvariantCulture),
                                     formatNumber(point.specificRatio),
                                     formatNumber(point.coreRatio),
                                     point.geneCount.ToString(CultureInfo.InvariantCulture)))
        Next
    End Sub

    ''' <summary>
    ''' 写出基因家族分布比例（两种口径的平均值 + 逐基因组明细）
    ''' </summary>
    Private Sub WriteCategoryPercent(w As StreamWriter)
        Dim data As CategoryPercentDataset = Me.CategoryPercent

        If data Is Nothing Then
            Call WriteMeta(w, "count", "0")
            Return
        End If

        Call WriteMeta(w, "categories", joinList(data.categories))
        Call WriteMeta(w, "colors", joinList(data.colors))
        Call WriteMeta(w, "genomes", joinList(data.genomes))
        Call WriteMeta(w, "byGeneCount", joinNumbers(data.byGeneCount))
        Call WriteMeta(w, "byFamilyCount", joinNumbers(data.byFamilyCount))
        Call WriteMeta(w, "count", data.categories.TryCount.ToString(CultureInfo.InvariantCulture))

        Call writePercentMatrix(w, "gene", data.genomeByGeneCount)
        Call writePercentMatrix(w, "family", data.genomeByFamilyCount)
    End Sub

    ''' <summary>
    ''' 写出一个 [类别][基因组] 的百分比明细矩阵：每一个类别一行，第一列是口径的标记
    ''' </summary>
    Private Shared Sub writePercentMatrix(w As StreamWriter, kind As String, matrix As Double()())
        For Each row As Double() In matrix.SafeQuery
            Dim values As String() = New String(row.Length) {}

            values(0) = kind

            For i As Integer = 0 To row.Length - 1
                values(i + 1) = formatNumber(row(i))
            Next

            Call w.WriteLine(JoinRow(values))
        Next
    End Sub

    ''' <summary>
    ''' 写出SV结构变异的信息熵散点图与KMeans聚类结果
    ''' </summary>
    Private Sub WriteSVEntropy(w As StreamWriter)
        Dim data As SVDomainEntropyDataset = Me.SVEntropy

        If data Is Nothing Then
            Call WriteMeta(w, "pointCount", "0")
            Return
        End If

        Call WriteMeta(w, "genomeCount", data.genomeCount.ToString(CultureInfo.InvariantCulture))
        Call WriteMeta(w, "familyCount", data.familyCount.ToString(CultureInfo.InvariantCulture))
        Call WriteMeta(w, "filteredCount", data.filteredCount.ToString(CultureInfo.InvariantCulture))
        Call WriteMeta(w, "clusterCount", data.clusterCount.ToString(CultureInfo.InvariantCulture))
        Call WriteMeta(w, "copyNumberEntropyLabel", nil(data.copyNumberEntropyLabel))
        Call WriteMeta(w, "medianEntropyLabel", nil(data.medianEntropyLabel))
        Call WriteMeta(w, "pointCount", data.points.TryCount.ToString(CultureInfo.InvariantCulture))

        For Each point As SVDomainEntropyPoint In data.points.SafeQuery
            Call w.WriteLine(JoinRow("point",
                                     nil(point.name),
                                     formatNumber(point.hCopyNumber),
                                     formatNumber(point.hMedian),
                                     formatNumber(point.zCopyNumber),
                                     formatNumber(point.zMedian),
                                     point.presentGenomes.ToString(CultureInfo.InvariantCulture),
                                     point.cluster.ToString(CultureInfo.InvariantCulture)))
        Next

        For Each cluster As SVEntropyCluster In data.clusters.SafeQuery
            Call w.WriteLine(JoinRow("cluster",
                                     cluster.cluster.ToString(CultureInfo.InvariantCulture),
                                     cluster.size.ToString(CultureInfo.InvariantCulture),
                                     formatNumber(cluster.meanCopyNumberEntropy),
                                     formatNumber(cluster.meanMedianEntropy),
                                     nil(cluster.label)))
        Next
    End Sub

    ''' <summary>
    ''' 写出一个元数据行（Key/Value）
    ''' </summary>
    Private Shared Sub WriteMeta(w As StreamWriter, key As String, value As String)
        Call w.WriteLine(JoinRow(MetaMark, key, value))
    End Sub

    Private Shared Function joinList(items As String()) As String
        If items Is Nothing Then
            Return ""
        Else
            Return String.Join(";"c, items)
        End If
    End Function

    Private Shared Function joinNumbers(numbers As Double()) As String
        If numbers Is Nothing Then
            Return ""
        Else
            Return String.Join(";"c, numbers.Select(AddressOf formatNumber))
        End If
    End Function

    Private Shared Function splitList(text As String) As String()
        If String.IsNullOrEmpty(text) Then
            Return New String() {}
        Else
            Return text.Split(";"c)
        End If
    End Function

    Private Shared Function splitNumbers(text As String) As Double()
        Dim items As String() = splitList(text)
        Dim numbers As Double() = New Double(items.Length - 1) {}

        For i As Integer = 0 To items.Length - 1
            numbers(i) = parseNumber(items(i))
        Next

        Return numbers
    End Function

#End Region

#Region "archive readers"

    Private Shared Sub ReadGenomeSizes(zip As ZipArchive, result As PanGenomeResult)
        For Each row As String() In ReadSection(zip, EntryGenomes)
            If row.Length >= 2 Then
                result.TotalGenesInGenomes(row(0)) = parseInt(row(1))
            End If
        Next
    End Sub

    Private Shared Sub ReadGeneFamilies(zip As ZipArchive, result As PanGenomeResult)
        Dim families As New Dictionary(Of String, String())()

        For Each row As String() In ReadSection(zip, EntryFamilies)
            If row.Length = 0 Then
                Continue For
            End If

            Dim genes As String()

            If row.Length > 1 AndAlso row(1).Length > 0 Then
                genes = row(1).Split(";"c)
            Else
                genes = New String() {}
            End If

            families(row(0)) = genes
        Next

        result.GeneFamilies = families
    End Sub

    Private Shared Sub ReadCategories(zip As ZipArchive, result As PanGenomeResult)
        Dim core As New List(Of String)()
        Dim softCore As New List(Of String)()
        Dim shell As New List(Of String)()
        Dim cloud As New List(Of String)()
        Dim specific As New List(Of String)()
        Dim dispensable As New List(Of String)()
        Dim singleCopy As New List(Of String)()

        For Each row As String() In ReadSection(zip, EntryCategories)
            If row.Length < 2 Then
                Continue For
            End If

            Select Case row(0)
                Case "core" : Call core.Add(row(1))
                Case "softcore" : Call softCore.Add(row(1))
                Case "shell" : Call shell.Add(row(1))
                Case "cloud" : Call cloud.Add(row(1))
                Case "specific" : Call specific.Add(row(1))
                Case "dispensable" : Call dispensable.Add(row(1))
                Case "singlecopy" : Call singleCopy.Add(row(1))
            End Select
        Next

        result.CoreGeneFamilies = core.ToArray
        result.SoftCoreGeneFamilies = softCore.ToArray
        result.ShellGeneFamilies = shell.ToArray
        result.CloudGeneFamilies = cloud.ToArray
        result.SpecificGeneFamilies = specific.ToArray
        result.DispensableGeneFamilies = dispensable.ToArray
        result.SingleCopyOrthologFamilies = singleCopy.ToArray
    End Sub

    ''' <summary>
    ''' 读取稀疏保存的PAV矩阵，并且使用全部的基因组键补齐为完整的行
    ''' </summary>
    Private Shared Sub ReadPAVMatrix(zip As ZipArchive, result As PanGenomeResult)
        Dim genomeNames As String() = result.TotalGenesInGenomes.Keys.ToArray()
        Dim pav As New Dictionary(Of String, Dictionary(Of String, Integer))()

        For Each row As String() In ReadSection(zip, EntryPAVMatrix)
            If row.Length = 0 Then
                Continue For
            End If

            Dim counts As New Dictionary(Of String, Integer)(genomeNames.Length)

            For Each genome As String In genomeNames
                counts.Add(genome, 0)
            Next

            For i As Integer = 1 To row.Length - 1
                Dim field As String = row(i)

                If field.Length = 0 Then
                    Continue For
                End If

                Dim pair As String() = field.Split(":"c)

                If pair.Length = 2 Then
                    counts(pair(0)) = parseInt(pair(1))
                End If
            Next

            pav(row(0)) = counts
        Next

        result.PAVMatrix = pav
    End Sub

    Private Shared Sub ReadPangenomeCurve(zip As ZipArchive, result As PanGenomeResult)
        Dim points As New List(Of PangenomeCurveData)()

        For Each row As String() In ReadSection(zip, EntryCurve)
            ' 旧版本(1.0)的归档之中只有3列，没有软核心这一列，读取的时候按照0来处理
            If row.Length >= 3 Then
                Call points.Add(New PangenomeCurveData With {
                    .GenomeCount = parseInt(row(0)),
                    .TotalGenes = parseInt(row(1)),
                    .CoreGenes = parseInt(row(2)),
                    .SoftCoreGenes = If(row.Length >= 4, parseInt(row(3)), 0)
                })
            End If
        Next

        result.PangenomeCurveData = points.ToArray()
    End Sub

    Private Shared Sub ReadGeneticDistance(zip As ZipArchive, result As PanGenomeResult)
        Dim distances As New Dictionary(Of String, Double)()

        For Each row As String() In ReadSection(zip, EntryDistance)
            If row.Length >= 2 Then
                distances(row(0)) = parseNumber(row(1))
            End If
        Next

        result.GeneticDistanceMatrix = distances
    End Sub

    Private Shared Sub ReadCollinearBlocks(zip As ZipArchive, result As PanGenomeResult)
        Dim blocks As New List(Of CollinearBlock)()

        For Each row As String() In ReadSection(zip, EntryCollinear)
            ' 旧版本(1.0)的归档之中没有区块的起止坐标，读取的时候按照0来处理
            If row.Length >= 6 Then
                Call blocks.Add(New CollinearBlock With {
                    .Genome1 = nil(row(0)),
                    .Genome2 = nil(row(1)),
                    .Chr1 = nil(row(2)),
                    .Chr2 = nil(row(3)),
                    .LinkCount = parseInt(row(4)),
                    .Score = parseNumber(row(5)),
                    .Start1 = If(row.Length >= 10, parseInt(row(6)), 0),
                    .End1 = If(row.Length >= 10, parseInt(row(7)), 0),
                    .Start2 = If(row.Length >= 10, parseInt(row(8)), 0),
                    .End2 = If(row.Length >= 10, parseInt(row(9)), 0)
                })
            End If
        Next

        ' 逐基因的同源配对数据（仅在保留了配对数据的时候才会存在这个条目）
        Dim index As Integer = -1
        Dim links As List(Of OrthologyLink) = Nothing

        For Each row As String() In ReadSection(zip, EntryCollinearLinks)
            If row.Length < 3 Then
                Continue For
            End If

            Dim blockIndex As Integer = parseInt(row(0))

            If blockIndex <> index Then
                Call setLinks(blocks, index, links)

                index = blockIndex
                links = New List(Of OrthologyLink)()
            End If

            Call links.Add(New OrthologyLink(nil(row(1)), nil(row(2))))
        Next

        Call setLinks(blocks, index, links)

        result.CollinearBlocks = blocks.ToArray()
    End Sub

    Private Shared Sub setLinks(blocks As List(Of CollinearBlock), index As Integer, links As List(Of OrthologyLink))
        If links Is Nothing OrElse index < 0 OrElse index >= blocks.Count Then
            Return
        End If

        blocks(index).OrthologyLinks = links.ToArray()
    End Sub

    Private Shared Sub ReadStructuralVariations(zip As ZipArchive, result As PanGenomeResult)
        Dim events As New List(Of StructuralVariation)()

        For Each row As String() In ReadSection(zip, EntrySV)
            If row.Length < 10 Then
                Continue For
            End If

            Dim sv As New StructuralVariation With {
                .SV_ID = nil(row(0)),
                .GenomeName = nil(row(2)),
                .FamilyID = nil(row(3)),
                .CopyNumber = parseInt(row(4)),
                .Median = parseNumber(row(5)),
                .Breakpoint_Chromosome = nil(row(6)),
                .Breakpoint_Position = parseInt(row(7)),
                .Description = nil(row(9))
            }

            If row(8).Length > 0 Then
                sv.RelatedGenes = row(8).Split(";"c)
            End If

            Dim type As SVType

            If [Enum].TryParse(row(1), type) Then
                sv.Type = type
            End If

            Call events.Add(sv)
        Next

        result.StructuralVariations = events.ToArray()
    End Sub

    ''' <summary>
    ''' 读取条目之内的元数据行（只处理以 <see cref="MetaMark"/> 开头的行）
    ''' </summary>
    Private Shared Function ReadMeta(rows As String()()) As Dictionary(Of String, String)
        Dim meta As New Dictionary(Of String, String)()

        For Each row As String() In rows
            If row.Length >= 3 AndAlso row(0) = MetaMark Then
                meta(row(1)) = row(2)
            End If
        Next

        Return meta
    End Function

    Private Shared Function metaValue(meta As Dictionary(Of String, String), key As String) As String
        Dim value As String = Nothing

        If meta.TryGetValue(key, value) Then
            Return nil(value)
        Else
            Return Nothing
        End If
    End Function

    Private Shared Function metaInt(meta As Dictionary(Of String, String), key As String) As Integer
        Dim value As String = Nothing

        If meta.TryGetValue(key, value) Then
            Return parseInt(value)
        Else
            Return 0
        End If
    End Function

    Private Shared Function metaList(meta As Dictionary(Of String, String), key As String) As String()
        Dim value As String = Nothing

        If meta.TryGetValue(key, value) Then
            Return splitList(value)
        Else
            Return New String() {}
        End If
    End Function

    Private Shared Function metaNumbers(meta As Dictionary(Of String, String), key As String) As Double()
        Dim value As String = Nothing

        If meta.TryGetValue(key, value) Then
            Return splitNumbers(value)
        Else
            Return New Double() {}
        End If
    End Function

    ''' <summary>
    ''' 读取基因组基本信息统计
    ''' </summary>
    Private Shared Sub ReadGenomeStats(zip As ZipArchive, result As PanGenomeResult)
        ' 条目不存在（旧版本的归档）：保持为Nothing，由取数方法在需要的时候重新计算
        If zip.GetEntry(EntryGenomeStats) Is Nothing Then
            Return
        End If

        Dim stats As New List(Of GenomeStatRow)()

        For Each row As String() In ReadSection(zip, EntryGenomeStats)
            If row.Length >= 4 AndAlso row(0) <> MetaMark Then
                Call stats.Add(New GenomeStatRow With {
                    .name = nil(row(0)),
                    .geneCount = parseInt(row(1)),
                    .specificCount = parseInt(row(2)),
                    .coreRatio = parseNumber(row(3))
                })
            End If
        Next

        If stats.Count = 0 Then
            Return
        End If

        result.GenomeStats = stats.ToArray()
    End Sub

    ''' <summary>
    ''' 读取PAV矩阵的PCA降维散点数据
    ''' </summary>
    Private Shared Sub ReadPCAData(zip As ZipArchive, result As PanGenomeResult)
        If zip.GetEntry(EntryPCA) Is Nothing Then
            Return
        End If

        Dim rows As String()() = ReadSection(zip, EntryPCA).ToArray
        Dim meta As Dictionary(Of String, String) = ReadMeta(rows)
        Dim points As New List(Of PCAPoint)()

        For Each row As String() In rows
            If row.Length >= 6 AndAlso row(0) <> MetaMark Then
                Call points.Add(New PCAPoint With {
                    .name = nil(row(0)),
                    .pc1 = parseNumber(row(1)),
                    .pc2 = parseNumber(row(2)),
                    .pc3 = parseNumber(row(3)),
                    .coreRatio = parseNumber(row(4)),
                    .geneCount = parseInt(row(5))
                })
            End If
        Next

        If points.Count = 0 Then
            Return
        End If

        result.PCAData = New PCAScatterDataset With {
            .points = points.ToArray,
            .pc1Label = metaValue(meta, "pc1Label"),
            .pc2Label = metaValue(meta, "pc2Label"),
            .pc3Label = metaValue(meta, "pc3Label"),
            .colorLabel = metaValue(meta, "colorLabel"),
            .familyCount = metaInt(meta, "familyCount"),
            .explained = metaNumbers(meta, "explained")
        }
    End Sub

    ''' <summary>
    ''' 读取基因组存在/缺失均衡度的香农信息熵散点数据
    ''' </summary>
    Private Shared Sub ReadGenomeEntropyData(zip As ZipArchive, result As PanGenomeResult)
        If zip.GetEntry(EntryEntropy) Is Nothing Then
            Return
        End If

        Dim rows As String()() = ReadSection(zip, EntryEntropy).ToArray
        Dim meta As Dictionary(Of String, String) = ReadMeta(rows)
        Dim points As New List(Of GenomeEntropyPoint)()

        For Each row As String() In rows
            If row.Length >= 7 AndAlso row(0) <> MetaMark Then
                Call points.Add(New GenomeEntropyPoint With {
                    .name = nil(row(0)),
                    .entropy = parseNumber(row(1)),
                    .presentFamilies = parseInt(row(2)),
                    .absentFamilies = parseInt(row(3)),
                    .specificRatio = parseNumber(row(4)),
                    .coreRatio = parseNumber(row(5)),
                    .geneCount = parseInt(row(6))
                })
            End If
        Next

        If points.Count = 0 Then
            Return
        End If

        result.GenomeEntropyData = New GenomeEntropyDataset With {
            .points = points.ToArray,
            .entropyLabel = metaValue(meta, "entropyLabel"),
            .specificRatioLabel = metaValue(meta, "specificRatioLabel"),
            .coreRatioLabel = metaValue(meta, "coreRatioLabel"),
            .familyCount = metaInt(meta, "familyCount")
        }
    End Sub

    ''' <summary>
    ''' 读取基因家族分布比例（两种口径的平均值 + 逐基因组明细）
    ''' </summary>
    Private Shared Sub ReadCategoryPercent(zip As ZipArchive, result As PanGenomeResult)
        If zip.GetEntry(EntryCategoryPercent) Is Nothing Then
            Return
        End If

        Dim rows As String()() = ReadSection(zip, EntryCategoryPercent).ToArray
        Dim meta As Dictionary(Of String, String) = ReadMeta(rows)
        Dim categories As String() = metaList(meta, "categories")
        Dim byGene As New List(Of Double())()
        Dim byFamily As New List(Of Double())()

        If categories.Length = 0 Then
            Return
        End If

        For Each row As String() In rows
            If row.Length < 2 OrElse row(0) = MetaMark Then
                Continue For
            End If

            Dim values As Double() = New Double(row.Length - 2) {}

            For i As Integer = 1 To row.Length - 1
                values(i - 1) = parseNumber(row(i))
            Next

            Select Case row(0)
                Case "gene" : Call byGene.Add(values)
                Case "family" : Call byFamily.Add(values)
            End Select
        Next

        result.CategoryPercent = New CategoryPercentDataset With {
            .categories = categories,
            .colors = metaList(meta, "colors"),
            .genomes = metaList(meta, "genomes"),
            .byGeneCount = metaNumbers(meta, "byGeneCount"),
            .byFamilyCount = metaNumbers(meta, "byFamilyCount"),
            .genomeByGeneCount = byGene.ToArray,
            .genomeByFamilyCount = byFamily.ToArray
        }
    End Sub

    ''' <summary>
    ''' 读取SV结构变异的信息熵散点图与KMeans聚类结果
    ''' </summary>
    Private Shared Sub ReadSVEntropy(zip As ZipArchive, result As PanGenomeResult)
        If zip.GetEntry(EntrySVEntropy) Is Nothing Then
            Return
        End If

        Dim rows As String()() = ReadSection(zip, EntrySVEntropy).ToArray
        Dim meta As Dictionary(Of String, String) = ReadMeta(rows)
        Dim points As New List(Of SVDomainEntropyPoint)()
        Dim clusters As New List(Of SVEntropyCluster)()

        For Each row As String() In rows
            If row.Length = 0 OrElse row(0) = MetaMark Then
                Continue For
            End If

            Select Case row(0)
                Case "point"
                    If row.Length >= 8 Then
                        Call points.Add(New SVDomainEntropyPoint With {
                            .name = nil(row(1)),
                            .hCopyNumber = parseNumber(row(2)),
                            .hMedian = parseNumber(row(3)),
                            .zCopyNumber = parseNumber(row(4)),
                            .zMedian = parseNumber(row(5)),
                            .presentGenomes = parseInt(row(6)),
                            .cluster = parseInt(row(7))
                        })
                    End If
                Case "cluster"
                    If row.Length >= 6 Then
                        Call clusters.Add(New SVEntropyCluster With {
                            .cluster = parseInt(row(1)),
                            .size = parseInt(row(2)),
                            .meanCopyNumberEntropy = parseNumber(row(3)),
                            .meanMedianEntropy = parseNumber(row(4)),
                            .label = nil(row(5))
                        })
                    End If
            End Select
        Next

        If points.Count = 0 Then
            Return
        End If

        result.SVEntropy = New SVDomainEntropyDataset With {
            .points = points.ToArray,
            .clusters = clusters.ToArray,
            .familyCount = metaInt(meta, "familyCount"),
            .filteredCount = metaInt(meta, "filteredCount"),
            .clusterCount = metaInt(meta, "clusterCount"),
            .copyNumberEntropyLabel = metaValue(meta, "copyNumberEntropyLabel"),
            .medianEntropyLabel = metaValue(meta, "medianEntropyLabel"),
            .genomeCount = metaInt(meta, "genomeCount")
        }
    End Sub

#End Region

End Class

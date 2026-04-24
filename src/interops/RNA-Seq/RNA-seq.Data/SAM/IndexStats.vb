#Region "Microsoft.VisualBasic::4081901616496600bd30aa74c49b7801, RNA-Seq\RNA-seq.Data\SAM\IndexStats.vb"

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

    '   Total Lines: 85
    '    Code Lines: 61 (71.76%)
    ' Comment Lines: 7 (8.24%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 17 (20.00%)
    '     File Size: 2.94 KB


    '     Class GeneData
    ' 
    '         Properties: GeneID, Length, RawCount, RPK, TPM
    ' 
    '     Class IndexStats
    ' 
    '         Properties: GeneID, Length, RawCount, UnmappedBases
    ' 
    '         Function: ConvertCountsToTPM, Parse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel

Namespace SAM

    ''' <summary>
    ''' gene abundance result
    ''' </summary>
    Public Class GeneData : Implements IExpressionValue

        Public Property GeneID As String Implements IExpressionValue.Identity
        Public Property Length As Double
        Public Property RawCount As Double
        Public Property RPK As Double
        Public Property TPM As Double Implements IExpressionValue.ExpressionValue
        Public Property FPKM As Double

    End Class

    ''' <summary>
    ''' A row of the samtool indexstats output
    ''' </summary>
    Public Class IndexStats

        Public Property GeneID As String
        Public Property Length As Integer
        Public Property RawCount As Integer
        Public Property UnmappedBases As Integer

        Public Shared Iterator Function Parse(file As Stream) As IEnumerable(Of IndexStats)
            Using str As New StreamReader(file)
                Dim line As Value(Of String) = ""

                Do While (line = str.ReadLine) IsNot Nothing
                    If String.IsNullOrWhiteSpace(line) OrElse line.StartsWith("*") OrElse line.StartsWith("@") Then
                        Continue Do
                    End If

                    Dim fields As String() = line.Split(vbTab)
                    Dim gene_count As New IndexStats With {
                        .GeneID = fields(0),
                        .Length = CInt(fields(1)),
                        .RawCount = CInt(fields(2)),
                        .UnmappedBases = CInt(Val(fields.ElementAtOrNull(3)))
                    }

                    Yield gene_count
                Loop
            End Using
        End Function

        ''' <summary>
        ''' 将 Raw Count 转化为 TPM 和 FPKM
        ''' </summary>
        ''' <param name="stats">the sam index stats table file</param>
        ''' <param name="totalMappedFragments">
        ''' Optional parameter specifying the total number of mapped fragments. 
        ''' If not provided, it will be approximated using the sum of raw counts.
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 使用了 `totalRawCount += hit.RawCount`，也就是**把传入的所有基因的 Raw Count 加起来**作为 Total Mapped Fragments。
        ''' 这种做法的好处是：如果用户没有提供 totalMappedFragments 参数，我们仍然可以计算出一个合理的 TPM 和 FPKM 值。
        ''' 
        ''' 这在大多数简单情况下是可行的，但在严格的生信分析中，**基因 Count 的总和并不等于实际的 Total Mapped Fragments**。原因如下：
        ''' 
        ''' 1. **未比对上的 Reads**：有些 Fragment 比对到了基因间区或内含子，不会被 `featureCounts` 计入任何基因的 Count 中，但它们依然是 Mapped Fragments。
        ''' 2. **多基因比对**：有些 Fragment 比对到了多个基因，`featureCounts` 默认会丢弃它们（不计入 RawCount），但它们客观存在于比对结果中。
        ''' 3. **线粒体基因/rRNA**：有时在计算 mRNA 表达量时，会故意剔除线粒体基因或 rRNA，但这会导致 Count 总和远小于 Total Mapped Fragments。
        ''' 
        ''' **更严谨的做法：**
        ''' 
        ''' 如果你手头有这个样本的**实际比对总数**（通常可以从 `featureCounts` 的运行日志 `.summary` 文件中读取到 `Assigned + Unassigned` 的总数，或者从 `samtools flagstat` 中获取），
        ''' 你应该将其作为参数传入函数，而不是在函数内部累加。
        ''' </remarks>
        Public Shared Iterator Function ConvertCountsToTPM(stats As IEnumerable(Of IndexStats), Optional totalMappedFragments As Long? = Nothing) As IEnumerable(Of GeneData)
            Dim genes As New List(Of GeneData)()
            Dim totalRPK As Double = 0.0
            ' 用于累计总 Count 数，作为 Total Mapped Fragments 的近似
            Dim totalRawCount As Long = 0L

            ' --- 第一步：计算 RPK 并累加 totalRPK 和 totalRawCount ---
            For Each hit As IndexStats In stats
                Dim gene As New GeneData() With {
                    .GeneID = hit.GeneID,
                    .Length = hit.Length,
                    .RawCount = hit.RawCount,
                    .RPK = (.RawCount * 1000.0) / .Length
                }

                genes.Add(gene)
                totalRPK += gene.RPK
                totalRawCount += hit.RawCount ' 累加原始计数
            Next

            ' --- 第二步：根据 totalRPK 计算 TPM ---
            If totalRPK = 0 Then
                Call "Warning: Total RPK is 0. All TPM values will be 0.".warning
            End If
            If totalRawCount = 0 Then
                Call "Warning: Total Raw Count is 0. All FPKM values will be 0.".warning
            End If

            If totalMappedFragments IsNot Nothing Then
                totalRawCount = totalMappedFragments
            End If

            For Each gene As GeneData In genes
                ' 计算 TPM
                If totalRPK > 0 Then
                    gene.TPM = (gene.RPK / totalRPK) * 1000000.0
                Else
                    gene.TPM = 0.0
                End If

                ' 计算 FPKM
                ' 利用已有的 RPK：FPKM = (RPK * 1,000,000) / totalRawCount
                ' 等效于原公式：(RawCount * 10^9) / (totalRawCount * Length)
                If totalRawCount > 0 Then
                    gene.FPKM = (gene.RPK * 1000000.0) / totalRawCount
                Else
                    gene.FPKM = 0.0
                End If

                Yield gene
            Next
        End Function

    End Class
End Namespace

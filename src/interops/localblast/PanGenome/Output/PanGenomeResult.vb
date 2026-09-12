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

Imports System.IO
Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework

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
    ''' save current pan-genome analysis result object as zip archive file
    ''' </summary>
    ''' <param name="file"></param>
    Public Sub Save(file As Stream)

    End Sub

    ''' <summary>
    ''' load pan-genome analysis result data from a given zip archive file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    Public Shared Function LoadStream(file As Stream) As PanGenomeResult

    End Function

End Class

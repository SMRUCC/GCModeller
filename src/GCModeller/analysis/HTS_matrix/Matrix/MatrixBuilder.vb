#Region "Microsoft.VisualBasic::c62a21b2d57fd182ddd2e87534b642fd, analysis\HTS_matrix\Matrix\MatrixBuilder.vb"

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

    '   Total Lines: 73
    '    Code Lines: 54 (73.97%)
    ' Comment Lines: 10 (13.70%)
    '    - Xml Docs: 90.00%
    ' 
    '   Blank Lines: 9 (12.33%)
    '     File Size: 3.06 KB


    ' Module MatrixBuilder
    ' 
    '     Function: BuildAndNormalizeAbundanceMatrix, MatrixInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' Expression abundance matrix builder
''' </summary>
Public Module MatrixBuilder

    ''' <summary>
    ''' 将所有样本的物种丰度结果构建并归一化为一个丰度矩阵。
    ''' </summary>
    ''' <param name="allSampleAbundances">聚合后的数据，Key为样本ID，Value为该样本的物种丰度字典。</param>
    ''' <param name="performNormalization">是否执行按列（样本）的总和归一化，使每个样本的相对丰度之和为1。</param>
    ''' <returns>一个包含物种和样本丰度信息的DataTable。</returns>
    Public Function BuildAndNormalizeAbundanceMatrix(allSampleAbundances As NamedValue(Of Dictionary(Of String, Double))(), Optional performNormalization As Boolean = True) As Matrix
        If allSampleAbundances.IsNullOrEmpty Then
            Call "no sample data to build expression matrix!".warning

            Return New Matrix With {
                .tag = "empty_samples",
                .expression = {},
                .sampleID = {}
            }
        End If

        Dim allSampleIds As String() = allSampleAbundances.Keys.ToArray
        Dim table As DataFrameRow() = allSampleAbundances _
            .ToDictionary(Function(a) a.Name,
                          Function(a)
                              Return a.Value
                          End Function) _
            .MatrixInternal(allSampleIds) _
            .ToArray
        Dim matrix As New Matrix With {
            .sampleID = allSampleIds,
            .expression = table,
            .tag = "expression_matrix"
        }

        If performNormalization Then
            Return TPM.Normalize(matrix)
        Else
            Return matrix
        End If
    End Function

    <Extension>
    Private Iterator Function MatrixInternal(allSamples As Dictionary(Of String, Dictionary(Of String, Double)), allSampleIds As String()) As IEnumerable(Of DataFrameRow)
        ' --- 步骤 1: 识别所有唯一的物种和样本 ---
        Dim featuresIds As List(Of String) = allSamples.Values _
            .SelectMany(Function(dict) dict.Keys) _
            .Distinct() _
            .OrderBy(Function(id) id) _
            .ToList()

        Call VBDebugger.EchoLine($"found {featuresIds.Count} unique features and {allSampleIds.Length} unique samples.")

        For Each id As String In featuresIds
            Dim v As IEnumerable(Of Double) =
                From sample_id As String
                In allSampleIds
                Let sample As Dictionary(Of String, Double) = allSamples(sample_id)
                Select sample.TryGetValue(id)

            Yield New DataFrameRow With {
                .experiments = v.ToArray,
                .geneID = id
            }
        Next
    End Function
End Module


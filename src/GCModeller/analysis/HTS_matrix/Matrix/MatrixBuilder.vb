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

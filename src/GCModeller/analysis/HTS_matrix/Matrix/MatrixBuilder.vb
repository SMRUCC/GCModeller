Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework.DATA
Imports Microsoft.VisualBasic.Data.Framework.IO
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
            Console.WriteLine("警告: 没有样本数据可用于构建丰度矩阵。")
            Return New Matrix
        End If

        Dim allSampleIds As String() = Nothing
        Dim table As DataTable = allSampleAbundances _
            .ToDictionary(Function(a) a.Name,
                          Function(a)
                              Return a.Value
                          End Function) _
            .MatrixInternal(performNormalization, allSampleIds)

        Return New Matrix With {
            .sampleID = allSampleIds,
            .expression = table _
                .GetMatrix _
                .Select(Function(r)
                            Return New DataFrameRow(r.ID, r(allSampleIds))
                        End Function) _
                .ToArray
        }
    End Function

    <Extension>
    Private Function MatrixInternal(allSampleAbundances As Dictionary(Of String, Dictionary(Of String, Double)), performNormalization As Boolean, ByRef allSampleIds As String()) As DataTable
        ' --- 步骤 1: 识别所有唯一的物种和样本 ---
        Dim allSpeciesIds As List(Of String) = allSampleAbundances.Values.
            SelectMany(Function(dict) dict.Keys).
            Distinct().
            OrderBy(Function(id) id).ToList()

        allSampleIds = allSampleAbundances.Keys.OrderBy(Function(id) id).ToArray

        Console.WriteLine($"找到 {allSpeciesIds.Count} 个唯一物种和 {allSampleIds.Count} 个唯一样本。")

        ' --- 步骤 2: 初始化 DataTable ---
        Dim abundanceTable As New DataTable()

        ' --- 步骤 3: 填充矩阵 ---
        For Each speciesId In allSpeciesIds
            Dim row = abundanceTable.NewRow(speciesId)

            For Each sampleId As String In allSampleIds
                Dim abundance As Double = 0.0
                ' 尝试从样本数据中获取物种丰度，如果不存在则默认为0
                allSampleAbundances(sampleId).TryGetValue(speciesId, abundance)
                row(sampleId) = abundance
            Next
        Next

        Console.WriteLine("原始丰度矩阵填充完成。")

        ' --- 步骤 4: (可选) 按列进行总和归一化 ---
        If performNormalization Then
            Console.WriteLine("开始执行按列归一化...")
            For Each sampleId As String In allSampleIds
                ' 计算该列的总和
                Dim columnSum As Double = abundanceTable.GetMatrix.Sum(Function(row) row(sampleId))

                ' 避免除以零
                If columnSum > 0 Then
                    ' 用该列的每个值除以总和
                    For Each row As DataSet In abundanceTable.GetMatrix
                        Dim currentValue As Double = row(sampleId)
                        row(sampleId) = currentValue / columnSum
                    Next
                End If
            Next
            Console.WriteLine("归一化完成。")
        End If

        Return abundanceTable
    End Function
End Module

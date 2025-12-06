Imports System.Data
Imports System.Linq

Namespace Kmers

    Public Module AbundanceMatrixBuilder

        ''' <summary>
        ''' 将所有样本的物种丰度结果构建并归一化为一个丰度矩阵。
        ''' </summary>
        ''' <param name="allSampleAbundances">聚合后的数据，Key为样本ID，Value为该样本的物种丰度字典。</param>
        ''' <param name="performNormalization">是否执行按列（样本）的总和归一化，使每个样本的相对丰度之和为1。</param>
        ''' <returns>一个包含物种和样本丰度信息的DataTable。</returns>
        Public Function BuildAndNormalizeAbundanceMatrix(
        allSampleAbundances As Dictionary(Of String, Dictionary(Of Integer, Double)),
        Optional performNormalization As Boolean = True
    ) As DataTable

            If allSampleAbundances Is Nothing OrElse allSampleAbundances.Count = 0 Then
                Console.WriteLine("警告: 没有样本数据可用于构建丰度矩阵。")
                Return New DataTable()
            End If

            ' --- 步骤 1: 识别所有唯一的物种和样本 ---
            Dim allSpeciesIds As List(Of Integer) = allSampleAbundances.Values.
            SelectMany(Function(dict) dict.Keys).
            Distinct().
            OrderBy(Function(id) id).ToList()

            Dim allSampleIds As List(Of String) = allSampleAbundances.Keys.OrderBy(Function(id) id).ToList()

            Console.WriteLine($"找到 {allSpeciesIds.Count} 个唯一物种和 {allSampleIds.Count} 个唯一样本。")

            ' --- 步骤 2: 初始化 DataTable ---
            Dim abundanceTable As New DataTable()
            abundanceTable.Columns.Add("SpeciesTaxID", GetType(Integer))

            For Each sampleId In allSampleIds
                abundanceTable.Columns.Add(sampleId, GetType(Double))
            Next

            ' --- 步骤 3: 填充矩阵 ---
            For Each speciesId In allSpeciesIds
                Dim row As DataRow = abundanceTable.NewRow()
                row("SpeciesTaxID") = speciesId

                For Each sampleId In allSampleIds
                    Dim abundance As Double = 0.0
                    ' 尝试从样本数据中获取物种丰度，如果不存在则默认为0
                    allSampleAbundances(sampleId).TryGetValue(speciesId, abundance)
                    row(sampleId) = abundance
                Next

                abundanceTable.Rows.Add(row)
            Next

            Console.WriteLine("原始丰度矩阵填充完成。")

            ' --- 步骤 4: (可选) 按列进行总和归一化 ---
            If performNormalization Then
                Console.WriteLine("开始执行按列归一化...")
                For Each sampleId As String In allSampleIds
                    ' 计算该列的总和
                    Dim columnSum As Double = abundanceTable.AsEnumerable().
                    Sum(Function(row) row.Field(Of Double)(sampleId))

                    ' 避免除以零
                    If columnSum > 0 Then
                        ' 用该列的每个值除以总和
                        For Each row As DataRow In abundanceTable.Rows
                            Dim currentValue As Double = row.Field(Of Double)(sampleId)
                            row(sampleId) = currentValue / columnSum
                        Next
                    End If
                Next
                Console.WriteLine("归一化完成。")
            End If

            Return abundanceTable
        End Function

        ''' <summary>
        ''' 将DataTable保存为CSV文件，方便后续在R或Python中分析。
        ''' </summary>
        Public Sub SaveDataTableToCsv(table As DataTable, filePath As String)
            Using writer As New IO.StreamWriter(filePath, False, System.Text.Encoding.UTF8)
                ' 写入列标题
                Dim columnNames As String() = table.Columns.Cast(Of DataColumn)().Select(Function(col) col.ColumnName).ToArray()
                writer.WriteLine(String.Join(",", columnNames))

                ' 写入数据行
                For Each row As DataRow In table.Rows
                    Dim fields As String() = row.ItemArray.Select(Function(field) field.ToString()).ToArray()
                    writer.WriteLine(String.Join(",", fields))
                Next
            End Using
            Console.WriteLine($"丰度矩阵已成功保存到: {filePath}")
        End Sub

    End Module

End Namespace
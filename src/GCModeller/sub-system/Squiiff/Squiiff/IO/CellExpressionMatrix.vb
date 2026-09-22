Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace IO

    ''' <summary>
    ''' 细胞 × 基因 表达矩阵（SquiDiff 的数据载体）。
    '''
    ''' 维度约定与张量一致：<c>Values(cell, gene)</c>，其中"细胞"维度对应批量维。
    ''' 注意 BNLearn 的 <c>GeneExpressionData.Matrix</c> 是 <c>[gene, sample]</c>，
    ''' 装载时需要转置（见 <see cref="ExpressionIO"/>）。
    ''' </summary>
    Public Class CellExpressionMatrix

        Public Sub New(cellNames As String(), geneNames As String(), values As Double(,))
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))

            Me.CellNames = If(cellNames, New String() {})
            Me.GeneNames = If(geneNames, New String() {})
            Me.Values = values
        End Sub

        ''' <summary>细胞名（长度 = 第 0 维）。</summary>
        Public ReadOnly Property CellNames As String()

        ''' <summary>基因名（长度 = 第 1 维）。</summary>
        Public ReadOnly Property GeneNames As String()

        ''' <summary>表达值 <c>[cell, gene]</c>。</summary>
        Public ReadOnly Property Values As Double(,)

        Public ReadOnly Property NCell As Integer
            Get
                Return Values.GetLength(0)
            End Get
        End Property

        Public ReadOnly Property NGene As Integer
            Get
                Return Values.GetLength(1)
            End Get
        End Property

        ''' <summary>转为张量 <c>[cell, gene]</c>（行优先拷贝）。</summary>
        Public Function ToTensor() As Tensor
            Dim rows = NCell
            Dim columns = NGene
            Dim t = New Tensor(New Integer() {rows, columns})
            Dim data = t.Data

            For i As Integer = 0 To rows - 1
                Dim offset = i * columns
                For j As Integer = 0 To columns - 1
                    data(offset + j) = Values(i, j)
                Next
            Next

            Call t.MarkHostModified()
            Return t
        End Function

        ''' <summary>由张量构造（<paramref name="t"/> 形状须为 <c>[cell, gene]</c>）。</summary>
        Public Shared Function FromTensor(t As Tensor, cellNames As String(), geneNames As String()) As CellExpressionMatrix
            Dim rows = t.Shape(0)
            Dim columns = t.Shape(1)
            Dim values(rows - 1, columns - 1) As Double
            Dim data = t.Data

            For i As Integer = 0 To rows - 1
                Dim offset = i * columns
                For j As Integer = 0 To columns - 1
                    values(i, j) = data(offset + j)
                Next
            Next

            Return New CellExpressionMatrix(cellNames, geneNames, values)
        End Function

        ''' <summary>按基因名取子集（保序，缺失的基因会跳过）。</summary>
        Public Function SubsetGenes(selected As String()) As CellExpressionMatrix
            Dim indexMap As New List(Of Integer)
            Dim names As New List(Of String)

            For Each name In selected
                Dim index = Array.IndexOf(Me.GeneNames, name)
                If index >= 0 Then
                    indexMap.Add(index)
                    names.Add(name)
                End If
            Next

            Dim values(NCell - 1, indexMap.Count - 1) As Double
            For i As Integer = 0 To NCell - 1
                For j As Integer = 0 To indexMap.Count - 1
                    values(i, j) = Me.Values(i, indexMap(j))
                Next
            Next

            Return New CellExpressionMatrix(Me.CellNames, names.ToArray(), values)
        End Function

        ''' <summary>取出全部细胞的平均表达谱（长度 = NGene）。</summary>
        Public Function MeanProfile() As Double()
            Dim result(NGene - 1) As Double
            If NCell = 0 Then Return result

            For j As Integer = 0 To NGene - 1
                Dim sum As Double = 0.0
                For i As Integer = 0 To NCell - 1
                    sum += Values(i, j)
                Next
                result(j) = sum / NCell
            Next

            Return result
        End Function

        ''' <summary>按行名取子集。</summary>
        Public Function SubsetCells(selected As String()) As CellExpressionMatrix
            Dim indexMap As New List(Of Integer)
            Dim names As New List(Of String)

            For Each name In selected
                Dim index = Array.IndexOf(Me.CellNames, name)
                If index >= 0 Then
                    indexMap.Add(index)
                    names.Add(name)
                End If
            Next

            Dim values(indexMap.Count - 1, NGene - 1) As Double
            For i As Integer = 0 To indexMap.Count - 1
                For j As Integer = 0 To NGene - 1
                    values(i, j) = Me.Values(indexMap(i), j)
                Next
            Next

            Return New CellExpressionMatrix(names.ToArray(), Me.GeneNames, values)
        End Function

        Public Overrides Function ToString() As String
            Return $"表达矩阵 {NCell} 细胞 × {NGene} 基因"
        End Function
    End Class
End Namespace

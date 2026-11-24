' ============================================================================
' EnsembleFeatureMatrix.vb
'
' Ensemble 框架只接受 0-based 稠密 Double()，而 metaTraits 的 SVM 路径使用
' 1-based 稀疏 Node()。这里提供两者之间的无损转换，并统一按照
' PfamEmbedding.dimensionNames 的顺序对齐特征。
' ============================================================================

Imports System.Linq
Imports Microsoft.VisualBasic.MachineLearning.SVM
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure

Namespace Traitar.Modules

    ''' <summary>
    ''' Pfam 特征在 Ensemble 稠密向量与 SVM 稀疏节点两种契约之间的转换
    ''' </summary>
    Public Module EnsembleFeatureMatrix

        ''' <summary>
        ''' 把多行 <see cref="SupportVector"/> 转换为 0-based 稠密矩阵，
        ''' 列顺序严格对齐 <paramref name="dimensionNames"/>。
        ''' </summary>
        ''' <param name="rows">训练样本行</param>
        ''' <param name="dimensionNames">Pfam 词表顺序</param>
        Public Function ToMatrix(rows As IEnumerable(Of SupportVector), dimensionNames As String()) As Double()()
            Dim list As SupportVector() = If(rows Is Nothing, New SupportVector() {}, rows.ToArray())
            Dim matrix As Double()() = New Double(list.Length - 1)() {}

            For i As Integer = 0 To list.Length - 1
                matrix(i) = ToVector(list(i).Properties, dimensionNames)
            Next

            Return matrix
        End Function

        ''' <summary>
        ''' 把「Pfam 编号 -&gt; 值」的稀疏字典展开为 0-based 稠密向量，
        ''' 缺失的维度填充 0。
        ''' </summary>
        ''' <param name="profile">Pfam 编号到归一化丰度值的映射</param>
        ''' <param name="dimensionNames">Pfam 词表顺序</param>
        Public Function ToVector(profile As IDictionary(Of String, Double), dimensionNames As String()) As Double()
            If dimensionNames Is Nothing Then
                Return New Double() {}
            End If

            Dim vector As Double() = New Double(dimensionNames.Length - 1) {}

            If profile Is Nothing Then
                Return vector
            End If

            For i As Integer = 0 To dimensionNames.Length - 1
                Dim value As Double = 0

                If profile.TryGetValue(dimensionNames(i), value) Then
                    vector(i) = value
                End If
            Next

            Return vector
        End Function

        ''' <summary>
        ''' 把 1-based 稀疏 <see cref="Node"/>() 转换为 0-based 稠密向量（缺失维度填 0）
        ''' </summary>
        ''' <param name="nodes">SVM 的稀疏节点</param>
        ''' <param name="dimensionCount">特征维度数量</param>
        Public Function FromNodes(nodes As Node(), dimensionCount As Integer) As Double()
            Dim vector As Double() = New Double(dimensionCount - 1) {}

            If nodes Is Nothing Then
                Return vector
            End If

            For Each node As Node In nodes
                Dim index As Integer = node.index - 1

                If index >= 0 AndAlso index < dimensionCount Then
                    vector(index) = node.value
                End If
            Next

            Return vector
        End Function

        ''' <summary>
        ''' 把 0-based 稠密向量转换为 SVM 所需的 1-based <see cref="Node"/>()，
        ''' 训练端与预测端必须使用同一个 <paramref name="offset"/>。
        ''' </summary>
        Public Function ToNodes(vector As Double(), Optional offset As Integer = 1) As Node()
            Dim nodes As Node() = New Node(vector.Length - 1) {}

            For i As Integer = 0 To vector.Length - 1
                nodes(i) = New Node(i + offset, vector(i))
            Next

            Return nodes
        End Function

    End Module
End Namespace

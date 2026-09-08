Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 生物学层级本体：基因 → 精细通路 → 逐级抽象的粗通路
''' </summary>
''' <remarks>
''' P-NET 的核心思想是把生物学先验知识直接"编译"进网络拓扑：网络的层数、每一层的节点数量
''' 以及层与层之间的连接方式，全部由这里所描述的层级包含关系决定，而不是由数据决定。
'''
''' 一个完整的本体由基因列表 <see cref="GeneNames"/> 以及若干层通路 <see cref="Levels"/> 组成：
'''
''' + <see cref="Levels"/>(0) 为最精细的通路层，其成员为基因；
''' + <see cref="Levels"/>(k) 的成员为 <see cref="Levels"/>(k - 1) 中的通路节点；
''' + 越往后的层级越抽象，节点数量越少，对应于 Reactome 中由精细通路逐级汇总到复杂生物过程的过程。
'''
''' 论文中的 P-NET 使用 5 层通路，因此本类型的标准形态为 1 层基因 + 5 层通路。
''' 但是本实现并不绑定固定的层数，替换为 KEGG / Gene Ontology 或者自定义的通路模块时
''' 只需要更换层级描述即可重建整个网络。
''' </remarks>
Public Class PathwayHierarchy

    ''' <summary>
    ''' 每一种基因组改变类型的后缀名，依次为突变、拷贝数扩增、拷贝数缺失
    ''' </summary>
    ''' <returns>改变类型后缀数组</returns>
    ''' <remarks>
    ''' 论文中每一个基因都被编码为 3 个二值特征（mutation / amplification / deletion），
    ''' 这直接决定了输入层到基因层的 3 → 1 块状稀疏连接模式。
    ''' </remarks>
    Public Shared ReadOnly Property AlterationTypes As String() = {"_mut", "_amp", "_del"}

    ''' <summary>
    ''' 基因名称列表，其顺序即为基因层节点的顺序
    ''' </summary>
    ''' <returns>基因名数组</returns>
    Public Property GeneNames As String()

    ''' <summary>
    ''' 由精细到粗逐级排列的通路层
    ''' </summary>
    ''' <returns>通路层列表，第 0 个元素为最精细的通路层</returns>
    Public Property Levels As List(Of HierarchyLevel)

    ''' <summary>
    ''' 创建一个空的本体对象
    ''' </summary>
    Public Sub New()
        Levels = New List(Of HierarchyLevel)()
    End Sub

    ''' <summary>
    ''' 使用给定的基因名与通路层创建本体对象
    ''' </summary>
    ''' <param name="geneNames">基因名数组</param>
    ''' <param name="levels">由精细到粗排列的通路层</param>
    Public Sub New(geneNames As String(), levels As IEnumerable(Of HierarchyLevel))
        Me.GeneNames = geneNames
        Me.Levels = New List(Of HierarchyLevel)(levels)
    End Sub

    ''' <summary>
    ''' 基因数量，即基因层节点数量
    ''' </summary>
    ''' <returns>基因数量</returns>
    Public ReadOnly Property GeneCount As Integer
        Get
            If GeneNames Is Nothing Then
                Return 0
            End If

            Return GeneNames.Length
        End Get
    End Property

    ''' <summary>
    ''' 输入层节点数量，等于基因数量的 3 倍（突变 / 扩增 / 缺失）
    ''' </summary>
    ''' <returns>输入特征维度</returns>
    Public ReadOnly Property InputSize As Integer
        Get
            Return GeneCount * 3
        End Get
    End Property

    ''' <summary>
    ''' 通路层的数量
    ''' </summary>
    ''' <returns>通路层数</returns>
    Public ReadOnly Property PathwayDepth As Integer
        Get
            If Levels Is Nothing Then
                Return 0
            End If

            Return Levels.Count
        End Get
    End Property

    ''' <summary>
    ''' 网络中的隐藏层数量，等于 1 层基因层加上所有通路层
    ''' </summary>
    ''' <returns>隐藏层数量</returns>
    Public ReadOnly Property LayerCount As Integer
        Get
            Return PathwayDepth + 1
        End Get
    End Property

    ''' <summary>
    ''' 生成输入层的特征名称，依次为每一个基因的突变、扩增、缺失三个特征
    ''' </summary>
    ''' <returns>输入特征名称数组，长度为 <see cref="InputSize"/></returns>
    Public Function GetInputFeatureNames() As String()
        Dim names As String() = New String(InputSize - 1) {}
        Dim p As Integer = 0

        For g As Integer = 0 To GeneCount - 1
            For t As Integer = 0 To AlterationTypes.Length - 1
                names(p) = GeneNames(g) & AlterationTypes(t)
                p += 1
            Next
        Next

        Return names
    End Function

    ''' <summary>
    ''' 获取指定网络层（不含输入层）的节点名称
    ''' </summary>
    ''' <param name="layerIndex">
    ''' 网络层索引，0 表示基因层，1 到 <see cref="PathwayDepth"/> 依次表示由精细到粗的通路层
    ''' </param>
    ''' <returns>节点名称数组</returns>
    Public Function GetNodeNames(layerIndex As Integer) As String()
        If layerIndex = 0 Then
            Return GeneNames
        End If

        Return Levels(layerIndex - 1).Nodes
    End Function

    ''' <summary>
    ''' 获取指定网络层的节点数量
    ''' </summary>
    ''' <param name="layerIndex">网络层索引，0 表示基因层</param>
    ''' <returns>节点数量</returns>
    Public Function GetLayerSize(layerIndex As Integer) As Integer
        If layerIndex = 0 Then
            Return GeneCount
        End If

        Return Levels(layerIndex - 1).Count
    End Function

    ''' <summary>
    ''' 获取指定网络层的名称
    ''' </summary>
    ''' <param name="layerIndex">网络层索引，0 表示基因层</param>
    ''' <returns>层名称</returns>
    Public Function GetLayerName(layerIndex As Integer) As String
        If layerIndex = 0 Then
            Return "Genes"
        End If

        Return Levels(layerIndex - 1).Name
    End Function

    ''' <summary>
    ''' 编译指定网络层的稀疏连接结构
    ''' </summary>
    ''' <param name="layerIndex">网络层索引，0 表示基因层（其输入为三倍基因数的改变特征）</param>
    ''' <returns>该层的稀疏连接边表</returns>
    ''' <remarks>
    ''' 对于基因层（<paramref name="layerIndex"/> = 0），每一个基因节点恰好连接到输入层中
    ''' 属于该基因的 3 个改变特征节点，形成 3 → 1 的块状稀疏结构；
    ''' 对于其余各层，则直接使用该通路层所描述的父子包含关系。
    ''' </remarks>
    Public Function BuildConnectivity(layerIndex As Integer) As SparseConnectivity
        If layerIndex = 0 Then
            Dim geneMembers As Integer()() = New Integer(GeneCount - 1)() {}

            For g As Integer = 0 To GeneCount - 1
                geneMembers(g) = New Integer() {g * 3, g * 3 + 1, g * 3 + 2}
            Next

            Return New SparseConnectivity(InputSize, GeneCount, geneMembers)
        End If

        Dim fanIn As Integer = GetLayerSize(layerIndex - 1)
        Dim level As HierarchyLevel = Levels(layerIndex - 1)

        Return New SparseConnectivity(fanIn, level.Count, level.Members)
    End Function

    ''' <summary>
    ''' 编译整个网络所有层的稀疏连接结构
    ''' </summary>
    ''' <returns>逐层稀疏连接边表数组，长度为 <see cref="LayerCount"/></returns>
    Public Function BuildAllConnectivity() As SparseConnectivity()
        Dim all As SparseConnectivity() = New SparseConnectivity(LayerCount - 1) {}

        For i As Integer = 0 To LayerCount - 1
            all(i) = BuildConnectivity(i)
        Next

        Return all
    End Function

    ''' <summary>
    ''' 计算指定网络层中每一个节点的度（入度与出度之和）
    ''' </summary>
    ''' <param name="layerIndex">网络层索引，0 表示基因层</param>
    ''' <returns>度数组，长度等于该层节点数</returns>
    ''' <remarks>
    ''' 论文在解释模型的时候发现，一个通路节点如果属于过多的父通路（枢纽节点、过度注释），
    ''' 会通过更多的路径汇聚归因分数，导致其重要性被系统性高估，因此需要使用节点度做偏倚校正。
    ''' </remarks>
    Public Function GetNodeDegrees(layerIndex As Integer) As Double()
        Dim n As Integer = GetLayerSize(layerIndex)
        Dim deg As Double() = New Double(n - 1) {}
        Dim conn As SparseConnectivity = BuildConnectivity(layerIndex)

        ' 入度：来自于下一层（更精细的层）的父节点连接数量
        For j As Integer = 0 To n - 1
            deg(j) += conn.GetFanIn(j)
        Next

        ' 出度：本层节点作为子节点出现在上一层（更粗的层）中的次数
        If layerIndex + 1 < LayerCount Then
            Dim upper As SparseConnectivity = BuildConnectivity(layerIndex + 1)

            For p As Integer = 0 To upper.EdgeCount - 1
                deg(upper.ChildIdx(p)) += 1
            Next
        Else
            ' 最粗的一层直接连接到输出预测头，每一个节点出度为 1
            For j As Integer = 0 To n - 1
                deg(j) += 1
            Next
        End If

        Return deg
    End Function

    ''' <summary>
    ''' 剔除空通路与孤立节点，并修正越界的成员索引
    ''' </summary>
    ''' <returns>经过清洗之后得到的新本体对象</returns>
    ''' <remarks>
    ''' 从公共数据库（例如 Reactome）导出的层级描述中往往会存在空通路、
    ''' 重名通路以及指向不存在的子节点的越界索引，在编译为网络之前必须先做清洗。
    ''' </remarks>
    Public Function Cleanup() As PathwayHierarchy
        Dim genes As String() = If(GeneNames, New String(-1) {})
        Dim result As New PathwayHierarchy() With {.GeneNames = genes}
        Dim previousNames As String() = genes
        Dim previousMap As Integer() = Nothing

        For Each rawLevel As HierarchyLevel In Levels
            Dim keepNodes As New List(Of String)()
            Dim keepMembers As New List(Of Integer())()
            Dim remap As New List(Of Integer)()

            For i As Integer = 0 To rawLevel.Count - 1
                Dim idx As New List(Of Integer)()

                If rawLevel.Members IsNot Nothing AndAlso i < rawLevel.Members.Length AndAlso rawLevel.Members(i) IsNot Nothing Then
                    For Each ci As Integer In rawLevel.Members(i)
                        If ci < 0 OrElse ci >= previousNames.Length Then
                            Continue For
                        End If
                        If previousMap IsNot Nothing Then
                            If previousMap(ci) < 0 Then
                                Continue For
                            End If
                            idx.Add(previousMap(ci))
                        Else
                            idx.Add(ci)
                        End If
                    Next
                End If

                If idx.Count = 0 Then
                    Continue For
                End If

                keepNodes.Add(rawLevel.Nodes(i))
                keepMembers.Add(idx.Distinct().OrderBy(Function(x) x).ToArray())
                remap.Add(i)
            Next

            ' 构造本层到上一层的映射表，供下一层重映射使用
            Dim map As Integer() = New Integer(rawLevel.Count - 1) {}

            For i As Integer = 0 To map.Length - 1
                map(i) = -1
            Next
            For i As Integer = 0 To remap.Count - 1
                map(remap(i)) = i
            Next

            Dim level As New HierarchyLevel(
                rawLevel.Name,
                keepNodes.ToArray(),
                keepMembers.ToArray()
            )

            result.Levels.Add(level)

            previousNames = level.Nodes
            previousMap = map
        Next

        Return result
    End Function

    ''' <summary>
    ''' 生成本体的字符串描述
    ''' </summary>
    ''' <returns>形如 ``Hierarchy[48 genes -> 16 -> 10 -> 6 -> 4 -> 2]`` 的描述文本</returns>
    Public Overrides Function ToString() As String
        Dim sizes As New List(Of String)() From {GeneCount.ToString()}

        For Each level As HierarchyLevel In Levels
            sizes.Add(level.Count.ToString())
        Next

        Return $"Hierarchy[{String.Join(" -> ", sizes)}]"
    End Function

End Class

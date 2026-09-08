Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 稀疏连接内核：由生物层级父子关系编译出来的边表
''' </summary>
''' <remarks>
''' P-NET 的每一个网络层都由一个二值掩码矩阵 <c>M</c> 约束其连接拓扑，
''' 前向传播为 <c>y = f[(M * W)&#8407;x + b]</c>。由于掩码在整个训练过程中保持不变，
''' 因此可以在网络构建阶段就将 <c>M</c> 编译为一个按父节点分组的边表，
''' 在运行期用 O(N * nnz) 的 scatter/gather 内核代替 O(N * fanIn * fanOut) 的稠密矩阵乘法。
''' 这正是论文 Extended Data Fig. 1c 所描述的 "patterned sparse matrix" 优化。
'''
''' 边表的存储布局为：
'''
''' + ``ChildIdx`` 按照父节点分组连续存放，父节点 ``j`` 的所有子节点索引位于区间
'''   ``[ParentStart(j), ParentStop(j))`` 之内；
''' + ``Weights`` 矩阵中的数据布局为行优先，即 ``W(i, j) = Data(i * FanOut + j)``，
'''   与 <see cref="Tensor"/> 的内存布局保持一致，方便直接对 <see cref="Tensor.Data"/> 做下标运算。
''' </remarks>
Public Class SparseConnectivity

    ''' <summary>
    ''' 输入维度（子节点数量，即掩码矩阵的行数）
    ''' </summary>
    ''' <returns>子节点数量</returns>
    Public ReadOnly Property FanIn As Integer

    ''' <summary>
    ''' 输出维度（父节点数量，即掩码矩阵的列数）
    ''' </summary>
    ''' <returns>父节点数量</returns>
    Public ReadOnly Property FanOut As Integer

    ''' <summary>
    ''' 掩码矩阵中非零元素的数量，即真实存在的生物学父子关系数量
    ''' </summary>
    ''' <returns>连接边数量</returns>
    Public ReadOnly Property EdgeCount As Integer

    ''' <summary>
    ''' 按父节点分组的子节点索引表，长度为 <see cref="EdgeCount"/>
    ''' </summary>
    ''' <returns>子节点索引数组</returns>
    Public ReadOnly Property ChildIdx As Integer()

    ''' <summary>
    ''' 每一个父节点在 <see cref="ChildIdx"/> 中的起始偏移，长度为 <see cref="FanOut"/> + 1
    ''' </summary>
    ''' <returns>偏移数组，``ParentStart(FanOut)`` 即为 <see cref="EdgeCount"/></returns>
    Public ReadOnly Property ParentStart As Integer()

    ''' <summary>
    ''' 创建一个空的稀疏连接对象（对应于全零掩码）
    ''' </summary>
    ''' <param name="fanIn">输入维度</param>
    ''' <param name="fanOut">输出维度</param>
    Public Sub New(fanIn As Integer, fanOut As Integer)
        Me.FanIn = fanIn
        Me.FanOut = fanOut
        Me.EdgeCount = 0
        Me.ChildIdx = New Integer(-1) {}
        Me.ParentStart = New Integer(fanOut) {}
    End Sub

    ''' <summary>
    ''' 由父子成员关系编译得到稀疏连接边表
    ''' </summary>
    ''' <param name="fanIn">输入维度（下一层的节点数量）</param>
    ''' <param name="fanOut">输出维度（当前层的节点数量）</param>
    ''' <param name="members">每一个父节点所包含的子节点索引，长度必须等于 <paramref name="fanOut"/></param>
    Public Sub New(fanIn As Integer, fanOut As Integer, members As Integer()())
        Me.FanIn = fanIn
        Me.FanOut = fanOut

        Dim start As Integer() = New Integer(fanOut) {}
        Dim total As Integer = 0
        Dim i As Integer

        For i = 0 To fanOut - 1
            start(i) = total

            If members IsNot Nothing AndAlso i < members.Length AndAlso members(i) IsNot Nothing Then
                total += members(i).Length
            End If
        Next

        start(fanOut) = total

        Dim child As Integer() = New Integer(total - 1) {}
        Dim p As Integer = 0

        For i = 0 To fanOut - 1
            If members IsNot Nothing AndAlso i < members.Length AndAlso members(i) IsNot Nothing Then
                For Each ci As Integer In members(i)
                    child(p) = ci
                    p += 1
                Next
            End If
        Next

        Me.EdgeCount = total
        Me.ChildIdx = child
        Me.ParentStart = start
    End Sub

    ''' <summary>
    ''' 获取指定父节点的子节点索引在 <see cref="ChildIdx"/> 中的起始位置
    ''' </summary>
    ''' <param name="parentIndex">父节点索引</param>
    ''' <returns>起始偏移</returns>
    Public Function Start(parentIndex As Integer) As Integer
        Return ParentStart(parentIndex)
    End Function

    ''' <summary>
    ''' 获取指定父节点的子节点索引在 <see cref="ChildIdx"/> 中的结束位置（不含）
    ''' </summary>
    ''' <param name="parentIndex">父节点索引</param>
    ''' <returns>结束偏移</returns>
    Public Function [Stop](parentIndex As Integer) As Integer
        Return ParentStart(parentIndex + 1)
    End Function

    ''' <summary>
    ''' 获取指定父节点的入度（子节点数量，即 fan-in）
    ''' </summary>
    ''' <param name="parentIndex">父节点索引</param>
    ''' <returns>入度</returns>
    Public Function GetFanIn(parentIndex As Integer) As Integer
        Return ParentStart(parentIndex + 1) - ParentStart(parentIndex)
    End Function

    ''' <summary>
    ''' 构建二值掩码矩阵 <c>M</c>，形状为 [FanIn, FanOut]
    ''' </summary>
    ''' <returns>元素取值为 0 或者 1 的 <see cref="Tensor"/> 掩码矩阵</returns>
    ''' <remarks>
    ''' 掩码矩阵仅在需要检视或者落盘的时候才会被创建，运行期的计算完全走
    ''' <see cref="ChildIdx"/> 边表，不会使用这个矩阵。
    ''' </remarks>
    Public Function BuildMask() As Tensor
        Dim mask As New Tensor(FanIn, FanOut)
        Dim data As Double() = mask.Data

        For j As Integer = 0 To FanOut - 1
            For p As Integer = ParentStart(j) To ParentStart(j + 1) - 1
                data(ChildIdx(p) * FanOut + j) = 1.0
            Next
        Next

        Return mask
    End Function

    ''' <summary>
    ''' 声明一个连接是否存在
    ''' </summary>
    ''' <param name="child">子节点索引</param>
    ''' <param name="parent">父节点索引</param>
    ''' <returns>存在则返回 True，否则返回 False</returns>
    Public Function HasEdge(child As Integer, parent As Integer) As Boolean
        For p As Integer = ParentStart(parent) To ParentStart(parent + 1) - 1
            If ChildIdx(p) = child Then
                Return True
            End If
        Next

        Return False
    End Function

    ''' <summary>
    ''' 生成稀疏连接的字符串描述
    ''' </summary>
    ''' <returns>形如 ``Sparse[180 -> 60, nnz=180, density=1.67%]`` 的描述文本</returns>
    Public Overrides Function ToString() As String
        Dim dense As Double = FanIn * FanOut
        Dim ratio As Double = If(dense > 0, EdgeCount / dense, 0.0)

        Return $"Sparse[{FanIn} -> {FanOut}, nnz={EdgeCount}, density={(ratio * 100).ToString("F2")}%]"
    End Function

End Class

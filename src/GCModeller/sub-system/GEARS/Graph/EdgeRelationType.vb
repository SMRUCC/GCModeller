Imports SMRUCC.genomics.Analysis.BNLearn

Namespace Graph

    ''' <summary>
    ''' 基因调控图中边的关系类型
    ''' </summary>
    ''' <remarks>
    ''' GEARS 将先验调控网络建模为异质图（heterogeneous graph），
    ''' 不同类型的调控关系在消息传递时使用不同的变换矩阵与符号，
    ''' 从而让模型能够学习到「激活边传递同向信号、抑制边传递反向信号」这样的生物学语义。
    ''' </remarks>
    Public Enum EdgeRelationType
        ''' <summary>自环边（节点保留自身信息）</summary>
        SelfLoop = 0
        ''' <summary>激活/正向调控（TF 促进靶基因表达）</summary>
        Activation = 1
        ''' <summary>抑制/负向调控（TF 抑制靶基因表达）</summary>
        Repression = 2
        ''' <summary>共表达边（由 control 表达谱相关性推断，无方向信息）</summary>
        CoExpression = 3
    End Enum

    ''' <summary>
    ''' <see cref="EdgeRelationType"/> 的工具方法集合
    ''' </summary>
    Public Module EdgeRelationTypes

        ''' <summary>关系类型的总数（用于分配每种类型专属的变换矩阵）</summary>
        ''' <returns>关系类型的数量</returns>
        Public Const NumRelationTypes As Integer = 4

        ''' <summary>
        ''' 获取指定关系类型在消息传递时的符号：
        ''' 激活为 +1，抑制为 -1，其余为 +1。
        ''' </summary>
        ''' <param name="type">边的关系类型</param>
        ''' <returns>消息符号，取值为 +1 或者 -1</returns>
        Public Function MessageSign(type As EdgeRelationType) As Double
            Select Case type
                Case EdgeRelationType.Repression
                    Return -1.0
                Case Else
                    Return 1.0
            End Select
        End Function

        ''' <summary>
        ''' 将所有关系类型的消息符号按照枚举值顺序导出为数组
        ''' </summary>
        ''' <returns>长度为 <see cref="NumRelationTypes"/> 的符号数组</returns>
        Public Function SignTable() As Double()
            Dim signs As Double() = New Double(NumRelationTypes - 1) {}

            For i As Integer = 0 To NumRelationTypes - 1
                signs(i) = MessageSign(CType(i, EdgeRelationType))
            Next

            Return signs
        End Function

        ''' <summary>
        ''' 从先验网络的效应器类型映射到图边的关系类型
        ''' </summary>
        ''' <param name="effector">BNLearn 先验网络中的调控效应类型</param>
        ''' <returns>对应的图边关系类型；未知效应按激活处理</returns>
        Public Function FromEffector(effector As Effector) As EdgeRelationType
            Select Case effector
                Case Effector.Inhibitor
                    Return EdgeRelationType.Repression
                Case Else
                    Return EdgeRelationType.Activation
            End Select
        End Function
    End Module
End Namespace

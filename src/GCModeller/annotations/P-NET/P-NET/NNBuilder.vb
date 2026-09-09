#Region "Microsoft.VisualBasic::eff1eb5d3c0af2974767d95f46de03fa, annotations\P-NET\P-NET\NNBuilder.vb"

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

    '   Total Lines: 3
    '    Code Lines: 2 (66.67%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 1 (33.33%)
    '     File Size: 35 B


    ' Class NNBuilder
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

''' <summary>
''' 网络构建配置
''' </summary>
''' <remarks>
''' 这里只放置"构建网络拓扑"阶段所需要的参数，与训练阶段有关的超参数
''' （学习率、批大小、迭代轮数等）放在 <c>TrainConfig</c> 之中。
''' </remarks>
Public Class PNETBuildConfig

    ''' <summary>
    ''' 权重初始化的随机数种子，给出之后网络初始化结果可复现
    ''' </summary>
    ''' <returns>种子值，取 Nothing 时每次随机初始化</returns>
    Public Property Seed As Integer? = Nothing

    ''' <summary>
    ''' 深度监督损失的深度加权系数，取值越大则深层预测头的损失占比越高
    ''' </summary>
    ''' <returns>加权系数，默认值为 1.0</returns>
    Public Property DeepSupervisionLambda As Double = 1.0

    ''' <summary>
    ''' 权重初始化的标准差，取 Nothing 时使用 Xavier 初始化
    ''' </summary>
    ''' <returns>标准差</returns>
    Public Property WeightInitStd As Double? = Nothing

    ''' <summary>
    ''' 是否在构建之前先对层级做清洗（剔除空通路与孤立节点）
    ''' </summary>
    ''' <returns>默认值为 True</returns>
    Public Property CleanupHierarchy As Boolean = True

End Class

''' <summary>
''' 把生物学层级"编译"为 P-NET 网络拓扑的构建器
''' </summary>
''' <remarks>
''' P-NET 与常规神经网络最大的区别在于：它的层数、每一层的节点数量以及层与层之间的连接方式
''' 全部由外部的生物通路数据库（Reactome 的 <c>.gmt</c> 文件）所决定，而不是由数据决定。
''' 因此"构建网络"这一步的本质就是把层级包含关系翻译为逐层的二值掩码矩阵。
'''
''' 典型用法：
'''
''' <code>
''' Dim hierarchy As PathwayHierarchy = DemoData.CreateHierarchy(seed:=123)
''' Dim model As PNETModel = NNBuilder.Build(hierarchy)
''' </code>
'''
''' 由于本类型原先已经在项目中被声明，这里保留了类型名与文件名，
''' 只是把它从空类型改写为真正的网络构建工厂。
''' </remarks>
Public Class NNBuilder

    ''' <summary>
    ''' 由生物层级本体构建 P-NET 网络
    ''' </summary>
    ''' <param name="hierarchy">生物层级本体（基因 + 由精细到粗排列的若干层通路）</param>
    ''' <param name="config">构建配置，取 Nothing 时使用默认配置</param>
    ''' <returns>构建完成的 P-NET 模型</returns>
    ''' <remarks>
    ''' 构建流程为：
    '''
    ''' 1. 清洗层级，剔除空通路与越界成员；
    ''' 2. 逐层编译稀疏连接边表（基因层为每基因 3 条边的块状稀疏结构）；
    ''' 3. 为每一层创建 <see cref="MaskedDenseLayer"/> 与对应的 <see cref="PredictionHead"/>；
    ''' 4. 按照深度线性递增分配深度监督预测头的损失权重。
    ''' </remarks>
    Public Shared Function Build(hierarchy As PathwayHierarchy, Optional config As PNETBuildConfig = Nothing) As PNETModel
        If hierarchy Is Nothing Then
            Throw New ArgumentNullException(NameOf(hierarchy))
        End If
        If config Is Nothing Then
            config = New PNETBuildConfig()
        End If

        Dim ontology As PathwayHierarchy = If(config.CleanupHierarchy, hierarchy.Cleanup(), hierarchy)

        If ontology.GeneCount = 0 Then
            Throw New ArgumentException("层级本体之中没有任何基因，无法构建 P-NET 网络")
        End If
        If ontology.PathwayDepth = 0 Then
            Throw New ArgumentException("层级本体之中没有任何通路层，无法构建 P-NET 网络")
        End If

        Dim connectivity As SparseConnectivity() = ontology.BuildAllConnectivity()
        Dim layers As New List(Of MaskedDenseLayer)()
        Dim heads As New List(Of PredictionHead)()
        Dim seed As Integer? = config.Seed

        For i As Integer = 0 To connectivity.Length - 1
            Dim conn As SparseConnectivity = connectivity(i)
            Dim name As String = ontology.GetLayerName(i)
            Dim layerSeed As Integer? = Nothing

            If seed.HasValue Then
                layerSeed = seed.Value + i * 7919
            End If

            Dim layer As New MaskedDenseLayer(conn, name, layerSeed, config.WeightInitStd)

            layers.Add(layer)
        Next

        For i As Integer = 0 To layers.Count - 1
            Dim headSeed As Integer? = Nothing

            If seed.HasValue Then
                headSeed = seed.Value + 104729 + i * 7919
            End If

            Dim head As New PredictionHead(layers(i).FanOut, $"head_{ontology.GetLayerName(i)}", 1.0, headSeed)

            heads.Add(head)
        Next

        Dim model As New PNETModel(ontology, layers, heads)

        Call model.SetDeepSupervisionWeights(config.DeepSupervisionLambda)

        Return model
    End Function

    ''' <summary>
    ''' 由一组按照由精细到粗顺序排列的 <c>.gmt</c> 文件构建 P-NET 网络
    ''' </summary>
    ''' <param name="gmtFiles">gmt 文件路径数组，第 0 个文件的成员必须为基因名</param>
    ''' <param name="config">构建配置，取 Nothing 时使用默认配置</param>
    ''' <returns>构建完成的 P-NET 模型</returns>
    Public Shared Function BuildFromGmt(gmtFiles As IEnumerable(Of String),
                                        Optional config As PNETBuildConfig = Nothing) As PNETModel
        Dim hierarchy As PathwayHierarchy = GmtIO.FromGmtFiles(gmtFiles)

        Return Build(hierarchy, config)
    End Function

    ''' <summary>
    ''' 计算给定的层级在稀疏与稠密两种连接模式下的参数量，用于在构建之前评估网络规模
    ''' </summary>
    ''' <param name="hierarchy">生物层级本体</param>
    ''' <returns>
    ''' 长度为 2 的数组，第 0 项为稀疏（P-NET）参数量，第 1 项为同等节点数稠密网络的参数量
    ''' </returns>
    Public Shared Function EstimateParameterCount(hierarchy As PathwayHierarchy) As Long()
        Dim ontology As PathwayHierarchy = hierarchy.Cleanup()
        Dim sparse As Long = 0
        Dim dense As Long = 0
        Dim connectivity As SparseConnectivity() = ontology.BuildAllConnectivity()

        For i As Integer = 0 To connectivity.Length - 1
            Dim conn As SparseConnectivity = connectivity(i)

            sparse += conn.EdgeCount + conn.FanOut
            dense += CLng(conn.FanIn) * conn.FanOut + conn.FanOut
        Next

        ' 每一个隐藏层之后都挂一个预测头
        For i As Integer = 0 To connectivity.Length - 1
            sparse += connectivity(i).FanOut + 1
            dense += connectivity(i).FanOut + 1
        Next

        Return New Long() {sparse, dense}
    End Function

End Class

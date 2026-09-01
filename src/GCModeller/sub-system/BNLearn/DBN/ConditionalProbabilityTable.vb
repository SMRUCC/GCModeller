#Region "Microsoft.VisualBasic::cc089f7c114d8920842d25f591fdda0b, sub-system\BNLearn\DBN\ConditionalProbabilityTable.vb"

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

    '   Total Lines: 94
    '    Code Lines: 46 (48.94%)
    ' Comment Lines: 28 (29.79%)
    '    - Xml Docs: 89.29%
    ' 
    '   Blank Lines: 20 (21.28%)
    '     File Size: 3.79 KB


    '     Class ConditionalProbabilityTable
    ' 
    '         Properties: ParentIds, States, Table, VariableId
    ' 
    '         Function: GetAllParentConfigurations, GetDistribution, GetKey
    ' 
    '         Sub: SetDistribution
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace DBN


    ' ==================== Conditional Probability Table ====================

    ''' <summary>
    ''' Conditional Probability Table (CPT) for a DBN node.
    ''' Stores P(node_state | parent_states) for all combinations of parent states.
    ''' 
    ''' The table is indexed by a string key formed by joining parent state values
    ''' with the "|" separator. Each entry contains a probability distribution over
    ''' the child node's states.
    ''' </summary>
    Public Class ConditionalProbabilityTable

        ''' <summary>ID of the variable (child node) this CPT belongs to</summary>
        Public Property VariableId As String

        ''' <summary>Ordered list of parent node IDs (order matters for key construction)</summary>
        Public Property ParentIds As New List(Of String)

        ''' <summary>Discrete states of the child node (e.g., "Low", "Medium", "High")</summary>
        Public Property States As New List(Of String)

        ''' <summary>
        ''' Probability table: key = "|"-joined parent states, value = probability array.
        ''' The array is aligned with the States list.
        ''' 
        ''' 注意：当父节点较多、配置总数（3^P）超过 <see cref="MaxCacheRows"/> 时，
        ''' 该表只保存"实际被访问过的配置"（惰性求值 + 记忆化），而不是完整笛卡尔积。
        ''' 缺失的配置由 <see cref="OnDemandProvider"/> 现场计算，结果与完整展开等价。
        ''' </summary>
        Public Property Table As New Dictionary(Of String, Double())

        ''' <summary>
        ''' 按需计算委托：表中不存在某个父配置时，用该委托现场计算其概率分布。
        ''' 
        ''' CPT 的行数等于各父节点状态数的乘积（默认 3 态即 3^P），父节点数不受限时
        ''' 完整展开会直接耗尽内存。由于拓扑先验分布本身是一个纯函数（noisy-OR/AND 得分
        ''' 的确定性映射），可以推迟到真正查询时再算，数学结果与全表展开完全一致。
        ''' 该委托为 Nothing 时，缺失配置回退为均匀分布（保持原有行为）。
        ''' </summary>
        Public Property OnDemandProvider As Func(Of List(Of String), Double())

        ''' <summary>
        ''' 惰性求值时允许缓存（记忆化）的最大条目数。超过该上限后不再写入缓存，
        ''' 每次查询都现场计算，避免稀疏缓存无限增长。
        ''' </summary>
        Public Property MaxCacheRows As Integer = 10000


        ''' <summary>Build the lookup key from an ordered list of parent state values</summary>
        Public Function GetKey(parentStates As List(Of String)) As String
            Return String.Join("|", parentStates)
        End Function


        ''' <summary>
        ''' Get the probability distribution over child states given a parent configuration.
        ''' 
        ''' 查表顺序：Table → <see cref="OnDemandProvider"/>（惰性求值并记忆化）→ 均匀分布。
        ''' </summary>
        ''' <param name="parentStates">父节点状态（顺序与 <see cref="ParentIds"/> 一致）</param>
        ''' <param name="copy">
        ''' 是否返回副本。只读场景传 False 可避免每次查询都分配一个新数组
        ''' （初始化/学习/预测等热路径的调用量是配置数量的量级，省下的分配非常可观）。
        ''' </param>
        Public Function GetDistribution(parentStates As List(Of String), Optional copy As Boolean = True) As Double()
            Dim key = GetKey(parentStates)
            Dim hit As Double() = Nothing

            If Table.TryGetValue(key, hit) Then
                Return If(copy, CType(hit.Clone(), Double()), hit)
            End If

            ' 惰性求值：配置未展开时现场计算，并把结果缓存起来（记忆化，避免重复计算）
            If OnDemandProvider IsNot Nothing Then
                Dim dist = OnDemandProvider(parentStates)

                If dist IsNot Nothing Then
                    If Table.Count < MaxCacheRows Then
                        Table(key) = dist
                    End If
                    Return If(copy, CType(dist.Clone(), Double()), dist)
                End If
            End If

            ' Fallback: uniform distribution
            Dim uniform(States.Count - 1) As Double
            For i = 0 To uniform.Length - 1
                uniform(i) = 1.0 / States.Count
            Next
            Return uniform
        End Function


        ''' <summary>Set the probability distribution for a parent configuration</summary>
        ''' <param name="parentStates">父节点状态（顺序与 <see cref="ParentIds"/> 一致）</param>
        ''' <param name="distribution">概率分布（长度需与 <see cref="States"/> 一致）</param>
        ''' <param name="copy">
        ''' 是否复制一份再保存。写入方持有的是刚生成的临时数组时传 False，
        ''' 可以省掉一次等长数组分配与拷贝（在 3^P 量级的初始化热路径上非常可观）。
        ''' </param>
        Public Sub SetDistribution(parentStates As List(Of String), distribution As Double(), Optional copy As Boolean = True)
            Dim key = GetKey(parentStates)
            Table(key) = If(copy, CType(distribution.Clone(), Double()), distribution)
        End Sub


        ''' <summary>
        ''' Enumerate all possible parent state combinations (Cartesian product).
        ''' Used for CPT initialization and parameter learning.
        ''' </summary>
        Public Function GetAllParentConfigurations(parentStatesMap As Dictionary(Of String, List(Of String))) As List(Of List(Of String))
            ' Start with one empty configuration
            Dim configs As New List(Of List(Of String)) From {
                New List(Of String)
            }

            For Each pid As String In ParentIds
                If Not parentStatesMap.ContainsKey(pid) Then Continue For
                Dim pStates = parentStatesMap(pid)
                Dim newConfigs As New List(Of List(Of String))
                For Each cfg In configs
                    For Each s In pStates
                        Dim newCfg = New List(Of String)(cfg)
                        newCfg.Add(s)
                        newConfigs.Add(newCfg)
                    Next
                Next
                configs = newConfigs
            Next

            Return configs
        End Function

    End Class

End Namespace

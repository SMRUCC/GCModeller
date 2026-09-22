#Region "Microsoft.VisualBasic::18d6cff8a9246f36935b0ad015879db9, engine\Cella\Networks\SubNetwork.vb"

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

    '   Total Lines: 12
    '    Code Lines: 8 (66.67%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (33.33%)
    '     File Size: 281 B


    ' Class SubNetwork
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================
' SubNetwork.vb - 细胞子网络统一契约
' ============================================================

''' <summary>
''' 虚拟细胞的一个子系统（转录调控 / 代谢 / 翻译 / 转运 / 周转 / 信号转导）
''' </summary>
''' <remarks>
''' 所有子网络只通过 <see cref="CellularState"/> 交换信息：从状态池读输入、
''' 把输出写回状态池，彼此之间不直接引用，避免循环依赖。
''' </remarks>
Public MustInherit Class SubNetwork

    Protected cell As VirtualCella

    ''' <summary>子系统名称，用于日志与快照</summary>
    Public ReadOnly Property Name As String

    ''' <summary>上一次推进实际使用的子步数（诊断用）</summary>
    Protected lastSubSteps As Integer = 0

    ''' <summary>累计推进失败次数（数值发散告警用）</summary>
    Public ReadOnly Property FailedSteps As Integer
        Get
            Return failedStepsCounter
        End Get
    End Property

    Private failedStepsCounter As Integer = 0

    Sub New(cell As VirtualCella, Optional name As String = Nothing)
        Me.cell = cell
        Me.Name = If(name, Me.GetType().Name)
    End Sub

    ''' <summary>
    ''' 把该子系统推进 <paramref name="dt"/> 个时间单位
    ''' </summary>
    Public MustOverride Sub Tick(dt As Double)

    ''' <summary>
    ''' 导出该子系统的关键统计量（诊断 / 快照 / 控制台打印）
    ''' </summary>
    Public MustOverride Function GetStats() As Dictionary(Of String, Double)

    ''' <summary>
    ''' 让本子系统的内部状态重新对齐 <see cref="CellularState"/>
    ''' </summary>
    ''' <remarks>
    ''' 只在细胞状态被外部整体改写之后调用：二分裂产生的子代继承了亲代的一半
    ''' 状态，需要让各子网络的积分器从新的状态重新起算。默认无内部状态需要同步。
    ''' </remarks>
    Public Overridable Sub Resync()
        ' 默认实现：本子系统不持有跨步的内部状态
    End Sub

    Protected Sub CountFailure()
        failedStepsCounter += 1
    End Sub

    Public Overrides Function ToString() As String
        Return $"{Name} (failed={failedStepsCounter})"
    End Function

End Class


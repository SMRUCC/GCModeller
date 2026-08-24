#Region "Microsoft.VisualBasic::dcae8f1a1ea0fe58654b42cbf1a6b77f, sub-system\BNLearn\StructureLearning\StructureLearningResult.vb"

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

    '   Total Lines: 24
    '    Code Lines: 9 (37.50%)
    ' Comment Lines: 8 (33.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (29.17%)
    '     File Size: 691 B


    '     Class StructureLearningResult
    ' 
    '         Properties: BICHistory, ElapsedMs, FinalBIC, Iterations, Network
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace StructureLearning

    ''' <summary>
    ''' 结构学习结果
    ''' </summary>
    Public Class StructureLearningResult

        ''' <summary>学习到的网络</summary>
        Public Property Network As Core.BayesianNetwork

        ''' <summary>最终 BIC 评分</summary>
        Public Property FinalBIC As Double

        ''' <summary>迭代次数</summary>
        Public Property Iterations As Integer

        ''' <summary>学习耗时（毫秒）</summary>
        Public Property ElapsedMs As Long

        ''' <summary>每步 BIC 变化记录</summary>
        Public Property BICHistory As New List(Of Double)()

    End Class
End Namespace

#Region "Microsoft.VisualBasic::2a5088ef15d49b3c1ec90cf5116e4282, Dynamics\Core\Mass\Variable.vb"

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

    '     Class Variable
    ' 
    '         Properties: Coefficient, IsTemplate, Mass
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports stdNum = System.Math

Namespace Core

    ''' <summary>
    ''' 针对描述某一个生物学功能的参数变量
    ''' </summary>
    Public Class Variable

        ''' <summary>
        ''' 对反应容器之中的某一种物质的引用
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Mass As Factor
        ''' <summary>
        ''' 在反应过程之中的变异系数，每完成一个单位的反应过程，当前的<see cref="Mass"/>
        ''' 将会丢失或者增加这个系数相对应的数量的含量
        ''' 
        ''' 这个参数应该是一个大于零的数
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Coefficient As Double

        ''' <summary>
        ''' 当前的这种物质因子在目标反应通道之中是否为模板物质？对于模板物质而言，其容量是不会被消耗掉的
        ''' 例如，转录过程或者翻译过程，基因对象或者mRNA对象为模板物质，其不会像小分子反应一样作为底物被消耗掉
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsTemplate As Boolean

        Sub New(mass As Factor, factor As Double, Optional isTemplate As Boolean = False)
            Me.Mass = mass
            Me.Coefficient = stdNum.Abs(factor)
            Me.IsTemplate = isTemplate
        End Sub

        Public Overrides Function ToString() As String
            If Not IsTemplate Then
                Return Mass.ToString
            Else
                Return $"[{Mass}]"
            End If
        End Function

    End Class
End Namespace

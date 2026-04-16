#Region "Microsoft.VisualBasic::5768936c4153ec8739f851bd885674e8, engine\Dynamics\Core\Kinetics\Controls\AdditiveControls.vb"

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

    '   Total Lines: 57
    '    Code Lines: 32 (56.14%)
    ' Comment Lines: 17 (29.82%)
    '    - Xml Docs: 76.47%
    ' 
    '   Blank Lines: 8 (14.04%)
    '     File Size: 2.17 KB


    '     Class AdditiveControls
    ' 
    '         Properties: activation, coefficient
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core

    ''' <summary>
    ''' 基于累加效应的动力学模型
    ''' 
    ''' 这个模型可能比较适用于基因表达过程的简略建模
    ''' </summary>
    Public Class AdditiveControls : Inherits Controls

        Public Property activation As Variable() = {}

        ''' <summary>
        ''' 计算出当前的调控效应单位
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property coefficient As Double
            Get
                If activation.IsNullOrEmpty AndAlso inhibition.IsNullOrEmpty Then
                    Return baseline
                End If

                Dim i = inhibition.Sum(Function(v) v.coefficient * v.mass.Value)
                Dim a = activation.Sum(Function(v) v.coefficient * v.mass.Value)

                ' 抑制的总量已经大于等于激活的总量的时候，返回零值，
                ' 则反应过程可能不会发生
                Return Math.Max((a + baseline) - i, 0)
            End Get
        End Property

        Public Overrides Function ToString() As String
            If coefficient > 0 AndAlso Not activation.IsNullOrEmpty Then
                Return $"[additive] active by {activation.Select(Function(a) a.mass.ID).GetJson}"
            ElseIf coefficient > 0 AndAlso activation.IsNullOrEmpty Then
                Return "[additive] baseline"
            Else
                Return "[additive] but no activity!"
            End If
        End Function

        ''' <summary>
        ''' 这个反应在当前方向上是自然发生的，速率为<see cref="baseline"/>
        ''' </summary>
        ''' <param name="base"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Widening Operator CType(base As Double) As AdditiveControls
            Return New AdditiveControls With {
                .baseline = base
            }
        End Operator
    End Class
End Namespace

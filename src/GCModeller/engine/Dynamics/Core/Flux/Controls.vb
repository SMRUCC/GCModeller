#Region "Microsoft.VisualBasic::0fb14cceff2f51b3fc6fffb4672a939a, Dynamics\Core\Flux\Controls.vb"

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

    '     Class Controls
    ' 
    '         Properties: activation, baseline, coefficient, inhibition
    ' 
    '         Function: ToString
    '         Operators: <, <>, =, >
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core

    ''' <summary>
    ''' 对反应过程的某一个方向的控制效应
    ''' </summary>
    Public Class Controls

        Public Property activation As Variable() = {}
        ''' <summary>
        ''' 如果抑制的总量大于激活的总量，那么这个调控的反应过程将不会进行
        ''' </summary>
        ''' <returns></returns>
        Public Property inhibition As Variable() = {}
        ''' <summary>
        ''' 没有任何调控的时候的基准反应单位，因为有些过程是不需要调控以及催化的
        ''' </summary>
        ''' <returns></returns>
        Public Property baseline As Double = 0.5

        ''' <summary>
        ''' 计算出当前的调控效应单位
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property coefficient As Double
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
            If coefficient > 0 Then
                Return $"active by {activation.Select(Function(a) a.mass.ID).GetJson}"
            Else
                Return "No activity!"
            End If
        End Function

        Public Shared Operator >(a As Controls, b As Controls) As Boolean
            If a Is Nothing Then
                Return False
            ElseIf b Is Nothing Then
                Return True
            Else
                Return a.coefficient > b.coefficient
            End If
        End Operator

        Public Shared Operator <(a As Controls, b As Controls) As Boolean
            Return Not a.coefficient > b.coefficient
        End Operator

        Public Shared Operator =(a As Controls, b As Controls) As Boolean
            If a Is Nothing AndAlso b Is Nothing Then
                Return True
            ElseIf a Is Nothing OrElse b Is Nothing Then
                Return False
            Else
                Return a.coefficient = b.coefficient
            End If
        End Operator

        Public Shared Operator <>(a As Controls, b As Controls) As Boolean
            Return Not a.coefficient = b.coefficient
        End Operator

        ''' <summary>
        ''' 这个反应在当前方向上是自然发生的，速率为<see cref="baseline"/>
        ''' </summary>
        ''' <param name="base"></param>
        ''' <returns></returns>
        Public Shared Widening Operator CType(base As Double) As Controls
            Return New Controls With {.baseline = base}
        End Operator

    End Class
End Namespace

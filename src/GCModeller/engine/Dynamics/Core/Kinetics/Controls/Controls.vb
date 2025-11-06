#Region "Microsoft.VisualBasic::63be76055c1b47f4c2b7d48197e7b5c4, engine\Dynamics\Core\Kinetics\Controls\Controls.vb"

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

    '   Total Lines: 61
    '    Code Lines: 40 (65.57%)
    ' Comment Lines: 11 (18.03%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (16.39%)
    '     File Size: 2.11 KB


    '     Class Controls
    ' 
    '         Properties: baseline, inhibition
    ' 
    '         Function: StaticControl
    '         Operators: <, <>, =, >
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace Core

    ''' <summary>
    ''' 对反应过程的某一个方向的控制效应
    ''' </summary>
    Public MustInherit Class Controls

        ''' <summary>
        ''' the regulation coefficient factor based on various related regulation factors
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride ReadOnly Property coefficient As Double

        ''' <summary>
        ''' 没有任何调控的时候的基准反应单位，因为有些过程是不需要调控以及催化的
        ''' </summary>
        ''' <returns></returns>
        Public Property baseline As Double = 0.5

        ''' <summary>
        ''' 如果抑制的总量大于激活的总量，那么这个调控的反应过程将不会进行
        ''' </summary>
        ''' <returns></returns>
        Public Property inhibition As Variable() = {}

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function StaticControl(baseline As Double) As Controls
            Return New AdditiveControls With {
                .baseline = baseline,
                .activation = {},
                .inhibition = {}
            }
        End Function

        ''' <summary>
        ''' Convert the control model as coefficient factor number
        ''' </summary>
        ''' <param name="ctl"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Narrowing Operator CType(ctl As Controls) As Double
            Return ctl.coefficient
        End Operator

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
    End Class
End Namespace

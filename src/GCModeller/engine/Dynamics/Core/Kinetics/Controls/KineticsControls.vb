#Region "Microsoft.VisualBasic::255558a4db374924cd43a12d261532db, engine\Dynamics\Core\Kinetics\Controls\KineticsControls.vb"

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

    '     Class KineticsControls
    ' 
    '         Properties: coefficient
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Core

    Public Class KineticsControls : Inherits Controls

        ''' <summary>
        ''' 计算出当前的调控效应单位
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property coefficient As Double
            Get
                If lambda Is Nothing AndAlso inhibition.IsNullOrEmpty Then
                    Return baseline
                End If

                Dim i = inhibition.Sum(Function(v) v.coefficient * v.mass.Value)
                Dim a = lambda(getMass)

                ' 抑制的总量已经大于等于激活的总量的时候，返回零值，
                ' 则反应过程可能不会发生
                Return Math.Max((a + baseline) - i, 0)
            End Get
        End Property

        ReadOnly lambda As Func(Of Func(Of String, Double), Double)
        ReadOnly getMass As Func(Of String, Double)
        ReadOnly raw As Model.Kinetics

        Sub New(env As Vessel, lambda As Model.Kinetics)
            Me.lambda = lambda.CompileLambda
            Me.raw = lambda
            Me.getMass = Function(id)
                             Return env.m_massIndex(id).Value
                         End Function
        End Sub

        Public Overrides Function ToString() As String
            Return raw.formula.ToString
        End Function
    End Class
End Namespace

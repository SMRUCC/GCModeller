﻿#Region "Microsoft.VisualBasic::ff28d3e4d44aac06bdfc2d8d7e1a3ec0, Microsoft.VisualBasic.Core\Extensions\Math\StatisticsMathExtensions\SampleObservation.vb"

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

    '     Class SampleObservation
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getRaw
    ' 
    '         Sub: Add
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Math.Statistics

    Public MustInherit Class SampleObservation

        Dim samples As New List(Of Double)

        Sub New(samples As IEnumerable(Of Double))
            Me.samples = New List(Of Double)(samples)
        End Sub

        Public Sub Add(observation As Double)
            Call samples.Add(observation)
            Call addObservation(observation)
        End Sub

        Protected MustOverride Sub addObservation(observation As Double)
        Protected MustOverride Function getEigenvalue() As Double

        Protected Iterator Function getRaw() As IEnumerable(Of Double)
            For Each obs As Double In samples
                Yield obs
            Next
        End Function

        Public Shared Narrowing Operator CType(observation As SampleObservation) As Double
            Return observation.getEigenvalue
        End Operator
    End Class
End Namespace

﻿#Region "Microsoft.VisualBasic::75f778e755b91dab0b2fcf45cb398236, Data_science\Mathematica\Math\DataFittings\TestPoint.vb"

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

    ' Structure TestPoint
    ' 
    '     Properties: Err, X, Y, Yfit
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

<XmlType("point", [Namespace]:="http://scibasic.net/math/Bootstrapping")>
Public Structure TestPoint

    <XmlAttribute("x")> Public Property X As Double
    <XmlAttribute("y")> Public Property Y As Double
    <XmlAttribute("fx")> Public Property Yfit As Double

    <XmlIgnore>
    Public ReadOnly Property Err As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Y - Yfit
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"[{X.ToString("F2")}, {Y.ToString("F2")}] {Yfit.ToString("F2")}"
    End Function

    Public Shared Narrowing Operator CType(point As TestPoint) As PointF
        Return New PointF(point.X, point.Y)
    End Operator
End Structure

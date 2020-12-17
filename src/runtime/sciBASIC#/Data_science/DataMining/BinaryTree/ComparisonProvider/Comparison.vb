﻿#Region "Microsoft.VisualBasic::fb8869c3d3c958480d4c78e2c9da5ab0, Data_science\DataMining\BinaryTree\ComparisonProvider\Comparison.vb"

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

    ' Class Comparison
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetSimilarity
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.DataFrame

Public Class Comparison : Inherits ComparisonProvider

    ReadOnly d As DistanceMatrix

    Sub New(d As DistanceMatrix, equals As Double, gt As Double)
        MyBase.New(equals, gt)
        Me.d = d
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Protected Overrides Function GetSimilarity(x As String, y As String) As Double
        Return d(x, y)
    End Function
End Class


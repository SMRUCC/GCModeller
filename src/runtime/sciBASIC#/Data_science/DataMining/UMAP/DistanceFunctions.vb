﻿#Region "Microsoft.VisualBasic::b15a65d43026ec288ce8b4932b0459d5, Data_science\DataMining\UMAP\DistanceFunctions.vb"

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

    ' Class DistanceFunctions
    ' 
    '     Function: Cosine, CosineForNormalizedVectors, Euclidean
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports stdNum = System.Math

Public NotInheritable Class DistanceFunctions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Cosine(lhs As Double(), rhs As Double()) As Double
        Return 1 - SIMD.DotProduct(lhs, rhs) / (SIMD.Magnitude(lhs) * SIMD.Magnitude(rhs))
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function CosineForNormalizedVectors(lhs As Double(), rhs As Double()) As Double
        Return 1 - SIMD.DotProduct(lhs, rhs)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Euclidean(lhs As Double(), rhs As Double()) As Double
        ' TODO: Replace with netcore3 MathF class when the framework is available
        Return stdNum.Sqrt(SIMD.Euclidean(lhs, rhs))
    End Function
End Class

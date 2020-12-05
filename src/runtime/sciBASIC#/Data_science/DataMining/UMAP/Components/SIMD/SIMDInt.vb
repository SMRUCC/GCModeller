﻿#Region "Microsoft.VisualBasic::40acb38285ba8b54e83abe70dfdc907b, Data_science\DataMining\UMAP\Components\SIMD\SIMDInt.vb"

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

    ' Module SIMDint
    ' 
    '     Sub: Uniform, Zero
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Friend Module SIMDint

    Private ReadOnly _vs1 As Integer = 8 ' Vector(Of Integer).Count
    Private ReadOnly _vs2 As Integer = 2 * _vs1 ' Vector(Of Integer).Count
    Private ReadOnly _vs3 As Integer = 3 * _vs1 ' Vector(Of Integer).Count
    Private ReadOnly _vs4 As Integer = 4 * _vs1 ' Vector(Of Integer).Count

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Zero(ByRef lhs As Integer())
        Dim count = lhs.Length
        Dim offset = 0

        While count >= SIMDint._vs4
            Vector.Zero.CopyTo(lhs, offset)
            Vector.Zero.CopyTo(lhs, offset + SIMDint._vs1)
            Vector.Zero.CopyTo(lhs, offset + SIMDint._vs2)
            Vector.Zero.CopyTo(lhs, offset + SIMDint._vs3)
            If count = SIMDint._vs4 Then Return
            count -= SIMDint._vs4
            offset += SIMDint._vs4
        End While

        If count >= SIMDint._vs2 Then
            Vector.Zero.CopyTo(lhs, offset)
            Vector.Zero.CopyTo(lhs, offset + SIMDint._vs1)
            If count = SIMDint._vs2 Then Return
            count -= SIMDint._vs2
            offset += SIMDint._vs2
        End If

        If count >= SIMDint._vs1 Then
            Vector.Zero.CopyTo(lhs, offset)
            If count = SIMDint._vs1 Then Return
            count -= SIMDint._vs1
            offset += SIMDint._vs1
        End If

        If count > 0 Then
            While count > 0
                lhs(offset) = 0
                offset += 1
                count -= 1
            End While
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Uniform(ByRef data As Double(), a As Double, random As IProvideRandomValues)
        Dim a2 = 2 * a
        Dim an = -a

        Call random.NextFloats(data)

        SIMD.Multiply(data, a2)
        SIMD.Add(data, an)
    End Sub
End Module

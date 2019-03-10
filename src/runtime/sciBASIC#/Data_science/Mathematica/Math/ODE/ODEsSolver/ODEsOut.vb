﻿#Region "Microsoft.VisualBasic::130a10e1308e31262f8321f55b73347c, Data_science\Mathematica\Math\ODE\ODEsSolver\ODEsOut.vb"

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

    ' Class ODEsOut
    ' 
    '     Properties: dx, HaveNaN, params, Resolution, x
    '                 y, y0
    ' 
    '     Function: GetEnumerator, GetY0, IEnumerable_GetEnumerator, Join, LoadFromDataFrame
    '               ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' ODEs output, this object can populates the <see cref="ODEsOut.y"/> 
''' variables values through its enumerator interface.
''' </summary>
Public Class ODEsOut : Implements IEnumerable(Of NamedCollection(Of Double))

    Public Property x As Double()
    Public Property y As Dictionary(Of NamedCollection(Of Double))

    ''' <summary>
    ''' 方程组的初始值，积分结果会受到初始值的极大的影响
    ''' </summary>
    ''' <returns></returns>
    Public Property y0 As Dictionary(Of String, Double)
    Public Property params As Dictionary(Of String, Double)

    ''' <summary>
    ''' 得到进行积分的步进值
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property dx As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return x(2) - x(1)
        End Get
    End Property

    ''' <summary>
    ''' 获取得到积分计算的分辨率的数值
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Resolution As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return (x.Last - x.First) / dx
        End Get
    End Property

    ''' <summary>
    ''' Using the first value of <see cref="y"/> as ``y0``
    ''' </summary>
    ''' <returns></returns>
    Public Function GetY0() As Dictionary(Of String, Double)
        Return y.ToDictionary(Function(x) x.Key,
                              Function(x)
                                  Return x.Value(Scan0)
                              End Function)
    End Function

    ''' <summary>
    ''' Is there NAN value in the function value <see cref="y"/> ???
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property HaveNaN As Boolean
        Get
            For Each val As NamedCollection(Of Double) In y.Values
                For Each x As Double In val.Value
                    If x.IsNaNImaginary Then
                        Return True
                    End If
                Next
            Next

            Return False
        End Get
    End Property

    ''' <summary>
    ''' Merge <see cref="y0"/> into <see cref="params"/>
    ''' </summary>
    Public Function Join() As ODEsOut
        Dim params As New Dictionary(Of String, Double)(Me.params)

        For Each v In y0
            Call params.Add(v.Key, v.Value)
        Next

        Return New ODEsOut With {
            .params = params,
            .x = x,
            .y = y,
            .y0 = y0
        }
    End Function

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="csv$"></param>
    ''' <param name="noVars">ODEs Parameter value is not exists in the data file?</param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadFromDataFrame(csv$, Optional noVars As Boolean = False) As ODEsOut
        Return StreamExtension.LoadFromDataFrame(csv, noVars)
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator(Of NamedCollection(Of Double)) Implements IEnumerable(Of NamedCollection(Of Double)).GetEnumerator
        For Each var As NamedCollection(Of Double) In y.Values
            Yield var
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class

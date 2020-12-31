#Region "Microsoft.VisualBasic::5977149fe292af55464149de24fc6a3e, Data_science\Mathematica\Math\DataFrame\Correlation\CorrelationMatrix.vb"

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

' Class CorrelationMatrix
' 
'     Properties: (+2 Overloads) pvalue
' 
'     Constructor: (+2 Overloads) Sub New
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Quantile
Imports stdNum = System.Math

Public Class CorrelationMatrix : Inherits DataMatrix

    ReadOnly pvalueMat As Double()()

    Public Property pvalue(a$, b$) As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return pvalue(names(a), names(b))
        End Get
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Set
            pvalue(names(a), names(b)) = Value
        End Set
    End Property

    Public Overridable Property pvalue(i%, j%) As Double
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return pvalueMat(j)(i)
        End Get
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Set(value As Double)
            pvalueMat(j)(i) = value
        End Set
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Sub New(names As IEnumerable(Of String))
        Call MyBase.New(names)
    End Sub

    Sub New(names As Index(Of String), matrix As Double()(), pvalue As Double()())
        Call MyBase.New(names, matrix)

        Me.pvalueMat = pvalue
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function GetUniqueTuples() As (a$, b$)()
        Return keys _
            .Select(Function(a)
                        Return keys _
                            .Where(Function(b) a <> b) _
                            .Select(Function(b) (a, b))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(t)
                         Return {t.a, t.b} _
                            .OrderBy(Function(str) str) _
                            .JoinBy("-")
                     End Function) _
            .Select(Function(t) t.First) _
            .ToArray
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetCorrelationQuantile() As QuantileEstimationGK
        Return GetUniqueTuples _
            .Select(Function(t)
                        Return stdNum.Abs(Me(t.a, t.b))
                    End Function) _
            .GKQuantile
    End Function

    ''' <summary>
    ''' note: this method returns the -log10(p.value)
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetPvalueQuantile() As QuantileEstimationGK
        Return GetUniqueTuples _
            .Select(Function(t)
                        Return -stdNum.Log10(pvalue(t.a, t.b))
                    End Function) _
            .GKQuantile
    End Function
End Class

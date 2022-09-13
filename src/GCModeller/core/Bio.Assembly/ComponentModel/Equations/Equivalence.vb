#Region "Microsoft.VisualBasic::b94dd325e5e61bd704be0ce3b14cb36c, GCModeller\core\Bio.Assembly\ComponentModel\Equations\Equivalence.vb"

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

    '   Total Lines: 90
    '    Code Lines: 73
    ' Comment Lines: 9
    '   Blank Lines: 8
    '     File Size: 3.93 KB


    '     Module Equivalence
    ' 
    '         Function: __reverseEquals, (+2 Overloads) Equals
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions

Namespace ComponentModel.EquaionModel

    Public Module Equivalence

        <Extension>
        Public Function Equals(Of T As ICompoundSpecies)(a As T, b As T, strict As Boolean) As Boolean
            If a.StoiChiometry <> b.StoiChiometry Then
                If strict Then
                    Return False
                End If
            End If

            If strict Then
                Return b.Key = a.Key
            Else
                Return String.Equals(b.Key, a.Key, StringComparison.OrdinalIgnoreCase)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <param name="cpEquals">反应物的等价性的判断方法</param>
        ''' <param name="strict"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Equals(Of T As ICompoundSpecies)(a As Equation(Of T),
                                                         b As Equation(Of T),
                                                         cpEquals As Func(Of T, T, Boolean, Boolean),
                                                         Optional strict As Boolean = False) As Boolean
            If a.Reversible <> b.Reversible Then
                If strict Then
                    Return False
                End If
            End If

            Dim equas = (From x In a.Reactants
                         Let my = (From m In b.Reactants Where cpEquals(m, x, strict) Select m).FirstOrDefault
                         Where my Is Nothing  ' 说明有一些是不同的，当前的代谢物没有相同的
                         Select x).FirstOrDefault
            If Not equas Is Nothing Then ' 查找到了不同的
                If strict Then
                    Return False
                Else
                    Return __reverseEquals(a, b, cpEquals, strict)
                End If
            Else  ' 可能是反向的
                equas = (From x In a.Products
                         Let my = (From m In b.Products Where cpEquals(m, x, strict) Select m).FirstOrDefault
                         Where my Is Nothing  ' 说明有一些是不同的
                         Select x).FirstOrDefault
                If Not equas Is Nothing Then ' 查找到了不同的
                    Return False
                Else
                    Return True
                End If
            End If
        End Function

        Private Function __reverseEquals(Of T As ICompoundSpecies)(a As Equation(Of T),
                                                                   b As Equation(Of T),
                                                                   cpEquals As Func(Of T, T, Boolean, Boolean),
                                                                   strict As Boolean) As Boolean
            Dim equas = (From x In a.Products
                         Let my = (From m In b.Reactants Where cpEquals(m, x, strict) Select m).FirstOrDefault
                         Where my Is Nothing  ' 说明有一些是不同的
                         Select x).FirstOrDefault
            If Not equas Is Nothing Then
                Return False
            End If

            equas = (From x In a.Reactants
                     Let my = (From m In b.Products Where cpEquals(m, x, strict) Select m).FirstOrDefault
                     Where my Is Nothing  ' 说明有一些是不同的
                     Select x).FirstOrDefault
            If Not equas Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Function
    End Module
End Namespace

#Region "Microsoft.VisualBasic::7c9b33cf9f0c0977622af354014d160f, sub-system\FBA\FBA.Core\Matrix.vb"

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

    '   Total Lines: 87
    '    Code Lines: 45 (51.72%)
    ' Comment Lines: 32 (36.78%)
    '    - Xml Docs: 96.88%
    ' 
    '   Blank Lines: 10 (11.49%)
    '     File Size: 2.89 KB


    ' Class Matrix
    ' 
    '     Properties: Compounds, Flux, Gaps, Matrix, NumOfCompounds
    '                 Targets
    ' 
    '     Function: AdjustRange, GetMatrix, GetTargetCoefficients
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

''' <summary>
''' 未知变量xyz为代谢过程，而在约束的目标函数之中则可以通过对代谢过程的约束计算出系数
''' </summary>
''' <remarks>
''' the flux data is constrain via a set of <see cref="DoubleRange"/>
''' </remarks>
Public Class Matrix

    ''' <summary>
    ''' 线性规划之中的行名称，约束值都是零
    ''' </summary>
    ''' <returns></returns>
    Public Property Compounds As String()
    ''' <summary>
    ''' 线性规划之中的列名称，即未知变量列表
    ''' </summary>
    ''' <returns></returns>
    Public Property Flux As Dictionary(Of String, DoubleRange)

    Public Property Gaps As String()

    ''' <summary>
    ''' 目标函数之中的计算目标，为Reaction <see cref="Flux"/>编号之中的一部分
    ''' </summary>
    ''' <returns></returns>
    Public Property Targets As String()

    ''' <summary>
    ''' 矩阵的结构为：
    ''' 
    ''' 行应该为Compound, 列应该为代谢过程
    ''' </summary>
    ''' <returns></returns>
    Public Property Matrix As Double()()

    ''' <summary>
    ''' 这个矩阵之中有多少列？即有多少个代谢物
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property NumOfCompounds As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Compounds.Length
        End Get
    End Property

    Public ReadOnly Property FluxNames As String()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Flux.Keys.ToArray
        End Get
    End Property

    ''' <summary>
    ''' 生成目标函数对未知变量，即flux的系数向量
    ''' </summary>
    ''' <returns></returns>
    Public Function GetTargetCoefficients() As Double()
        With Targets.Indexing
            Return Flux.Keys _
                .Select(Function(name)
                            If .IndexOf(name) > -1 Then
                                Return 1.0
                            Else
                                Return 0.0
                            End If
                        End Function) _
                .ToArray
        End With
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetMatrix() As Double(,)
        Return Matrix.ToMatrix
    End Function

    Public Function AdjustRange(ranges As IEnumerable(Of NamedValue(Of DoubleRange))) As Matrix
        For Each newRange As NamedValue(Of DoubleRange) In ranges
            If Flux.ContainsKey(newRange.Name) Then
                Flux(newRange.Name) = newRange.Value
            Else
                Flux.Add(newRange.Name, newRange.Value)
            End If
        Next

        Return Me
    End Function
End Class

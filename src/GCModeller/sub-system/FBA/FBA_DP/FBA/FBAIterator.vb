#Region "Microsoft.VisualBasic::a1051a1224bfe5a3f0e6b41dd99e85f9, sub-system\FBA\FBA_DP\FBA\FBAIterator.vb"

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

    ' Class FBAIterator
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: __buildMAT, __isDIR, __lowerBounds, __overridesBounds, __upperBounds
    '               Run
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.FBA_DP.Models.rFBA

''' <summary>
''' FBA状态迭代器
''' </summary>
Public Class FBAIterator : Implements ITaskDriver

    ''' <summary>
    ''' FBA计算模型
    ''' </summary>
    ReadOnly __lpModel As lpSolveRModel
    ReadOnly __metabolites As SortedDictionary(Of String, Value(Of Double))
    ReadOnly __solver As FBAlpRSolver
    ReadOnly __TEMP As String
    ReadOnly __metaboliteStat As SortedDictionary(Of String, RPKMStat)
    ReadOnly __rxnStat As SortedDictionary(Of String, RPKMStat)

    ''' <summary>
    ''' 迭代的次数
    ''' </summary>
    Dim _iterates As Integer

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="getInit">Get metabolite init amount.</param>
    Sub New(model As lpSolveRModel, getInit As Func(Of String, Double), iterates As Integer,
            Optional R_HOME As String = "",
            Optional TEMP As String = "")

        __lpModel = model
        __metabolites = New SortedDictionary(Of String, Value(Of Double))(
            __lpModel.allCompounds _
                .ToDictionary(Function(x) x,
                              Function(x) New Value(Of Double)(getInit(x))))
        __metaboliteStat = New SortedDictionary(Of String, RPKMStat)(
            __lpModel.allCompounds _
                .ToDictionary(Function(x) x,
                              Function(x) New RPKMStat With {.Locus = x}))
        __rxnStat = New SortedDictionary(Of String, RPKMStat)(
            __lpModel.fluxColumns _
                .ToDictionary(Function(x) x,
                              Function(x) New RPKMStat With {.Locus = x}))
        __stoMAT = __buildMAT(__lpModel)
        __TEMP = TEMP
        _iterates = iterates
        __lpModel.BoundsOverrides = New BoundsOverrides(
            AddressOf __upperBounds,
            AddressOf __lowerBounds)

        If Not R_HOME.DirectoryExists Then
            R_HOME = GCModeller.FileSystem.GetR_HOME
        End If

        __solver = New FBAlpRSolver(R_HOME)
    End Sub

    ''' <summary>
    ''' 每迭代一个就会计算出每一种代谢物的被消耗掉的量。
    ''' （假若FBA的代谢物的约束为等于0的话，则这个计算模型不可用，但是更改了约束条件还可以被称作为FBA么？）
    ''' </summary>
    ''' <returns></returns>
    Public Function Run() As Integer Implements ITaskDriver.Run
        Dim script As String = ""
        Dim n As Integer = _iterates
        Dim ASCII As Encoding = Encodings.ASCII.CodePage

        Do While _iterates > 0
            Dim result As lpOUT = __solver.RSolving(__lpModel, script)
            Dim i As String = CStr(n - _iterates + 1)
            Dim buf = result.CreateDataFile(__lpModel)  ' 得到每一个反应的流量
            Dim mcur As New Dictionary(Of String, Value(Of Double))

            Call script.SaveTo(__TEMP & $"/iterates{i}.R", ASCII)

            For Each rxn As FBA_OUTPUT.TabularOUT In buf
                For Each x In Me.__metabolites
                    Dim val = x.Value
                    Dim c As Double = __lpModel.__getStoichiometry(x.Key, rxn.Rxn)

                    If c <> 0 Then
                        Dim changes As Double = rxn.Flux * c
                        val.value += changes

                        If Not mcur.ContainsKey(x.Key) Then
                            Call mcur.Add(x.Key, New Value(Of Double))
                        End If

                        mcur(x.Key).value += changes

                        If val.value <= 0 Then
                            val.value = 0
                        End If
                    End If
                Next

                Call __rxnStat(rxn.Rxn).Properties.Add(i, rxn.Flux)
            Next

            For Each x In mcur
                Call __metaboliteStat(x.Key).Properties.Add(i, x.Value.value)
            Next

            Call Console.Write(".")

            _iterates -= 1
        Loop

        Return 0
    End Function

#Region "Overrides"

    ReadOnly __stoMAT As Dictionary(Of String, Dictionary(Of String, Double))

    Private Shared Function __buildMAT(model As lpSolveRModel) As Dictionary(Of String, Dictionary(Of String, Double))
        Dim MAT As New Dictionary(Of String, Dictionary(Of String, Double))

        For Each rxn As String In model.fluxColumns
            Dim sto = (From x As String
                       In model.allCompounds
                       Let n As Double = model.__getStoichiometry(x, rxn)
                       Where n <> 0R
                       Select met = x,
                           n).ToDictionary(Function(x) x.met,
                                           Function(x) x.n)
            Call MAT.Add(rxn, sto)
        Next

        Return MAT
    End Function

    ' 在这里根据代谢物的剩余的数量更新每一个rxn的上下限
    ' curr 参数都是假设该flux的最大参数值，由于代谢物的容量有限，所以这个有些可能不会那么大，所以这个时候，约束复写就开始起对代谢组的修正的作用了

    ''' <summary>
    ''' 在这里根据代谢物的剩余的数量更新每一个rxn的上下限
    ''' </summary>
    Private Function __upperBounds(rxn As String, curr As Double) As Double
        Return __overridesBounds(rxn, 1, curr)
    End Function

    ' r * sto = da/dt
    ' r * sto = da = A - A'
    ' A' = A - r * sto

    ''' <summary>
    ''' 根据物质守恒计算出上界
    ''' </summary>
    ''' <param name="rxn"></param>
    ''' <param name="dir">表示约束的方向，只有1和-1这两个值</param>
    ''' <param name="curr"></param>
    ''' <returns></returns>
    Private Function __overridesBounds(rxn As String, dir As Integer, curr As Double) As Double
        Dim dm As Dictionary(Of String, Double) = __stoMAT(rxn)
        Dim left = (From x As KeyValuePair(Of String, Double) In dm
                    Where __isDIR(x.Value, dir)  ' 上限的约束是和左边的代谢物的量有关的
                    Select sId = x.Key,
                        sto = x.Value,
                        n = x.Value * curr).ToArray  ' sto * r
        Dim list As New List(Of Double)

        For Each x In left
            Dim da As Double = x.n   ' 最大速率下的变化值,  da
            Dim A As Double = __metabolites(x.sId).value    ' da = A - A'  => A' = A - da
            Dim A1 As Double = A - da

            If A1 < 0 Then  '  下一个状态已经出现负数了，说明当前的这个代谢物已经到达了上线了，则执行约束
                ' 假设A刚好被消耗完，则A1等于0
                ' A' = A - da = 0 =>  A = da  => r = da / sto = A / sto 
                Call list.Add(A / x.sto)
            End If
        Next

        If list.Count = 0 Then
            Return curr   ' 还很充足，使用最大速率进行
        Else  ' 已经到达了某个代谢物的界限了，则
            Return list.Min * dir  '  以最小的进行约束
        End If
    End Function

    Private Function __isDIR(sto As Double, dir As Integer) As Boolean
        If dir = 1 Then ' 正向，求上限
            Return sto < 0
        Else
            Return sto > 0
        End If
    End Function

    Private Function __lowerBounds(rxn As String, curr As Double) As Double
        Return __overridesBounds(rxn, -1, curr)
    End Function

#End Region

End Class

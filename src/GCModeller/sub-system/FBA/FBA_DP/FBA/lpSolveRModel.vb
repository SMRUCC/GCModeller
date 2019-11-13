#Region "Microsoft.VisualBasic::ef369b7b38c9a61a1465b3b2396b6943, sub-system\FBA\FBA_DP\FBA\lpSolveRModel.vb"

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

    ' Class lpSolveRModel
    ' 
    '     Properties: BoundsOverrides, CDirect, Objectives
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: __add_constraint, __buildLine, __objFunc, __R_script, __set_bounds
    '               __set_objfn, CreateResultFile, GetEquation, getLowerbound, getMatrix
    '               GetName, getObjectFunction, getUpbound
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.ObjectModel
Imports System.Text
Imports Microsoft.VisualBasic.Data.csv
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

''' <summary>
''' Class object using for generate the R script for the R package lpSolveAPI.(用于生成lpSolverAPI所需的脚本的类型对象)
''' </summary>
''' <remarks></remarks>
Public MustInherit Class lpSolveRModel : Inherits IRScript

    Public Const OBJECT_LPREC As String = "lprec"

    ''' <summary>
    ''' Direction of Objective Function.(目标函数的约束方向)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CDirect As String = "max"

    ''' <summary>
    ''' changes = 0;  
    ''' 对于FBA模型而言，每一种代谢物的浓度都是被假设为稳定状态，即在平衡态之中没有浓度变化
    ''' </summary>
    ''' <remarks></remarks>
    Protected Friend Shared ReadOnly _constraint As KeyValuePair(Of String, Double) =
        New KeyValuePair(Of String, Double)("=", 0.0R)

    Sub New()
        Requires = {"lpSolveAPI"}
    End Sub

#Region "Must Override Functions"

    Protected Overridable Function getObjectFunction() As Double()
        Dim LQuery = (From rxn As String In fluxColumns Select __objFunc(rxn)).ToArray
        Return LQuery
    End Function

    Protected Function __objFunc(locus As String) As Double
        If __fluxObjective.IndexOf(locus) > -1 Then
            Return 1
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' 横行为代谢物，列为代谢过程，元素为化学计量数
    ''' </summary>
    ''' <returns></returns>
    Protected Overridable Function getMatrix() As Double()()
        Dim MAT As New List(Of Double())

        For Each metabolite As String In allCompounds
            Dim LQuery = (From flux As String In fluxColumns Select __getStoichiometry(metabolite, flux)).ToArray
            Call MAT.Add(LQuery)  ' 由于矩阵之中是有顺序的，所以在这个函数里面不可以再使用并行化了
        Next

        Return MAT.ToArray
    End Function

    Public Overridable Function GetEquation(rxn As String) As String
        Return ""
    End Function

    Public Overridable Function GetName(rxn As String) As String
        Return rxn
    End Function

    ''' <summary>
    ''' 获得每一个反应活动的流量上限
    ''' </summary>
    ''' <returns></returns>
    Protected Friend MustOverride Function __getUpbound() As Double()
    ''' <summary>
    ''' 获得每一个反应活动的流量下线
    ''' </summary>
    ''' <returns></returns>
    Protected Friend MustOverride Function __getLowerbound() As Double()
    ''' <summary>
    ''' 得到每一个反应过程的标识符
    ''' </summary>
    ''' <returns></returns>
    Protected Friend MustOverride ReadOnly Property fluxColumns() As ReadOnlyCollection(Of String)
    Protected Friend MustOverride ReadOnly Property allCompounds() As ReadOnlyCollection(Of String)

    ''' <summary>
    ''' {direction, constraint}
    ''' </summary>
    ''' <param name="metabolite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Friend MustOverride Function getConstraint(metabolite As String) As KeyValuePair(Of String, Double)

    ''' <summary>
    ''' <see cref="lpSolveRModel.SetObjectiveFunc(String())"/> 转换映射得到的一个反应过程的集合
    ''' </summary>
    Protected Friend __fluxObjective As ReadOnlyCollection(Of String)

    Public ReadOnly Property Objectives As String()
        Get
            Return __fluxObjective.ToArray
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="factors">可以是基因号或者反应的编号，基因号需要在这个函数里面进行映射转换</param>
    Public MustOverride Sub SetObjectiveFunc(factors As String())
#End Region
    Protected Friend MustOverride Function __getStoichiometry(metabolite As String, rxn As String) As Double

    ''' <summary>
    ''' 将LpSolveAPI返回的结果生成数据文件以进行导出
    ''' </summary>
    ''' <param name="resultData">{ObjectiveFunctionValue, FluxDistributions}</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function CreateResultFile(resultData As KeyValuePair(Of String, String())) As IO.File
        Dim tabBuf As IO.File = New IO.File
        Call tabBuf.Add(New String() {"Objective Function Value"})
        Call tabBuf.Last.AddRange(fluxColumns)
        Call tabBuf.Add(New String() {resultData.Key})
        Call tabBuf.Last.AddRange(resultData.Value)

        Return tabBuf
    End Function

#Region "Shared Function For generate R Script"

    ''' <summary>
    ''' Generate objective function for FBA model in lpSolveAPI.  
    ''' </summary>
    ''' <param name="vectorData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Friend Shared Function __set_objfn(vectorData As Double(), direction As String) As String
        Dim vector As String = __buildLine(vectorData)
        Dim scriptLine As StringBuilder = New StringBuilder(1024)
        Call scriptLine.AppendLine(String.Format("set.objfn({0}, c({1}));", OBJECT_LPREC, vector))
        Call scriptLine.AppendLine(String.Format("lp.control(lprec, sense = ""{0}"");", direction))
        Return scriptLine.ToString
    End Function

    ''' <summary>
    ''' Generate a S matrix line.(生成用于表示FBA模型中的S矩阵中的一行的R脚本)
    ''' </summary>
    ''' <param name="vectorData"></param>
    ''' <param name="direction"></param>
    ''' <param name="constraintValue"></param>
    ''' <returns>add.constraint(lprec, c(0.24, 0, 11.31, 0), "=", 14.8)</returns>
    ''' <remarks></remarks>
    Protected Friend Shared Function __add_constraint(vectorData As Double(), direction As String, constraintValue As Double) As String
        Dim vector As String = __buildLine(vectorData)
        Dim scriptLine As String = String.Format("add.constraint({0}, c({1}), ""{2}"", {3});", OBJECT_LPREC, vector, direction, constraintValue)
        Return scriptLine
    End Function

    Protected Friend Shared Function __set_bounds(boundType As String, vectorData As Double()) As String
        Dim vectorBuilder As StringBuilder = New StringBuilder(1024)
        Dim columnBuilder As StringBuilder = New StringBuilder(1024)
        For i As Integer = 0 To vectorData.Count - 1
            Call vectorBuilder.AppendFormat("{0}, ", vectorData(i))
            Call columnBuilder.AppendFormat("{0}, ", i + 1)   ' <- 下标是1到数目上限 
        Next
        Call vectorBuilder.Remove(vectorBuilder.Length - 2, 2)
        Call columnBuilder.Remove(columnBuilder.Length - 2, 2)

        Dim scriptLine As String = String.Format("set.bounds({0}, {1} = c({2}), columns = c({3}));", OBJECT_LPREC, boundType, vectorBuilder.ToString, columnBuilder.ToString)
        Return scriptLine
    End Function

    Private Shared Function __buildLine(vectorData As Double()) As String
        Dim vectorBuilder As StringBuilder = New StringBuilder(1024)
        For i As Integer = 0 To vectorData.Count - 2
            Call vectorBuilder.AppendFormat("{0}, ", vectorData(i))
        Next
        Call vectorBuilder.Append(vectorData.Last)
        Return vectorBuilder.ToString
    End Function
#End Region

#Region "Overrides"
    Public Property BoundsOverrides As BoundsOverrides

    Private Function getUpbound() As Double()
        If BoundsOverrides Is Nothing Then
            Return __getUpbound()
        Else
            Return BoundsOverrides.OverridesUpper(fluxColumns, __getUpbound)
        End If
    End Function

    Private Function getLowerbound() As Double()
        If BoundsOverrides Is Nothing Then
            Return __getLowerbound()
        Else
            Return BoundsOverrides.OverridesUpper(fluxColumns, __getLowerbound)
        End If
    End Function
#End Region

#Region "Implements Of IRScript"

    Protected Overrides Function __R_script() As String
        Dim scriptBuilder As StringBuilder = New StringBuilder(4096)

        Call scriptBuilder.AppendLine(String.Format("{0} <- make.lp(0, {1});", OBJECT_LPREC, fluxColumns.Count))
        Call scriptBuilder.AppendLine(__set_objfn(vectorData:=getObjectFunction, direction:=CDirect))

        Dim S = getMatrix()
        For rowId As Long = 0 To S.Length - 1
            Dim constraint = getConstraint(rowId)
            Dim row As String =
                __add_constraint(vectorData:=S(rowId),
                                 direction:=constraint.Key,
                                 constraintValue:=constraint.Value)
            Call scriptBuilder.AppendLine(row)
        Next
        Call scriptBuilder.AppendLine(__set_bounds("lower", vectorData:=getLowerbound))
        Call scriptBuilder.AppendLine(__set_bounds("upper", vectorData:=getUpbound))
        Call scriptBuilder.AppendLine(String.Format("solve({0})", OBJECT_LPREC))

        Return scriptBuilder.ToString
    End Function
#End Region
End Class

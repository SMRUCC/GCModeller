﻿#Region "Microsoft.VisualBasic::daadb75d3ebfdafd8d130606a00f4a38, Data_science\Mathematica\Math\ODE\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: correlationImpl, Let, Pcc, Solve, SPcc
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module Extensions

    ''' <summary>
    ''' 使用PCC来了解各个变量之间的相关度
    ''' </summary>
    ''' <param name="df"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function Pcc(df As ODEsOut) As IEnumerable(Of DataSet)
        Return df.correlationImpl(AddressOf Correlations.GetPearson)
    End Function

    <Extension>
    Private Iterator Function correlationImpl(df As ODEsOut, correlation As Correlations.ICorrelation) As IEnumerable(Of DataSet)
        Dim vars$() = df.y.Keys.ToArray

        For Each var As NamedCollection(Of Double) In df
            Dim x As New DataSet With {
                .ID = var.Name,
                .Properties = New Dictionary(Of String, Double)
            }

            For Each name$ In vars
                x.Properties(name$) = correlation(var.Value, df.y(name).Value)
            Next

            Yield x
        Next
    End Function

    ''' <summary>
    ''' 使用sPCC来了解各个变量之间的相关度
    ''' </summary>
    ''' <param name="df"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Public Function SPcc(df As ODEsOut) As IEnumerable(Of DataSet)
        Return df.correlationImpl(AddressOf Correlations.Spearman)
    End Function

    ''' <summary>
    ''' Solve the target ODEs dynamics system by using the RK4 solver.
    ''' </summary>
    ''' <param name="system"></param>
    ''' <returns></returns>
    <Extension> Public Function Solve(system As IEnumerable(Of var), dt As (from#, to#, step#)) As ODEsOut
        Dim vector As var() = system.ToArray
        Dim df = Sub(dx#, ByRef dy As Vector)
                     For Each x As var In vector
                         dy(x) = x.Evaluate()
                     Next
                 End Sub
        Dim ODEs As New GenericODEs(system.ToArray, df)

        With dt
            Dim result As ODEsOut = ODEs _
                .Solve((.to - .from) / .step, .from, .to)
            Return result
        End With
    End Function

    ''' <summary>
    ''' Create VisualBasic variables.(使用这个函数来进行初始化是为了在赋值的同时还对新创建的对象赋予名称，方便将结果写入数据集)
    ''' </summary>
    ''' <param name="list"></param>
    ''' <returns></returns>
    <Extension> Public Function Let$(list As Expression(Of Func(Of var())))
        Dim unaryExpression As NewArrayExpression = DirectCast(list.Body, NewArrayExpression)
        Dim arrayData = unaryExpression _
            .Expressions _
            .Select(Function(b) DirectCast(b, BinaryExpression)) _
            .ToArray
        Dim var As New Dictionary(Of String, Double)

        For Each expr As BinaryExpression In arrayData
            Dim member = DirectCast(expr.Left, MemberExpression)
            Dim name As String = member.Member.Name.Replace("$VB$Local_", "")
            Dim field As FieldInfo = DirectCast(member.Member, FieldInfo)
            Dim value As Object = DirectCast(expr.Right, ConstantExpression).Value
            Dim obj = DirectCast(member.Expression, ConstantExpression).Value

            Call field.SetValue(obj, New var(name, CDbl(value)))
        Next

        Return var.GetJson
    End Function
End Module

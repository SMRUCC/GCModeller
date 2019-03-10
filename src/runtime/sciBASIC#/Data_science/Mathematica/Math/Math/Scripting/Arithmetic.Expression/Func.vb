﻿#Region "Microsoft.VisualBasic::becb200e524a76e48782bb058232f3e8, Data_science\Mathematica\Math\Math\Scripting\Arithmetic.Expression\Func.vb"

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

    '     Class Func
    ' 
    '         Properties: Args, Expression, Name
    ' 
    '         Function: GetExpression, ToString
    '         Class __callerHelper
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: Evaluate, getValue
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.Types

Namespace Scripting

    ''' <summary>
    ''' User define function.(用户自定义函数)
    ''' </summary>
    Public Class Func

        ''' <summary>
        ''' 函数名
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        ''' <summary>
        ''' 参数列表
        ''' </summary>
        ''' <returns></returns>
        Public Property Args As String()
        ''' <summary>
        ''' 函数表达式
        ''' </summary>
        ''' <returns></returns>
        Public Property Expression As String

        ''' <summary>
        ''' 从数据模型之中创建对象模型
        ''' </summary>
        ''' <param name="engine"></param>
        ''' <returns></returns>
        Public Function GetExpression(engine As Expression) As Func(Of Double(), Double)
            Dim helper As New __callerHelper(Args.Select(Function(x) x.ToLower).ToArray) With {
                .__engine = engine
            }
            Dim expr As SimpleExpression =
                ExpressionParser.TryParse(Expression,
                                          AddressOf helper.getValue,
                                          AddressOf engine.Functions.Evaluate)
            helper.__expr = expr
            Return AddressOf helper.Evaluate
        End Function

        Private Class __callerHelper

            Public __engine As Expression

            ''' <summary>
            ''' 默认全部都是变量
            ''' </summary>
            ReadOnly __args As Dictionary(Of String, Value(Of Double))
            ReadOnly __names As String()

            Sub New(args As String())
                __args = args.ToDictionary(Function(x) x, Function(null) New Value(Of Double))
                __names = args
            End Sub

            Public Function getValue(name As String) As Double
                name = name.ToLower

                If __args.ContainsKey(name.ToLower) Then
                    Return __args(name).value
                Else
                    Return __engine.GetValue(name)
                End If
            End Function

            Public __expr As SimpleExpression

            Public Function Evaluate(args As Double()) As Double
                For Each x As SeqValue(Of Double) In args.SeqIterator  ' 对lambda表达式设置环境变量
                    __args(__names(x.i)).value = x.value
                Next

                Return __expr.Evaluate
            End Function
        End Class

        Public Overrides Function ToString() As String
            Dim args As String = Me.Args.JoinBy(", ")
            Return $"{Name}({args}) {Expression}"
        End Function
    End Class
End Namespace

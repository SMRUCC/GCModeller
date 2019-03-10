﻿#Region "Microsoft.VisualBasic::dc1422d810f3c786177f5343c73cad8a, Data_science\Mathematica\Math\Math\Scripting\Arithmetic.Expression\FuncCaller.vb"

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

    '     Class FuncCaller
    ' 
    '         Properties: Name, Params
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Evaluate, ToString
    ' 
    '     Delegate Function
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.Scripting.Types
Imports Microsoft.VisualBasic.Linq

Namespace Scripting

    ''' <summary>
    ''' Function object model.(调用函数的方法)
    ''' </summary>
    Public Class FuncCaller

        Public ReadOnly Property Name As String
        Public ReadOnly Property Params As New List(Of SimpleExpression)

        ReadOnly __calls As IFuncEvaluate

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Name">The function name</param>
        ''' <param name="evaluate">Engine handle</param>
        Sub New(Name As String, evaluate As IFuncEvaluate)
            Me.Name = Name
            Me.__calls = evaluate
        End Sub

        Public Overrides Function ToString() As String
            Dim args As String() = Params.Select(Function(x) x.ToString).ToArray
            Return $"{Name}({args.JoinBy(", ")})"
        End Function

        Public Function Evaluate() As Double
            Return __calls(Name, Params.Select(Function(x) x.Evaluate).ToArray)
        End Function
    End Class

    Public Delegate Function IFuncEvaluate(name As String, args As Double()) As Double
End Namespace

#Region "Microsoft.VisualBasic::f233fe544d3ca62b5d108602756e68f5, RDotNET\RDotNET\Expression.vb"

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

    ' Class Expression
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Evaluate, TryEvaluate
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals

''' <summary>
''' An expression object.
''' </summary>
Public Class Expression
	Inherits SymbolicExpression
	''' <summary>
	''' Creates an expression object.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="pointer">The pointer to an expression.</param>
	Protected Friend Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Evaluates the expression in the specified environment.
	''' </summary>
	''' <param name="environment">The environment.</param>
	''' <returns>The evaluation result.</returns>
	Public Function Evaluate(environment As REnvironment) As SymbolicExpression
		If environment Is Nothing Then
			Throw New ArgumentNullException("environment")
		End If
		If Engine IsNot environment.Engine Then
			Throw New ArgumentException(Nothing, "environment")
		End If

		Return New SymbolicExpression(Engine, Me.GetFunction(Of Rf_eval)()(handle, environment.DangerousGetHandle()))
	End Function

	''' <summary>
	''' Evaluates the expression in the specified environment.
	''' </summary>
	''' <param name="environment">The environment.</param>
	''' <param name="result">The evaluation result, or <c>null</c> if the evaluation failed</param>
	''' <returns><c>True</c> if the evaluation succeeded.</returns>
	Public Function TryEvaluate(environment As REnvironment, ByRef result As SymbolicExpression) As Boolean
		If environment Is Nothing Then
			Throw New ArgumentNullException("environment")
		End If
		If Engine IsNot environment.Engine Then
			Throw New ArgumentException(Nothing, "environment")
		End If

		Dim errorOccurred As Boolean
		Dim pointer As IntPtr = Me.GetFunction(Of R_tryEval)()(handle, environment.DangerousGetHandle(), errorOccurred)
		result = If(errorOccurred, Nothing, New SymbolicExpression(Engine, pointer))
		Return Not errorOccurred
	End Function
End Class


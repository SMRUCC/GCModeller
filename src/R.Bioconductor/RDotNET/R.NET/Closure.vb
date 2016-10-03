#Region "Microsoft.VisualBasic::0326c40b7f30ee62d48c87aa6ff08b68, ..\R.Bioconductor\RDotNET\R.NET\Closure.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Collections.Generic
Imports RDotNet.Internals
Imports System.Linq

''' <summary>
''' A closure.
''' </summary>
Public Class Closure
	Inherits [Function]
	''' <summary>
	''' Creates a closure object.
	''' </summary>
	''' <param name="engine">The engine.</param>
	''' <param name="pointer">The pointer.</param>
	Protected Friend Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Gets the arguments list.
	''' </summary>
	Public ReadOnly Property Arguments() As Pairlist
		Get
			Dim sexp As SEXPREC = GetInternalStructure()
			Return New Pairlist(Engine, sexp.closxp.formals)
		End Get
	End Property

	''' <summary>
	''' Gets the body.
	''' </summary>
	Public ReadOnly Property Body() As Language
		Get
			Dim sexp As SEXPREC = GetInternalStructure()
			Return New Language(Engine, sexp.closxp.body)
		End Get
	End Property

	''' <summary>
	''' Gets the environment.
	''' </summary>
	Public ReadOnly Property Environment() As REnvironment
		Get
			Dim sexp As SEXPREC = GetInternalStructure()
			Return New REnvironment(Engine, sexp.closxp.env)
		End Get
	End Property

	''' <summary>
	''' Invoke this function, using an ordered list of unnamed arguments.
	''' </summary>
	''' <param name="args">The arguments of the function</param>
	''' <returns>The result of the evaluation</returns>
	Public Overrides Function Invoke(ParamArray args As SymbolicExpression()) As SymbolicExpression
		'int count = Arguments.Count;
		'if (args.Length > count)
		'   throw new ArgumentException("Too many arguments provided for this function", "args");
		Return InvokeOrderedArguments(args)
	End Function

	''' <summary>
	''' Invoke this function, using named arguments provided as key-value pairs
	''' </summary>
	''' <param name="args">the representation of named arguments, as a dictionary</param>
	''' <returns>The result of the evaluation</returns>
	Public Overrides Function Invoke(args As IDictionary(Of String, SymbolicExpression)) As SymbolicExpression
		Dim a = args.ToArray()
		Return InvokeViaPairlist(Array.ConvertAll(a, Function(x) x.Key), Array.ConvertAll(a, Function(x) x.Value))
	End Function

	Private Function GetArgumentNames() As String()
		Return Arguments.[Select](Function(arg) arg.PrintName).ToArray()
	End Function
End Class

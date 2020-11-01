#Region "Microsoft.VisualBasic::0c9c4e4eba016cc15c6be92b2f7e854c, RDotNET\RDotNET\BuiltinFunction.vb"

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

    ' Class BuiltinFunction
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+2 Overloads) Invoke
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

''' <summary>
''' A built-in function.
''' </summary>
Public Class BuiltinFunction
	Inherits [Function]
	''' <summary>
	''' Creates a built-in function proxy.
	''' </summary>
	''' <param name="engine">The engine.</param>
	''' <param name="pointer">The pointer.</param>
	Protected Friend Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Invoke this builtin function, using an ordered list of unnamed arguments.
	''' </summary>
	''' <param name="args">The arguments of the function</param>
	''' <returns>The result of the evaluation</returns>
	Public Overrides Function Invoke(ParamArray args As SymbolicExpression()) As SymbolicExpression
		Return InvokeOrderedArguments(args)
	End Function

	''' <summary>
	''' NotSupportedException
	''' </summary>
	''' <param name="args">key-value pairs</param>
	''' <returns>Always throws an exception</returns>
	Public Overrides Function Invoke(args As IDictionary(Of String, SymbolicExpression)) As SymbolicExpression
		Throw New NotSupportedException()
	End Function
End Class


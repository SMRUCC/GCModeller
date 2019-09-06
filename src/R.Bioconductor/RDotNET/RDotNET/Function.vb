#Region "Microsoft.VisualBasic::776a48cdf9f6e55e53fa027f703e1048, RDotNET\RDotNET\Function.vb"

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

    ' Class [Function]
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: createCallAndEvaluate, evaluateCall, InvokeNamed, InvokeNamedFast, InvokeOrderedArguments
    '               InvokeStrArgs, InvokeViaPairlist
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.ExceptionServices
Imports System.Security

''' <summary>
''' A function is one of closure, built-in function, or special function.
''' </summary>
Public MustInherit Class [Function]
	Inherits SymbolicExpression
	''' <summary>
	''' Creates a function object.
	''' </summary>
	''' <param name="engine">The engine.</param>
	''' <param name="pointer">The pointer.</param>
	Protected Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Executes the function. Match the function arguments by order.
	''' </summary>
	''' <param name="args">The arguments.</param>
	''' <returns>The result of the function evaluation</returns>
	Public MustOverride Function Invoke(ParamArray args As SymbolicExpression()) As SymbolicExpression

	''' <summary>
	''' A convenience method to executes the function. Match the function arguments by order, after evaluating each to an R expression.
	''' </summary>
	''' <param name="args">string representation of the arguments; each is evaluated to symbolic expression before being passed as argument to this object (i.e. this Function)</param>
	''' <returns>The result of the function evaluation</returns>
	''' <example>
	''' <code>
	''' </code>
	''' </example>
	Public Function InvokeStrArgs(ParamArray args As String()) As SymbolicExpression
		Return Invoke(Array.ConvertAll(args, Function(x) Engine.Evaluate(x)))
	End Function

	''' <summary>
	''' Executes the function. Match the function arguments by name.
	''' </summary>
	''' <param name="args">The arguments, indexed by argument name</param>
	''' <returns>The result of the function evaluation</returns>
	Public MustOverride Function Invoke(args As IDictionary(Of String, SymbolicExpression)) As SymbolicExpression

	''' <summary>
	''' Executes the function. Match the function arguments by name.
	''' </summary>
	''' <param name="args">one or more tuples, conceptually a pairlist of arguments. The argument names must be unique</param>
	''' <returns>The result of the function evaluation</returns>
	Public Function InvokeNamed(ParamArray args As Tuple(Of String, SymbolicExpression)()) As SymbolicExpression
		Return InvokeNamedFast(args)
		' 2015-01-04 used to call InvokeViaPairlist
		' If no unforeseen changes (all unit tests pass), just remove this comment
		' return InvokeViaPairlist(Array.ConvertAll(args, x => x.Item1), Array.ConvertAll(args, x => x.Item2));
	End Function

	''' <summary>
	''' Executes the function. Match the function arguments by name.
	''' </summary>
	''' <param name="argNames">The names of the arguments. These can be empty strings for unnamed function arguments</param>
	''' <param name="args">The arguments passed to the function</param>
	''' <returns></returns>
	Protected Function InvokeViaPairlist(argNames As String(), args As SymbolicExpression()) As SymbolicExpression
		Dim names = New CharacterVector(Engine, argNames)
		Dim arguments = New GenericVector(Engine, args)
		arguments.SetNames(names)
		Dim argPairList = arguments.ToPairlist()

		'IntPtr newEnvironment = Engine.GetFunction<Rf_allocSExp>()(SymbolicExpressionType.Environment);
		'IntPtr result = Engine.GetFunction<Rf_applyClosure>()(Body.DangerousGetHandle(), handle,
		'                                                      argPairList.DangerousGetHandle(),
		'                                                      Environment.DangerousGetHandle(), newEnvironment);
		Return createCallAndEvaluate(argPairList.DangerousGetHandle())
	End Function

	' http://msdn.microsoft.com/en-us/magazine/dd419661.aspx
	<HandleProcessCorruptedStateExceptions> _
	<SecurityCritical> _
	Private Function evaluateCall([call] As IntPtr) As ProtectedPointer
		Dim result As ProtectedPointer
		Dim errorOccurred As Boolean = False
		Try
			result = New ProtectedPointer(Engine, Engine.GetFunction(Of R_tryEval)()([call], Engine.GlobalEnvironment.DangerousGetHandle(), errorOccurred))
		Catch ex As Exception
			' TODO: this is usually dubious to catch all that, but given the inner exception is preserved
			Throw New EvaluationException(Engine.LastErrorMessage, ex)
		End Try
		If errorOccurred Then
			Throw New EvaluationException(Engine.LastErrorMessage)
		End If
		Return result
	End Function

	''' <summary>
	''' Invoke this function with unnamed arguments.
	''' </summary>
	''' <param name="args">The arguments passed to function call.</param>
	''' <returns>The result of the function evaluation.</returns>
	Protected Function InvokeOrderedArguments(args As SymbolicExpression()) As SymbolicExpression
		Dim argument As IntPtr = Engine.NilValue.DangerousGetHandle()
		For Each arg As SymbolicExpression In args.Reverse()
			argument = Me.GetFunction(Of Rf_cons)()(arg.DangerousGetHandle(), argument)
		Next
		Return createCallAndEvaluate(argument)
	End Function

	Private Function createCallAndEvaluate(argument As IntPtr) As SymbolicExpression
		Using [call] = New ProtectedPointer(Engine, Me.GetFunction(Of Rf_lcons)()(handle, argument))
			Using result = evaluateCall([call])
				Return New SymbolicExpression(Engine, result)
			End Using
		End Using
	End Function

	''' <summary>
	''' Invoke the function with optionally named arguments by order.
	''' </summary>
	''' <param name="args">one or more tuples, conceptually a pairlist of arguments.
	''' The argument names must be unique; null or empty string indicates unnamed argument. </param>
	''' <returns>The result of the function evaluation</returns>
	Private Function InvokeNamedFast(ParamArray args As Tuple(Of String, SymbolicExpression)()) As SymbolicExpression
		Dim argument As IntPtr = Engine.NilValue.DangerousGetHandle()
		Dim rfInstall = GetFunction(Of Rf_install)()
		Dim rSetTag = GetFunction(Of SET_TAG)()
        Dim rfCons = GetFunction(Of Rf_cons)()

        For Each arg In args.Reverse()
            Dim sexp = arg.Item2
            argument = rfCons(sexp.DangerousGetHandle(), argument)
            Dim name As String = arg.Item1
            If Not String.IsNullOrEmpty(name) Then
                rSetTag(argument, rfInstall(name))
            End If
        Next
        Return createCallAndEvaluate(argument)
	End Function
End Class


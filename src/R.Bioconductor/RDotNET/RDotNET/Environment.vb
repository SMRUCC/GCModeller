#Region "Microsoft.VisualBasic::16e246e7c69289f2d3fe412c825e6f11, RDotNET\RDotNET\Environment.vb"

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

    ' Class REnvironment
    ' 
    '     Properties: Parent
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: GetSymbol, GetSymbolNames
    ' 
    '     Sub: SetSymbol
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports RDotNet.Utilities
Imports System.Runtime.InteropServices

''' <summary>
''' An environment object.
''' </summary>
Public Class REnvironment
	Inherits SymbolicExpression
	''' <summary>
	''' Creates an environment object.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="pointer">The pointer to an environment.</param>
	Protected Friend Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Creates a new environment object.
	''' </summary>
	''' <param name="engine">The engine.</param>
	''' <param name="parent">The parent environment.</param>
	Public Sub New(engine As REngine, parent As REnvironment)
		MyBase.New(engine, engine.GetFunction(Of Rf_NewEnvironment)()(engine.NilValue.DangerousGetHandle(), engine.NilValue.DangerousGetHandle(), parent.handle))
	End Sub

	''' <summary>
	''' Gets the parental environment.
	''' </summary>
	Public ReadOnly Property Parent() As REnvironment
		Get
			Dim sexp As SEXPREC = GetInternalStructure()
            Dim parentPtr As IntPtr = sexp.envsxp.enclos
            Return If(Engine.EqualsRNilValue(parentPtr), Nothing, New REnvironment(Engine, parentPtr))
        End Get
	End Property

	''' <summary>
	''' Gets a symbol defined in this environment.
	''' </summary>
	''' <param name="name">The name.</param>
	''' <returns>The symbol.</returns>
	Public Function GetSymbol(name As String) As SymbolicExpression
		If name Is Nothing Then
			Throw New ArgumentNullException()
		End If
		If name = String.Empty Then
			Throw New ArgumentException()
		End If

		Dim installedName As IntPtr = Me.GetFunction(Of Rf_install)()(name)
		Dim value As IntPtr = Me.GetFunction(Of Rf_findVar)()(installedName, handle)
		If Engine.CheckUnbound(value) Then
			Throw New EvaluationException(String.Format("Error: object '{0}' not found", name))
		End If

		Dim sexp = CType(Marshal.PtrToStructure(value, GetType(SEXPREC)), SEXPREC)
		If sexp.sxpinfo.type = SymbolicExpressionType.Promise Then
			value = Me.GetFunction(Of Rf_eval)()(value, handle)
		End If
		Return New SymbolicExpression(Engine, value)
	End Function

	''' <summary>
	''' Defines a symbol in this environment.
	''' </summary>
	''' <param name="name">The name.</param>
	''' <param name="expression">The symbol.</param>
	Public Sub SetSymbol(name As String, expression As SymbolicExpression)
		If name Is Nothing Then
			Throw New ArgumentNullException("name", "'name' cannot be null")
		End If
		If name = String.Empty Then
			Throw New ArgumentException("'name' cannot be an empty string")
		End If
		If expression Is Nothing Then
			expression = Engine.NilValue
		End If
		If expression.Engine IsNot Me.Engine Then
			Throw New ArgumentException()
		End If
		Dim installedName As IntPtr = Me.GetFunction(Of Rf_install)()(name)
		Me.GetFunction(Of Rf_defineVar)()(installedName, expression.DangerousGetHandle(), handle)
	End Sub

	''' <summary>
	''' Gets the symbol names defined in this environment.
	''' </summary>
	''' <param name="all">Including special functions or not.</param>
	''' <returns>Symbol names.</returns>
	Public Function GetSymbolNames(Optional all As Boolean = False) As String()
		Dim symbolNames = New CharacterVector(Engine, Me.GetFunction(Of R_lsInternal)()(handle, all))
		Dim length As Integer = symbolNames.Length
		Dim copy = New String(length - 1) {}
		symbolNames.CopyTo(copy, length)
		Return copy
	End Function
End Class


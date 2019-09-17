#Region "Microsoft.VisualBasic::8d14bde0c9443a0c1d04ffd7654e369f, RDotNET\RDotNET\Symbol.vb"

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

    ' Class Symbol
    ' 
    '     Properties: Internal, PrintName, Value
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetOffsetOf
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports RDotNet.Utilities
Imports System.Runtime.InteropServices

''' <summary>
''' A symbol object.
''' </summary>
Public Class Symbol
	Inherits SymbolicExpression
	''' <summary>
	''' Creates a symbol.
	''' </summary>
	''' <param name="engine">The engine.</param>
	''' <param name="pointer">The pointer.</param>
	Protected Friend Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Gets and sets the name.
	''' </summary>
	Public Property PrintName() As String
		Get
			Dim sexp As SEXPREC = GetInternalStructure()
			Return New InternalString(Engine, sexp.symsxp.pname).ToString()
		End Get
		Set
			Dim pointer As IntPtr = (If(value Is Nothing, Engine.NilValue, New InternalString(Engine, value))).DangerousGetHandle()
			Dim offset As Integer = GetOffsetOf("pname")
			Marshal.WriteIntPtr(handle, offset, pointer)
		End Set
	End Property

	''' <summary>
	''' Gets the internal function.
	''' </summary>
	Public ReadOnly Property Internal() As SymbolicExpression
		Get
			Dim sexp As SEXPREC = GetInternalStructure()
			If Engine.EqualsRNilValue(sexp.symsxp.value) Then
				Return Nothing
			End If
			Return New SymbolicExpression(Engine, sexp.symsxp.internal)
		End Get
	End Property

	''' <summary>
	''' Gets the symbol value.
	''' </summary>
	Public ReadOnly Property Value() As SymbolicExpression
		Get
			Dim sexp As SEXPREC = GetInternalStructure()
			If Engine.EqualsRNilValue(sexp.symsxp.value) Then
				Return Nothing
			End If
			Return New SymbolicExpression(Engine, sexp.symsxp.value)
		End Get
	End Property

	Private Shared Function GetOffsetOf(fieldName As String) As Integer
		Return Marshal.OffsetOf(GetType(SEXPREC), "u").ToInt32() + Marshal.OffsetOf(GetType(symsxp), fieldName).ToInt32()
	End Function
End Class


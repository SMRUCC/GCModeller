#Region "Microsoft.VisualBasic::9c85a331f3676ad17016705b224a0d14, ..\R.Bioconductor\RDotNET\R.NET\Symbol.vb"

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

Imports RDotNet.Internals
Imports System.Runtime.InteropServices

Imports RDotNET.REngineExtension
Imports RDotNET.SymbolicExpressionExtension

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
            If CheckNil(Engine, sexp.symsxp.value) Then
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
            If CheckNil(Engine, sexp.symsxp.value) Then
                Return Nothing
            End If
			Return New SymbolicExpression(Engine, sexp.symsxp.value)
		End Get
	End Property

	Private Shared Function GetOffsetOf(fieldName As String) As Integer
		Return Marshal.OffsetOf(GetType(SEXPREC), "u").ToInt32() + Marshal.OffsetOf(GetType(symsxp), fieldName).ToInt32()
	End Function
End Class

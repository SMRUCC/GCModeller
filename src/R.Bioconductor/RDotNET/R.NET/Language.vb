#Region "Microsoft.VisualBasic::b1c40925b68d1eb218ea425f10a8ab2a, ..\R.Bioconductor\RDotNET\R.NET\Language.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

''' <summary>
''' A language object.
''' </summary>
Public Class Language
	Inherits SymbolicExpression
	''' <summary>
	''' Creates a language object.
	''' </summary>
	''' <param name="engine">The engine</param>
	''' <param name="pointer">The pointer.</param>
	Protected Friend Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Gets function calls.
	''' </summary>
	Public ReadOnly Property FunctionCall() As Pairlist
		Get
			Dim count As Integer = Me.GetFunction(Of Rf_length)()(handle)
			' count == 1 for empty call.
			If count < 2 Then
				Return Nothing
			End If
			Dim sexp As SEXPREC = GetInternalStructure()
			Return New Pairlist(Engine, sexp.listsxp.cdrval)
		End Get
	End Property
End Class

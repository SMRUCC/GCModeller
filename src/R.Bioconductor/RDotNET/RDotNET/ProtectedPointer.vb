#Region "Microsoft.VisualBasic::46027a798ba39b89b30ef85e516c127c, RDotNET\RDotNET\ProtectedPointer.vb"

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

    ' Class ProtectedPointer
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: GetFunction
    ' 
    '     Sub: Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports System.Security.Permissions

<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Friend Class ProtectedPointer
	Implements IDisposable
	Private ReadOnly engine As REngine

	Protected Function GetFunction(Of TDelegate As Class)() As TDelegate
		Return engine.GetFunction(Of TDelegate)()
	End Function

	Private ReadOnly sexp As IntPtr

	Public Sub New(engine As REngine, sexp As IntPtr)
		Me.sexp = sexp
		Me.engine = engine

		Me.GetFunction(Of Rf_protect)()(Me.sexp)
	End Sub

	Public Sub New(sexp As SymbolicExpression)
		Me.sexp = sexp.DangerousGetHandle()
		Me.engine = sexp.Engine

		Me.GetFunction(Of Rf_protect)()(Me.sexp)
	End Sub

	#Region "IDisposable Members"

	Public Sub Dispose() Implements IDisposable.Dispose
		Me.GetFunction(Of Rf_unprotect_ptr)()(Me.sexp)
	End Sub

	#End Region

	Public Shared Widening Operator CType(p As ProtectedPointer) As IntPtr
		Return p.sexp
	End Operator
End Class


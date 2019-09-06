#Region "Microsoft.VisualBasic::77acea5d074df2c07c032ba8b7f86841, RDotNET\RDotNET\ExpressionVector.vb"

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

    ' Class ExpressionVector
    ' 
    '     Properties: DataSize, Item
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetArrayFast, GetValue
    ' 
    '     Sub: SetValue, SetVectorDirect
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.InteropServices
Imports System.Security.Permissions

''' <summary>
''' A vector of S expressions
''' </summary>
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class ExpressionVector
	Inherits Vector(Of Expression)
	''' <summary>
	''' Creates a new instance for an expression vector.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to an expression vector.</param>
	Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets/sets the expression for an index
	''' </summary>
	''' <param name="index">index value</param>
	''' <returns>The Expression at a given index.</returns>
	Public Overrides Default Property Item(index As Integer) As Expression
		Get
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Return GetValue(index)
			End Using
		End Get
		Set
			If index < 0 OrElse Length <= index Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				SetValue(index, value)
			End Using
		End Set
	End Property

	''' <summary>
	''' Gets an array representation of a vector of SEXP in R. Note that the implementation cannot be particularly "fast" in spite of the name.
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function GetArrayFast() As Expression()
		Dim res = New Expression(Me.Length - 1) {}
		For i As Integer = 0 To res.Length - 1
			res(i) = GetValue(i)
		Next
		Return res
	End Function

	Private Function GetValue(index As Integer) As Expression
		Dim offset As Integer = GetOffset(index)
		Dim pointer As IntPtr = Marshal.ReadIntPtr(DataPointer, offset)
		Return New Expression(Engine, pointer)
	End Function

	Private Sub SetValue(index As Integer, value As Expression)
		Dim offset As Integer = GetOffset(index)
		Marshal.WriteIntPtr(DataPointer, offset, (If(value, Engine.NilValue)).DangerousGetHandle())
	End Sub

	''' <summary>
	''' Efficient initialisation of R vector values from an array representation in the CLR
	''' </summary>
	Protected Overrides Sub SetVectorDirect(values As Expression())
		For i As Integer = 0 To values.Length - 1
			SetValue(i, values(i))
		Next
	End Sub

	''' <summary>
	''' Gets the size of a pointer in byte.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return Marshal.SizeOf(GetType(IntPtr))
		End Get
	End Property
End Class


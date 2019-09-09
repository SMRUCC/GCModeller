#Region "Microsoft.VisualBasic::607a4a7e6d4ee2cd09ce9df6b43b6e65, RDotNET\RDotNET\InternalString.vb"

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

    ' Class InternalString
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: GetInternalValue, NativeUtf8FromString, StringFromNativeUtf8, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Internals
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports System.Text

''' <summary>
''' Internal string.
''' </summary>
<DebuggerDisplay("Content = {ToString()}; RObjectType = {Type}")> _
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class InternalString
	Inherits SymbolicExpression
	''' <summary>
	''' Convert string to utf8
	''' </summary>
	''' <param name="stringToConvert">string to convert</param>

	Public Shared Function NativeUtf8FromString(stringToConvert As String) As IntPtr
		Dim len As Integer = Encoding.UTF8.GetByteCount(stringToConvert)
		Dim buffer As Byte() = New Byte(len) {}
		Encoding.UTF8.GetBytes(stringToConvert, 0, stringToConvert.Length, buffer, 0)
		Dim nativeUtf8 As IntPtr = Marshal.AllocHGlobal(buffer.Length)
		Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length)
		Return nativeUtf8
	End Function

	''' <summary>
	''' Convert utf8 to string
	''' </summary>
	''' <param name="utf8">utf8 to convert</param>

	Public Shared Function StringFromNativeUtf8(utf8 As IntPtr) As String
		Dim len As Integer = 0
		While Marshal.ReadByte(utf8, len) <> 0
			len += 1
		End While
		Dim buffer As Byte() = New Byte(len - 1) {}
		Marshal.Copy(utf8, buffer, 0, buffer.Length)
		Return Encoding.UTF8.GetString(buffer)
	End Function

	''' <summary>
	''' Creates a new instance.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="pointer">The pointer to a string.</param>
	Public Sub New(engine As REngine, pointer As IntPtr)
		MyBase.New(engine, pointer)
	End Sub

	''' <summary>
	''' Creates a new instance.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="s">The string</param>
	Public Sub New(engine As REngine, s As String)
		MyBase.New(engine, engine.GetFunction(Of Rf_mkChar)()(s))
	End Sub

	''' <summary>
	''' Converts to the string into .NET Framework string.
	''' </summary>
	''' <param name="s">The R string.</param>
	''' <returns>The .NET Framework string.</returns>
	Public Shared Widening Operator CType(s As InternalString) As String
		Return s.ToString()
	End Operator

	''' <summary>
	''' Gets the string representation of the string object.
	''' This returns <c>"NA"</c> if the value is <c>NA</c>, whereas <see cref="GetInternalValue()"/> returns <c>null</c>.
	''' </summary>
	''' <returns>The string representation.</returns>
	''' <seealso cref="GetInternalValue()"/>
	Public Overrides Function ToString() As String
		Dim pointer As IntPtr = IntPtr.Add(handle, Marshal.SizeOf(GetType(VECTOR_SEXPREC)))
		Return StringFromNativeUtf8(pointer)
	End Function

	''' <summary>
	''' Gets the string representation of the string object.
	''' This returns <c>null</c> if the value is <c>NA</c>, whereas <see cref="ToString()"/> returns <c>"NA"</c>.
	''' </summary>
	''' <returns>The string representation.</returns>
	Public Function GetInternalValue() As String
		If handle = Engine.NaStringPointer Then
			Return Nothing
		End If
		Dim pointer As IntPtr = IntPtr.Add(handle, Marshal.SizeOf(GetType(VECTOR_SEXPREC)))
		Return StringFromNativeUtf8(pointer)
	End Function
End Class


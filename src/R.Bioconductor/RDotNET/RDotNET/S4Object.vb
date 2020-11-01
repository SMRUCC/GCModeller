#Region "Microsoft.VisualBasic::8b354d8064b09292cc2366fef6c37e7d, RDotNET\RDotNET\S4Object.vb"

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

    ' Class S4Object
    ' 
    '     Properties: Item, SlotCount, SlotNames
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetClassDefinition, GetSlotTypes, HasSlot
    ' 
    '     Sub: checkSlotName
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Diagnostics
Imports RDotNet.Internals
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Security.Permissions

''' <summary>
''' An S4 object
''' </summary>
<DebuggerDisplay("SlotCount = {SlotCount}; RObjectType = {Type}")> _
<DebuggerTypeProxy(GetType(S4ObjectDebugView))> _
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class S4Object
	Inherits SymbolicExpression
	''' <summary>
	''' Function .slotNames
	''' </summary>
	''' <remarks>
	''' slotNames, when used on the class representation object, returns the slot names of
	''' instances of the class, rather than the slot names of the class object itself. '.slotNames' is what we want.
	''' </remarks>
	Private Shared dotSlotNamesFunc As [Function] = Nothing

    ''' <summary>
    ''' Create a new S4 object
    ''' </summary>
    ''' <param name="engine">R engine</param>
    ''' <param name="pointer">pointer to native S4 SEXP</param>
    Protected Friend Sub New(engine As REngine, pointer As IntPtr)
        MyBase.New(engine, pointer)
        If dotSlotNamesFunc Is Nothing Then
			dotSlotNamesFunc = Engine.Evaluate("invisible(.slotNames)").AsFunction()
		End If
	End Sub

	''' <summary>
	''' Gets/sets the value of a slot
	''' </summary>
	''' <param name="name"></param>
	''' <returns></returns>
	Public Default Property Item(name As String) As SymbolicExpression
		Get
			checkSlotName(name)
			Dim slotValue As IntPtr
			Using s = New ProtectedPointer(Engine, GetFunction(Of Rf_mkString)()(InternalString.NativeUtf8FromString(name)))
				slotValue = Me.GetFunction(Of R_do_slot)()(Me.DangerousGetHandle(), s)
			End Using
			Return New SymbolicExpression(Engine, slotValue)
		End Get
		Set
			checkSlotName(name)
			Using s = New ProtectedPointer(Engine, GetFunction(Of Rf_mkString)()(InternalString.NativeUtf8FromString(name)))
				Using New ProtectedPointer(Me)
					Me.GetFunction(Of R_do_slot_assign)()(Me.DangerousGetHandle(), s, value.DangerousGetHandle())
				End Using
			End Using
		End Set
	End Property

	Private Sub checkSlotName(name As String)
		If Not SlotNames.Contains(name) Then
			Throw New ArgumentException(String.Format("Invalid slot name '{0}'", name), "name")
		End If
	End Sub

	''' <summary>
	''' Is a slot name valid.
	''' </summary>
	''' <param name="slotName">the name of the slot</param>
	''' <returns>whether a slot name is present in the object</returns>
	Public Function HasSlot(slotName As String) As Boolean
		Using s = New ProtectedPointer(Engine, GetFunction(Of Rf_mkString)()(InternalString.NativeUtf8FromString(slotName)))
			Return Me.GetFunction(Of R_has_slot)()(Me.DangerousGetHandle(), s)
		End Using
	End Function

	Private m_slotNames As String() = Nothing

	''' <summary>
	''' Gets the slot names for this object. The values are cached once retrieved the first time.
	''' Note this is equivalent to the function '.slotNames' in R, not 'slotNames'
	''' </summary>
	Public ReadOnly Property SlotNames() As String()
		Get
			If m_slotNames Is Nothing Then
				m_slotNames = dotSlotNamesFunc.Invoke(Me).AsCharacter().ToArray()
			End If
			Return DirectCast(m_slotNames.Clone(), String())
		End Get
	End Property

	''' <summary>
	''' Gets the number of slot names
	''' </summary>
	Public ReadOnly Property SlotCount() As Integer
		Get
			Return SlotNames.Length
		End Get
	End Property

	''' <summary>
	''' Gets the class representation.
	''' </summary>
	''' <returns>The class representation of the S4 class.</returns>
	Public Function GetClassDefinition() As S4Object
		Dim classSymbol = Engine.GetPredefinedSymbol("R_ClassSymbol")
		Dim className = GetAttribute(classSymbol).AsCharacter().First()
		Dim definition = Engine.GetFunction(Of R_getClassDef)()(className)
		Return New S4Object(Engine, definition)
	End Function

	''' <summary>
	''' Gets slot names and types.
	''' </summary>
	''' <returns>Slot names.</returns>
	Public Function GetSlotTypes() As IDictionary(Of String, String)
		Dim definition = GetClassDefinition()
		Dim slots = definition("slots")
		Dim namesSymbol = Engine.GetPredefinedSymbol("R_NamesSymbol")
		Return slots.GetAttribute(namesSymbol).AsCharacter().Zip(slots.AsCharacter(), Function(name, type) New With { _
			Key .Name = name, _
			Key .Type = type _
		}).ToDictionary(Function(t) t.Name, Function(t) t.Type)
	End Function
End Class


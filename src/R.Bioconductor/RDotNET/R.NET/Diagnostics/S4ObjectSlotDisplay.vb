#Region "Microsoft.VisualBasic::1101e83198e854b8f269eb5d036ddb42, ..\R.Bioconductor\RDotNET\R.NET\Diagnostics\S4ObjectSlotDisplay.vb"

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

Imports System.Diagnostics
Imports System.Linq

Namespace Diagnostics
	<DebuggerDisplay("{Display,nq}")> _
	Friend Class S4ObjectSlotDisplay
		<DebuggerBrowsable(DebuggerBrowsableState.Never)> _
		Private ReadOnly s4obj As S4Object

		<DebuggerBrowsable(DebuggerBrowsableState.Never)> _
		Private ReadOnly name As String

		Public Sub New(obj As S4Object, name As String)
			Me.s4obj = obj
			Me.name = name
		End Sub

		<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)> _
		Public ReadOnly Property Value() As SymbolicExpression
			Get
				Return s4obj(name)
			End Get
		End Property

		<DebuggerBrowsable(DebuggerBrowsableState.Never)> _
		Public ReadOnly Property Display() As String
			Get
				Dim slot = Me.Value
				Dim names = Me.s4obj.SlotNames
				If names Is Nothing OrElse Not names.Contains(name) Then
					Return [String].Format("NA ({0})", slot.Type)
				Else
					Return [String].Format("""{0}"" ({1})", name, slot.Type)
				End If
			End Get
		End Property
	End Class
End Namespace

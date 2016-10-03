#Region "Microsoft.VisualBasic::ad0192aefd5d6bab0ee4db0493fb2dd6, ..\R.Bioconductor\RDotNET\R.NET\Diagnostics\S4ObjectDebugView.vb"

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

Imports System.Diagnostics
Imports System.Linq

Namespace Diagnostics
	Friend Class S4ObjectDebugView
		Private ReadOnly s4obj As S4Object

		Public Sub New(obj As S4Object)
			Me.s4obj = obj
		End Sub

		<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)> _
		Public ReadOnly Property Slots() As S4ObjectSlotDisplay()
			Get
				Return Me.s4obj.SlotNames.AsEnumerable().[Select](Function(name) New S4ObjectSlotDisplay(Me.s4obj, name)).ToArray()
			End Get
		End Property
	End Class
End Namespace

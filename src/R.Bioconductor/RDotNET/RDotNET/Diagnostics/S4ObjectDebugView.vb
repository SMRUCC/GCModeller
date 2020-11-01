#Region "Microsoft.VisualBasic::cba1abc433900d3c3d1d9135a6078b34, RDotNET\RDotNET\Diagnostics\S4ObjectDebugView.vb"

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

    ' 	Class S4ObjectDebugView
    ' 
    ' 	    Properties: Slots
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

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


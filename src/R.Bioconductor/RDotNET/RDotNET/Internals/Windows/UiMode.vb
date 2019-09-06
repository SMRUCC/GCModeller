#Region "Microsoft.VisualBasic::0468351e8a85c72ac4b4525076ffe3c3, RDotNET\RDotNET\Internals\Windows\UiMode.vb"

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

    ' 	Enum UiMode
    ' 
    ' 	    		LinkDll, 		RGui, 		RTerminal
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Internals.Windows
	''' <summary>
	''' User interface mode
	''' </summary>
	Public Enum UiMode
		''' <summary>
		''' R graphical user interface
		''' </summary>
		RGui

		''' <summary>
		''' R terminal console
		''' </summary>
		RTerminal

		''' <summary>
		''' R dynamic (shared) library
		''' </summary>
		LinkDll
	End Enum
End Namespace


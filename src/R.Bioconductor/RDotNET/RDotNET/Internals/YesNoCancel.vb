﻿#Region "Microsoft.VisualBasic::7bc2480c6e9f0e7086fe4431918f0761, RDotNET\RDotNET\Internals\YesNoCancel.vb"

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

    ' 	Enum YesNoCancel
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Internals
	''' <summary>
	''' User's decision.
	''' </summary>
	Public Enum YesNoCancel
		''' <summary>
		''' User agreed.
		''' </summary>
		Yes = 1

		''' <summary>
		''' User disagreed.
		''' </summary>
		No = -1

		''' <summary>
		''' User abandoned to answer.
		''' </summary>
		Cancel = 0
	End Enum
End Namespace


#Region "Microsoft.VisualBasic::eaee64dcdfd556d2d343892f35cafd46, ..\R.Bioconductor\RDotNET\R.NET\Internals\OutputMode.vb"

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

Namespace Internals
	''' <summary>
	''' Specifies output mode.
	''' </summary>
	<Flags> _
	Public Enum OutputMode
		''' <summary>
		''' No option.
		''' </summary>
		None = &H0

		''' <summary>
		''' Quiet mode.
		''' </summary>
		Quiet = &H1

		''' <summary>
		''' Slave mode.
		''' </summary>
		Slave = &H2

		''' <summary>
		''' Verbose mode.
		''' </summary>
		Verbose = &H4
	End Enum
End Namespace

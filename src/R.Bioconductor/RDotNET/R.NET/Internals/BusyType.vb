#Region "Microsoft.VisualBasic::fcddb649d509aaf38e59ff4ad5604832, ..\R.Bioconductor\RDotNET\R.NET\Internals\BusyType.vb"

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
	''' Type of R's working.
	''' </summary>
	Public Enum BusyType
		''' <summary>
		''' Terminated states of business.
		''' </summary>
		None = 0

		''' <summary>
		''' Embarks on an extended computation
		''' </summary>
		ExtendedComputation = 1
	End Enum
End Namespace

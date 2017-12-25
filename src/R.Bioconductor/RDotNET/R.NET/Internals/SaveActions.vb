#Region "Microsoft.VisualBasic::5713c14ab59b927bccc8effa851b048d, ..\R.Bioconductor\RDotNET\R.NET\Internals\SaveActions.vb"

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

Namespace Internals
	''' <summary>
	''' Specifies the restore action.
	''' </summary>
	Public Enum StartupRestoreAction
		''' <summary>
		''' Not restoring.
		''' </summary>
		NoRestore = 0

		''' <summary>
		''' Restoring.
		''' </summary>
		Restore = 1

		''' <summary>
		''' The default value.
		''' </summary>
		[Default] = 2
	End Enum

	''' <summary>
	''' Specifies the save action.
	''' </summary>
	Public Enum StartupSaveAction
		''' <summary>
		''' The default value.
		''' </summary>
		[Default] = 2

		''' <summary>
		''' No saving.
		''' </summary>
		NoSave = 3

		''' <summary>
		''' Saving.
		''' </summary>
		Save = 4

		''' <summary>
		''' Asking user.
		''' </summary>
		Ask = 5

		''' <summary>
		''' Terminates without any actions.
		''' </summary>
		Suicide = 6
	End Enum
End Namespace

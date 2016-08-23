#Region "Microsoft.VisualBasic::713323438aa92fc8c01399b4685959c1, ..\R.Bioconductor\RDotNET\R.NET\Internals\ParseStatus.vb"

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
	''' Parsing status enumeration.
	''' </summary>
	Public Enum ParseStatus
		''' <summary>
		''' The default value.
		''' </summary>
		Null

		''' <summary>
		''' No error.
		''' </summary>
		OK

		''' <summary>
		''' Statement is incomplete.
		''' </summary>
		Incomplete

		''' <summary>
		''' Error occurred.
		''' </summary>
		[Error]

		''' <summary>
		''' EOF.
		''' </summary>
		EOF

		'#region Original Definitions
		'[Obsolete("Use ParseStatus.Null instead.")]
		'PARSE_NULL = Null,
		'[Obsolete("Use ParseStatus.OK instead.")]
		'PARSE_OK = OK,
		'[Obsolete("Use ParseStatus.Incomplete instead.")]
		'PARSE_INCOMPLETE = Incomplete,
		'[Obsolete("Use ParseStatus.Error instead.")]
		'PARSE_ERROR = Error,
		'[Obsolete("Use ParseStatus.EOF instead.")]
		'PARSE_EOF = EOF,
		'#endregion
	End Enum
End Namespace


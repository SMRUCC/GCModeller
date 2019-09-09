#Region "Microsoft.VisualBasic::8bbfd39e0d070f4c847362349b940026, RDotNET\RDotNET\Internals\ParseStatus.vb"

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

    ' 	Enum ParseStatus
    ' 
    ' 	    		[Error], 		EOF, 		Incomplete, 		Null, 		OK
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


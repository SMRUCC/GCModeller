#Region "Microsoft.VisualBasic::4707f1fdce39e752cc380ab408793f9f, data\Xfam\Pfam\Pipeline\Database\Entry.vb"

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

    '     Class Entry
    ' 
    ' 
    ' 
    '     Class Header
    ' 
    ' 
    ' 
    '     Class Reference
    ' 
    ' 
    ' 
    '     Class Comment
    ' 
    ' 
    ' 
    '     Class Alignment
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Pipeline.Database

    ''' <summary>
    ''' Pfam Entry, The format of Pfam entries has become stricter and we now enforce some ordering of the fields.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Entry

    End Class

    ''' <summary>
    ''' The header section mainly contains compulsory fields. These include Pfam specific information such as 
    ''' accession numbers and identifiers, as well as a short description of the family. The only 
    ''' non-compulsory field in the header section is the PI field.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Header

    End Class

    ''' <summary>
    ''' The reference section mainly contains cross-links to other databases, and literature references.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Reference

    End Class

    ''' <summary>
    ''' The comment section contains functional information about the Pfam family.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Comment

    End Class

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Alignment

    End Class
End Namespace

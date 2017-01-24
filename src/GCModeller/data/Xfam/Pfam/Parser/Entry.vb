#Region "Microsoft.VisualBasic::5f57e63b30395e2d24235001a68de5ce, ..\GCModeller\data\Xfam\Pfam\Parser\Entry.vb"

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

Namespace PfamFastaComponentModels

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

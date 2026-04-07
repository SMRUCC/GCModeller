#Region "Microsoft.VisualBasic::6e696ff8d47fdfbb7b576ae7a1584f69, engine\Model\LocalRepository\Molecule.vb"

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


    ' Code Statistics:

    '   Total Lines: 23
    '    Code Lines: 13 (56.52%)
    ' Comment Lines: 4 (17.39%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (26.09%)
    '     File Size: 566 B


    '     Class Molecule
    ' 
    '         Properties: db_xrefs, formula, id, name, symbol
    ' 
    '     Class DBXref
    ' 
    '         Properties: dbname, xref_id
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace WebJSON

    Public Class Molecule

        ''' <summary>
        ''' usually be an integer id of this metabolite molecule
        ''' </summary>
        ''' <returns></returns>
        Public Property id As String
        Public Property name As String
        Public Property symbol As String
        Public Property formula As String
        Public Property db_xrefs As DBXref()

    End Class

    Public Class DBXref

        Public Property dbname As String
        Public Property xref_id As String

    End Class
End Namespace

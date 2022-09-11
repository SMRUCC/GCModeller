#Region "Microsoft.VisualBasic::e33a1340c649963790292fdae23c7271, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\bGetObject.vb"

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

    '   Total Lines: 17
    '    Code Lines: 9
    ' Comment Lines: 3
    '   Blank Lines: 5
    '     File Size: 417 B


    '     Class bGetObject
    ' 
    '         Properties: Definition, Entry, Name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' dbget-bin/www_bget
    ''' </summary>
    Public MustInherit Class bGetObject

        Public MustOverride ReadOnly Property Code As String

        Public Property Entry As String
        Public Property Name As String
        Public Property Definition As String

    End Class
End Namespace

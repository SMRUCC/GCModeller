#Region "Microsoft.VisualBasic::2b016c17b772b5bfa649eed22fddfa41, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\UniProt\Web\Entry.vb"

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

    '   Total Lines: 18
    '    Code Lines: 15
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 572 B


    '     Class Entry
    ' 
    '         Properties: Entry, EntryName, GeneNames, Length, Organism
    '                     ProteinNames, StatusReviewed
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.Uniprot.Web

    Public Class Entry
        Public Property Entry As KeyValuePair
        Public Property EntryName As String
        Public Property StatusReviewed As Boolean
        Public Property ProteinNames As String
        Public Property GeneNames As String
        Public Property Organism As String
        Public Property Length As String

        Public Overrides Function ToString() As String
            Return Entry.ToString
        End Function
    End Class
End Namespace

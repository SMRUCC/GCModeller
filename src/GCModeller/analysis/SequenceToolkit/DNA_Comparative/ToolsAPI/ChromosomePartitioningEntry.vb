#Region "Microsoft.VisualBasic::4be80344bc3fa26b94aeeb9b425a35bc, GCModeller\analysis\SequenceToolkit\DNA_Comparative\ToolsAPI\ChromosomePartitioningEntry.vb"

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

    '   Total Lines: 10
    '    Code Lines: 8
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 286 B


    ' Class ChromosomePartitioningEntry
    ' 
    '     Properties: ORF, PartitioningTag
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language

Public Class ChromosomePartitioningEntry

    <Column("QueryProtein")>
    Public Property ORF As String
    <Column("Tag")>
    Public Property PartitioningTag As String
End Class

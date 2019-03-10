#Region "Microsoft.VisualBasic::25b23196341446e6f499adf70e912073, Bio.Repository\KEGG\ReactionRepository\PathwayRepository.vb"

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

    ' Class PathwayRepository
    ' 
    '     Properties: PathwayMaps
    ' 
    '     Function: ScanModels
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Language

Public Class PathwayRepository : Inherits XmlDataModel

    Public Property PathwayMaps As PathwayMap()

    Public Shared Function ScanModels(directory As String) As PathwayRepository
        Dim maps As New List(Of PathwayMap)

        For Each file As String In ls - l - r - "*.Xml" <= directory
            maps += file.LoadXml(Of PathwayMap)
        Next

        Return New PathwayRepository With {
            .PathwayMaps = maps
        }
    End Function
End Class

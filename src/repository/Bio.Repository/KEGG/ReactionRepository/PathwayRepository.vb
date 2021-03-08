#Region "Microsoft.VisualBasic::a80924671571ba8944bb7a1de1958538, core\Bio.Repository\KEGG\ReactionRepository\PathwayRepository.vb"

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
    '     Properties: GlobalAndOverviewMaps, PathwayMaps
    ' 
    '     Function: GenericEnumerator, GetEnumerator, ScanModels
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject

Public Class PathwayRepository : Inherits XmlDataModel
    Implements Enumeration(Of PathwayMap)

    Public Property PathwayMaps As PathwayMap()

    Public ReadOnly Property GlobalAndOverviewMaps As PathwayMap()
        Get
            Dim entries As Index(Of String) = BriteHEntry.Pathway _
                .GetGlobalAndOverviewMaps _
                .Select(Function(term) term.name) _
                .Indexing

            Return PathwayMaps _
                .Where(Function(map) map.briteID Like entries) _
                .ToArray
        End Get
    End Property

    Public Shared Function ScanModels(directory As String) As PathwayRepository
        Dim maps As New List(Of PathwayMap)

        For Each file As String In ls - l - r - "*.Xml" <= directory
            maps += file.LoadXml(Of PathwayMap)
        Next

        Return New PathwayRepository With {
            .PathwayMaps = maps
        }
    End Function

    Public Iterator Function GenericEnumerator() As IEnumerator(Of PathwayMap) Implements Enumeration(Of PathwayMap).GenericEnumerator
        For Each map As PathwayMap In PathwayMaps
            Yield map
        Next
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of PathwayMap).GetEnumerator
        Yield GenericEnumerator()
    End Function
End Class

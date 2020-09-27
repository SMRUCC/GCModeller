#Region "Microsoft.VisualBasic::e74bfeb5a99f14549e12c9b5b13a19f8, models\Networks\KEGG\BiologicalObjectCluster.vb"

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

    ' Module BiologicalObjectCluster
    ' 
    '     Function: (+2 Overloads) CompoundsMap, GetMapCategories, ReactionMap
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module BiologicalObjectCluster

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CompoundsMap(map As Pathway) As NamedCollection(Of String)
        Return New NamedCollection(Of String) With {
            .name = map.EntryId,
            .value = map.compound.Keys
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CompoundsMap(map As Map) As NamedCollection(Of String)
        Return New NamedCollection(Of String) With {
            .name = map.id,
            .value = map.shapes _
                .Select(Function(a) a.IDVector) _
                .IteratesALL _
                .Where(Function(id) id.IsPattern("C\d+")) _
                .Distinct _
                .ToArray
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ReactionMap(map As Map) As NamedCollection(Of String)
        Return New NamedCollection(Of String) With {
            .name = map.id,
            .value = map.shapes _
                .Select(Function(a) a.IDVector) _
                .IteratesALL _
                .Where(Function(id) id.IsPattern("R\d+")) _
                .Distinct _
                .ToArray
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetMapCategories() As Dictionary(Of String, BriteHEntry.Pathway)
        Return BriteHEntry.Pathway _
            .LoadFromResource _
            .ToDictionary(Function(map) map.EntryId)
    End Function
End Module

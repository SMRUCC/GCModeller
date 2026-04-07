#Region "Microsoft.VisualBasic::22e53e4cc1df0c9de679e0f99f5d4bb1, annotations\GSEA\GSEA\KnowledgeBase\KEGG\KOMapCluster.vb"

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

    '   Total Lines: 70
    '    Code Lines: 55 (78.57%)
    ' Comment Lines: 5 (7.14%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (14.29%)
    '     File Size: 2.68 KB


    ' Class KOMapCluster
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: KEGGIdVector, KEGGMapRelation, KOIDMap, SelectTerms
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML

Public Class KOMapCluster

    ReadOnly maps As Dictionary(Of String, Map)
    ReadOnly termMaps As Dictionary(Of String, String())

    Sub New(maps As IEnumerable(Of Map), Optional multipleOmics As Boolean = False)
        Me.maps = maps.ToDictionary(Function(m) m.EntryId)
        Me.termMaps = KEGGMapRelation(Me.maps.Values, multipleOmics)
    End Sub

    ''' <summary>
    ''' ``KO ~ map id[]`` 
    ''' </summary>
    ''' <param name="maps"></param>
    ''' <returns></returns>
    Public Shared Function KEGGMapRelation(maps As IEnumerable(Of Map), Optional multipleOmics As Boolean = False) As Dictionary(Of String, String())
        Return maps _
            .Select(Function(m)
                        Return SelectTerms(m, multipleOmics)
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(ko) ko.ko) _
            .ToDictionary(Function(g) g.Key,
                            Function(g)
                                Return g.Select(Function(t) t.mapID) _
                                        .Distinct _
                                        .ToArray
                            End Function)
    End Function

    Private Shared Iterator Function SelectTerms(map As Map, Optional multipleOmics As Boolean = False) As IEnumerable(Of (mapID As String, ko As String))
        Dim idvec As IEnumerable(Of String) =
            From id As String
            In KEGGIdVector(map)
            Where If(multipleOmics, id.IsPattern("[KC]\d+"), id.IsPattern("K\d+"))
            Distinct

        For Each id As String In idvec
            Yield (mapID:=map.EntryId, KO:=id)
        Next
    End Function

    Private Shared Function KEGGIdVector(map As Map) As IEnumerable(Of String)
        Return map.shapes.mapdata _
            .Select(Function(a) a.IDVector) _
            .IteratesALL
    End Function

    Public Function KOIDMap(koid As String) As NamedValue(Of String)()
        If Not termMaps.ContainsKey(koid) Then
            Return {}
        End If

        Return termMaps(koid) _
            .Select(Function(map_id)
                        Dim map As Map = maps(map_id)

                        Return New NamedValue(Of String)(
                            name:=map.EntryId,
                            value:=map.name,
                            describ:=map.description
                        )
                    End Function) _
            .ToArray
    End Function
End Class


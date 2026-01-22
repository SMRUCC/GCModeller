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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Module CreateKEGGClusters

    <Extension>
    Public Function KEGGClusters(maps As IEnumerable(Of MapIndex)) As GetClusterTerms
        Dim mapsList As Dictionary(Of String, MapIndex) = maps.ToDictionary(Function(m) m.id)
        Dim clusters = mapsList.Values _
            .Select(Function(map)
                        Return map.KOIndex.Objects.Select(Function(ko) (ko, map.id))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(ko) ko.ko) _
            .ToDictionary(Function(ko) ko.Key,
                          Function(map)
                              Return maps _
                                  .Select(Function(a) a.id) _
                                  .Distinct _
                                  .ToArray
                          End Function)

        Return Function(id)
                   If clusters.ContainsKey(id) Then
                       Return Iterator Function() As IEnumerable(Of NamedValue(Of String))
                                  For Each mapId As String In clusters(id)
                                      Yield New NamedValue(Of String) With {
                                          .Name = mapId,
                                          .Value = mapsList(mapId).Name,
                                          .Description = mapsList(mapId).URL
                                      }
                                  Next
                              End Function().ToArray
                   Else
                       Return {}
                   End If
               End Function
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="maps">ko00001</param>
    ''' <returns></returns>
    <Extension>
    Public Function KEGGClusters(maps As htext) As GetClusterTerms
        Dim table = maps.Deflate("K\d+").ToArray
        Dim terms As Dictionary(Of String, NamedValue(Of String)()) = table _
            .GroupBy(Function(a) a.kegg_id) _
            .ToDictionary(Function(a) a.Key,
                          Function(group)
                              Return group _
                                  .Select(Function(a)
                                              Dim ref = a.subcategory.GetTagValue
                                              Dim name As New NamedValue(Of String) With {
                                                  .Name = "map" & ref.Name,
                                                  .Value = ref.Value,
                                                  .Description = ref.Description
                                              }
                                          End Function) _
                                  .ToArray
                          End Function)

        Return Function(id) terms.TryGetValue(id)
    End Function

    <Extension>
    Public Function KEGGClusters(maps As IEnumerable(Of Map)) As GetClusterTerms
        Dim mapsList = maps.ToDictionary(Function(m) m.id)
        Dim clusters = mapsList.Values.KEGGMapRelation

        Return Function(id)
                   If clusters.ContainsKey(id) Then
                       Return Iterator Function() As IEnumerable(Of NamedValue(Of String))
                                  For Each mapID As String In clusters(id)
                                      Yield New NamedValue(Of String) With {
                                          .Name = mapID,
                                          .Value = mapsList(mapID).Name,
                                          .Description = mapsList(mapID).URL
                                      }
                                  Next
                              End Function().ToArray
                   Else
                       Return {}
                   End If
               End Function
    End Function
End Module

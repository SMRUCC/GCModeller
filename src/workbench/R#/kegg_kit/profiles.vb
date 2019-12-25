Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

<Package("kegg.profiles")>
Module profiles

    <ExportAPI("compounds.pathway.index")>
    Public Function CompoundPathwayIndex(pathways As PathwayMap()) As Dictionary(Of String, Index(Of String))
        Return pathways _
            .GroupBy(Function(p) p.briteID) _
            .ToDictionary(Function(p)
                              Return p.Key
                          End Function,
                          Function(p)
                              Return p.First.KEGGCompound _
                                  .SafeQuery _
                                  .Keys _
                                  .Indexing
                          End Function)
    End Function

    <ExportAPI("compounds.pathway.profiles")>
    Public Function CompoundPathwayProfiles(pathways As Dictionary(Of String, Index(Of String)), compounds As String()) As Dictionary(Of String, Integer)
        Return pathways _
            .ToDictionary(Function(p) p.Key,
                          Function(p)
                              Return p.Value _
                                  .Intersect(collection:=compounds) _
                                  .Distinct _
                                  .Count
                          End Function)
    End Function

    <ExportAPI("kegg.category_profiles")>
    Public Function KEGGCategoryProfiles(profiles As Dictionary(Of String, Double)) As EntityObject()
        Return profiles.KEGGCategoryProfiles _
            .Select(Function(category)
                        Return category.Value _
                            .Select(Function(term)
                                        Return New EntityObject With {
                                            .ID = term.Name,
                                            .Properties = New Dictionary(Of String, String) From {
                                                {"class", category.Key},
                                                {"count", term.Value}
                                            }
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
    End Function
End Module

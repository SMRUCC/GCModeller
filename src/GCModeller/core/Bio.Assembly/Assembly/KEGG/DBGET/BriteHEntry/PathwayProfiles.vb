Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Module PathwayProfiles

        <Extension>
        Private Function doProfiles(group As Pathway(), profiles As Dictionary(Of String, Double)) As NamedValue(Of Double)()
            Return group _
               .Where(Function(p)
                          Return profiles.ContainsKey(p.EntryId) AndAlso profiles(p.EntryId) > 0
                      End Function) _
               .Select(Function(p As Pathway)
                           Return New NamedValue(Of Double) With {
                               .Name = p.entry.text,
                               .Value = profiles(p.EntryId),
                               .Description = p.entry.text
                           }
                       End Function) _
               .ToArray
        End Function

        <ExportAPI("kegg.category_profiles")>
        <Extension>
        Public Function KEGGCategoryProfiles(profiles As Dictionary(Of String, Double)) As Dictionary(Of String, NamedValue(Of Double)())
            Dim brite As Dictionary(Of String, BriteHEntry.Pathway()) = BriteHEntry.Pathway _
                .LoadDictionary _
                .GroupBy(Function(p) p.Value.class) _
                .ToDictionary(Function(p) p.Key,
                              Function(p)
                                  Return p.Values
                              End Function)

            Return brite _
               .ToDictionary(Function(p) p.Key,
                             Function(group)
                                 Return group.Value _
                                    .doProfiles(profiles) _
                                    .OrderByDescending(Function(t) t.Value) _
                                    .ToArray
                             End Function)
        End Function
    End Module
End Namespace
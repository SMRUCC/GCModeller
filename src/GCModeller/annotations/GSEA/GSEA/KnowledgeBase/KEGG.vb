Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Public Module KEGG

    <Extension>
    Public Function KO_category(category As BriteHText) As IEnumerable(Of Cluster)
        Return category.categoryItems _
            .SafeQuery _
            .Select(Function(subtype)
                        Return subtype.subtypeClusters
                    End Function) _
            .IteratesALL
    End Function

    <Extension>
    Private Function subtypeClusters(subtype As BriteHText) As IEnumerable(Of Cluster)
        Return subtype.categoryItems _
            .SafeQuery _
            .Select(Function(pathway)
                        Return New Cluster With {
                            .ID = "map" & pathway.entryID,
                            .description = pathway _
                                .ToString _
                                .Replace("[BR:ko]", "") _
                                .Replace("[PATH:ko]", "") _
                                .Trim,
                            .names = pathway.description _
                                .Replace("[BR:ko]", "") _
                                .Replace("[PATH:ko]", "") _
                                .Trim,
                            .members = pathway.categoryItems _
                                .SafeQuery _
                                .Select(Function(ko)
                                            Return New BackgroundGene With {
                                                .accessionID = ko.entryID,
                                                .[alias] = {ko.entryID},
                                                .locus_tag = New NamedValue With {.name = ko.entryID, .text = ko.description},
                                                .name = ko.description,
                                                .term_id = {ko.entryID}
                                            }
                                        End Function) _
                                .ToArray
                        }
                    End Function)
    End Function
End Module

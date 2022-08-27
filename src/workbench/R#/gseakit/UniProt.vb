Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports stdNum = System.Math

''' <summary>
''' The uniprot background model handler
''' </summary>
''' 
<Package("UniProt")>
Module UniProt

    <ExportAPI("subcellular_location")>
    <RApiReturn(GetType(Background))>
    Public Function SubcellularLocation(<RRawVectorArgument> uniprot As Object,
                                        Optional env As Environment = Nothing) As Object

        Dim base = pipeline.TryCreatePipeline(Of entry)(uniprot, env)

        If base.isError Then
            Return base.getError
        Else
            Return base.populates(Of entry)(env).SubcellularLocation
        End If
    End Function

    <ExportAPI("uniprot_keywords")>
    <RApiReturn(GetType(Background))>
    Public Function uniprotKeywords(<RRawVectorArgument> uniprot As Object,
                                    Optional env As Environment = Nothing) As Object

        Dim base = pipeline.TryCreatePipeline(Of entry)(uniprot, env)

        If base.isError Then
            Return base.getError
        Else
            Return base.populates(Of entry)(env).UniprotKeywordsModel
        End If
    End Function

    <ExportAPI("keyword_profiles")>
    Public Function keywordsProfiles(enrichment As EnrichmentResult(), keywords As dataframe) As CatalogProfiles
        Dim catalogs As New Dictionary(Of String, CatalogProfile)
        Dim id As String() = keywords("Keyword ID")
        Dim Category As String() = keywords("Category")
        Dim groups = Category _
            .Select(Function(tag, i) (tag, id(i))) _
            .GroupBy(Function(tag) tag.tag) _
            .ToArray
        Dim enrich = enrichment.ToDictionary(Function(a) a.term)
        Dim profile As NamedValue(Of Double)()

        For Each group In groups
            id = group.Select(Function(i) i.Item2).ToArray
            profile = id _
                .Where(Function(a) enrich.ContainsKey(a)) _
                .Select(Function(a)
                            Return New NamedValue(Of Double) With {
                                .Name = a,
                                .Value = -stdNum.Log10(enrich(a).pvalue),
                                .Description = enrich(a).name
                            }
                        End Function) _
                .ToArray

            catalogs.Add(group.Key, New CatalogProfile(data:=profile))
        Next

        Return New CatalogProfiles With {
            .catalogs = catalogs
        }
    End Function
End Module

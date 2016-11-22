Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Namespace Metagenomics

    Public Module BIOMTaxonomy

        ''' <summary>
        ''' ``k__{x.superkingdom};p__{x.phylum};c__{x.class};o__{x.order};f__{x.family};g__{x.genus};s__{x.species}``
        ''' </summary>
        Public ReadOnly Property BIOMPrefix As String() = {"k__", "p__", "c__", "o__", "f__", "g__", "s__"}

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Example:
        ''' 
        ''' ```
        ''' no rank__cellular organisms;superkingdom__Eukaryota;no rank__Opisthokonta;kingdom__Fungi;subkingdom__Dikarya;phylum__Ascomycota;no rank__saccharomyceta;subphylum__Pezizomycotina;no rank__leotiomyceta;no rank__dothideomyceta;class__Dothideomycetes;no rank__Dothideomycetes incertae sedis;order__Botryosphaeriales;family__Botryosphaeriaceae;genus__Macrophomina;species__Macrophomina phaseolina;no rank__Macrophomina phaseolina MS6
        ''' ```
        ''' </remarks>
        Public ReadOnly Property BIOMPrefixAlt As String() = {
            "superkingdom__", "phylum__", "class__", "order__", "family__", "genus__", "species__"
        }

        ''' <summary>
        ''' For <see cref="BIOMPrefix"/>
        ''' </summary>
        ''' <param name="taxonomy$"></param>
        ''' <returns></returns>
        Public Function TaxonomyParser(taxonomy$) As Dictionary(Of String, String)
            Dim tokens$() = taxonomy.Split(";"c)
            Dim catalogs As NamedValue(Of String)() = tokens _
                .Select(Function(t) t.GetTagValue("__")) _
                .ToArray
            Dim out As New Dictionary(Of String, String)

            For Each x As NamedValue(Of String) In catalogs
                Dim name$ = x.Value

                Select Case x.Name
                    Case "k" : Call out.Add(NcbiTaxonomyTree.superkingdom, name)
                    Case "p" : Call out.Add(NcbiTaxonomyTree.phylum, name)
                    Case "c" : Call out.Add(NcbiTaxonomyTree.class, name)
                    Case "o" : Call out.Add(NcbiTaxonomyTree.order, name)
                    Case "f" : Call out.Add(NcbiTaxonomyTree.family, name)
                    Case "g" : Call out.Add(NcbiTaxonomyTree.genus, name)
                    Case "s" : Call out.Add(NcbiTaxonomyTree.species, name)
                    Case Else
                End Select
            Next

            Return out
        End Function

        ''' <summary>
        ''' For <see cref="BIOMPrefixAlt"/>
        ''' </summary>
        ''' <param name="taxonomy$"></param>
        ''' <returns></returns>
        Public Function TaxonomyParserAlt(taxonomy$) As Dictionary(Of String, String)
            Dim tokens$() = taxonomy.Split(";"c)
            Dim catalogs As NamedValue(Of String)() = tokens _
                .Select(Function(t) t.GetTagValue("__")) _
                .ToArray
            Dim out As New Dictionary(Of String, String)

            For Each x As NamedValue(Of String) In catalogs
                If Array.IndexOf(BIOMPrefixAlt, x.Name) > -1 Then
                    Call out.Add(x.Name, x.Value)
                End If
            Next

            Return out
        End Function
    End Module
End Namespace
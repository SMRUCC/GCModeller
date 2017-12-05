Imports Field = Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps.DataFrameColumnAttribute

Namespace ARDB

    ''' <summary>
    ''' ``origin_type.tab``
    ''' </summary>
    Public Class origin_type

        ''' <summary>
        ''' #1 is the type name of the antibiotic resistance gene
        ''' </summary>
        ''' <returns></returns>
        <Field(1)> Public Property type_name As String
        ''' <summary>
        ''' #2 is the NCBI accession number of the representative gene for this resistance type
        ''' </summary>
        ''' <returns></returns>
        <Field(2)> Public Property NCBI_acc As String
        ''' <summary>
        ''' #3 is the resistance class name to which the resistance type belongs.
        ''' </summary>
        ''' <returns></returns>
        <Field(3)> Public Property resistance_type As String
        ''' <summary>
        ''' #4 is the BLAST similarity cutoff value used to identify further resistance genes
        ''' belonging to this type.
        ''' </summary>
        ''' <returns></returns>
        <Field(4)> Public Property similarity As String

    End Class

    ''' <summary>
    ''' ``ar_genes.tab``
    ''' </summary>
    Public Class ar_genes

        ''' <summary>
        ''' #1 is the accession number of antibiotic resistance gene in NCBI Protein Database.
        ''' </summary>
        ''' <returns></returns>
        <Field(1)> Public Property NCBI_acc_prot As String
        ''' <summary>
        ''' #2 is the type name of antibiotic resistance gene.
        ''' </summary>
        ''' <returns></returns>
        <Field(2)> Public Property type_name As String
        ''' <summary>
        ''' #3 is the NCBI Taxon ID of the organism from which this resistance gene is isolated.
        ''' </summary>
        ''' <returns></returns>
        <Field(3)> Public Property NCBI_taxon As String
        ''' <summary>
        ''' #4 is the accession number of antibiotic resistance gene in NCBI Nucleotide Database.
        ''' 0’ represents not available.
        ''' </summary>
        ''' <returns></returns>
        <Field(4)> Public Property NCBI_acc_nucl As String
        ''' <summary>
        ''' #5 represents if this gene is a non-redundant record. ‘Y’ represents non-redundant and
        ''' 'N’ represents redundant. Redundant means this is a duplicate record of another nonredundant
        ''' gene record.
        ''' </summary>
        ''' <returns></returns>
        <Field(5)> Public Property non_redundant As String
        ''' <summary>
        ''' #6 represents if this gene sequence is complete. ‘Y’ represents complete and ‘N’
        ''' represents incomplete.
        ''' </summary>
        ''' <returns></returns>
        <Field(6)> Public Property complete As String

    End Class

    ''' <summary>
    ''' ``resistance_profile.tab``
    ''' </summary>
    Public Class resistance_profile

        ''' <summary>
        ''' #1 is the name of the antibiotic resistance gene
        ''' </summary>
        ''' <returns></returns>
        <Field(1)> Public Property gene_name As String
        ''' <summary>
        ''' #2 is the name of the antibiotic to which the gene is resistant.
        ''' </summary>
        ''' <returns></returns>
        <Field(2)> Public Property antibiotic As String

    End Class

    ''' <summary>
    ''' ``type2cdd.tab``
    ''' </summary>
    Public Class type2cdd

        ''' <summary>
        ''' #1 is the name of the resistance type.
        ''' </summary>
        ''' <returns></returns>
        <Field(1)> Public Property resistance_type As String
        ''' <summary>
        ''' #2 is the associated CDD ID for each type.
        ''' </summary>
        ''' <returns></returns>
        <Field(2)> Public Property CDD_ID As String

    End Class

    ''' <summary>
    ''' ``type2cog.tab``
    ''' </summary>
    Public Class type2cog

        ''' <summary>
        ''' #1 is the name of the resistance type.
        ''' </summary>
        ''' <returns></returns>
        <Field(1)> Public Property resistance_type As String
        ''' <summary>
        ''' #2 is the associated COG ID for each type.
        ''' </summary>
        ''' <returns></returns>
        <Field(2)> Public Property COG_ID As String

    End Class

    ''' <summary>
    ''' ``type2genus.tab``
    ''' </summary>
    Public Class type2genus

        ''' <summary>
        ''' #1 is the name of the resistance type.
        ''' </summary>
        ''' <returns></returns>
        <Field(1)> Public Property resistance_type As String
        ''' <summary>
        ''' #2 is the taxon id of the genus in which the resistance genes have been found.
        ''' </summary>
        ''' <returns></returns>
        <Field(2)> Public Property taxon_id As String
        ''' <summary>
        ''' #3 is the number of resistance genes that have been found in the genus.
        ''' </summary>
        ''' <returns></returns>
        <Field(3)> Public Property resistance_genes As String

    End Class

    ''' <summary>
    ''' ``type2ref.tab``
    ''' </summary>
    Public Class type2ref

        ''' <summary>
        ''' #1 is the name of the resistance type.
        ''' </summary>
        ''' <returns></returns>
        <Field(1)> Public Property resistance_type As String
        ''' <summary>
        ''' #2 is the associated PubMed ID of reference.
        ''' </summary>
        ''' <returns></returns>
        <Field(2)> Public Property PubMed_ID As String

    End Class

    ''' <summary>
    ''' ``type2rq.tab``
    ''' </summary>
    Public Class type2rq

        ''' <summary>
        ''' #1 is the name of the resistance type.
        ''' </summary>
        ''' <returns></returns>
        <Field(1)> Public Property resistance_type As String
        ''' <summary>
        ''' #2 is the name of the resistance type that is required by the #1 to confer resistance.
        ''' </summary>
        ''' <returns></returns>
        <Field(2)> Public Property confer_name As String

    End Class

    ''' <summary>
    ''' ``class2go.tab``
    ''' </summary>
    Public Class class2go

        ''' <summary>
        ''' #1 is the name of antibiotic resistance genes class
        ''' </summary>
        ''' <returns></returns>
        <Field(1)> Public Property class_name As String
        ''' <summary>
        ''' #2 is the Gene Ontology (GO) term ID (http://www.geneontology.org/). GO represents
        ''' standard GO terms In GO database. AR represents ontology terms created by our
        ''' database And it Is available here http://ardb.cbcb.umd.edu/go.shtml.
        ''' </summary>
        ''' <returns></returns>
        <Field(2)> Public Property GO_ID As String

    End Class

    ''' <summary>
    ''' ``class2info.tab``
    ''' </summary>
    Public Class class2info

        ''' <summary>
        ''' #1 is the name of antibiotic resistance genes class.
        ''' </summary>
        ''' <returns></returns>
        Public Property class_name As String
        ''' <summary>
        ''' #2 is the description of this antibiotic resistance genes class.
        ''' </summary>
        ''' <returns></returns>
        Public Property description As String

    End Class

    ''' <summary>
    ''' ``ab2chebi.tab``
    ''' </summary>
    Public Class ab2chebi

        ''' <summary>
        ''' Column 1 (#1) is the name of antibiotic
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
        ''' <summary>
        ''' #2 is the Chemical Entities of Biological Interest (ChEBI) ID. http://www.ebi.ac.uk/chebi/
        ''' </summary>
        ''' <returns></returns>
        Public Property ChEBI_ID As String

    End Class

    ''' <summary>
    ''' ``ab2kegginfo.tab``
    ''' </summary>
    Public Class ab2kegginfo
        ''' <summary>
        ''' #1 is the name of antibiotic.
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
        ''' <summary>
        ''' #2 is the KEGG DRUG database ID. http://www.genome.jp/kegg/drug/
        ''' </summary>
        ''' <returns></returns>
        Public Property KEGG_DRUG_ID As String
        ''' <summary>
        ''' #3 is the PubChem database ID (http://pubchem.ncbi.nlm.nih.gov/) extracted from the
        ''' information in KEGG DRUG database.
        ''' </summary>
        ''' <returns></returns>
        Public Property PubChem As String
        ''' <summary>
        ''' #4 is the description of action target for the antibiotic extracted from the information in
        ''' KEGG DRUG database.
        ''' </summary>
        ''' <returns></returns>
        Public Property description As String
    End Class

    ''' <summary>
    ''' ``ab2mesh.tab``
    ''' </summary>
    Public Class ab2mesh
        ''' <summary>
        ''' #1 is the name of antibiotic.
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
        ''' <summary>
        ''' #2 is the description of antibiotic extracted from Medical Subject Headings (Mesh)
        ''' database. http//www.nlm.nih.gov/mesh/MBrowser.html
        ''' </summary>
        ''' <returns></returns>
        Public Property description As String
    End Class

    ''' <summary>
    ''' ``genomeinfo.tab``
    ''' </summary>
    Public Class genomeinfo

        ''' <summary>
        ''' #1 is the accession number of a bacterial chromosome or plasmid in NCBI.
        ''' </summary>
        ''' <returns></returns>
        Public Property NCBI_acc As String
        ''' <summary>
        ''' #2 is the GenBank accession number of a bacterial chromosome or plasmid in NCBI.
        ''' </summary>
        ''' <returns></returns>
        Public Property GenBank_acc As String
        ''' <summary>
        ''' #3 is the length of the chromosome or plasmid.
        ''' </summary>
        ''' <returns></returns>
        Public Property length As String
        ''' <summary>
        ''' #4 is the Taxon ID of the bacteria.
        ''' </summary>
        ''' <returns></returns>
        Public Property Taxon_ID As String
        ''' <summary>
        ''' #5 is the genome project ID in NCBI.
        ''' </summary>
        ''' <returns></returns>
        Public Property genome_project_ID As String
        ''' <summary>
        ''' #6 is the Taxon name of the bacteria.
        ''' </summary>
        ''' <returns></returns>
        Public Property Taxon_name As String
        ''' <summary>
        ''' #7 is the name of replicon (chromosome or plasmid).
        ''' </summary>
        ''' <returns></returns>
        Public Property replicon_name As String
        ''' <summary>
        ''' #8 is the creation date.
        ''' </summary>
        ''' <returns></returns>
        Public Property [date] As String
    End Class

    ''' <summary>
    ''' ``genomeblast.tab``
    ''' </summary>
    Public Class genomeblast

        ''' <summary>
        ''' #1 is the accession number of a bacterial genome in NCBI.
        ''' http://www.ncbi.nlm.nih.gov/genomes/genlist.cgi?taxid=2&type=0&name=Complete%20B
        ''' acteria
        ''' </summary>
        ''' <returns></returns>
        Public Property genome_acc As String
        ''' <summary>
        ''' #2 is the protein sequence accession number of identified antibiotic resistance gene in
        ''' this genome(BLAST query sequence).
        ''' </summary>
        ''' <returns></returns>
        Public Property protein_acc As String
        ''' <summary>
        ''' #3 is the protein sequence accession number of best BLAST hit antibiotic resistance
        ''' gene (BLAST hit sequence).
        ''' </summary>
        ''' <returns></returns>
        Public Property hit_acc As String
        ''' <summary>
        ''' #4 is the percent identity of High Scoring Pair (HSP).
        ''' </summary>
        ''' <returns></returns>
        Public Property identity As String
        ''' <summary>
        ''' #5 is the bits score.
        ''' </summary>
        ''' <returns></returns>
        Public Property bits_score As String
        ''' <summary>
        ''' #6 is the E value.
        ''' </summary>
        ''' <returns></returns>
        Public Property Evalue As String
        ''' <summary>
        ''' #7 is the query sequence length.
        ''' </summary>
        ''' <returns></returns>
        Public Property query_length As String
        ''' <summary>
        ''' #8 is the hit sequence length.
        ''' </summary>
        ''' <returns></returns>
        Public Property hit_length As String
        ''' <summary>
        ''' #9 is the number of identical amino acids in the HSP.
        ''' </summary>
        ''' <returns></returns>
        Public Property hits As String
        ''' <summary>
        ''' #10 is the length of HSP.
        ''' </summary>
        ''' <returns></returns>
        Public Property HSP_length As String
        ''' <summary>
        ''' #11 is the start coordinate of query sequence in the HSP.
        ''' </summary>
        ''' <returns></returns>
        Public Property query_start As String
        ''' <summary>
        ''' #12 is the end coordinate of query sequence in the HSP.
        ''' </summary>
        ''' <returns></returns>
        Public Property query_end As String
        ''' <summary>
        ''' #13 is the start coordinate of hit sequence in the HSP.
        ''' </summary>
        ''' <returns></returns>
        Public Property hit_start As String
        ''' <summary>
        ''' #14 is the end coordinate of hit sequence in the HSP.
        ''' </summary>
        ''' <returns></returns>
        Public Property hit_end As String

    End Class

    ''' <summary>
    ''' ``cogfun.tab``
    ''' </summary>
    Public Class cogfun
        ''' <summary>
        ''' #1 is the one-letter functional classification used in the Cluster of Orthologous Groups
        ''' (COG) database (http//www.ncbi.nlm.nih.gov/COG/).
        ''' </summary>
        ''' <returns></returns>
        Public Property COG As String
        ''' <summary>
        ''' #2 explains the one-letter functional classification.
        ''' </summary>
        ''' <returns></returns>
        Public Property description As String
    End Class

    ''' <summary>
    ''' ``cog.tab``
    ''' </summary>
    Public Class cog
        ''' <summary>
        ''' #1 is the COG ID.
        ''' </summary>
        ''' <returns></returns>
        Public Property COG As String
        ''' <summary>
        ''' #2 is the one-letter functional code.
        ''' </summary>
        ''' <returns></returns>
        Public Property code As String
        ''' <summary>
        ''' #3 is the funtion description.
        ''' </summary>
        ''' <returns></returns>
        Public Property description As String
    End Class

    ''' <summary>
    ''' ``cdd.tab``
    ''' </summary>
    Public Class cdd
        Public Property PSSM_ID As String
        Public Property CDD_PSSM_ID As String
        Public Property name As String
    End Class

    ''' <summary>
    ''' ``go.tab``
    ''' </summary>
    Public Class go
        Public Property GO_term As String
        Public Property description As String
        Public Property [namespace] As String
    End Class

    ''' <summary>
    ''' ``kegg2syno.tab``
    ''' </summary>
    Public Class kegg2syno
        Public Property KEGG_DRUG_ID As String
        Public Property synonym As String

    End Class

    ''' <summary>
    ''' ``refs.tab``
    ''' </summary>
    Public Class refs
        Public Property PubMed_ID As String
        Public Property author_last_name As String
        Public Property initials As String
        Public Property pubDate As String
        Public Property volume As String
        Public Property issue As String
        Public Property page As String
        Public Property title As String
        Public Property journal As String
    End Class

    ''' <summary>
    ''' ``taxid2genusid.tab``
    ''' </summary>
    Public Class taxid2genusid
        Public Property species As String
        Public Property genus As String
    End Class

    ''' <summary>
    ''' ``taxon_names.tab``
    ''' </summary>
    Public Class taxon_names
        ''' <summary>
        ''' #1 is the NCBI taxon id.
        ''' </summary>
        ''' <returns></returns>
        Public Property taxon_id As String
        ''' <summary>
        ''' #2 is the name of the taxon.
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
    End Class
End Namespace
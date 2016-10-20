Module CLIGrouping

    Public Const TaxonomyTools$ = "NCBI taxonomy tools"
    Public Const ExportTools$ = "NCBI data export tools"
    Public Const NTTools$ = "NCBI ``nt`` database tools"
    Public Const GITools$ = "NCBI GI tools(Obsolete from NCBI, 2016-10-20)"

    Public Const GIWasObsoleted$ =
        "> https://www.ncbi.nlm.nih.gov/news/03-02-2016-phase-out-of-GI-numbers/

###### NCBI is phasing out sequence GIs - use Accession.Version instead!

As of September 2016, the integer sequence identifiers known as ""GIs"" will no longer be included in the GenBank, GenPept, and FASTA formats supported by NCBI for sequence records. The FASTA header will be further simplified to report only the sequence accession.version and record title for accessions managed by the International Sequence Database Collaboration (INSDC) and NCBI’s Reference Sequence (RefSeq) project. As NCBI makes this transition, we encourage any users who have workflows that depend on GI's to begin planning to use accession.version identifiers instead. After September 2016, any processes solely dependent on GIs will no longer function as expected."

End Module

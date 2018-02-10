Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ContextModel.Promoter
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.NCBI
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module ModelLoader

    ''' <summary>
    ''' Load kegg organism model
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function LoadKEGGModels(repo As String) As Dictionary(Of String, OrganismModel)
        Return (ls - l - r - "*.Xml" <= repo) _
            .Select(AddressOf LoadXml(Of OrganismModel)) _
            .ToDictionary(Function(org)

                              ' 2018-2-10 因为这两个物种都具有相同的Taxonomy编号，所以在这里就不适合使用taxon_id来作为唯一标识符了
                              '
                              ' TAX: 611301
                              ' Xanthomonas citri subsp. citri mf20
                              ' TAX: 611301
                              ' Xanthomonas citri subsp. citri MN10

                              Return org.GetGenbankSource
                          End Function)
    End Function

    Public Iterator Function LoadGenomic(assembly$, kegg$) As IEnumerable(Of genomic)
        Dim models = LoadKEGGModels(repo:=kegg)

        For Each genome As (name$, org As OrganismModel) In models.EnumerateTuples
            Dim name$ = genome.name
            Dim search$ = RepositoryExtensions.GetAssemblyPath(assembly, name)
            Dim gb As GBFF.File = RepositoryExtensions.GetGenomeData(gb:=search)
            Dim nt As FastaSeq = gb.Origin.ToFasta

            Yield New genomic With {
                .nt = nt,
                .organism = genome.org,
                .context = gb.GbffToPTT(ORF:=True)
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function GetUpstreams(genome As genomic, length%) As Dictionary(Of String, FastaSeq)
        Return New PromoterRegionParser(genome.nt, PTT:=genome.context).GetRegionCollectionByLength(length)
    End Function
End Module

Public Structure genomic

    Dim nt As FastaSeq
    Dim organism As OrganismModel
    Dim context As PTT

    Public Overrides Function ToString() As String
        Return organism.ToString
    End Function
End Structure
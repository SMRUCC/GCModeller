Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Model.Network.Microbiome
Imports SMRUCC.genomics.Metagenomics
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream

Partial Module CLI

    <ExportAPI("/microbiome.metabolic.network")>
    <Usage("/microbiome.metabolic.network /metagenome <list.txt/OTU.tab> /ref <reaction.repository.XML> /uniprot <repository.XML> [/out <network.directory>]")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    Public Function MetabolicComplementationNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/metagenome"
        Dim ref As ReactionRepository = args("/ref").LoadXml(Of ReactionRepository)
        Dim UniProt As TaxonomyRepository = args("/uniprot").LoadXml(Of TaxonomyRepository)
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.network/"
        Dim list$()

        If [in].ExtensionSuffix.TextEquals("csv") Then
            list = EntityObject.LoadDataSet([in]).Keys
        Else
            list = [in].ReadAllLines
        End If

        Dim taxonomy As Taxonomy() = list _
            .Distinct _
            .Select(Function(tax) New Taxonomy(BIOMTaxonomy.TaxonomyParser(tax))) _
            .ToArray
        Dim models As IEnumerable(Of TaxonomyRef) = UniProt.PopulateModels(taxonomy, distinct:=True)
        Dim network = models.BuildMicrobiomeMetabolicNetwork(reactions:=ref)

        Return network _
            .Tabular _
            .Save(out) _
            .CLICode
    End Function

    <ExportAPI("/Metagenome.UniProt.Ref")>
    <Usage("/Metagenome.UniProt.Ref /in <uniprot.ultralarge.xml> [/out <out.XML>]")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    Public Function BuildUniProtReference(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or ([in].TrimSuffix & ".taxonomy_ref.Xml")
        Dim ref = UniProtXML.EnumerateEntries([in]).ScanUniProt

        Return ref _
            .GetXml _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/UniProt.screen.model")>
    <Usage("/UniProt.screen.model /in <model.Xml> [/coverage <default=0.6> /terms <default=1000> /out <subset.xml>]")>
    <Argument("/in", Description:="The metagenome network UniProt reference database that build from ``/Metagenome.UniProt.Ref`` command.")>
    <Group(CLIGroups.MicrobiomeNetwork_cli)>
    Public Function ScreenModels(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim coverage# = args.GetValue("/coverage", 0.6)
        Dim terms% = args.GetValue("/terms", 1000)
        Dim out$ = args("/out") Or $"{[in].TrimSuffix},coverage={coverage},terms={terms}.Xml"
        Dim models As TaxonomyRepository = [in].LoadXml(Of TaxonomyRepository)

        models.Taxonomy = models.Taxonomy _
            .Where(Function(genome)
                       Return (Not genome.organism Is Nothing) AndAlso
                                   genome.Coverage >= coverage AndAlso
                                   genome.genome.Terms.Length >= terms
                   End Function) _
            .ToArray

        Return models _
            .GetXml _
            .SaveTo(out) _
            .CLICode
    End Function
End Module
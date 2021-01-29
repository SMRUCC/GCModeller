
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("regprecise", Description:="[Regprecise database] [Collections of regulogs classified by transcription factors]",
                        Cites:="Novichkov, P. S., et al. (2013). ""RegPrecise 3.0--a resource For genome-scale exploration Of transcriptional regulation In bacteria."" BMC Genomics 14: 745.
<p>BACKGROUND: Genome-scale prediction of gene regulation and reconstruction of transcriptional regulatory networks in prokaryotes is one of the critical tasks of modern genomics. Bacteria from different taxonomic groups, whose lifestyles and natural environments are substantially different, possess highly diverged transcriptional regulatory networks. The comparative genomics approaches are useful for in silico reconstruction of bacterial regulons and networks operated by both transcription factors (TFs) and RNA regulatory elements (riboswitches). DESCRIPTION: RegPrecise (http://regprecise.lbl.gov) is a web resource for collection, visualization and analysis of transcriptional regulons reconstructed by comparative genomics. We significantly expanded a reference collection of manually curated regulons we introduced earlier. RegPrecise 3.0 provides access to inferred regulatory interactions organized by phylogenetic, structural and functional properties. Taxonomy-specific collections include 781 TF regulogs inferred in more than 160 genomes representing 14 taxonomic groups of Bacteria. TF-specific collections include regulogs for a selected subset of 40 TFs reconstructed across more than 30 taxonomic lineages. Novel collections of regulons operated by RNA regulatory elements (riboswitches) include near 400 regulogs inferred in 24 bacterial lineages. RegPrecise 3.0 provides four classifications of the reference regulons implemented as controlled vocabularies: 55 TF protein families; 43 RNA motif families; ~150 biological processes or metabolic pathways; and ~200 effectors or environmental signals. Genome-wide visualization of regulatory networks and metabolic pathways covered by the reference regulons are available for all studied genomes. A separate section of RegPrecise 3.0 contains draft regulatory networks in 640 genomes obtained by an conservative propagation of the reference regulons to closely related genomes. 
                        <p>CONCLUSIONS: RegPrecise 3.0 gives access to the transcriptional regulons reconstructed in bacterial genomes. Analytical capabilities include exploration of: regulon content, structure and function; TF binding site motifs; conservation and variations in genome-wide regulatory networks across all taxonomic groups of Bacteria. RegPrecise 3.0 was selected as a core resource on transcriptional regulation of the Department of Energy Systems Biology Knowledgebase, an emerging software and data environment designed to enable researchers to collaboratively generate, test and share new hypotheses about gene and protein functions, perform large-scale analyses, and model interactions in microbes, plants, and their communities.          ",
                        Url:="http://regprecise.lbl.gov",
                        Publisher:="PSNovichkov@lbl.gov; <br />
                        rodionov@burnham.org")>
<Cite(Title:="RegPrecise 3.0--a resource for genome-scale exploration of transcriptional regulation in bacteria",
          ISSN:="1471-2164 (Electronic);
1471-2164 (Linking)", Issue:="", Year:=2013, Journal:="BMC Genomics", Pages:="745",
          DOI:="10.1186/1471-2164-14-745",
          Abstract:="BACKGROUND: Genome-scale prediction of gene regulation and reconstruction of transcriptional regulatory networks in prokaryotes is one of the critical tasks of modern genomics. 
Bacteria from different taxonomic groups, whose lifestyles and natural environments are substantially different, possess highly diverged transcriptional regulatory networks. 
The comparative genomics approaches are useful for in silico reconstruction of bacterial regulons and networks operated by both transcription factors (TFs) and RNA regulatory elements (riboswitches). 

<p>DESCRIPTION: RegPrecise (http://regprecise.lbl.gov) is a web resource for collection, visualization and analysis of transcriptional regulons reconstructed by comparative genomics. 
We significantly expanded a reference collection of manually curated regulons we introduced earlier. RegPrecise 3.0 provides access to inferred regulatory interactions organized by phylogenetic, structural and functional properties. 
Taxonomy-specific collections include 781 TF regulogs inferred in more than 160 genomes representing 14 taxonomic groups of Bacteria. 
TF-specific collections include regulogs for a selected subset of 40 TFs reconstructed across more than 30 taxonomic lineages. Novel collections of regulons operated by RNA regulatory elements (riboswitches) include near 400 regulogs inferred in 24 bacterial lineages. 
RegPrecise 3.0 provides four classifications of the reference regulons implemented as controlled vocabularies: 55 TF protein families; 43 RNA motif families; ~150 biological processes or metabolic pathways; and ~200 effectors or environmental signals. 
Genome-wide visualization of regulatory networks and metabolic pathways covered by the reference regulons are available for all studied genomes. 
A separate section of RegPrecise 3.0 contains draft regulatory networks in 640 genomes obtained by an conservative propagation of the reference regulons to closely related genomes. 
          
<p>CONCLUSIONS: RegPrecise 3.0 gives access to the transcriptional regulons reconstructed in bacterial genomes. 
Analytical capabilities include exploration of: regulon content, structure and function; TF binding site motifs; conservation and variations in genome-wide regulatory networks across all taxonomic groups of Bacteria. 
RegPrecise 3.0 was selected as a core resource on transcriptional regulation of the Department of Energy Systems Biology Knowledgebase, an emerging software and data environment designed to enable researchers to collaboratively generate, test and share new hypotheses about gene and protein functions, perform large-scale analyses, and model interactions in microbes, plants, and their communities.",
          AuthorAddress:="Lawrence Berkeley National Laboratory, Berkeley 94710, CA, USA. PSNovichkov@lbl.gov.", PubMed:=24175918, Keywords:="Bacteria/classification/*genetics
*Databases, Genetic
Gene Regulatory Networks/genetics
*Genome, Bacterial
Internet
Metabolic Networks and Pathways/genetics
Transcription Factors/genetics
User-Computer Interface", Authors:="Novichkov, P. S.
Kazakov, A. E.
Ravcheev, D. A.
Leyn, S. A.
Kovaleva, G. Y.
Sutormin, R. A.
Kazanov, M. D.
Riehl, W.
Arkin, A. P.
Dubchak, I.
Rodionov, D. A.", Volume:=14)>
Public Module RegPrecise

    ''' <summary>
    ''' load regprecise database from a given file.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.regprecise")>
    Public Function readRegPrecise(file As String) As TranscriptionFactors
        Return file.LoadXml(Of TranscriptionFactors)
    End Function

    <ExportAPI("motif.raw")>
    Public Function exportRegPrecise(regprecise As TranscriptionFactors) As list
        Return regprecise _
            .ExportByFamily _
            .ToDictionary(Function(name) name.Key,
                          Function(family)
                              Return CObj(family.Value)
                          End Function) _
            .DoCall(Function(data)
                        Return New list With {.slots = data}
                    End Function)
    End Function

    <ExportAPI("read.regulome")>
    Public Function readRegulome(xml As String) As BacteriaRegulome
        Return xml.LoadXml(Of BacteriaRegulome)
    End Function

    <ExportAPI("regulators")>
    Public Function FromGenome(regulome As BacteriaRegulome, info As list, Optional env As Environment = Nothing) As RegulatorTable()
        Return RegulatorTable _
            .FromGenome(regulome, Function(locus_tag) info.getValue(locus_tag, env, "")) _
            .ToArray
    End Function
End Module

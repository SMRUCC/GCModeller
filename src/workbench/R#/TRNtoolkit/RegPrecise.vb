#Region "Microsoft.VisualBasic::f6291955274e322858805c3063e26d94, R#\TRNtoolkit\RegPrecise.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 150
'    Code Lines: 118 (78.67%)
' Comment Lines: 14 (9.33%)
'    - Xml Docs: 35.71%
' 
'   Blank Lines: 18 (12.00%)
'     File Size: 10.90 KB


' Module RegPrecise
' 
'     Function: exportRegPrecise, FromGenome, readMotifSites, readOperon, readRegPrecise
'               readRegulators, readRegulome, regJoin
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Development.NetCoreApp
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

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

    ''' <summary>
    ''' export the raw motif site sequence in fasta file format
    ''' </summary>
    ''' <param name="regprecise"></param>
    ''' <returns></returns>
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

    '<ExportAPI("loadScanner")>
    '<RApiReturn(GetType(RegPreciseScan))>
    'Public Function LoadScanner(<RRawVectorArgument> regDb As Object, Optional env As Environment = Nothing) As Object
    '    Dim genomes As pipeline = pipeline.TryCreatePipeline(Of BacteriaRegulome)(regDb, env)

    '    If genomes.isError Then
    '        Return genomes.getError
    '    End If

    '    Return RegPreciseScan.CreateFromRegPrecise(genomes.populates(Of BacteriaRegulome)(env))
    'End Function

    <ExportAPI("regulators")>
    Public Function FromGenome(regulome As BacteriaRegulome, info As list, Optional env As Environment = Nothing) As RegulatorTable()
        Return RegulatorTable _
            .FromGenome(regulome, Function(locus_tag) info.getValue(locus_tag, env, "")) _
            .ToArray
    End Function

    <ExportAPI("join")>
    <RApiReturn(GetType(RegpreciseBBH))>
    Public Function regJoin(<RRawVectorArgument> blast As Object, <RRawVectorArgument> regulators As Object, Optional env As Environment = Nothing) As Object
        Dim bbh As pipeline = pipeline.TryCreatePipeline(Of BiDirectionalBesthit)(blast, env, suppress:=True)
        Dim reg As pipeline = pipeline.TryCreatePipeline(Of RegulatorTable)(regulators, env)

        If reg.isError Then
            Return reg.getError
        End If

        If bbh.isError Then
            bbh = pipeline.TryCreatePipeline(Of BestHit)(blast, env)

            If bbh.isError Then
                Return bbh.getError
            End If

            Return RegpreciseBBH.JoinTable(
                sbh:=bbh.populates(Of BestHit)(env),
                regulators:=reg.populates(Of RegulatorTable)(env)
            ) _
            .ToArray
        Else
            Return RegpreciseBBH.JoinTable(
                bbh:=bbh.populates(Of BiDirectionalBesthit)(env),
                regulators:=reg.populates(Of RegulatorTable)(env)
            ) _
            .ToArray
        End If
    End Function

    <ExportAPI("read.regulators")>
    Public Function readRegulators(file As String) As RegpreciseBBH()
        Return file.LoadCsv(Of RegpreciseBBH)
    End Function

    <ExportAPI("read.operon")>
    Public Function readOperon(file As String) As Operon()
        Return RegulateGraph.ParseStream(file.LineIterators).ToArray
    End Function

    <ExportAPI("read.motifs")>
    Public Function readMotifSites(file As String) As Regulator()
        Return RegulateGraph.ParseMotifSites(file.LineIterators).ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ncbi"></param>
    ''' <param name="regprecise"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("match_taxonomy")>
    <RApiReturn(GetType(TranscriptionFactors), GetType(BacteriaRegulome))>
    Public Function match_taxonomy(ncbi As AssemblySummaryGenbank, regprecise As Object, Optional env As Environment = Nothing) As Object
        If regprecise Is Nothing Then
            Call "the given database for make matches with ncbi genbank database is nothing!".warning
            Return Nothing
        End If

        If TypeOf regprecise Is BacteriaRegulome Then
            Dim target As BacteriaRegulome = DirectCast(regprecise, BacteriaRegulome)
            Dim match = ncbi.GetBestMatch(target.genome.name, 0.85)

            If Not match Is Nothing Then
                target.genome.name = match.organism_name
                target.genome.taxonomyId = match.taxid
            Else
                Call $"the bacterial genome '{target.genome.name}' is not existsed inside ncbi genbank database!".warning
            End If

            Return target
        ElseIf TypeOf regprecise Is TranscriptionFactors Then
            Dim db As TranscriptionFactors = DirectCast(regprecise, TranscriptionFactors)
            Dim missing As New List(Of String)

            For Each target As BacteriaRegulome In TqdmWrapper.Wrap(db.AsEnumerable.ToArray)
                Dim match = ncbi.GetBestMatch(target.genome.name, 0.85)

                If Not match Is Nothing Then
                    target.genome.name = match.organism_name
                    target.genome.taxonomyId = match.taxid
                Else
                    Call missing.Add(target.genome.name)
                End If
            Next

            If missing.Any Then
                Call $"found {missing.Count} bacterial genomes is missing from the ncbi genbank database: {missing.JoinBy("; ")}".warning
            End If

            Return db
        Else
            Return Message.InCompatibleType(GetType(TranscriptionFactors), regprecise.GetType, env)
        End If
    End Function
End Module

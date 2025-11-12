#Region "Microsoft.VisualBasic::ad461e5977adb49159bbb42bbf3455fa, Bio.Repository\NCBI\FtpIndex.vb"

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

    '   Total Lines: 432
    '    Code Lines: 47 (10.88%)
    ' Comment Lines: 380 (87.96%)
    '    - Xml Docs: 95.79%
    ' 
    '   Blank Lines: 5 (1.16%)
    '     File Size: 22.12 KB


    ' Class FtpIndex
    ' 
    '     Properties: annotation_date, annotation_name, annotation_provider, asm_name, asm_not_live_date
    '                 asm_submitter, assembly_accession, assembly_level, assembly_type, bioproject
    '                 biosample, contig_count, excluded_from_refseq, ftp_path, gbrs_paired_asm
    '                 gc_percent, genome_rep, genome_size, genome_size_ungapped, group
    '                 infraspecific_name, isolate, non_coding_gene_count, organism_name, paired_asm_comp
    '                 protein_coding_gene_count, pubmed_id, refseq_category, relation_to_type_material, release_type
    '                 replicon_count, scaffold_count, seq_rel_date, species_taxid, taxid
    '                 total_gene_count, version_status, wgs_master
    ' 
    '     Function: LoadIndex
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.IO.Linq
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection

''' <summary>
''' assembly_summary_genbank.txt
''' 
''' > See ftp://ftp.ncbi.nlm.nih.gov/genomes/README_assembly_summary.txt for a description of the columns in this file.
''' </summary>
''' <remarks>
''' ################################################################################
''' README for the assembly_summary_genbank.txt, assembly_summary_refseq.txt and 
''' assembly_summary.txt files found on the NCBI genomes FTP site:
'''   https://ftp.ncbi.nlm.nih.gov/genomes 
''' 
''' Last updated: December 30, 2024
''' ################################################################################
''' 
''' ======================
''' ASSEMBLY SUMMARY FILES
''' ======================
''' 
''' The assembly_summary files report metadata for the genome assemblies on the 
''' NCBI genomes FTP site.
''' 
''' Four master files reporting data for either GenBank or RefSeq genome assemblies 
''' are available under https://ftp.ncbi.nlm.nih.gov/genomes/ASSEMBLY_REPORTS/
''' assembly_summary_genbank.txt            - current GenBank genome assemblies
''' assembly_summary_genbank_historical.txt - replaced and suppressed GenBank genome
'''                                           assemblies
''' assembly_summary_refseq.txt             - current RefSeq genome assemblies
''' assembly_summary_refseq_historical.txt  - replaced and suppressed RefSeq genome
'''                                           assemblies
''' 
''' assembly_summary_genbank.txt and assembly_summary_genbank_historical.txt are 
''' also available at:
''' https://ftp.ncbi.nlm.nih.gov/genomes/genbank/assembly_summary_genbank.txt
''' https://ftp.ncbi.nlm.nih.gov/genomes/genbank/assembly_summary_genbank_historical.txt
''' 
''' 
''' assembly_summary_refseq.txt and assembly_summary_refseq_historical.txt are 
''' also available at:
''' https://ftp.ncbi.nlm.nih.gov/genomes/refseq/assembly_summary_refseq.txt
''' https://ftp.ncbi.nlm.nih.gov/genomes/refseq/assembly_summary_refseq_historical.txt
''' 
''' The assembly_summary.txt files in the directories named for taxonomic groups or
''' species contain the relevant subsets of the data from the master files.
''' 
''' =====================================
''' HOW TO USE THE ASSEMBLY SUMMARY FILES
''' =====================================
''' 
''' The metadata provided in the assembly_summary.txt files can be used to identify
''' assemblies of interest for subsequent download. 
''' 
''' The Genomes FTP FAQ provides examples of how to use the assembly_summary.txt 
''' files to download sets of assemblies. See: 
''' 
''' How can I download only the current version of each assembly?
''' https://www.ncbi.nlm.nih.gov/genome/doc/ftpfaq/#current
''' 
''' How can I download RefSeq data for all complete bacterial genomes?
''' https://www.ncbi.nlm.nih.gov/genome/doc/ftpfaq/#allcomplete
''' 
''' Other sets of assemblies of interest can be downloaded using variations on 
''' these instructions.
''' 
''' ________________________________________________________________________________
''' National Center for Biotechnology Information (NCBI)
''' National Library of Medicine
''' National Institutes of Health
''' 8600 Rockville Pike
''' Bethesda, MD 20894, USA
''' tel: (301) 496-2475
''' fax: (301) 480-9241
''' e-mail: info@ncbi.nlm.nih.gov
''' ________________________________________________________________________________
''' </remarks>
Public Class GenBankAssemblyIndex

    ''' <summary>
    ''' Assembly accession: the assembly accession.version reported in this field is 
    '''    a unique identifier for the set of sequences in this particular version of 
    '''    the genome assembly.
    ''' </summary>
    ''' <returns></returns>
    <Column("#assembly_accession")> Public Property assembly_accession As String

    ''' <summary>
    ''' BioProject: accession for the BioProject which produced the sequences in the 
    '''    genome assembly. A BioProject is a collection of biological data related to a
    '''    single initiative, originating from a single organization or from a 
    '''    consortium. A BioProject record provides users a single place to find links 
    '''    to the diverse data types generated for that project. The record can be 
    '''    retrieved from the NCBI BioProject resource:
    '''    https://www.ncbi.nlm.nih.gov/bioproject/
    ''' </summary>
    ''' <returns></returns>
    Public Property bioproject As String
    ''' <summary>
    ''' BioSample: accession for the BioSample from which the sequences in the genome
    '''    assembly were obtained. A BioSample record contains a description of the 
    '''    biological source material used in experimental assays. The record can be 
    '''    retrieved from the NCBI BioSample resource:
    '''    https://www.ncbi.nlm.nih.gov/biosample/
    ''' </summary>
    ''' <returns></returns>
    Public Property biosample As String
    ''' <summary>
    ''' WGS-master: the GenBank Nucleotide accession and version for the master 
    '''    record of the Whole Genome Shotgun (WGS) project for the genome assembly. The
    '''    master record can be retrieved from the NCBI Nucleotide resource: 
    '''    https://www.ncbi.nlm.nih.gov/nuccore
    '''    Genome assemblies that are complete genomes, and those that are clone-based,
    '''    do not have WGS-master records in which case this field will be empty.
    ''' </summary>
    ''' <returns></returns>
    Public Property wgs_master As String
    ''' <summary>
    ''' RefSeq Category: whether the assembly is a reference genome in the NCBI
    '''    Reference Sequence (RefSeq) project classification. 
    '''    Values:
    '''            reference genome - a genome computationally or manually selected
    '''                               as a reference for the species. 
    '''            na               - no RefSeq category assigned to this assembly
    '''    Details of the selection of reference are at: 
    '''    https://www.ncbi.nlm.nih.gov/datasets/docs/v2/policies-annotation/genome-processing/reference-selection/
    ''' </summary>
    ''' <returns></returns>
    Public Property refseq_category As String
    ''' <summary>
    ''' Taxonomy ID: the NCBI taxonomy identifier for the organism from which the 
    '''    genome assembly was derived. The NCBI Taxonomy Database is a curated 
    '''    classification and nomenclature for all of the organisms in the public 
    '''    sequence databases. The taxonomy record can be retrieved from the NCBI 
    '''    Taxonomy resource:
    '''    https://www.ncbi.nlm.nih.gov/taxonomy/
    ''' </summary>
    ''' <returns></returns>
    Public Property taxid As String
    ''' <summary>
    ''' Species taxonomy ID: the NCBI taxonomy identifier for the species from which 
    '''    the genome assembly was derived. The species taxid will differ from the 
    '''    organism taxid (column 6) only when the organism was reported at a sub-
    '''    species or strain level.
    ''' </summary>
    ''' <returns></returns>
    Public Property species_taxid As String
    ''' <summary>
    ''' Organism name: the scientific name of the organism from which the sequences 
    '''    in the genome assembly were derived. This name is taken from the NCBI 
    '''    Taxonomy record for the taxid specified in column 6. Some older taxids were 
    '''    assigned at the strain level and for these the organism name will include the
    '''    strain. Current practice is only to assign taxids at the species level; for 
    '''    these the organism name will be just the species, however, the strain name 
    '''    will be reported in the infraspecific_name field (column 9).
    ''' </summary>
    ''' <returns></returns>
    Public Property organism_name As String
    ''' <summary>
    ''' Infraspecific name: the strain, breed, cultivar or ecotype of the organism 
    '''    from which the sequences in the genome assembly were derived. Data are 
    '''    reported in the form tag=value, e.g. strain=AF16. Strain, breed, cultivar 
    '''    and ecotype are not expected to be used together, however, if they are then 
    '''    they will be reported in a list separated by ", /". Empty if no strain, 
    '''    breed, cultivar or ecotype is specified on the genomic sequence records.
    ''' </summary>
    ''' <returns></returns>
    Public Property infraspecific_name As String
    ''' <summary>
    ''' Isolate: the individual isolate from which the sequences in the genome 
    '''    assembly were derived. Empty if no isolate is specified on the genomic 
    '''    sequence records.
    ''' </summary>
    ''' <returns></returns>
    Public Property isolate As String
    ''' <summary>
    ''' Version status: the release status for the genome assembly version.
    '''    Values:
    '''            latest     - the most recent of all the versions for this assembly 
    '''                         chain
    '''            replaced   - this version has been replaced by a newer version of the
    '''                         assembly in the same chain
    '''            suppressed - this version of the assembly has been suppressed 
    '''    An assembly chain is the collection of all versions for the same assembly 
    '''    accession.
    ''' </summary>
    ''' <returns></returns>
    Public Property version_status As String
    ''' <summary>
    ''' Assembly level: the highest level of assembly for any object in the genome 
    '''    assembly.
    '''    Values:
    '''       Complete genome - all chromosomes are gapless and have no runs of 10 or 
    '''                         more ambiguous bases (Ns), there are no unplaced or 
    '''                         unlocalized scaffolds, and all the expected chromosomes
    '''                         are present (i.e. the assembly is not noted as having 
    '''                         partial genome representation). Plasmids and organelles
    '''                         may or may not be included in the assembly but if 
    '''                         present then the sequences are gapless.
    '''       Chromosome      - there is sequence for one or more chromosomes. This 
    '''                         could be a completely sequenced chromosome without gaps
    '''                         or a chromosome containing scaffolds or contigs with 
    '''                         gaps between them. There may also be unplaced or 
    '''                         unlocalized scaffolds.
    '''       Scaffold        - some sequence contigs have been connected across gaps to
    '''                         create scaffolds, but the scaffolds are all unplaced or 
    '''                         unlocalized.
    '''       Contig          - nothing is assembled beyond the level of sequence 
    '''                         contigs
    ''' </summary>
    ''' <returns></returns>
    Public Property assembly_level As String
    ''' <summary>
    ''' Release type: whether this version of the genome assembly is a major, minor 
    '''    or patch release.
    '''    Values:
    '''            Major - changes from the previous assembly version result in a 
    '''                    significant change to the coordinate system. The first 
    '''                    version of an assembly is always a major release. Most 
    '''                    subsequent genome assembly updates are also major releases.
    '''            Minor - changes from the previous assembly version are limited to the
    '''                    following changes, none of which result in a significant 
    '''                    change to the coordinate system of the primary assembly-unit:
    '''                    - adding, removing or changing a non-nuclear assembly-unit
    '''                    - dropping unplaced or unlocalized scaffolds
    '''                    - adding up to 50 unplaced or unlocalized scaffolds which are
    '''                      shorter than the current scaffold-N50 value
    '''                    - replacing a component with a gap of the same length
    '''            Patch - the only change is the addition or modification of a patch 
    '''                    assembly-unit. 
    '''    See the NCBI Assembly model web page (https://www.ncbi.nlm.nih.gov/assembly/
    '''    model/#asmb_def) for definitions of assembly-units and genome patches.
    ''' </summary>
    ''' <returns></returns>
    Public Property release_type As String
    ''' <summary>
    ''' Genome representation: whether the goal for the assembly was to represent the
    '''    whole genome or only part of it.
    '''    Values:
    '''       Full    - the data used to generate the assembly was obtained from the 
    '''                 whole genome, as in Whole Genome Shotgun (WGS) assemblies for 
    '''                 example. There may still be gaps in the assembly.
    '''       Partial - the data used to generate the assembly came from only part of 
    '''                 the genome. 
    '''    Most assemblies have full genome representation with a minority being partial
    '''    genome representation. See the Assembly help web page 
    '''    (https://www.ncbi.nlm.nih.gov/assembly/help/) for reasons that the genome 
    '''    representation would be set to partial.
    ''' </summary>
    ''' <returns></returns>
    Public Property genome_rep As String
    ''' <summary>
    ''' Sequence release date: the date the sequences in the genome assembly were 
    '''    released in the International Nucleotide Sequence Database Collaboration 
    '''    (INSDC) databases, i.e. DDBJ, ENA or GenBank.
    ''' </summary>
    ''' <returns></returns>
    Public Property seq_rel_date As String
    ''' <summary>
    ''' Assembly name: the submitter's name for the genome assembly, when one was 
    '''    provided, otherwise a default name, in the form ASM#####v#, is provided by 
    '''    NCBI. Assembly names are not unique.
    ''' </summary>
    ''' <returns></returns>
    Public Property asm_name As String
    ''' <summary>
    ''' Assembly submitter: the submitting consortium or first position if a list of
    '''    organizations. The full submitter information is available in the NCBI 
    '''    BioProject resource: www.ncbi.nlm.nih.gov/bioproject/
    ''' </summary>
    ''' <returns></returns>
    Public Property asm_submitter As String
    ''' <summary>
    ''' GenBank/RefSeq paired assembly: the accession.version of the GenBank assembly
    '''    that is paired to the given RefSeq assembly, or vice-versa. "na" is reported 
    '''    if the assembly is unpaired.
    ''' </summary>
    ''' <returns></returns>
    Public Property gbrs_paired_asm As String
    ''' <summary>
    ''' Paired assembly comparison: whether the paired GenBank &amp; RefSeq assemblies 
    '''    are identical or different.
    '''    Values:
    '''       identical - GenBank and RefSeq assemblies are identical
    '''       different - GenBank and RefSeq assemblies are not identical
    '''       na        - not applicable since the assembly is unpaired
    ''' </summary>
    ''' <returns></returns>
    Public Property paired_asm_comp As String
    ''' <summary>
    ''' FTP path: the path to the directory on the NCBI genomes FTP site from which 
    '''    data for this genome assembly can be downloaded.
    ''' </summary>
    ''' <returns></returns>
    Public Property ftp_path As String
    ''' <summary>
    ''' Excluded from RefSeq: reasons the assembly was excluded from the NCBI 
    '''    Reference Sequence (RefSeq) project, including any assembly anomalies. See:
    '''    https://www.ncbi.nlm.nih.gov/assembly/help/anomnotrefseq/
    ''' </summary>
    ''' <returns></returns>
    Public Property excluded_from_refseq As String
    ''' <summary>
    ''' Relation to type material: contains a value if the sequences in the genome 
    '''    assembly were derived from type material.
    '''    Values:
    '''       assembly from type material - the sequences in the genome assembly were 
    '''          derived from type material
    '''       assembly from synonym type material - the sequences in the genome assembly
    '''          were derived from synonym type material
    '''       assembly from pathotype material - the sequences in the genome assembly
    '''          were derived from pathovar material
    '''       assembly designated as neotype - the sequences in the genome assembly 
    '''          were derived from neotype material
    '''       assembly designated as reftype - the sequences in the genome assembly 
    '''          were derived from reference material where type material never was 
    '''          available and is not likely to ever be available
    '''       ICTV species exemplar - the International Committee on Taxonomy of Viruses
    '''          (ICTV) designated the genome assembly as the exemplar for the virus 
    '''          species
    '''       ICTV additional isolate - the International Committee on Taxonomy of 
    '''          Viruses (ICTV) designated the genome assembly an additional isolate for
    '''          the virus species
    ''' </summary>
    ''' <returns></returns>
    Public Property relation_to_type_material As String
    ''' <summary>
    ''' Assembly no longer live date: the date the assembly transitioned from 
    ''' 	version_status latest to either replaced or suppressed. When the assembly is
    ''' 	in status latest, "na" is reported. 
    ''' </summary>
    ''' <returns></returns>
    Public Property asm_not_live_date As String
    ''' <summary>
    ''' Assembly type: the type of assembly, including haploid, haploid-with-alt,
    ''' 	diploid, unresolved-diploid, alternate-pseudohaplotype
    ''' </summary>
    ''' <returns></returns>
    Public Property assembly_type As String
    ''' <summary>
    ''' Taxonomy Group: commonly used organism groups: 
    ''' 		Eukaryota: Animals, Fungi, Plants, Protists;                           
    '''         Prokaryota: group corresponds to phylum; 
    '''         Viruses: groups defined as the first level (not ranked)                        
    '''                      below the kingdom of Viruses
    ''' </summary>
    ''' <returns></returns>
    Public Property group As String
    ''' <summary>
    ''' Genome Size: total length of all top-level sequences in the primary 
    ''' 	assembly
    ''' </summary>
    ''' <returns></returns>
    Public Property genome_size As String
    ''' <summary>
    ''' Genome Size - Ungapped length: total length of all top-level sequences 
    ''' 	in the primary assembly ignoring gaps. Any stretch of 10 or more Ns in a
    ''' 	sequence is treated like a gap
    ''' </summary>
    ''' <returns></returns>
    Public Property genome_size_ungapped As String
    ''' <summary>
    ''' GC%: Percent of nitrogenous bases (guanine or cytosine) in DNA submitted
    ''' 	for the assembly, rounded to the nearest 0.5%
    ''' </summary>
    ''' <returns></returns>
    Public Property gc_percent As String
    ''' <summary>
    ''' Replicon count:  total number of chromosomes, organelle genomes, and
    ''' 	plasmids in the primary assembly
    ''' </summary>
    ''' <returns></returns>
    Public Property replicon_count As String
    ''' <summary>
    ''' Scaffold count: number of scaffolds including placed, unlocalized, 
    ''' 	unplaced, alternate loci and patch scaffolds in the primary assembly
    ''' </summary>
    ''' <returns></returns>
    Public Property scaffold_count As String
    ''' <summary>
    ''' Contig count: number of contigs in the primary assembly
    ''' </summary>
    ''' <returns></returns>
    Public Property contig_count As String
    ''' <summary>
    ''' Annotation provider: the group that provided the annotation on the assembly
    ''' </summary>
    ''' <returns></returns>
    Public Property annotation_provider As String
    ''' <summary>
    ''' Annotation name: the name of the annotation that is on the assembly
    ''' </summary>
    ''' <returns></returns>
    Public Property annotation_name As String
    ''' <summary>
    ''' Annotation date: the date that the assembly was annotated 
    ''' </summary>
    ''' <returns></returns>
    Public Property annotation_date As String
    ''' <summary>
    ''' Total gene count: the total number of genes that are annotated on the
    ''' 	assembly. If an assembly has no genes, or is lacking a gene count, it will 
    ''' 	display "na". If an assembly has no features but is annotated, then it will
    ''' 	show 0. 
    ''' </summary>
    ''' <returns></returns>
    Public Property total_gene_count As String
    ''' <summary>
    ''' Protein coding gene count: the total number of protein coding genes on the
    ''' 	assembly
    ''' </summary>
    ''' <returns></returns>
    Public Property protein_coding_gene_count As String
    ''' <summary>
    ''' Non-coding gene count: the total number of non-coding genes on the assembly
    ''' </summary>
    ''' <returns></returns>
    Public Property non_coding_gene_count As String
    ''' <summary>
    ''' PubMed ID: The PubMed ID(s) that are associated with the assembly, 
    ''' 	separated by commas 
    ''' </summary>
    ''' <returns></returns>
    <Collection("pubmed_id", ";")> Public Property pubmed_id As String()

    Public Shared Function LoadIndex(file As String) As IEnumerable(Of GenBankAssemblyIndex)
        Dim loader As New DataStream(file, trim:=True, skip:=1, tsv:=True)
        Dim refs As IEnumerable(Of GenBankAssemblyIndex) = loader.AsLinq(Of GenBankAssemblyIndex)(, silent:=True)
        Return refs
    End Function

End Class

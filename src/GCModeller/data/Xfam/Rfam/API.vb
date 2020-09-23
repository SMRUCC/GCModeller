#Region "Microsoft.VisualBasic::26d698bea20473b11e57e9b359733706, data\Xfam\Rfam\API.vb"

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

    ' Module API
    ' 
    '     Function: __assignPrefix, __createObject, LoadRfam, ReadDb, Rfam2Feature
    '               (+3 Overloads) RfamAnalysis, RfamAnalysisBatch, RfamToGBK
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.SequenceModel

''' <summary>
''' Rfam is a database of structure-annotated multiple sequence alignments,
''' convariance models And family annotation For a number Of non-coding RNA,
''' cis-regulatory And self - splicing intron families. Rfam 12.0 contains 2450
''' families.The seed alignments are hand curated And aligned using available
''' sequence And Structure data, And covariance models are built from these
''' alignments Using the INFERNAL 1.1 software suite (http://infernal.janelia.org).
''' The full regions list Is created by searching the RFAMSEQ database (described
''' below) Using the covariance model, And Then listing all hits above a family
''' specific threshold To the model. Rfam 12.0 annotates 19,623,515 regions In the
''' RFAMSEQ database.
''' 
''' Rfam Is maintained by a consortium Of researchers at the EMBL European
''' Bioinformatics Institute, Hinxton, UK And the Howard Huges Medical Institute,
''' Janelia Farm Research Campus, Ashburn, Virginia, USA. We are very keen To hear
''' any feedback, positive Or negative, that you may have On Rfam - please contact
''' rfam-help@ebi.ac.uk.
''' 
''' Rfam Is freely available And In the Public domain under the Creative Commons
''' Zero licence. See ftp://ftp.ebi.ac.uk/pub/databases/Rfam/CURRENT/COPYING for
''' more information.
''' </summary>
<Package("Sanger.Rfam", Category:=APICategories.ResearchTools,
                  Description:="Rfam: Multiple alignments, secondary structures and covariance models of non-coding RNA families
                  Rfam is a database of structure-annotated multiple sequence alignments,
convariance models and family annotation for a number of non-coding RNA,
cis-regulatory and self-splicing intron families. Rfam 12.0 contains 2450
families. The seed alignments are hand curated and aligned using available
sequence and structure data, and covariance models are built from these
alignments using the INFERNAL 1.1 software suite (http://infernal.janelia.org).
The full regions list is created by searching the RFAMSEQ database (described
below) using the covariance model, and then listing all hits above a family
specific threshold to the model. Rfam 12.0 annotates 19,623,515 regions in the
RFAMSEQ database.

Rfam is maintained by a consortium of researchers at the EMBL European
Bioinformatics Institute, Hinxton, UK and the Howard Huges Medical Institute,
Janelia Farm Research Campus, Ashburn, Virginia, USA. We are very keen to hear
any feedback, positive or negative, that you may have on Rfam - please contact
rfam-help@ebi.ac.uk.

Rfam is freely available and in the public domain under the Creative Commons
Zero licence. See ftp://ftp.ebi.ac.uk/pub/databases/Rfam/CURRENT/COPYING for
more information.",
                  Publisher:="",
                  Url:="http://rfam.xfam.org",
                  Cites:="<li>Burge SW, Daub J, Eberhardt R, Tate JG, Barquist L, Nawrocki E, Eddy S, Gardner
PP, Bateman A
Rfam 11.0: 10 years of RNA families.
Nucleic Acids Res. 2012 Nov;</li>

<li>Gardner PP, Daub J, Tate JG, Nawrocki EP, Kolbe DL, Lindgreen S,
Wilkinson AC, Finn RD, Griffiths-Jones S, Eddy SR, Bateman A
Rfam: updates to the RNA families database.
Nucleic Acids Res. 2008 Oct;</li>

<li>Daub J, Gardner PP, Tate J, Ramsköld D, Manske M, Scott WG,
Weinberg Z, Griffiths-Jones S, Bateman A
The RNA WikiProject: Community annotation of RNA families.
RNA. 2008 Dec; 14:(12)2462-2464</li>

<li>Rfam: annotating non-coding RNAs in complete genomes
Sam Griffiths-Jones, Simon Moxon, Mhairi Marshall, Ajay Khanna,
Sean R. Eddy and Alex Bateman
Nucleic Acids Res. 2005 33:D121-D124</li>

<li>Rfam: an RNA family database.
Sam Griffiths-Jones, Alex Bateman, Mhairi Marshall, Ajay Khanna
and Sean R. Eddy.
Nucleic Acids Res. 2003 31:439-441</li>")>
<Cite("%A Griffiths-Jones, Sam
%A Bateman, Alex
%A Marshall, Mhairi
%A Khanna, Ajay
%A Eddy, Sean R.
%T Rfam: an RNA family database
%0 Journal Article
%D 2003 
%8 January 1, 2003 
%J Nucleic Acids Research 
%P 439-441 
%R 10.1093/nar/gkg006 
%V 31 
%N 1 
%U http://nar.oxfordjournals.org/content/31/1/439.abstract 
%X Rfam is a collection of multiple sequence alignments and covariance models representing non-coding RNA families. Rfam is available on the web in the UK at http://www.sanger.ac.uk/Software/Rfam/ and in the US at http://rfam.wustl.edu/. These websites allow the user to search a query sequence against a library of covariance models, and view multiple sequence alignments and family annotation. The database can also be downloaded in flatfile form and searched locally using the INFERNAL package (http://infernal.wustl.edu/). The first release of Rfam (1.0) contains 25 families, which annotate over 50 000 non-coding RNA genes in the taxonomic divisions of the EMBL nucleotide database. ",
      URL:="", AuthorAddress:="HHMI Janelia Farm Research Campus, 19700 Helix Drive, Ashburn, VA 20147 USA, European Molecular Biology Laboratory, European Bioinformatics Institute (EMBL-EBI), Wellcome Trust Genome Campus, Hinxton, Cambridge CB10 1SD, UK, Wellcome Trust Sanger Institute, Wellcome Trust Genome Campus, Hinxton, Cambridge CB10 1SA, UK, MRC Functional Genomics Unit, Department of Physiology, Anatomy and Genetics, University of Oxford, Oxford, OX1 3QX, UK, Institute of Biotechnology and Department of Biological and Environmental Sciences, University of Helsinki, PO Box 56 (Viikinkaari 5), 00014 Helsinki, Finland and Stockholm Bioinformatics Center, Swedish eScience Research Center, Department of Biochemistry and Biophysics, Science for Life Laboratory, Stockholm University, PO Box 1031, SE-17121 Solna, Sweden.",
      PubMed:=24288371,
      Keywords:="*Databases, Protein
Internet
Intrinsically Disordered Proteins/chemistry
Protein Conformation
Proteins/chemistry/classification/genetics
Proteome/chemistry
*Sequence Alignment
Sequence Analysis, DNA
*Sequence Analysis, Protein")>
Public Module API

    <ExportAPI("Rfam.Read")>
    Public Function LoadRfam(Db As String) As Dictionary(Of String, Stockholm)
        Dim buffer As List(Of Stockholm) = Db.LoadCsv(Of Stockholm)
        Return buffer.ToDictionary(Function(x) x.AC)
    End Function

    <ExportAPI("Read.Rfam.Seed")>
    Public Function ReadDb(path As String) As Dictionary(Of String, Stockholm)
        Return Stockholm.DatabaseParser(path)
    End Function

    <ExportAPI("Rfam")>
    Public Function RfamAnalysis(Rfam As Stockholm,
                                 blastn As IEnumerable(Of BlastnMapping),
                                 PTT As PTT,
                                 reader As IPolymerSequenceModel,
                                 <Parameter("Source.Directed?")>
                                 Optional sourceDirect As Boolean = True) As Rfamily()
        Dim lstData As Rfamily() = blastn.Select(Function(x) __createObject(Rfam, x, PTT, reader, sourceDirect))
        Return lstData
    End Function

    Private Function __createObject(Rfam As Stockholm, blastn As BlastnMapping, PTT As PTT, reader As IPolymerSequenceModel, sourceDirect As Boolean) As Rfamily
        Dim result As New Rfamily With {
            .Evalue = blastn.Evalue,
            .Hit = blastn.ReadQuery.Split.First,
            .Identities = blastn.identitiesValue,
            .Left = blastn.ReferenceLeft,
            .Right = blastn.ReferenceRight,
            .Strand = blastn.ReferenceStrand.GetBriefCode,
            .Name = Rfam.ID,
            .Rfam = Rfam.AC
        }
        Dim related = PTT.GetRelatedGenes(result.MappingLocation)

        result.SequenceData = reader.CutSequenceLinear(result.MappingLocation).SequenceData
        result.Location = related.Select(Function(x) x.ToString).JoinBy("; ")
        result.Relates = related.Select(Function(g) g.Gene.Synonym)
        result.rLociStrand = New String(related.Select(Function(g) g.Gene.Location.Strand.GetBriefCode.First))

        If String.IsNullOrEmpty(result.Location) Then
            result.Location = "Intergenic Region"
        End If

        Return result
    End Function

    <ExportAPI("Rfam")>
    Public Function RfamAnalysis(blastn As String,
                                 Rfam As Dictionary(Of String, Stockholm),
                                 PTT As PTT,
                                 reader As IPolymerSequenceModel,
                                 <Parameter("Source.Directed?")> Optional sourceDirect As Boolean = True) As Rfamily()
        Dim sId As String = BaseName(blastn)
        Dim RfamAnno As Stockholm = Rfam(sId)
        Dim blastnHits = blastn.LoadCsv(Of BlastnMapping)
        Return RfamAnalysis(RfamAnno, blastnHits, PTT, reader, sourceDirect)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="blastn"></param>
    ''' <param name="Rfam"></param>
    ''' <param name="PTT_DIR"></param>
    ''' <param name="locusPrefix"></param>
    ''' <param name="offset">用于合并位点的偏移量</param>
    ''' <returns></returns>
    <ExportAPI("Rfam.Batch")>
    Public Function RfamAnalysisBatch(<Parameter("blastn.DIR", "DIR which contains the alignment export of file blastnMappings.Csv for each Rfam.fasta.")>
                                      blastn As String,
                                      Rfam As String,
                                      <Parameter("PTT.DIR")> PTT_DIR As String,
                                      Optional locusPrefix As String = "",
                                      Optional offset As Integer = 3,
                                      <Parameter("Source.Directed?")> Optional sourceDirect As Boolean = True) As Rfamily()
        Dim lstMappings As String() = FileIO.FileSystem.GetFiles(blastn, FileIO.SearchOption.SearchTopLevelOnly, "*.csv").ToArray
        Dim RfamDb = API.LoadRfam(Rfam)
        Dim Genbank As New PTTDbLoader(PTT_DIR)
        Dim reader = Genbank.GenomeFasta
        Dim allGenes As New PTT(Genbank.ORF_PTT.GeneObjects.Join(Genbank.RNARnt.GeneObjects))
        Dim LQuery = (From file As String In lstMappings.AsParallel
                      Select RfamAnalysis(file, RfamDb, allGenes, reader, sourceDirect)).ToArray
        Dim result As Rfamily() = LQuery.ToVector
        Call $"Start to assign locusId for {result.Length} predicted RNA sites...".__DEBUG_ECHO
        result = __assignPrefix(locusPrefix, allGenes, result, offset)
        Return result
    End Function

    Private Function __assignPrefix(prefix As String, PTT As PTT, data As Rfamily(), offset As Integer) As Rfamily()
        If String.IsNullOrEmpty(prefix) Then
            Dim allPrefix As String = (From pid In (From pf As String
                                                    In PTT.GeneObjects.Select(Function(g) Regex.Replace(g.Synonym, "\d+", ""))
                                                    Select pf
                                                    Group pf By pf Into Count)
                                       Select pid
                                       Order By pid.Count Descending).First.pf
            prefix = allPrefix
        End If

        Call $"Grouping loci data...".__DEBUG_ECHO

        Dim groups = data.Group(offset)
        Dim LQuery = (From gr In groups Select gr Order By gr.Value.First.Left Ascending).ToArray
        Dim idx As i32 = 1

        Call $"Assigning locusId....".__DEBUG_ECHO

        For Each gr In LQuery
            Dim locusId As String = $"{prefix}{STDIO.ZeroFill(++idx, 4)}"
            For Each RNA In gr.Value  ' 同一个位点比对上了数据库之中的不同记录，都分配相同的id编号
                RNA.LocusId = locusId
            Next
        Next

        data = (From x In data Select x Order By x.LocusId Ascending).ToArray
        Call "[Job DONE!]".__DEBUG_ECHO

        Return data
    End Function

    <ExportAPI("Rfam")>
    Public Function RfamAnalysis(blastn As String, Rfam As String, <Parameter("PTT.DIR")> PTT_DIR As String) As Rfamily()
        Dim RfamDb = API.LoadRfam(Rfam)
        Dim Genbank As New PTTDbLoader(PTT_DIR)
        Dim allGenes As New PTT(Genbank.ORF_PTT.GeneObjects.Join(Genbank.RNARnt.GeneObjects))
        Return RfamAnalysis(blastn, RfamDb, allGenes, Genbank.GenomeFasta)
    End Function

    <ExportAPI("Rfam2GBK")>
    Public Function RfamToGBK(Rfam As IEnumerable(Of Rfamily), copyFrom As GBFF.File) As GBFF.File
        Dim gb As New GBFF.File
        gb.Accession = copyFrom.Accession
        gb.Comment = New GBFF.Keywords.COMMENT With {
            .Comment = "RNA genes predicted from genome " & copyFrom.Definition.Value
        }
        gb.Definition = New GBFF.Keywords.DEFINITION With {.Value = gb.Comment.Comment}
        gb.Keywords = New GBFF.Keywords.KEYWORDS With {.KeyWordList = New List(Of String) From {"Rfam"}}
        gb.Version = New GBFF.Keywords.VERSION With {.Ver = "1.0"}
        ' gb.Features = New Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.FEATURES With {.SourceFeature }
    End Function

    <ExportAPI("Rfam2Feature")>
    <Extension>
    Public Function Rfam2Feature(rfam As Rfamily) As Feature

    End Function
End Module

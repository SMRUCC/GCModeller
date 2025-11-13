#Region "Microsoft.VisualBasic::5981bcb0c0637e9720f0c36ab9e1ead5, data\RegulonDatabase\Regprecise\RegpreciseAPI.vb"

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

    '   Total Lines: 456
    '    Code Lines: 336 (73.68%)
    ' Comment Lines: 58 (12.72%)
    '    - Xml Docs: 67.24%
    ' 
    '   Blank Lines: 62 (13.60%)
    '     File Size: 28.17 KB


    '     Module RegpreciseAPI
    ' 
    '         Function: __exportMotif, __exportMotifs, __getFastaCollection, __matches, (+2 Overloads) Compile
    '                   Distinct, Download, DownloadRegulatorSequence, ExportByFamily, ExportBySpecies
    '                   FamilyStatics, FamilyStatics2, GenerateDatabase, GenerateFastaData, GetTfFamilies
    '                   InsertRegulatoryRecord, LoadRegulationDb, ReadCsv, ReadXml, ReGenerate
    '                   RegpreciseRegulatorMatch, SaveGenomes, WriteMatches, WriteRegprecise
    ' 
    '         Sub: __mergeAction
    '         Interface IRegulatorMatched
    ' 
    '             Properties: Address, Family, locusId
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.Extensions
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Regprecise

    <Package("Regprecise", Description:="[Regprecise database] [Collections of regulogs classified by transcription factors]",
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
    Public Module RegpreciseAPI

        <ExportAPI("Write.Xml.Genomes")>
        Public Function SaveGenomes(genomes As IEnumerable(Of JSON.genome), SaveTo As String) As Boolean
            Return genomes.ToArray.GetXml.SaveTo(SaveTo)
        End Function

        ''' <summary>
        ''' Download the whole regprecise database for each bacteria genome.
        ''' </summary>
        ''' <param name="outDIR"></param>
        ''' <returns></returns>
        Public Function Download(<Parameter("Export.DIR", "Directory for save the temp data.")> Optional outDIR As String = "") As TranscriptionFactors
            If String.IsNullOrEmpty(outDIR) Then
                outDIR = TempFileSystem.TempDir
            End If

            Dim Regprecise As TranscriptionFactors = WebAPI.Download(outDIR)
            Return Regprecise
        End Function

        <ExportAPI("Db.CompileGeneration")>
        Public Function GenerateDatabase(DIR As String) As TranscriptionFactors
            Dim LQuery = (From File As String In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchAllSubDirectories, "*.xml").AsParallel
                          Let Bacteria As Regprecise.BacteriaRegulome = Distinct(File.LoadXml(Of Regprecise.BacteriaRegulome)())
                          Select Bacteria
                          Order By Bacteria.genome.name Ascending).ToArray
            Return New TranscriptionFactors With {
                .genomes = LQuery,
                .update = Now.ToString
            }
        End Function

        Public Function Distinct(data As BacteriaRegulome) As BacteriaRegulome
            Dim Regulators = (From reg As Regulator
                              In data.regulome.regulators
                              Select reg.locus_tag.name
                              Distinct).ToArray
            If Regulators.Length = data.numOfRegulons Then
                Return data       '没有重复的数据，则直接返回
            End If

            Dim DistinctedRegulators = (From sId As String
                                        In Regulators
                                        Select RegulatorId = sId,
                                            ddata = (From reg As Regulator In data.regulome.regulators
                                                     Where String.Equals(reg.locus_tag.name, sId)
                                                     Select reg).ToArray).ToArray
            Dim LQuery = (From Line In DistinctedRegulators
                          Let Sites = (From item In Line.ddata Select item.regulatorySites).ToVector
                          Let DistinctedSites = (From SiteId As String In (From item In Sites Select item.UniqueId Distinct).ToArray Let site = Sites.GetItem(SiteId) Select site).ToArray
                          Select Regulator = Line.ddata.First,
                              DistinctedSites).ToArray
            For i As Integer = 0 To LQuery.Length - 1
                Dim Regulator = LQuery(i)
                Regulator.Regulator.regulatorySites = Regulator.DistinctedSites
            Next
            data.regulome.regulators = (From item In LQuery Select item.Regulator).ToArray
            Return data
        End Function

        ''' <summary>
        ''' Download regprecise regulator protein sequence from kegg database.
        ''' </summary>
        ''' <param name="Regprecise"></param>
        ''' <param name="EXPORT"></param>
        ''' <returns></returns>
        Public Function DownloadRegulatorSequence(Regprecise As TranscriptionFactors, Optional EXPORT As String = "") As FASTA.FastaFile
            If String.IsNullOrEmpty(EXPORT) Then
                EXPORT = TempFileSystem.TempDir
            End If

            Return WebAPI.DownloadRegulatorSequence(Regprecise, EXPORT)
        End Function

        <ExportAPI("Write.Xml.Regprecise")>
        Public Function WriteRegprecise(Regprecise As TranscriptionFactors, saveXml As String) As Boolean
            Return Regprecise.GetXml.SaveTo(saveXml)
        End Function

        <ExportAPI("add.regulatory_data")>
        Public Function InsertRegulatoryRecord(Regprecise As TranscriptionFactors,
                                               Family As String,
                                               Bacteria As String,
                                               Regulator As String,
                                               <Parameter("Regulatory.Sites")> RegulatorySites As String) As Boolean
            Return Regprecise.InsertRegulog(Family, Bacteria, FASTA.FastaFile.Read(RegulatorySites), Regulator)
        End Function

        Public Interface IRegulatorMatched
            Property Address As String
            Property locusId As String
            Property Family As String
        End Interface

        <ExportAPI("regprecise.matches_regulator")>
        Public Function RegpreciseRegulatorMatch(Regprecise As TranscriptionFactors, bbh As IEnumerable(Of BiDirectionalBesthit)) As RegPreciseRegulatorMatch()
            Dim LQuery = (From BacteriaGenome As BacteriaRegulome In Regprecise.genomes.AsParallel
                          Select BacteriaGenome.__matches(bbh)).ToArray
            Return LQuery.ToVector
        End Function

        <Extension>
        Private Function __matches(genome As BacteriaRegulome, bbh As IEnumerable(Of BiDirectionalBesthit)) As RegPreciseRegulatorMatch()
            'Dim LQuery = (From RegpreciseRegulator In genome.regulons.regulators
            '              Let Regulator As String = RegpreciseRegulator.regulator.name
            '              Let mapped = (From maps As BiDirectionalBesthit In bbh
            '                            Where String.Equals(maps.HitName, Regulator)
            '                            Select maps.QueryName).ToArray
            '              Let sites = (From site In RegpreciseRegulator.regulatorySites
            '                           Select String.Format("{0}:{1}", site.locus_tag, site.position)).ToArray
            '              Let match As RegPreciseRegulatorMatch = New RegPreciseRegulatorMatch With {
            '                  .Regulator = Regulator,
            '                  .RegulonSites = sites,
            '                  .Query = mapped
            '              }
            '              Select match).ToArray
            'Return LQuery

            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' read the local regprecise database file
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function ReadXml(path As String) As TranscriptionFactors
            Return TranscriptionFactors.Load(path)
        End Function

        <ExportAPI("Read.Csv.Regprecise")>
        Public Function ReadCsv(path As String) As Regprecise.RegPreciseRegulatorMatch()
            Return path.LoadCsv(Of Regprecise.RegPreciseRegulatorMatch)(False).ToArray
        End Function

        <ExportAPI("Family.Statics")>
        Public Function FamilyStatics(Matches As IEnumerable(Of IRegulatorMatched)) As IO.File
            Dim LQuery = (From item As KeyValuePair(Of String, String()) In FamilyStatics2(Matches)
                          Let Family As String = item.Key
                          Let Counts As String = item.Value.Count
                          Let Regulators As String = String.Join("; ", item.Value)
                          Select New IO.RowObject From {Family, Counts, Regulators}).ToArray
            Dim array = New IO.RowObject From {"Family", "Regulator Counts"}.Join(LQuery)
            Return New IO.File(array)
        End Function

        Public Function FamilyStatics2(Matches As Generic.IEnumerable(Of IRegulatorMatched)) As KeyValuePair(Of String, String())()
            Dim Families As String() = (From item In Matches Select item.Family Distinct Order By Family Ascending).ToArray
            Dim LQuery As KeyValuePair(Of String, String())() = (
                From Family As String In Families
                Let FamilyRegulators = (From item In Matches Where String.Equals(Family, item.Family) Select item.locusId Distinct).ToArray
                Select New KeyValuePair(Of String, String())(Family, FamilyRegulators)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 还存在问题
        ''' </summary>
        ''' <param name="Regprecise"></param>
        ''' <param name="ExportDir"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Regprecise.ExportMotifs.By.Species")>
        Public Function ExportBySpecies(Regprecise As TranscriptionFactors,
                                        <Parameter("DIR.Export")> ExportDir As String) As Boolean
            Dim TFFamilies As String() = GetTfFamilies(Regprecise)
            Dim TFSitesFasta = getFastaCollection(Regprecise)
            Dim LQuery = (From Family As String
                          In TFFamilies.AsParallel
                          Select __exportMotif(Family, TFSitesFasta, Regprecise, ExportDir)).ToArray
            Return True
        End Function

        Private Function __exportMotif(Family As String,
                                       TFSitesFasta As KeyValuePairData(Of Regtransbase.WebServices.MotifFasta)(),
                                       Regprecise As TranscriptionFactors,
                                       ExportDir As String) As Integer

            Dim TFBSListQuery = (From site In TFSitesFasta
                                 Where String.Equals(site.Key, Family, StringComparison.OrdinalIgnoreCase)
                                 Select Specie = Strings.Split(site.Value, " - ").Last,
                                     Fasta = site.DataObject).ToArray
            Dim Species = (From item In TFBSListQuery
                           Let SpeciesId As String = item.Specie
                           Select SpeciesId
                           Distinct).ToArray
            Dim MergedSmalls As List(Of KeyValuePair(Of String, Regtransbase.WebServices.MotifFasta)) =
                New List(Of KeyValuePair(Of String, Regtransbase.WebServices.MotifFasta))
            For Each SpeciesId As String In Species
                Dim FastaCollection = (From item In TFBSListQuery
                                       Where String.Equals(item.Specie, SpeciesId, StringComparison.OrdinalIgnoreCase)
                                       Select New KeyValuePair(Of String, Regtransbase.WebServices.MotifFasta)(SpeciesId, item.Fasta)).ToArray

                If FastaCollection.Length < 2 Then
                    Call MergedSmalls.AddRange(FastaCollection)
                    Continue For
                End If

                Call __mergeAction(FastaCollection, SpeciesId, Family, Regprecise, ExportDir)
            Next

            Return 0
        End Function

        Private Sub __mergeAction(Collection As IEnumerable(Of KeyValuePair(Of String, Regtransbase.WebServices.MotifFasta)),
                                       SpeciesId As String,
                                       Family As String,
                                       Regprecise As TranscriptionFactors,
                                       ExportDir As String)
            Dim FastaFile = CType(__exportMotifs(Collection, Family, Regprecise), FASTA.FastaFile)

            Family = Family.Replace("[", "").Replace("]", "").NormalizePathString
            SpeciesId = SpeciesId.NormalizePathString(True).Replace("-", "_").Replace(" ", "_")

            Dim Path As String = $"{ExportDir}/{Family}.{SpeciesId}.fasta"
            Call FastaFile.Save(Path, Encoding.ASCII)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="Family"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __exportMotifs(source As IEnumerable(Of KeyValuePair(Of String, Regtransbase.WebServices.MotifFasta)),
                                        Family As String,
                                        Regprecise As TranscriptionFactors) As FASTA.FastaSeq()

            Dim LQuery = (From i As Integer In source.Sequence
                          Let FastaObject = source(i)
                          Let Regulator As String = Regprecise.GetRegulatorId(FastaObject.Value.locus_tag, FastaObject.Value.position)
                          Select GenerateFastaData(FastaObject.Value, Family, FastaObject.Key, lcl:=i, Regulator:=Regulator)).ToArray
            Return LQuery
        End Function

        Public Function GenerateFastaData(Site As Regtransbase.WebServices.MotifFasta,
                                          Family As String,
                                          Species As String,
                                          lcl As Integer,
                                          Regulator As String) As FASTA.FastaSeq

            Dim title As String = $"{Site.locus_tag}:{Site.position}_lcl.{lcl} [regulator={Regulator}] [family={Family}] [regulog={Family} - {Species}]"

            Return New FASTA.FastaSeq With {
                .SequenceData = Regtransbase.WebServices.Regulator.SequenceTrimming(Site),
                .Headers = New String() {title}
            }
        End Function

        Private Function getFastaCollection(Regprecise As TranscriptionFactors) As KeyValuePairData(Of Regtransbase.WebServices.MotifFasta)()
            Dim ChunkBuffer As List(Of KeyValuePairData(Of Regtransbase.WebServices.MotifFasta)) =
                New List(Of KeyValuePairData(Of Regtransbase.WebServices.MotifFasta))

            For Each BacterialGenome As BacteriaRegulome In Regprecise.genomes
                For Each Regulon As Regulator In BacterialGenome.regulome.regulators
                    If Regulon.type = Types.RNA Then
                        Continue For
                    End If

                    Dim array = (From x As Regtransbase.WebServices.MotifFasta
                                 In Regulon.regulatorySites
                                 Let site = New KeyValuePairData(Of Regtransbase.WebServices.MotifFasta) With {
                                     .Key = Regulon.family,
                                     .Value = Regulon.regulog.name,
                                     .DataObject = x
                                 }
                                 Select site).ToArray
                    Call ChunkBuffer.AddRange(array)
                Next
            Next

            Return ChunkBuffer.ToArray
        End Function

        ''' <summary>
        ''' Export regulatory site from given database by family
        ''' </summary>
        ''' <param name="Regprecise"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function ExportByFamily(Regprecise As TranscriptionFactors) As Dictionary(Of String, FastaFile)
            Dim TFFamilies = GetTfFamilies(Regprecise)
            Dim buffer As Dictionary(Of String, FastaFile) = TFFamilies _
                .ToDictionary(Function(name) name,
                              Function()
                                  Return New FastaFile
                              End Function)

            For Each genome As BacteriaRegulome In Regprecise.genomes
                For Each regulon As Regulator In genome.regulome.regulators
                    If regulon.type = Types.RNA Then
                        Continue For
                    ElseIf regulon.family.StringEmpty Then
                        Dim sites As FastaSeq() = regulon.ExportMotifs.ToArray
                        Dim family = sites.First.Title.Matches("\[.+?\]").Last.GetStackValue("[", "]").GetTagValue("=").Name

                        If Not buffer.ContainsKey(family) Then
                            buffer.Add(family, New FastaFile)
                        End If

                        Call buffer(family).AddRange(sites)
                    Else
                        Call buffer(regulon.family.Split("/"c, "\"c).First).AddRange(regulon.ExportMotifs)
                    End If
                Next
            Next

            Return buffer
        End Function

        <Extension>
        Public Function GetTfFamilies(RegPrecise As TranscriptionFactors) As String()
            Return RegPrecise.genomes _
                .Select(Function(genome) genome.regulome.regulators) _
                .IteratesALL _
                .Where(Function(reg) reg.type = Types.TF AndAlso Not reg.family.StringEmpty) _
                .Select(Function(reg) reg.family.Split("/"c, "\"c).First) _
                .Distinct _
                .OrderBy(Function(name) name) _
                .ToArray
        End Function

        ''' <summary>
        ''' 当有时候向RegulatorSequerncede Fasta文件之中添加了新的Regprecise数据库之中没有的蛋白质序列数据之后，可能会出现
        ''' TFBS序列和Regulator之间的关系无法对应的情况，则这个时候可以使用本方法来重新刷新着两个Fasta序列文件
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>对于调控因子序列仅仅取出LocusTAG以及Description数据，TFBS文件是重新生成的</remarks>
        ''' 
        <ExportAPI("DB.Fasta.ReGenerate")>
        Public Function ReGenerate(Regprecise As Regprecise.TranscriptionFactors,
                                   Regulators As String,
                                   Optional Export As String = "") As Boolean

            Dim FileData As FASTA.FastaFile = New FASTA.FastaFile
            Dim lcl As Long = 0

            If String.IsNullOrEmpty(Export) Then
                Export = App.CurrentDirectory
            End If

            For Each Bacteria In Regprecise.genomes
                For Each Regulator In Bacteria.regulome.regulators
                    Dim Path As String = String.Format("{0}/{1}.fasta", Regulators, Regulator.locus_tag.name)

                    If Regulator.type = Types.RNA Then
                        Continue For
                    End If

                    If Not FileIO.FileSystem.FileExists(Path) Then
                        Call Console.WriteLine("EMPTY!!! {0}  -  {1}", Regulator.locus_tag.name, Regulator.regulator.name)
                        Continue For
                    End If

                    Dim FastaSequence As FastaReaders.Regulator = FastaReaders.Regulator.LoadDocument(FASTA.FastaSeq.Load(Path))
                    Dim RegpreciseProperty As String = String.Format("[Regulog={0}] [tfbs={1}]",
                                                                     Regulator.regulog.name,
                                                                     String.Join(";", (From site In Regulator.regulatorySites Select String.Format("{0}:{1}", site.locus_tag, site.position)).ToArray))
                    lcl += 1
                    FastaSequence.Headers = New String() {String.Format("lcl{0}", lcl), FastaSequence.Headers(1), RegpreciseProperty}

                    Call FileData.Add(FastaSequence)
                Next
            Next

            Dim f1 As Boolean = Regprecise.Export_TFBSInfo.Save(String.Format("{0}/Regprecise_TFBS.fasta", Export), encoding:=Encoding.ASCII)
            Dim f2 As Boolean = FileData.Save(String.Format("{0}/Regprecise_Regulators.fasta", Export), encoding:=Encoding.ASCII)

            Return f1 And f2
        End Function

        <ExportAPI("Regprecise.Compile")>
        Public Function Compile(Regprecise As TranscriptionFactors) As Regulations

        End Function

        <ExportAPI("Regprecise.Compile")>
        Public Function Compile(Regprecise As IEnumerable(Of JSON.genome), repository As String) As Regulations

        End Function

        ''' <summary>
        ''' 加载自有的源之中的调控数据库
        ''' </summary>
        ''' <returns></returns>
        <ExportAPI("")>
        Public Function LoadRegulationDb() As Regulations
            'Dim Xml As String = GCModeller.FileSystem.GetRegulations
            'Return Xml.LoadXml(Of Regulations)
        End Function
    End Module
End Namespace

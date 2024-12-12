﻿#Region "Microsoft.VisualBasic::0eb510bccfd1ffd4debbdeca540ab798, R#\seqtoolkit\Annotations\uniprot.vb"

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

'   Total Lines: 302
'    Code Lines: 243 (80.46%)
' Comment Lines: 32 (10.60%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 27 (8.94%)
'     File Size: 14.34 KB


' Module uniprot
' 
'     Function: getProteinSeq, IdUnify, metaboliteSet, openUniprotXmlAssembly, parseUniProt
'               proteinTable
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File

''' <summary>
''' The Universal Protein Resource (UniProt)
''' </summary>
<Package("uniprot",
         Category:=APICategories.UtilityTools,
         Description:="The Universal Protein Resource (UniProt)",
         Url:="https://www.uniprot.org/")>
<Cite(
Authors:="Amos Bairoch, Rolf Apweiler, Cathy H. Wu, Winona C. Barker, Brigitte Boeckmann, Serenella Ferro, Elisabeth Gasteiger, Hongzhan Huang, Rodrigo Lopez, Michele Magrane, Maria J. Martin, Darren A. Natale, Claire O'Donovan, Nicole Redaschi and Lai-Su L. Yeh",
Abstract:="The Universal Protein Resource (UniProt) provides the scientific community with 
a single, centralized, authoritative resource for protein sequences and functional information. 
Formed by uniting the Swiss-Prot, TrEMBL and PIR protein database activities, the UniProt 
consortium produces three layers of protein sequence databases: the UniProt Archive (UniParc), 
the UniProt Knowledgebase (UniProt) and the UniProt Reference (UniRef) databases. 
The UniProt Knowledgebase is a comprehensive, fully classified, richly and accurately annotated 
protein sequence knowledgebase with extensive cross-references. This centrepiece consists of 
two sections: UniProt/Swiss-Prot, with fully, manually curated entries; and UniProt/TrEMBL, 
enriched with automated classification and annotation. During 2004, tens of thousands of 
Knowledgebase records got manually annotated or updated; we introduced a new comment line 
topic: TOXIC DOSE to store information on the acute toxicity of a toxin; the UniProt keyword 
list got augmented by additional keywords; we improved the documentation of the keywords and 
are continuously overhauling and standardizing the annotation of post-translational modifications. 
Furthermore, we introduced a new documentation file of the strains and their synonyms. 
Many new database cross-references were introduced and we started to make use of Digital 
Object Identifiers. We also achieved in collaboration with the Macromolecular Structure 
Database group at EBI an improved integration with structural databases by residue level mapping 
of sequences from the Protein Data Bank entries onto corresponding UniProt entries. 
For convenient sequence searches we provide the UniRef non-redundant sequence databases. 
The comprehensive UniParc database stores the complete body of publicly available protein 
sequence data. The UniProt databases can be accessed online (http://www.uniprot.org) or 
downloaded in several formats (ftp://ftp.uniprot.org/pub). New releases are published every 
two weeks",
DOI:="10.1093/nar/gki070",
Journal:="Nucleic Acids Research",
Year:=2004,
Title:="The Universal Protein Resource (UniProt)",
PubMed:=540024,
Issue:="Database issue",
URL:="https://www.uniprot.org/")>
<RTypeExport("uniprot_kb", GetType(entry))>
Module uniprot

    Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(entry()), AddressOf uniprotProteinTable)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function uniprotProteinTable(uniprot As entry(), args As list, env As Environment) As DataFrame
        Return proteinTable(uniprot, env)
    End Function

    ''' <summary>
    ''' open a uniprot database file
    ''' </summary>
    ''' <param name="files"></param>
    ''' <param name="isUniParc"></param>
    ''' <returns>
    ''' this function returns a pipeline stream of the uniprot protein entries.
    ''' </returns>
    <ExportAPI("open.uniprot")>
    <RApiReturn(GetType(IEnumerable(Of entry)))>
    Public Function openUniprotXmlAssembly(<RRawVectorArgument>
                                           files As Object,
                                           Optional isUniParc As Boolean = False,
                                           Optional ignoreError As Boolean = True,
                                           Optional tqdm As Boolean = True,
                                           Optional env As Environment = Nothing) As Object

        Dim fileList As pipeline = pipeline.TryCreatePipeline(Of String)(files, env)
        Dim fileSet As String()

        If fileList.isError Then
            Return fileList
        Else
            fileSet = fileList _
                .populates(Of String)(env) _
                .ToArray

            For Each file As String In fileSet
                If Not file.FileExists Then
                    Return RInternal.debug.stop({
                        $"uniprot database file is not found!",
                        $"missing file: {file}"
                    }, env)
                End If
            Next
        End If

        Return UniProtXML _
            .EnumerateEntries(
                files:=fileSet,
                isUniParc:=isUniParc,
                ignoreError:=ignoreError,
                tqdm:=tqdm
            ) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
    End Function

    <ExportAPI("parseUniProt")>
    Public Function parseUniProt(xml As String) As entry()
        Dim uniprot As UniProtXML = UniProtXML.LoadXml(xml)
        Dim proteins As entry() = uniprot.entries

        Return proteins
    End Function

    ''' <summary>
    ''' read uniprot protein export output tsv file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("read.proteinTable")>
    <RApiReturn(GetType(SMRUCC.genomics.Assembly.Uniprot.Web.Entry))>
    Public Function readProteinTable(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        Dim df As New csv(FileLoader.Load(buf.TryCast(Of Stream), trimBlanks:=False, isTsv:=True))

    End Function

    ''' <summary>
    ''' export protein annotation data as data frame.
    ''' </summary>
    ''' <param name="uniprot"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("proteinTable")>
    Public Function proteinTable(<RRawVectorArgument> uniprot As Object, Optional env As Environment = Nothing) As Object
        Dim source = getUniprotData(uniprot, env)

        If source Like GetType(Message) Then
            Return source.TryCast(Of Message)
        End If

        Dim all As entry() = source.TryCast(Of IEnumerable(Of entry)).ToArray
        Dim uniprotId As String() = all.Select(Function(p) p.accessions(Scan0)).ToArray
        Dim name As String() = all.Select(Function(p) p.name).ToArray
        Dim geneName As String() = all.Select(Function(p) p.gene?.Primary.JoinBy("; ")).ToArray
        Dim fullName As String() = all.Select(Function(p) p.proteinFullName).ToArray
        Dim organism As String() = all.Select(Function(p) p.OrganismScientificName).ToArray
        Dim NCBITaxonomyId As String() = all.Select(Function(p) p.NCBITaxonomyId).ToArray
        Dim ECnumber As String() = all.Select(Function(p) p.ECNumberList.JoinBy("; ")).ToArray
        Dim GOterms As String() = all.Select(Function(p) p.GO.Select(Function(r) r.id).Distinct.JoinBy("; ")).ToArray
        Dim EMBL As String() = all.Select(Function(p) p.DbReferenceId("EMBL")).ToArray
        Dim Ensembl As String() = all.Select(Function(p) p.DbReferenceId("Ensembl")).ToArray
        Dim Ensembl_protein As String() = all _
            .Select(Function(p)
                        Dim ref = p.xrefs.TryGetValue("Ensembl")?.FirstOrDefault

                        If ref Is Nothing Then
                            Return ""
                        Else
                            Return ref("protein sequence ID")
                        End If
                    End Function) _
            .ToArray
        Dim Ensembl_geneID As String() = all _
            .Select(Function(p)
                        Dim ref = p.xrefs.TryGetValue("Ensembl")?.FirstOrDefault

                        If ref Is Nothing Then
                            Return ""
                        Else
                            Return ref("gene ID")
                        End If
                    End Function) _
            .ToArray
        Dim Proteomes As String() = all.Select(Function(p) p.DbReferenceId("Proteomes")).ToArray
        Dim Bgee As String() = all.Select(Function(p) p.DbReferenceId("Bgee")).ToArray
        Dim eggNOG As String() = all.Select(Function(p) p.DbReferenceId("eggNOG")).ToArray
        Dim RefSeq As String() = all.Select(Function(p) p.DbReferenceId("RefSeq")).ToArray
        Dim KEGG As String() = all.Select(Function(p) p.DbReferenceId("KEGG")).ToArray
        Dim motif As String() = all _
            .Select(Function(p)
                        Return p.GetDomainData _
                            .Select(Function(d) $"{d.DomainId}({d.start}|{d.ends})") _
                            .JoinBy("+")
                    End Function) _
            .ToArray

        Return New DataFrame With {
            .columns = New Dictionary(Of String, Array) From {
                {"uniprotId", uniprotId},
                {"name", name},
                {"geneName", geneName},
                {"fullName", fullName},
                {"EC_number", ECnumber},
                {"GO", GOterms},
                {"EMBL", EMBL},
                {"Ensembl", Ensembl},
                {"Ensembl_protein", Ensembl_protein},
                {"Ensembl_geneID", Ensembl_geneID},
                {"Proteomes", Proteomes},
                {"Bgee", Bgee},
                {"eggNOG", eggNOG},
                {"RefSeq", RefSeq},
                {"KEGG", KEGG},
                {"features", motif},
                {"NCBI_taxonomyId", NCBITaxonomyId},
                {"organism", organism}
            }
        }
    End Function

    <ExportAPI("get_sequence")>
    Public Function get_sequence(prot As entry) As FastaSeq
        Dim seq As String = DirectCast(prot, IPolymerSequenceModel).SequenceData
        Dim name As String = prot.name
        Dim fa As New FastaSeq With {.Headers = {name}, .SequenceData = seq}
        Return fa
    End Function

    <ExportAPI("get_description")>
    Public Function get_description(prot As entry) As String()
        Dim fullnames = prot.proteinFullName
        Dim funcs = prot.CommentList.TryGetValue("function").SafeQuery.Select(Function(comment) comment.GetText).ToArray
        Return {fullnames}.JoinIterates(funcs).Where(Function(str) Not str.StringEmpty(, True)).ToArray
    End Function

    ''' <summary>
    ''' get subcellular location of current protein
    ''' </summary>
    ''' <param name="prot"></param>
    ''' <returns></returns>
    <ExportAPI("get_subcellularlocation")>
    Public Function get_subcellularlocation(prot As entry) As Object
        Dim locs = prot.CommentList.TryGetValue("subcellular location") _
            .SafeQuery _
            .Select(Function(c) c.subcellularLocations) _
            .IteratesALL _
            .ToArray
        Dim df As New DataFrame With {.columns = New Dictionary(Of String, Array)}
        Dim flat_all = locs.Select(Function(c) c.locations.SafeQuery.Select(Function(l) (l, c.topology))).IteratesALL.ToArray

        Call df.add("location", From loc In flat_all Select loc.l.value)
        Call df.add("topology", From loc In flat_all Select loc.topology?.value)

        Return df
    End Function

    ''' <summary>
    ''' get related pathway names of current protein
    ''' </summary>
    ''' <param name="prot"></param>
    ''' <returns></returns>
    <ExportAPI("get_pathways")>
    Public Function get_pathwayNames(prot As entry) As String()
        Dim pathways = prot.CommentList _
            .TryGetValue("pathway") _
            .SafeQuery _
            .Select(Function(c) c.GetText.StringSplit(";\s*")) _
            .IteratesALL _
            .Distinct _
            .ToArray

        Return pathways
    End Function

    <ExportAPI("get_reactions")>
    Public Function get_reactions(prot As entry) As Object
        Dim catalytic = prot.CommentList.TryGetValue("catalytic activity")
        Dim list As list = list.empty

        For Each reaction As reaction In catalytic.Select(Function(r) r.reaction)
            If reaction Is Nothing Then
                Continue For
            End If

            list.add(reaction.text, New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"equation", reaction.text},
                    {"ec_number", reaction.GetECNumber},
                    {"metabolites", reaction.GetChEBI}
                }
            })
        Next

        Return list
    End Function

    ''' <summary>
    ''' get external database reference id set
    ''' </summary>
    ''' <param name="prot"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the uniprot database name will be named as: ``UniProtKB/Swiss-Prot`` for
    ''' make unify with the genebank feature xrefs
    ''' </remarks>
    <ExportAPI("get_xrefs")>
    Public Function get_xrefs(prot As entry) As Object
        Dim list As list = list.empty
        Dim xrefs As list = list.empty

        ' 20240804
        ' /db_xref="UniProtKB/Swiss-Prot:P0AD65"
        '
        ' make unify with the genebank entry when insert into biocad_registry
        xrefs.add("UniProtKB/Swiss-Prot", prot.accessions)

        For Each link In prot.dbReferences.GroupBy(Function(r) r.type)
            Call xrefs.add(link.Key, From i In link Select i.id)
        Next

        list.add("name", If(prot.name, prot.accessions.First))
        list.add("xrefs", xrefs)

        Return list
    End Function

    ''' <summary>
    ''' populate all protein fasta sequence from the given uniprot database reader
    ''' </summary>
    ''' <param name="uniprot">a collection of the uniprot protein ``entry`` data.</param>
    ''' <param name="extractAll"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("protein.seqs")>
    Public Function getProteinSeq(<RRawVectorArgument> uniprot As Object,
                                  Optional extractAll As Boolean = False,
                                  Optional KOseq As Boolean = False,
                                  Optional env As Environment = Nothing) As pipeline

        Dim source = getUniprotData(uniprot, env)
        Dim protFa = Iterator Function(prot As entry) As IEnumerable(Of FastaSeq)
                         If KOseq Then
                             Dim KO As dbReference = prot.KO

                             If Not KO Is Nothing Then
                                 Yield New FastaSeq With {
                                    .Headers = {KO.id, prot.proteinFullName},
                                    .SequenceData = prot.ProteinSequence
                                 }
                             End If
                         ElseIf extractAll Then
                             For Each accid As String In prot.accessions
                                 Yield New FastaSeq With {
                                    .Headers = {accid},
                                    .SequenceData = prot.ProteinSequence
                                 }
                             Next
                         Else
                             Yield New FastaSeq With {
                                .Headers = {prot.accessions(Scan0)},
                                .SequenceData = prot.ProteinSequence
                             }
                         End If
                     End Function

        If source Like GetType(Message) Then
            Return source.TryCast(Of Message)
        Else
            Return source.TryCast(Of IEnumerable(Of entry)) _
                .Select(Function(prot)
                            Return protFa(prot)
                        End Function) _
                .IteratesALL _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        End If
    End Function

    ''' <summary>
    ''' id unify mapping
    ''' </summary>
    ''' <param name="uniprot">a uniprot dataabse pipeline stream</param>
    ''' <param name="id"></param>
    ''' <param name="target">the database name for map to</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("id_unify")>
    Public Function IdUnify(<RRawVectorArgument> uniprot As Object,
                            <RRawVectorArgument> id As Object,
                            Optional target As String = Nothing,
                            Optional env As Environment = Nothing) As Object

        Dim uniprotData As pipeline = pipeline.TryCreatePipeline(Of entry)(uniprot, env)

        If uniprotData.isError Then
            Return uniprotData
        End If

        Dim rawIdList As String() = CLRVector.asCharacter(id)
        Dim mapId As Func(Of String, String) = GetIDs.IdMapping(uniprotData.populates(Of entry)(env), target)

        Return rawIdList _
            .Select(mapId) _
            .ToArray
    End Function

    <ExportAPI("metaboliteSet")>
    Public Function metaboliteSet(<RRawVectorArgument> uniprot As Object, Optional env As Environment = Nothing) As Object
        Dim uniprotData As pipeline = pipeline.TryCreatePipeline(Of entry)(uniprot, env)

        If uniprotData.isError Then
            Return uniprotData
        End If

        Return uniprotData _
            .populates(Of entry)(env) _
            .ToDictionary(Function(p) p.accessions(Scan0),
                          Function(p)
                              Dim comments = p.comments _
                                  .Where(Function(t) t.type = "catalytic activity") _
                                  .ToArray
                              Dim features = p.features _
                                  .Where(Function(t) t.type = "binding site") _
                                  .Where(Function(t) t.ligand IsNot Nothing AndAlso t.ligand.dbReference IsNot Nothing) _
                                  .ToArray
                              Dim substrates = comments _
                                  .Select(Function(t) t.reaction.dbReferences) _
                                  .IteratesALL _
                                  .Where(Function(r) r.type = "ChEBI") _
                                  .Select(Function(c) c.id) _
                                  .ToArray

                              Return features _
                                  .Select(Function(f) f.ligand.dbReference.id) _
                                  .JoinIterates(substrates) _
                                  .Distinct _
                                  .ToArray
                          End Function)
    End Function
End Module

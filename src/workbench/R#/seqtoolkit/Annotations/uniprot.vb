#Region "Microsoft.VisualBasic::4b719c0ce272e511ca6ced7c78b23e5a, R#\seqtoolkit\Annotations\uniprot.vb"

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

'   Total Lines: 288
'    Code Lines: 229
' Comment Lines: 30
'   Blank Lines: 29
'     File Size: 13.51 KB


' Module uniprot
' 
'     Function: getProteinSeq, IdUnify, openUniprotXmlAssembly, parseUniProt, proteinTable
'               writePtfFile
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

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
Module uniprot

    ''' <summary>
    ''' open a uniprot database file
    ''' </summary>
    ''' <param name="files"></param>
    ''' <param name="isUniParc"></param>
    ''' <returns>
    ''' this function returns a pipeline stream of the uniprot protein entries.
    ''' </returns>
    <ExportAPI("open.uniprot")>
    Public Function openUniprotXmlAssembly(<RRawVectorArgument>
                                           files As Object,
                                           Optional isUniParc As Boolean = False,
                                           Optional ignoreError As Boolean = True,
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
                    Return Internal.debug.stop({
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
                ignoreError:=ignoreError
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

        Return New dataframe With {
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

    ''' <summary>
    ''' populate all protein fasta sequence from the given uniprot database reader
    ''' </summary>
    ''' <param name="uniprot"></param>
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
    ''' <param name="uniprot"></param>
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

        Dim rawIdList As String() = REnv.asVector(Of String)(id)
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

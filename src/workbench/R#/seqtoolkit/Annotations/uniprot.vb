﻿#Region "Microsoft.VisualBasic::9b3597b69589873b196ad0ef1b4e669c, seqtoolkit\Annotations\uniprot.vb"

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

    ' Module uniprot
    ' 
    '     Function: getProteinSeq, openUniprotXmlAssembly, writePtfFile
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data
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
    ''' <returns></returns>
    <ExportAPI("open.uniprot")>
    Public Function openUniprotXmlAssembly(<RRawVectorArgument> files As Object, Optional isUniParc As Boolean = False, Optional env As Environment = Nothing) As pipeline
        Dim fileList As pipeline = pipeline.TryCreatePipeline(Of String)(files, env)

        If fileList.isError Then
            Return fileList
        End If

        Return UniProtXML _
            .EnumerateEntries(fileList.populates(Of String)(env).ToArray, isUniParc) _
            .DoCall(AddressOf pipeline.CreateFromPopulator)
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
                         If extractAll Then
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

    <ExportAPI("cache.ptf")>
    Public Function writePtfFile(<RRawVectorArgument>
                                 uniprot As Object,
                                 file As Object,
                                 Optional cacheTaxonomy As Boolean = False,
                                 Optional env As Environment = Nothing) As Object

        Dim source = getUniprotData(uniprot, env)

        If source Like GetType(Message) Then
            Return source.TryCast(Of Message)
        Else
            Dim stream As StreamWriter

            If file Is Nothing Then
                Return Internal.debug.stop({"file output can not be nothing!"}, env)
            ElseIf TypeOf file Is String Then
                stream = DirectCast(file, String).OpenWriter
            ElseIf TypeOf file Is Stream Then
                stream = New StreamWriter(DirectCast(file, Stream)) With {.NewLine = True}
            ElseIf TypeOf file Is StreamWriter Then
                stream = DirectCast(file, StreamWriter)
            Else
                Return Internal.debug.stop(Message.InCompatibleType(GetType(Stream), file.GetType, env,, NameOf(file)), env)
            End If

            Call source.TryCast(Of IEnumerable(Of entry)).WritePtfCache(stream, cacheTaxonomy)
            Call stream.Flush()

            If TypeOf file Is String Then
                Call stream.Close()
            End If

            Return Nothing
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

        Return rawIdList.Select(mapId).ToArray
    End Function
End Module

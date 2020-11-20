#Region "Microsoft.VisualBasic::e6d2ce634b6fc7f36064e523c3c8eff6, seqtoolkit\Annotations\uniprot.vb"

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
'     Function: getProteinSeq, getUniprotData, openUniprotXmlAssembly, writePtfFile
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

<Package("uniprot", Category:=APICategories.UtilityTools)>
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
End Module

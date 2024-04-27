﻿#Region "Microsoft.VisualBasic::b00603ba2816840afed27b79cc49629e, G:/GCModeller/src/workbench/R#/annotationKit//PtfCache.vb"

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

    '   Total Lines: 277
    '    Code Lines: 206
    ' Comment Lines: 42
    '   Blank Lines: 29
    '     File Size: 10.90 KB


    ' Module PTFCache
    ' 
    '     Function: createCluster, getDatabaseList, IDMapping, loadModel, loadXrefs
    '               readPtf, summaryofXrefs, writePtfFile, writePtfInternal
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' The protein annotation metadata
''' </summary>
<Package("ptf")> Module PTFCache

    ''' <summary>
    ''' enumerate all database name from a HDS stream
    ''' </summary>
    ''' <param name="ptf"></param>
    ''' <returns></returns>
    <ExportAPI("list.xrefs")>
    Public Function getDatabaseList(ptf As StreamPack) As String()
        Return New PtfReader(ptf).getExternalReferenceList
    End Function

    <ExportAPI("summary.xrefs")>
    Public Function summaryofXrefs(ptf As StreamPack, xrefs As String()) As Object
        Dim cache As New PtfReader(ptf)
        Dim data As New list With {
            .slots = New Dictionary(Of String, Object)
        }

        For Each name As String In xrefs.SafeQuery
            Dim model = cache.LoadCrossReference(key:=name)
            Dim info As New list With {
                .slots = New Dictionary(Of String, Object) From {
                    {"clusters", model.Count},
                    {"unique_size", model.Values.IteratesALL.Distinct.Count}
                }
            }

            Call data.add(name, info)
        Next

        Return data
    End Function

    ''' <summary>
    ''' load the cross reference id set
    ''' </summary>
    ''' <param name="ptf"></param>
    ''' <param name="database">the database name</param>
    ''' <returns></returns>
    <ExportAPI("load_xref")>
    <RApiReturn(TypeCodes.list)>
    Public Function loadXrefs(ptf As StreamPack, database As String) As Object
        Return New PtfReader(ptf).LoadCrossReference(key:=database)
    End Function

    ''' <summary>
    ''' read the protein annotation database
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.ptf")>
    Public Function readPtf(file As String) As ProteinAnnotation()
        If file.ExtensionSuffix("txt", "ptf") Then
            Return PtfFile.Load(file).proteins
        ElseIf file.ExtensionSuffix("db") Then
            Throw New NotImplementedException
        Else
            Throw New NotImplementedException
        End If
    End Function

    ''' <summary>
    ''' do id mapping via the protein annotation cache
    ''' </summary>
    ''' <param name="proteins">the protein annotation data</param>
    ''' <param name="id">a character vector that will be do mapping,
    ''' this parameter will make id subset of the id mapping result,
    ''' nothing means no subset</param>
    ''' <param name="mapTo">
    ''' external xrefs database name that the id will mapping to
    ''' </param>
    ''' <returns>A character vector of the mapping result id set, 
    ''' unmapped id will be leaves blank in this result.
    ''' </returns>
    <ExportAPI("ID_mapping")>
    <RApiReturn(GetType(list))>
    Public Function IDMapping(proteins As ProteinAnnotation(),
                              [from] As String,
                              mapTo As String,
                              Optional id As String() = Nothing) As Object

        Dim proteinIndex As Dictionary(Of String, ProteinAnnotation())

        Select Case from.ToLower
            Case "genename", "gene_name"
                proteinIndex = proteins _
                    .Where(Function(prot) Not prot.geneName.StringEmpty) _
                    .GroupBy(Function(prot) Strings.LCase(prot.geneName)) _
                    .ToDictionary(Function(prot) prot.Key,
                                  Function(prot)
                                      Return prot.ToArray
                                  End Function)
            Case Else
                proteinIndex = proteins _
                    .Where(Function(prot) prot.has(from)) _
                    .GroupBy(Function(prot) prot.attr(from)) _
                    .ToDictionary(Function(prot) prot.Key,
                                  Function(prot)
                                      Return prot.ToArray
                                  End Function)
        End Select

        Dim result As New Dictionary(Of String, Object)
        Dim maps As String()

        If id.IsNullOrEmpty Then
            result = proteinIndex.ToDictionary(Function(prot) prot.Key,
                                               Function(prot) As Object
                                                   Return prot.Value _
                                                      .Where(Function(p) p.has(mapTo)) _
                                                      .Select(Function(p) p.attributes(mapTo)) _
                                                      .IteratesALL _
                                                      .Distinct _
                                                      .ToArray
                                               End Function)
        Else
            For i As Integer = 0 To id.Length - 1
                Dim str As String = Strings.LCase(id(i))

                If Not proteinIndex.ContainsKey(str) Then
                    result.Add(id(i), Nothing)
                Else
                    proteins = proteinIndex(str)
                    maps = proteins _
                        .Select(Function(prot) prot(mapTo)) _
                        .Where(Function(mapId) Not mapId.StringEmpty) _
                        .ToArray
                    result.Add(id(i), maps)
                End If
            Next
        End If

        Return New list With {
            .slots = result
        }
    End Function

    <ExportAPI("loadBackgroundModel")>
    Public Function loadModel(ptf As StreamPack, database As String) As Background
        Dim data = New PtfReader(ptf).LoadCrossReference(key:=database)
        Dim clusters As Cluster() = data _
            .Select(Function(c)
                        Return c.createCluster
                    End Function) _
            .ToArray

        Return New Background With {
            .id = database,
            .name = database,
            .build = Now,
            .clusters = clusters,
            .comments = database
        }
    End Function

    <Extension>
    Private Function createCluster(c As KeyValuePair(Of String, String())) As Cluster
        Dim metadata = c.Key.Split("@"c)
        Dim cid As String = metadata(0)
        Dim name As String = metadata.ElementAtOrDefault(1, "")

        Return New Cluster With {
            .ID = cid,
            .description = c.Key,
            .names = name,
            .members = c.Value _
                .Select(Function(id)
                            Return New BackgroundGene With {
                                .accessionID = id,
                                .[alias] = {id},
                                .locus_tag = New NamedValue With {.name = id, .text = id},
                                .name = id,
                                .term_id = BackgroundGene.UnknownTerms(id).ToArray
                            }
                        End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' create a protein annotation metadata file
    ''' </summary>
    ''' <param name="uniprot"></param>
    ''' <param name="file"></param>
    ''' <param name="db_xref"></param>
    ''' <param name="cacheTaxonomy"></param>
    ''' <param name="hds_stream"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("cache.ptf")>
    Public Function writePtfFile(<RRawVectorArgument>
                                 uniprot As Object,
                                 file As Object,
                                 <RRawVectorArgument(GetType(String))>
                                 Optional db_xref As Object = "Bgee|KEGG|KO|GO|Pfam|RefSeq|EC|InterPro|BioCyc|eggNOG|keyword",
                                 Optional cacheTaxonomy As Boolean = False,
                                 Optional hds_stream As Boolean = False,
                                 Optional env As Environment = Nothing) As Object

        Dim source = getUniprotData(uniprot, env)
        Dim keys As String() = CLRVector.asCharacter(db_xref)

        If source Like GetType(Message) Then
            Return source.TryCast(Of Message)
        Else
            Return source.TryCast(Of IEnumerable(Of entry)).writePtfInternal(
                file:=file,
                cacheTaxonomy:=cacheTaxonomy,
                hds_stream:=hds_stream,
                env:=env,
                idMapping:=keys
            )
        End If
    End Function

    <Extension>
    Private Function writePtfInternal(source As IEnumerable(Of entry),
                                      file As Object,
                                      idMapping As String(),
                                      cacheTaxonomy As Boolean,
                                      hds_stream As Boolean,
                                      env As Environment) As Object
        Dim stream As StreamWriter
        Dim buffer = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Write, env)
        Dim keys As String = idMapping.JoinBy(",")

        If TypeOf file Is StreamWriter Then
            stream = file
        ElseIf buffer Like GetType(Message) Then
            Return buffer.TryCast(Of Message)
        End If

        If hds_stream Then
            Dim fileObj As Stream = buffer.TryCast(Of Stream)
            Call fileObj.SetLength(0)

            Using cache As New PtfWriter(fileObj, idMapping)
                For Each protein As ProteinAnnotation In source.Select(Function(p) AnnotationCache.toPtf(p, cacheTaxonomy, keys))
                    Call cache.AddProtein(protein)
                Next
            End Using
        Else
            stream = New StreamWriter(buffer.TryCast(Of Stream))

            Call source.WritePtfCache(stream, cacheTaxonomy, keys)
            Call stream.Flush()

            If TypeOf file Is String Then
                Call stream.Close()
            End If
        End If

        Return Nothing
    End Function
End Module

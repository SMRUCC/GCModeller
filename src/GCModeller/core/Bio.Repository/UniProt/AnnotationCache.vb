﻿#Region "Microsoft.VisualBasic::ca45a6bb3381ae53d6f3b116db2d4e61, core\Bio.Repository\UniProt\AnnotationCache.vb"

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

' Module AnnotationCache
' 
'     Function: toPtf
' 
'     Sub: SplitAnnotations, WritePtfCache
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.Uniprot.XML

Public Module AnnotationCache

    <Extension>
    Public Sub WritePtfCache(proteins As IEnumerable(Of entry), cache As TextWriter, Optional includesNCBITaxonomy As Boolean = False, Optional keys$ = "KEGG,KO,GO,Pfam,RefSeq,EC,InterPro,BioCyc,eggNOG")
        For Each protein As entry In proteins
            Call cache.WriteLine(PtfFile.ToString(toPtf(protein, includesNCBITaxonomy, keys)))
        Next
    End Sub

    <Extension>
    Public Function toPtf(protein As entry, includesNCBITaxonomy As Boolean, Optional keys$ = "KEGG,KO,GO,Pfam,RefSeq,EC,InterPro,BioCyc,eggNOG") As ProteinAnnotation
        Dim dbxref As New Dictionary(Of String, String())
        Dim refList As String()
        Dim dbNames As String()

        Static dbNameList As New Dictionary(Of String, String())

        dbNames = dbNameList.ComputeIfAbsent(keys, Function() keys.StringSplit("[,|;+]"))
        dbxref.Add("synonym", protein.accessions)

        For Each refDb As String In dbNames
            If protein.xrefs.ContainsKey(refDb) Then
                refList = protein.xrefs(refDb) _
                    .Select(Function(ref) ref.id) _
                    .ToArray

                Call dbxref.Add(refDb.ToLower, refList)
            End If
        Next

        If includesNCBITaxonomy Then
            Dim ncbi_id As String = protein.NCBITaxonomyId

            If Not ncbi_id.StringEmpty Then
                Call dbxref.Add("ncbi_taxonomy", {ncbi_id})
            End If
        End If

        Return New ProteinAnnotation With {
            .geneId = protein.accessions(Scan0),
            .description = protein.proteinFullName,
            .attributes = dbxref
        }
    End Function

    <Extension>
    Public Sub SplitAnnotations(proteins As IEnumerable(Of ProteinAnnotation), key As String, outputdir As String)
        Dim [handles] As New Dictionary(Of String, StreamWriter) From {
            {"na", $"{outputdir}/na.ptf".OpenWriter(bufferSize:=1024)}
        }

        For Each protein As ProteinAnnotation In proteins
            If Not protein.attributes.ContainsKey(key) Then
                Call [handles]("na").WriteLine(PtfFile.ToString(protein))
            Else
                For Each name As String In protein.attributes(key)
                    If Not [handles].ContainsKey(name) Then
                        [handles].Add(name, $"{outputdir}/{name.NormalizePathString}.ptf".OpenWriter(bufferSize:=1024))
                    End If

                    [handles](name).WriteLine(PtfFile.ToString(protein))
                Next
            End If
        Next

        For Each key In [handles].Keys
            Call [handles](key).Flush()
            Call [handles](key).Dispose()
        Next
    End Sub
End Module

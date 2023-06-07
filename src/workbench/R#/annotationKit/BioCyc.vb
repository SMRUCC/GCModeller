#Region "Microsoft.VisualBasic::4ff35f92948d6e3d60d444fd63af6770, R#\annotationKit\BioCyc.vb"

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

'   Total Lines: 112
'    Code Lines: 88
' Comment Lines: 10
'   Blank Lines: 14
'     File Size: 3.75 KB


' Module BioCycRepository
' 
'     Function: (+2 Overloads) createBackground, formulaString, getCompounds, openBioCyc
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("BioCyc")>
Public Module BioCycRepository

    ''' <summary>
    ''' open a directory path as the biocyc workspace
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <ExportAPI("open.biocyc")>
    Public Function openBioCyc(repo As String) As Workspace
        Return New Workspace(repo)
    End Function

    <ExportAPI("getCompounds")>
    <RApiReturn(GetType(compounds))>
    Public Function getCompounds(repo As Object, Optional env As Environment = Nothing) As Object
        Dim file As AttrDataCollection(Of compounds) = Nothing

        If repo Is Nothing Then
            Return Nothing
        End If

        If TypeOf repo Is Workspace Then
            file = DirectCast(repo, Workspace).compounds
        ElseIf repo.GetType.IsInheritsFrom(GetType(Stream)) Then
            file = compounds.OpenFile(DirectCast(repo, Stream))
        ElseIf TypeOf repo Is String Then
            If DirectCast(repo, String).FileExists Then
                file = compounds.OpenFile(DirectCast(repo, String))
            Else
                file = compounds.ParseText(repo)
            End If
        Else
            Return Message.InCompatibleType(GetType(Workspace), repo.GetType, env)
        End If

        If Not file Is Nothing Then
            Return file.features.ToArray
        Else
            Return Nothing
        End If
    End Function

    <ExportAPI("formula")>
    Public Function formulaString(meta As compounds) As String
        If meta.chemicalFormula.IsNullOrEmpty Then
            Return ""
        Else
            Return meta.chemicalFormula _
                .Select(Function(d)
                            Return d.Trim(" "c, "("c, ")"c).Replace(" ", "")
                        End Function) _
                .JoinBy("")
        End If
    End Function

    ''' <summary>
    ''' Create pathway background model 
    ''' </summary>
    ''' <param name="biocyc"></param>
    ''' <returns></returns>
    <ExportAPI("createBackground")>
    Public Function createBackground(biocyc As Workspace) As Background
        Dim pathways As Cluster() = biocyc.pathways _
            .features _
            .Select(Function(pwy)
                        Return biocyc.createBackground(pwy)
                    End Function) _
            .ToArray

        Return New Background With {
             .build = Now,
             .comments = "MetaCyc Background",
             .name = "biocyc",
             .id = "biocyc",
             .clusters = pathways
        }
    End Function

    <Extension>
    Private Function createBackground(biocyc As Workspace, pathway As pathways) As Cluster
        Dim compounds As New Dictionary(Of String, BackgroundGene)
        Dim reactions = biocyc.reactions
        Dim metadata = biocyc.compounds

        For Each linkId As String In pathway.reactionList
            Dim reaction As reactions = reactions(linkId)

            If reaction Is Nothing Then
                Continue For
            End If

            Dim all = reaction.left _
                .JoinIterates(reaction.right) _
                .ToArray

            For Each c As CompoundSpecieReference In all
                If compounds.ContainsKey(c.ID) Then
                    Continue For
                End If

                Dim metainfo As compounds = metadata(c.ID)
                Dim name As String = metainfo?.commonName
                Dim cpd As New BackgroundGene With {
                    .accessionID = c.ID,
                    .name = If(name, c.ID),
                    .term_id = BackgroundGene.UnknownTerms(c.ID).ToArray,
                    .[alias] = {c.ID},
                    .locus_tag = New NamedValue With {
                        .name = c.ID,
                        .text = If(name, c.ID)
                    }
                }

                Call compounds.Add(c.ID, cpd)
            Next
        Next

        Return New Cluster With {
            .ID = pathway.uniqueId,
            .description = pathway.comment,
            .names = pathway.commonName,
            .members = compounds _
                .Values _
                .ToArray
        }
    End Function
End Module

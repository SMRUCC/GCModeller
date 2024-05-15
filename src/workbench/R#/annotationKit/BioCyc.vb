﻿#Region "Microsoft.VisualBasic::962dcea90e40f76f925d80a6f3b2136e, R#\annotationKit\BioCyc.vb"

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

    '   Total Lines: 168
    '    Code Lines: 128
    ' Comment Lines: 19
    '   Blank Lines: 21
    '     File Size: 6.13 KB


    ' Module BioCycRepository
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+2 Overloads) createBackground, formulaString, getCompounds, getCompoundsTable, openBioCyc
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
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("BioCyc")>
Public Module BioCycRepository

    Sub New()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(compounds()), AddressOf getCompoundsTable)
    End Sub

    Private Function getCompoundsTable(compounds As compounds(), args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = compounds.Select(Function(c) c.uniqueId).ToArray
        }

        Call df.add("ID", compounds.Select(Function(c) c.uniqueId))
        Call df.add("name", compounds.Select(Function(c) c.commonName))
        Call df.add("formula", compounds.Select(Function(c) formulaString(c)))
        Call df.add("exactMass", compounds.Select(Function(c) c.exactMass))
        Call df.add("SMILES", compounds.Select(Function(c) c.SMILES))
        Call df.add("InChIKey", compounds.Select(Function(c) c.InChIKey))
        Call df.add("InChI", compounds.Select(Function(c) c.InChI))
        Call df.add("synonyms", compounds.Select(Function(c) c.synonyms.JoinBy("; ")))

        Return df
    End Function

    ''' <summary>
    ''' open a directory path as the biocyc workspace
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <ExportAPI("open.biocyc")>
    Public Function openBioCyc(repo As String) As Workspace
        Return New Workspace(repo)
    End Function

    ''' <summary>
    ''' get compounds list data from a given biocyc workspace context
    ''' </summary>
    ''' <param name="repo">
    ''' this repository data could be a opened biocyc workspace, target
    ''' file path, or text content of the compounds data
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
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
            Return compounds.FormulaString(meta)
        End If
    End Function

    ''' <summary>
    ''' Create pathway enrichment background model 
    ''' </summary>
    ''' <param name="biocyc"></param>
    ''' <returns></returns>
    <ExportAPI("createBackground")>
    Public Function createBackground(biocyc As Workspace) As Background
        Dim pathways As Cluster() = biocyc.pathways.features _
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

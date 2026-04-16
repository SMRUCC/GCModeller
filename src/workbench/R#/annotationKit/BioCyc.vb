#Region "Microsoft.VisualBasic::dfb5f7ddea4e6464393abe7bd095e3cb, R#\annotationKit\BioCyc.vb"

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

    '   Total Lines: 376
    '    Code Lines: 289 (76.86%)
    ' Comment Lines: 39 (10.37%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 48 (12.77%)
    '     File Size: 14.04 KB


    ' Module BioCycRepository
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: (+2 Overloads) createBackground, formulaString, getCompounds, getCompoundsTable, GetDbLinks
    '               getGenes, getProteins, getReactions, openBioCyc
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
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' The BioCyc database collection is an assortment of organism specific Pathway/Genome Databases (PGDBs) 
''' that provide reference to genome and metabolic pathway information for thousands of organisms.
''' As of July 2023, there were over 20,040 databases within BioCyc.[2] SRI International,[3] based in 
''' Menlo Park, California, maintains the BioCyc database family.
''' </summary>
<Package("BioCyc")>
Public Module BioCycRepository

    Sub New()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(compounds()), AddressOf getCompoundsTable)
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

    <ExportAPI("getReactions")>
    <RApiReturn(GetType(reactions))>
    Public Function getReactions(repo As Object, Optional env As Environment = Nothing) As Object
        Dim file As AttrDataCollection(Of reactions) = Nothing

        If repo Is Nothing Then
            Return Nothing
        End If

        If TypeOf repo Is Workspace Then
            file = DirectCast(repo, Workspace).reactions
        ElseIf repo.GetType.IsInheritsFrom(GetType(Stream)) Then
            file = reactions.OpenFile(DirectCast(repo, Stream))
        ElseIf TypeOf repo Is String Then
            If DirectCast(repo, String).FileExists Then
                file = reactions.OpenFile(DirectCast(repo, String))
            Else
                file = reactions.ParseText(repo)
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

    <ExportAPI("getGenes")>
    <RApiReturn(GetType(genes))>
    Public Function getGenes(repo As Object,
                             Optional dnaseq As String = Nothing,
                             Optional env As Environment = Nothing) As Object

        Dim file As AttrDataCollection(Of genes) = Nothing
        Dim seqfile As FastaFile = Nothing

        If repo Is Nothing Then
            Return Nothing
        End If
        If dnaseq.FileExists Then
            seqfile = FastaFile.LoadNucleotideData(dnaseq)
        End If

        If TypeOf repo Is Workspace Then
            file = DirectCast(repo, Workspace).genes
        ElseIf repo.GetType.IsInheritsFrom(GetType(Stream)) Then
            file = genes.OpenFile(DirectCast(repo, Stream))
        ElseIf TypeOf repo Is String Then
            If DirectCast(repo, String).FileExists Then
                file = genes.OpenFile(DirectCast(repo, String))
            Else
                file = genes.ParseText(repo)
            End If
        Else
            Return Message.InCompatibleType(GetType(Workspace), repo.GetType, env)
        End If

        If Not file Is Nothing Then
            Dim seqs As Dictionary(Of String, FastaSeq) = Nothing

            If Not seqfile Is Nothing Then
                seqs = Workspace.CreateSequenceIndex(seqfile)
            End If

            If seqs.IsNullOrEmpty Then
                Return file.features.ToArray
            Else
                Return file.features _
                    .Select(Function(gene)
                                If seqs.ContainsKey(gene.uniqueId) Then
                                    gene.dnaseq = seqs(gene.uniqueId).SequenceData
                                End If

                                Return gene
                            End Function) _
                    .ToArray
            End If
        Else
            Return Nothing
        End If
    End Function

    <ExportAPI("getProteins")>
    <RApiReturn(GetType(proteins))>
    Public Function getProteins(repo As Object,
                                Optional protseq As String = Nothing,
                                Optional env As Environment = Nothing) As Object

        Dim file As AttrDataCollection(Of proteins) = Nothing
        Dim seqfile As FastaFile = Nothing

        If repo Is Nothing Then
            Return Nothing
        End If
        If protseq.FileExists Then
            seqfile = FastaFile.Read(protseq, strict:=False)
        End If

        If TypeOf repo Is Workspace Then
            file = DirectCast(repo, Workspace).proteins
        ElseIf repo.GetType.IsInheritsFrom(GetType(Stream)) Then
            file = proteins.OpenFile(DirectCast(repo, Stream))
        ElseIf TypeOf repo Is String Then
            If DirectCast(repo, String).FileExists Then
                file = proteins.OpenFile(DirectCast(repo, String))
            Else
                file = proteins.ParseText(repo)
            End If
        Else
            Return Message.InCompatibleType(GetType(Workspace), repo.GetType, env)
        End If

        If Not file Is Nothing Then
            Dim seqs As Dictionary(Of String, FastaSeq) = Nothing

            If Not seqfile Is Nothing Then
                seqs = Workspace.CreateSequenceIndex(seqfile)
            End If

            If seqs.IsNullOrEmpty Then
                Return file.features.ToArray
            Else
                Return file.features _
                    .Select(Function(prot)
                                If seqs.ContainsKey(prot.uniqueId) Then
                                    prot.protseq = seqs(prot.uniqueId).SequenceData
                                End If

                                Return prot
                            End Function) _
                    .ToArray
            End If
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' get formula string of the given object model
    ''' </summary>
    ''' <param name="x">
    ''' 1. for <see cref="compounds"/> model, get molecular formula string
    ''' 2. for <see cref="reactions"/> model, get the reaction equation string.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("formula")>
    <RApiReturn(TypeCodes.string)>
    Public Function formulaString(x As Object, Optional env As Environment = Nothing) As Object
        If TypeOf x Is compounds Then
            Dim meta As compounds = DirectCast(x, compounds)

            If meta.chemicalFormula.IsNullOrEmpty Then
                Return ""
            Else
                Return compounds.FormulaString(meta)
            End If
        ElseIf TypeOf x Is reactions Then
            Dim rxn As reactions = DirectCast(x, reactions)

            If rxn.equation Is Nothing Then
                Return ""
            Else
                Return rxn.equation.ToString
            End If
        Else
            Return Message.InCompatibleType(GetType(compounds), x.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' get external database cross reference id of current metabolite compound object
    ''' </summary>
    ''' <param name="meta"></param>
    ''' <returns></returns>
    <ExportAPI("db_links")>
    <RApiReturn(TypeCodes.list)>
    Public Function GetDbLinks(meta As Model, Optional env As Environment = Nothing) As Object
        Dim dblinks As DBLink() = Nothing

        If meta Is Nothing Then
            Call env.AddMessage("the given biocyc element model object is nothing!")
            Return Nothing
        End If

        If TypeOf meta Is compounds Then
            dblinks = compounds.GetDbLinks(DirectCast(meta, compounds)).ToArray
        ElseIf TypeOf meta Is genes Then
            dblinks = DirectCast(meta, genes).db_links
        ElseIf TypeOf meta Is proteins Then
            dblinks = DirectCast(meta, proteins).db_links
        Else
            Return Message.InCompatibleType(GetType(compounds), meta.GetType, env)
        End If

        Dim dbgroups = dblinks _
            .GroupBy(Function(a) a.DBName.ToLower) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return CObj(a.Select(Function(ai) ai.entry).ToArray)
                          End Function)

        Return New list(dbgroups)
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

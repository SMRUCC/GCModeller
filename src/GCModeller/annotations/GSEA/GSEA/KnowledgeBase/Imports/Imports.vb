#Region "Microsoft.VisualBasic::6714231e149d643474c4af77bd026e36, annotations\GSEA\GSEA\KnowledgeBase\Imports\Imports.vb"

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

    '   Total Lines: 191
    '    Code Lines: 137 (71.73%)
    ' Comment Lines: 32 (16.75%)
    '    - Xml Docs: 96.88%
    ' 
    '   Blank Lines: 22 (11.52%)
    '     File Size: 7.62 KB


    ' Module [Imports]
    ' 
    ' 
    '     Delegate Function
    ' 
    '         Function: CreateBackground, CreateCluster, getTermInternal, ImportsUniProt, KEGGMapRelation
    '                   UniProtGetGOTerms, UniProtGetKOTerms
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.Assembly.Uniprot.XML

''' <summary>
''' 进行富集计算分析所需要的基因组背景模型的导入模块
''' </summary>
Public Module [Imports]

    ''' <summary>
    ''' [geneID => [clusterID, name, description]]
    ''' </summary>
    ''' <param name="geneID"></param>
    ''' <returns></returns>
    Public Delegate Function GetClusterTerms(geneID As String) As NamedValue(Of String)()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="db">从UniProt数据库之中下载的基因组Xml文件的迭代器</param>
    ''' <param name="genomeName$"></param>
    ''' <param name="define">Functional cluster define function</param>
    ''' <returns></returns>
    <Extension>
    Public Function ImportsUniProt(db As IEnumerable(Of entry),
                                   getTerm As Func(Of entry, String()),
                                   define As GetClusterTerms,
                                   Optional genomeName$ = Nothing,
                                   Optional outputAll As Boolean = False) As Background

        With db.ToArray
            Dim taxonomy As String = Nothing

            For Each protein As entry In .AsEnumerable
                taxonomy = protein.organism?.lineage?.taxonlist.JoinBy("; ")

                If Not taxonomy.StringEmpty Then
                    Exit For
                End If
            Next

            Return .CreateBackground(
                getTerms:=getTerm,
                define:=define,
                createGene:=AddressOf uniprotGeneModel,
                genomeName:=genomeName,
                taxonomy:=taxonomy,
                outputAll:=outputAll
            )
        End With
    End Function

    ''' <summary>
    ''' a generic method for create background model
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="db"></param>
    ''' <param name="createGene"></param>
    ''' <param name="getTerms"></param>
    ''' <param name="define"></param>
    ''' <param name="genomeName$"></param>
    ''' <param name="taxonomy$"></param>
    ''' <param name="outputAll"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateBackground(Of T)(db As IEnumerable(Of T),
                                           createGene As Func(Of T, String(), BackgroundGene),
                                           getTerms As Func(Of T, String()),
                                           define As GetClusterTerms,
                                           Optional genomeName$ = Nothing,
                                           Optional taxonomy$ = Nothing,
                                           Optional outputAll As Boolean = False) As Background

        Dim clusters As New Dictionary(Of String, List(Of BackgroundGene))
        Dim clusterNotes As New Dictionary(Of String, NamedValue(Of String))
        Dim genomeSize%

        For Each protein As T In db
            Dim terms As String() = getTerms(protein)
            Dim clusterNames As NamedValue(Of String)()

            genomeSize += 1

            If terms.IsNullOrEmpty Then
                clusterNames = {}
            Else
                clusterNames = terms.Select(Function(geneID) define(geneID)) _
                                    .IteratesALL _
                                    .ToArray
            End If

            For Each clusterID As NamedValue(Of String) In clusterNames
                If clusterID.Name.StringEmpty Then
                    Continue For
                End If

                If Not clusters.ContainsKey(clusterID.Name) Then
                    Call clusters.Add(clusterID.Name, New List(Of BackgroundGene))
                    Call clusterNotes.Add(clusterID.Name, clusterID)
                End If

                clusters(clusterID.Name) += createGene(protein, terms)
            Next
        Next

        Return New Background With {
            .name = genomeName,
            .build = Now,
            .clusters = clusters _
                .Where(Function(c)
                           If outputAll Then
                               Return True
                           Else
                               Return c.Value.Count > 0
                           End If
                       End Function) _
                .Select(Function(c)
                            Dim geneIDs As IEnumerable(Of BackgroundGene) = c.Value
                            Dim note = clusterNotes(c.Key)

                            Return geneIDs.CreateCluster(c.Key, note)
                        End Function) _
                .ToArray,
            .size = genomeSize,
            .comments = taxonomy
        }
    End Function

    <Extension>
    Public Function CreateCluster(members As IEnumerable(Of BackgroundGene), clusterID$, note As NamedValue(Of String)) As Cluster
        Return New Cluster With {
            .ID = clusterID,
            .members = members.ToArray,
            .description = note.Description,
            .names = note.Value
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function UniProtGetKOTerms() As Func(Of entry, String())
        Return getTermInternal("KO")
    End Function

    Private Function getTermInternal(type As String) As Func(Of entry, String())
        Return Function(protein)
                   If protein.xrefs.ContainsKey(type) Then
                       Return protein.xrefs(type) _
                           .Select(Function(ref) ref.id) _
                           .ToArray
                   Else
                       Return {}
                   End If
               End Function
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function UniProtGetGOTerms() As Func(Of entry, String())
        Return getTermInternal("GO")
    End Function
End Module

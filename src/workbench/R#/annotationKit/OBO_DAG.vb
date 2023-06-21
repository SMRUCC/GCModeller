#Region "Microsoft.VisualBasic::e5586f868a26d1f3b15b925d518ca59b, R#\annotationKit\OBO_DAG.vb"

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

    '   Total Lines: 71
    '    Code Lines: 53
    ' Comment Lines: 8
    '   Blank Lines: 10
    '     File Size: 2.37 KB


    ' Module OBO_DAG
    ' 
    '     Function: filterObsolete, filterProperty, readOboDAG, saveObo
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO

''' <summary>
''' The Open Biological And Biomedical Ontology (OBO) Foundry
''' </summary>
<Package("OBO")>
Module OBO_DAG

    ''' <summary>
    ''' parse the obo file 
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <ExportAPI("read.obo")>
    Public Function readOboDAG(path As String) As GO_OBO
        Return GO_OBO.LoadDocument(path)
    End Function

    <ExportAPI("ontologyTree")>
    Public Function ontologyTree(obo As GO_OBO) As TermTree(Of Term)
        Dim hash = obo.CreateTermTable
        Dim index As New Dictionary(Of String, TermTree(Of Term))

        For Each term As Term In hash.Values
            Dim is_a As is_a() = term.is_a _
                .SafeQuery _
                .Select(Function(si) New is_a(si)) _
                .ToArray
            Dim node As TermTree(Of Term)

            If index.ContainsKey(term.id) Then
                node = index(term.id)
            Else
                node = New TermTree(Of Term) With {.Data = term, .label = term.name, .Childs = New Dictionary(Of String, Tree(Of Term, String))}
                index.Add(term.id, node)
            End If

            For Each link As is_a In is_a
                If Not index.ContainsKey(link.term_id) Then
                    index.Add(link.term_id, New TermTree(Of Term) With {.label = link.name, .Childs = New Dictionary(Of String, Tree(Of Term, String))})
                End If

                node.Parent = index(link.term_id)
                index(link.term_id).Childs.Add(term.id, node)
            Next
        Next

        Dim ontology As TermTree(Of Term)

        If index.Count > 0 Then
            ontology = TermTree(Of Term).FindRoot(index.Values.First)
        Else
            ontology = Nothing
        End If

        Return ontology
    End Function

    <ExportAPI("ontologyLeafs")>
    Public Function ontologyLeafs(tree As TermTree(Of Term)) As TermTree(Of Term)()
        Dim leafs As New List(Of TermTree(Of Term))
        Dim popAllNodes = tree.EnumerateChilds(popAll:=True).ToArray

        For Each term As TermTree(Of Term) In popAllNodes
            If term.IsLeaf Then
                Call leafs.Add(term)
            End If
        Next

        Return leafs.ToArray
    End Function

    <ExportAPI("filter.is_obsolete")>
    Public Function filterObsolete(obo As GO_OBO) As GO_OBO
        obo = New GO_OBO With {
            .headers = obo.headers,
            .typedefs = obo.typedefs,
            .terms = obo.terms _
                .Where(Function(t) Not t.is_obsolete.TextEquals("true")) _
                .ToArray
        }

        Return obo
    End Function

    <ExportAPI("filter_properties")>
    Public Function filterProperty(obo As GO_OBO, excludes As String()) As GO_OBO
        If excludes.IsNullOrEmpty Then
            Return obo
        End If

        obo = New GO_OBO With {
            .headers = obo.headers,
            .typedefs = obo.typedefs,
            .terms = obo.terms _
                .AsParallel _
                .Select(Function(t)
                            If Not t.property_value.IsNullOrEmpty Then
                                t.property_value = t.property_value _
                                    .Where(Function(str)
                                               Return Not excludes.Any(Function(assert) str.StartsWith(assert))
                                           End Function) _
                                    .ToArray
                            End If

                            Return t
                        End Function) _
                .ToArray
        }

        Return obo
    End Function

    <ExportAPI("write.obo")>
    Public Function saveObo(obo As GO_OBO, path As String, Optional excludes As String() = Nothing) As Boolean
        Using file As Stream = path.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call obo.Save(file, excludes)
        End Using

        Return True
    End Function
End Module

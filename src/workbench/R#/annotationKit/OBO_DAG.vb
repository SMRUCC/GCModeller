#Region "Microsoft.VisualBasic::c332114f4651bc7a1c6598b42c31d46e, R#\annotationKit\OBO_DAG.vb"

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

    '   Total Lines: 212
    '    Code Lines: 160 (75.47%)
    ' Comment Lines: 18 (8.49%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 34 (16.04%)
    '     File Size: 7.39 KB


    ' Module OBO_DAG
    ' 
    '     Function: filterObsolete, filterProperty, getOboTerms, lineage_term, ontologyLeafs
    '               ontologyNodes, ontologyTree, openOboFile, readOboDAG, saveObo
    '               termTable
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures.Tree
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' The Open Biological And Biomedical Ontology (OBO) Foundry
''' </summary>
<Package("OBO")>
Module OBO_DAG

    Sub Main()
        Call Internal.Object.Converts.makeDataframe.addHandler(GetType(Term()), AddressOf termTable)
    End Sub

    Private Function termTable(terms As Term(), args As list, env As Environment) As Object
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = terms.Select(Function(t) t.id).ToArray
        }

        Call df.add("name", terms.Select(Function(t) t.name))
        Call df.add("def", terms.Select(Function(t) t.def))

        Return df
    End Function

    ''' <summary>
    ''' open the ontology obo file reader
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' This obo file reader object could be used as the data source for read 
    ''' other database
    ''' </remarks>
    <ExportAPI("open.obo")>
    <RApiReturn(GetType(OBOFile))>
    Public Function openOboFile(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim buf = SMRUCC.Rsharp.GetFileStream(file, FileAccess.Read, env)

        If buf Like GetType(Message) Then
            Return buf.TryCast(Of Message)
        End If

        Return New OBOFile(buf.TryCast(Of Stream), encoding:=Encodings.UTF8)
    End Function

    ''' <summary>
    ''' parse the gene ontology obo file 
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    <ExportAPI("read.obo")>
    Public Function readOboDAG(path As String) As GO_OBO
        Return GO_OBO.LoadDocument(path)
    End Function

    <ExportAPI("obo_terms")>
    Public Function getOboTerms(obo As GO_OBO) As Term()
        Return obo.terms.ToArray
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
                node.Data = term
            Else
                node = New TermTree(Of Term) With {
                    .Data = term,
                    .label = term.name,
                    .Childs = New Dictionary(Of String, Tree(Of Term, String))
                }
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

    <ExportAPI("ontologyNodes")>
    Public Function ontologyNodes(tree As TermTree(Of Term)) As TermTree(Of Term)()
        Return tree.EnumerateAllChilds _
            .Select(Function(a) DirectCast(a, TermTree(Of Term))) _
            .ToArray
    End Function

    <ExportAPI("ontologyLeafs")>
    Public Function ontologyLeafs(tree As TermTree(Of Term)) As TermTree(Of Term)()
        Dim leafs As New List(Of TermTree(Of Term))
        Dim popAllNodes = tree.EnumerateAllChilds.ToArray

        For Each term As TermTree(Of Term) In popAllNodes
            If term.IsLeaf Then
                Call leafs.Add(term)
            End If
        Next

        Return leafs.ToArray
    End Function

    <ExportAPI("lineage_term")>
    Public Function lineage_term(term As TermTree(Of Term)) As list
        Dim data As New list With {.slots = New Dictionary(Of String, Object)}
        Dim lineage As New List(Of Term)
        Dim node As TermTree(Of Term) = term

        Do While node.Parent IsNot Nothing
            lineage.Add(node.Data)
            node = node.Parent
        Loop

        Call lineage.Add(node.Data)
        Call lineage.Reverse()
        Call data.add("id", term.Data.id)
        Call data.add("name", term.Data.name)
        Call data.add("namespace", term.Data.namespace)
        Call data.add("def", term.Data.def)
        Call data.add("lineage", lineage.ToArray)

        Return data
    End Function

    ''' <summary>
    ''' removes all terms which is ``is_obsolete``
    ''' </summary>
    ''' <param name="obo"></param>
    ''' <returns></returns>
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

    ''' <summary>
    ''' make data filter of the ``property_value`` in a term
    ''' </summary>
    ''' <param name="obo"></param>
    ''' <param name="excludes">
    ''' the property name list for make excludes
    ''' </param>
    ''' <returns></returns>
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

    ''' <summary>
    ''' set remakrs comment text into the header of the obo file object.
    ''' </summary>
    ''' <param name="obo"></param>
    ''' <param name="remarks"></param>
    ''' <param name="append"></param>
    ''' <returns></returns>
    <ExportAPI("set_remarks")>
    Public Function setRemark(obo As GO_OBO, remarks As String(), Optional append As Boolean = True) As GO_OBO
        If obo Is Nothing Then
            Call "the given obo object for set header remarks is nothing.".Warning
            Return Nothing
        ElseIf obo.headers Is Nothing Then
            obo.headers = New header
        End If

        If Not append Then
            obo.headers.Remark = remarks
        Else
            obo.headers.Remark = obo.headers.Remark.JoinIterates(remarks).ToArray
        End If

        Return obo
    End Function

    ''' <summary>
    ''' set property_value list to a term or obo file headers
    ''' 
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="property_value"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("set_propertyValue")>
    Public Function setPropertyValues(x As Object,
                                      Optional append As Boolean = True,
                                      <RListObjectArgument>
                                      Optional property_value As list = Nothing,
                                      Optional env As Environment = Nothing) As Object

        Dim list_vals As Dictionary(Of String, String()) = property_value.AsGeneric(Of String())(env)
        Dim vals As String() = {}

        If list_vals IsNot Nothing Then
            vals = list_vals _
                .Select(Function(p)
                            Return p.Value _
                                .SafeQuery _
                                .Select(Function(val) $"{p.Key} {val}")
                        End Function) _
                .IteratesALL _
                .ToArray
        End If

        If TypeOf x Is GO_OBO Then
            Dim obo As GO_OBO = x

            If obo.headers Is Nothing Then
                obo.headers = New header
            End If
            If append Then
                obo.headers.property_value = obo.headers.property_value _
                    .JoinIterates(vals) _
                    .ToArray
            Else
                obo.headers.property_value = vals
            End If

            Return obo
        ElseIf TypeOf x Is Term Then
            Dim term As Term = x

            If append Then
                term.property_value = term.property_value _
                    .JoinIterates(vals) _
                    .ToArray
            Else
                term.property_value = vals
            End If

            Return term
        Else
            Return Message.InCompatibleType(GetType(GO_OBO), x.GetType, env)
        End If
    End Function

    ''' <summary>
    ''' write ontology file as ascii plant text file
    ''' </summary>
    ''' <param name="obo"></param>
    ''' <param name="path"></param>
    ''' <param name="excludes"></param>
    ''' <returns></returns>
    <ExportAPI("write.obo")>
    Public Function saveObo(obo As GO_OBO, path As String,
                            Optional excludes As String() = Nothing,
                            Optional strip_namespace_prefix As String = Nothing,
                            Optional strip_property_unit As Boolean = False) As Boolean

        Using file As Stream = path.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call obo.Save(file, excludes, strip_namespace_prefix, strip_property_unit)
        End Using

        Return True
    End Function
End Module

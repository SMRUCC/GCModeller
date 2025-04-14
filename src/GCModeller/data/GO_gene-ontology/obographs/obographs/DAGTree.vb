#Region "Microsoft.VisualBasic::7b370870cb10e6537310491ea5443de3, data\GO_gene-ontology\obographs\obographs\DAGTree.vb"

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

    '   Total Lines: 67
    '    Code Lines: 51 (76.12%)
    ' Comment Lines: 4 (5.97%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (17.91%)
    '     File Size: 2.34 KB


    ' Class DAGTree
    ' 
    '     Properties: dag, nodes
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Class DAGTree

    Public Property dag As TermTree(Of Term)

    ''' <summary>
    ''' index by <see cref="OBO.Term.id"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property nodes As Dictionary(Of String, TermTree(Of Term))

    Default Public ReadOnly Property Term(id As String) As TermTree(Of Term)
        Get
            Return nodes.TryGetValue(id)
        End Get
    End Property

    Sub New(obo As GO_OBO, Optional verbose_progress As Boolean = True)
        Dim hash As Dictionary(Of String, Term) = obo.CreateTermTable
        Dim index As New Dictionary(Of String, TermTree(Of Term))

        For Each term As Term In TqdmWrapper.Wrap(hash.Values, wrap_console:=verbose_progress)
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

        nodes = index
        dag = ontology
    End Sub
End Class

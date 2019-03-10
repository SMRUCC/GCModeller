﻿#Region "Microsoft.VisualBasic::1da7bc51b55add5a2de049cd074a3896, data\GO_gene-ontology\GeneOntology\DAG\Graph.vb"

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

    '     Class Graph
    ' 
    '         Properties: header
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: Family, ToString
    '         Structure InheritsChain
    ' 
    '             Properties: [Namespace], Family, Top
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Function: Level, Strip, ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.foundation.OBO_Foundry

Namespace DAG

    ''' <summary>
    ''' GO DAG graph
    ''' </summary>
    Public Class Graph

        ReadOnly __DAG As Dictionary(Of TermNode)
        ReadOnly _file$

        Public ReadOnly Property header As header

        ''' <summary>
        ''' Creates GO DAG graph from ``go.obo`` file.
        ''' </summary>
        ''' <param name="path$">File path of the GO database: ``go.obo``</param>
        Sub New(path$)
            Call Me.New(GO_OBO.LoadDocument(path$).Terms, trace:=path)
        End Sub

        ''' <summary>
        ''' Or build DAG graph tree from a specific GO_term collection <paramref name="terms"/>
        ''' </summary>
        ''' <param name="terms"></param>
        Sub New(terms As IEnumerable(Of Term), <CallerMemberName> Optional trace$ = Nothing)
            __DAG = terms.BuildTree
            _file = trace
        End Sub

        Public Overrides Function ToString() As String
            Return _file.ToFileURL
        End Function

        ''' <summary>
        ''' These terms describe a component of a cell that is part of a larger object, such as an anatomical structure 
        ''' (e.g. rough endoplasmic reticulum or nucleus) or a gene product group (e.g. ribosome, proteasome or a protein dimer).
        ''' </summary>
        Const cellular_component$ = NameOf(cellular_component)
        ''' <summary>
        ''' A biological process term describes a series of events accomplished by one or more organized assemblies of molecular functions. 
        ''' Examples of broad biological process terms are "cellular physiological process" or "signal transduction". Examples of more 
        ''' specific terms are "pyrimidine metabolic process" or "alpha-glucoside transport". The general rule to assist in distinguishing 
        ''' between a biological process and a molecular function is that a process must have more than one distinct steps.
        ''' A biological process Is Not equivalent To a pathway. At present, the GO does Not Try To represent the dynamics Or dependencies 
        ''' that would be required To fully describe a pathway.
        ''' </summary>
        Const biological_process$ = NameOf(biological_process)
        ''' <summary>
        ''' Molecular function terms describes activities that occur at the molecular level, such as "catalytic activity" or "binding activity". 
        ''' GO molecular function terms represent activities rather than the entities (molecules or complexes) that perform the actions, 
        ''' and do not specify where, when, or in what context the action takes place. Molecular functions generally correspond to activities 
        ''' that can be performed by individual gene products, but some activities are performed by assembled complexes of gene products. 
        ''' Examples of broad functional terms are "catalytic activity" and "transporter activity"; examples of narrower functional terms are 
        ''' "adenylate cyclase activity" or "Toll receptor binding".
        ''' It Is easy To confuse a gene product name With its molecular Function; For that reason GO molecular functions are often appended 
        ''' With the word "activity".
        ''' </summary>
        Const molecular_function$ = NameOf(molecular_function)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="id$"><see cref="Term.id"/></param>
        ''' <returns></returns>
        Public Function Family(id As String) As IEnumerable(Of InheritsChain)
            Dim term As TermNode = __DAG(id)

            If term Is Nothing Then
                Return {}
            ElseIf term.is_a.IsNullOrEmpty Then
                Dim break As New InheritsChain With {
                    .Route = New List(Of TermNode) From {term}
                }

                Return {break}
            Else
                Dim routes As New List(Of InheritsChain)

                For Each parent In term.is_a
                    Dim chain As New InheritsChain With {
                        .Route = New List(Of TermNode)
                    }

                    Dim parentChains = Family(parent.term_id).ToArray

                    For Each c As InheritsChain In parentChains
                        c.Route.Insert(0, parent.term)
                        routes.Add(c)
                    Next
                Next

                Return routes
            End If
        End Function

        Public Structure InheritsChain

            Dim Route As List(Of TermNode)

            ReadOnly Tree As TermNode()

            Public ReadOnly Property Top As TermNode
                Get
                    Return Route.Last
                End Get
            End Property

            Public ReadOnly Property [Namespace] As String
                Get
                    Return Top.GO_term.namespace
                End Get
            End Property

            Public ReadOnly Property Family As String()
                Get
                    Return Route _
                        .AsEnumerable _
                        .Reverse _
                        .Select(Function(t) t.GO_term.name) _
                        .ToArray
                End Get
            End Property

            Private Sub New(tree As TermNode())
                Me.Tree = tree
            End Sub

            ''' <summary>
            ''' 这个函数会自动将<paramref name="lv"/>等级减1转换为向量之中的顶点下表值
            ''' </summary>
            ''' <param name="lv%"></param>
            ''' <returns></returns>
            Public Function Level(lv%) As Term
                ' lv 是从1开始的，所以需要在这里减去1才能够转换为数组的下标值
                Return Tree.ElementAtOrDefault(lv - 1)?.GO_term
            End Function

            Public Function Strip() As InheritsChain
                Dim route = Me.Route.Distinct.AsList
                Dim tree = route _
                    .AsEnumerable _
                    .Reverse _
                    .ToArray

                Return New InheritsChain(tree) With {
                    .Route = route
                }
            End Function

            Public Overrides Function ToString() As String
                Return $"[{[Namespace]}] {Family.JoinBy(" -> ")}"
            End Function
        End Structure

        'Public Function Infer(a$, b$) As Relationship

        'End Function
    End Class
End Namespace

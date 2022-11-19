#Region "Microsoft.VisualBasic::5804eb807eb5f38719f4402a12278d47, GCModeller\data\GO_gene-ontology\GeneOntology\Files\Obo\OntologyRelations.vb"

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

    '   Total Lines: 81
    '    Code Lines: 56
    ' Comment Lines: 13
    '   Blank Lines: 12
    '     File Size: 2.96 KB


    '     Class OntologyRelations
    ' 
    '         Properties: is_a
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GenericEnumerator, GetEnumerator, parseRelationships, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Data.GeneOntology.DAG

Namespace OBO

    ''' <summary>
    ''' A Ontology relationship collection.
    ''' </summary>
    Public Class OntologyRelations : Implements Enumeration(Of Relationship)

        ''' <summary>
        ''' ```
        ''' is_a: GO:0048311 ! mitochondrion distribution
        ''' ```
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property is_a As NamedValue(Of String)()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return allRelations _
                    .Where(Function(r) r.type = GeneOntology.OntologyRelations.is_a) _
                    .Select(Function(t) t.parent) _
                    .ToArray
            End Get
        End Property

        ReadOnly allRelations As Relationship()
        ReadOnly term As Term

        ''' <summary>
        ''' 解析出一个新的关系集合
        ''' </summary>
        ''' <param name="term"></param>
        Sub New(term As Term)
            Dim relations As New List(Of Relationship)

            relations += term.is_a _
                .SafeQuery _
                .Select(Function(s) s.GetTagValue(" ! ", trim:=True)) _
                .Select(Function(id)
                            Return New Relationship With {
                                .type = GeneOntology.OntologyRelations.is_a,
                                .parent = id
                            }
                        End Function)
            relations += parseRelationships(term.relationship)

            Me.allRelations = relations
            Me.term = term
        End Sub

        Public Overrides Function ToString() As String
            Return $"{term.id} have {allRelations.Length} relationships."
        End Function

        Private Shared Iterator Function parseRelationships(dataLines As String()) As IEnumerable(Of Relationship)
            For Each line As String In dataLines.SafeQuery
                Dim tokens = line.GetTagValue(trim:=True)
                Dim type = Relationship.relationshipParser(tokens.Name)

                Yield New Relationship With {
                    .type = type,
                    .parent = tokens.Value.GetTagValue(" ! ", trim:=True)
                }
            Next
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of Relationship) Implements Enumeration(Of Relationship).GenericEnumerator
            For Each item As Relationship In allRelations
                Yield item
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of Relationship).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class
End Namespace

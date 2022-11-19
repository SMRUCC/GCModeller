#Region "Microsoft.VisualBasic::bef4d11d681523a9f67dab0d25e7b983, GCModeller\data\GO_gene-ontology\GeneOntology\DAG\Axioms.vb"

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

    '   Total Lines: 72
    '    Code Lines: 43
    ' Comment Lines: 20
    '   Blank Lines: 9
    '     File Size: 2.46 KB


    ' Module Axioms
    ' 
    '     Function: (+2 Overloads) Infer, InferRule
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO

''' <summary>
''' 任意距离的term之间的关系推断规则
''' </summary>
Public Module Axioms

    ''' <summary>
    ''' 推断出两个GO词条<paramref name="a"/>和<paramref name="b"/>之间的关系
    ''' </summary>
    ''' <param name="go">The go database</param>
    ''' <param name="a"><see cref="Term.id"/></param>
    ''' <param name="b"><see cref="Term.id"/></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Infer(go As GO_OBO, a$, b$) As OntologyRelations
        Return go.CreateTermTable.Infer(a, b)
    End Function

    <Extension>
    Public Function Infer(go As Dictionary(Of String, Term), a$, b$) As OntologyRelations
        Dim term As Term = go(a)
        Dim relations As New OBO.OntologyRelations(term)
        Dim relationship As OntologyRelations

        For Each rel As Relationship In relations.AsEnumerable
            If rel.parent.Name = b Then
                Return rel.type
            Else
                relationship = go.Infer(rel.parent.Name, b)
            End If

            If relationship <> OntologyRelations.none Then
                Return Axioms.InferRule(rel.type, relationship)
            End If
        Next

        Return OntologyRelations.none
    End Function

    ReadOnly regulates As Index(Of OntologyRelations) = {
        OntologyRelations.negatively_regulates,
        OntologyRelations.positively_regulates
    }

    ''' <summary>
    ''' 计算出``A -> C``的关系，C是A和B的基础类型
    ''' </summary>
    ''' <param name="from">A -> B</param>
    ''' <param name="[to]">B -> C</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' https://github.com/SMRUCC/GCModeller/blob/master/src/GCModeller/data/GO_gene-ontology/docs/Ontology/Ontology_Relations/README.md
    ''' </remarks>
    Public Function InferRule(from As OntologyRelations, [to] As OntologyRelations) As OntologyRelations
        If from Like regulates AndAlso [to] = OntologyRelations.part_of Then
            Return OntologyRelations.regulates
        End If

        If [to] > from Then
            Return [to]
        Else
            Return from
        End If
    End Function
End Module

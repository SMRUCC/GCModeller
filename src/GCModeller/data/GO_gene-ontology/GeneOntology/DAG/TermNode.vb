#Region "Microsoft.VisualBasic::ebb44c932e84e259bd34aed71b37b3d8, data\GO_gene-ontology\GeneOntology\DAG\TermNode.vb"

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

    '   Total Lines: 34
    '    Code Lines: 18 (52.94%)
    ' Comment Lines: 11 (32.35%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (14.71%)
    '     File Size: 1.20 KB


    '     Class TermNode
    ' 
    '         Properties: [namespace], GO_term, id, is_a, relationship
    '                     synonym, xref
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Namespace DAG

    ''' <summary>
    ''' The DAG node of a specific GO <see cref="Term"/>
    ''' </summary>
    Public Class TermNode : Implements INamedValue

        Public Property xref As NamedValue(Of String)()
        ''' <summary>
        ''' 当前的这个term是子节点，则这个属性内的所有的节点都是这个节点的父节点
        ''' </summary>
        ''' <returns></returns>
        Public Property is_a As is_a()
        Public Property synonym As synonym()
        Public Property relationship As Relationship()

        ''' <summary>
        ''' <see cref="Term.id"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property id As String Implements INamedValue.Key
        Public Property [namespace] As String
        Public Property GO_term As Term

        ''' <summary>
        ''' 获取得到当前的这个节点的所有的父节点引用(合并``is_a``与``relationship``)
        ''' </summary>
        ''' <param name="relations">
        ''' 除了``is_a``之外还需要参与计算的relationship关系类型列表，
        ''' 为空的时候则只使用``is_a``关系
        ''' </param>
        ''' <returns>
        ''' 这里只会返回在DAG图之中真实存在的父节点，悬空的``Nothing``引用会被自动过滤掉
        ''' </returns>
        Public Function AllParents(Optional relations As OntologyRelations() = Nothing) As TermNode()
            Dim list As New List(Of TermNode)

            For Each rel As is_a In Me.is_a
                If rel Is Nothing OrElse rel.term Is Nothing Then
                    Continue For
                End If

                list.Add(rel.term)
            Next

            If Not relations.IsNullOrEmpty Then
                Dim allow As New HashSet(Of OntologyRelations)(relations)

                For Each rel As Relationship In Me.relationship
                    If rel.term Is Nothing Then
                        Continue For
                    End If
                    If Not allow.Contains(rel.type) Then
                        Continue For
                    End If

                    list.Add(rel.term)
                Next
            End If

            Return list.ToArray
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace

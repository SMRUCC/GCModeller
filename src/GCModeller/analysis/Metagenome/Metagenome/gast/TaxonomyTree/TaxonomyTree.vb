#Region "Microsoft.VisualBasic::e65ccdd19918b750c34bef7524b26e51, GCModeller\analysis\Metagenome\Metagenome\gast\TaxonomyTree\TaxonomyTree.vb"

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

    '   Total Lines: 74
    '    Code Lines: 47
    ' Comment Lines: 12
    '   Blank Lines: 15
    '     File Size: 2.32 KB


    '     Class TaxonomyTree
    ' 
    '         Properties: childs, hits, lineage, parent, root
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: BuildTree, PopulateTaxonomy, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Metagenomics

Namespace gast

    ''' <summary>
    ''' Tree node, (<see cref="Taxonomy.consensus(Taxonomy(), Double)"/>对象存在bug，当最低百分比低于50%的时候，
    ''' 会造成重复的同等级分类出现，使用这个树对象来避免这种情况的发生)
    ''' </summary>
    Public Class TaxonomyTree : Inherits Taxonomy

        Public Property childs As New List(Of TaxonomyTree)
        Public Property parent As TaxonomyTree
        Public Property lineage As String

        ''' <summary>
        ''' Count of the hits numbers on this node rank 
        ''' </summary>
        ''' <returns></returns>
        Public Property hits As Integer

        ''' <summary>
        ''' get the tree root
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property root As TaxonomyTree
            Get
                If parent Is Nothing OrElse parent.lineage = "*" Then
                    Return Me
                Else
                    Return parent.root
                End If
            End Get
        End Property

        Sub New(taxonomy As String)
            Call MyBase.New(taxonomy)
        End Sub

        Sub New()
            Call MyBase.New({})
        End Sub

        Friend Sub New(copy As TaxonomyTree)
            For i As Integer = 0 To 7
                Me(i) = copy(i)
            Next

            lineage = copy.lineage
            parent = copy
        End Sub

        Public Function PopulateTaxonomy(level As TaxonomyRanks) As IEnumerable(Of TaxonomyTree)
            Return TreePopulator.PopulateTaxonomy(Me, level)
        End Function

        Public Overrides Function ToString() As String
            Dim lineage$ = MyBase.ToString.Trim(";"c)
            Dim rank As TaxonomyRanks

            Call GetDepth(rank)

            If lineage.StringEmpty AndAlso Me.lineage = "*" Then
                lineage = "*"
            End If

            Return $"{lineage} ({rank.ToString}={hits})"
        End Function

        Public Shared Function BuildTree(hits As IEnumerable(Of Metagenomics.Taxonomy), ByRef taxa_counts%(), ByRef minrank$) As TaxonomyTree
            Return TreeBuilder.BuildTree(hits, taxa_counts, minrank)
        End Function
    End Class
End Namespace

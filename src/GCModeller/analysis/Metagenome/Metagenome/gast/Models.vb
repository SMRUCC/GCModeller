#Region "Microsoft.VisualBasic::5c9c611f9730277c8165c1b98228d1c6, analysis\Metagenome\Metagenome\gast\Models.vb"

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

    '     Class Names
    ' 
    '         Properties: Composition, distance, members, NumOfSeqs, refs
    '                     taxonomy, Unique
    ' 
    '         Function: ToString
    ' 
    '     Class gastOUT
    ' 
    '         Properties: counts, distance, max_pcts, minrank, na_pcts
    '                     rank, read_id, refhvr_ids, refssu_count, taxa_counts
    '                     taxonomy, vote
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Metagenomics

Namespace gast

    ''' <summary>
    ''' *.names
    ''' </summary>
    Public Class Names : Implements INamedValue
        Implements ITaxonomyLineage

        Public Property Unique As String Implements INamedValue.Key
        <Ignored>
        Public Property members As String()
        Public Property NumOfSeqs As Integer
        Public Property taxonomy As String Implements ITaxonomyLineage.Taxonomy
        Public Property distance As Double
        Public Property refs As String
        <Meta>
        Public Property Composition As Dictionary(Of String, String)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class gastOUT : Implements INamedValue
        Implements ITaxonomyLineage

        ''' <summary>
        ''' 可以将这个属性看作为OTU的编号
        ''' </summary>
        ''' <returns></returns>
        Public Property read_id As String Implements INamedValue.Key
        ''' <summary>
        ''' lineage，物种分类信息
        ''' </summary>
        ''' <returns></returns>
        Public Property taxonomy As String Implements ITaxonomyLineage.Taxonomy
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property distance As Double
        Public Property rank As String
        Public Property refssu_count As String
        Public Property vote As String
        Public Property minrank As String
        Public Property taxa_counts As String
        Public Property max_pcts As String
        Public Property na_pcts As String
        Public Property refhvr_ids As String
        ''' <summary>
        ''' 当前的这个OTU的丰度数量
        ''' </summary>
        ''' <returns></returns>
        Public Property counts As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace

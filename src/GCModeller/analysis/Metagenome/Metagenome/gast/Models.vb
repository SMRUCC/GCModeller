#Region "Microsoft.VisualBasic::f3d9d6084337ed45963e863736dedec0, GCModeller\analysis\Metagenome\Metagenome\gast\Models.vb"

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

    '   Total Lines: 75
    '    Code Lines: 41
    ' Comment Lines: 26
    '   Blank Lines: 8
    '     File Size: 2.63 KB


    '     Class Names
    ' 
    '         Properties: composition, distance, members, numOfSeqs, refs
    '                     taxonomy, unique
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
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Metagenomics

Namespace gast

    ''' <summary>
    ''' ``*.names`` 相当于一个OTU数据 
    ''' </summary>
    Public Class Names : Implements INamedValue
        Implements ITaxonomyLineage
        Implements ISample

        Public Property unique As String Implements INamedValue.Key
        <Ignored>
        Public Property members As String()
        Public Property numOfSeqs As Integer
        Public Property taxonomy As String Implements ITaxonomyLineage.Taxonomy
        Public Property distance As Double
        Public Property refs As String

        ''' <summary>
        ''' 当前的这个OTU在不同的样本间的丰度构成 ``[sampleName => abundance]``
        ''' </summary>
        ''' <returns></returns>
        <Meta>
        Public Property composition As Dictionary(Of String, Double) Implements ISample.Samples

        Public Overrides Function ToString() As String
            Return $"{taxonomy}: {composition.GetJson(indent:=True)}"
        End Function
    End Class

    ''' <summary>
    ''' The gast script OTU count result table 
    ''' </summary>
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
        ''' 序列片段的距离
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

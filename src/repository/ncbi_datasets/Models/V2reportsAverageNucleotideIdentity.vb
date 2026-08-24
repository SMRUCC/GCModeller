#Region "Microsoft.VisualBasic::4cd52af71db37064491d5ee5387c6ed6, ncbi_datasets\Models\V2reportsAverageNucleotideIdentity.vb"

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

    '   Total Lines: 65
    '    Code Lines: 23 (35.38%)
    ' Comment Lines: 29 (44.62%)
    '    - Xml Docs: 82.76%
    ' 
    '   Blank Lines: 13 (20.00%)
    '     File Size: 1.91 KB


    '     Class V2reportsAverageNucleotideIdentity
    ' 
    '         Properties: BestAniMatch, Category, Comment, MatchStatus, SubmittedAniMatch
    '                     SubmittedOrganism, SubmittedSpecies, TaxonomyCheckStatus
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsAverageNucleotideIdentity.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsAverageNucleotideIdentity
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsAverageNucleotideIdentity

        ''' <summary>
        ''' taxonomy_check_status 属性
        ''' </summary>
        <Field("taxonomy_check_status")>
        Public Property TaxonomyCheckStatus As Object

        ''' <summary>
        ''' match_status 属性
        ''' </summary>
        <Field("match_status")>
        Public Property MatchStatus As Object

        ''' <summary>
        ''' submitted_organism 属性
        ''' </summary>
        <Field("submitted_organism")>
        Public Property SubmittedOrganism As String

        ''' <summary>
        ''' submitted_species 属性
        ''' </summary>
        <Field("submitted_species")>
        Public Property SubmittedSpecies As String

        ''' <summary>
        ''' category 属性
        ''' </summary>
        <Field("category")>
        Public Property Category As Object

        ''' <summary>
        ''' submitted_ani_match 属性
        ''' </summary>
        <Field("submitted_ani_match")>
        Public Property SubmittedAniMatch As Object

        ''' <summary>
        ''' best_ani_match 属性
        ''' </summary>
        <Field("best_ani_match")>
        Public Property BestAniMatch As Object

        ''' <summary>
        ''' comment 属性
        ''' </summary>
        <Field("comment")>
        Public Property Comment As String

    End Class

End Namespace


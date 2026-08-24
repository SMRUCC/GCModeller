#Region "Microsoft.VisualBasic::b71b127d62f225b0d5a4f13c4aa7b3fb, ncbi_datasets\Models\V2reportsCheckM.vb"

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

    '   Total Lines: 59
    '    Code Lines: 21 (35.59%)
    ' Comment Lines: 26 (44.07%)
    '    - Xml Docs: 80.77%
    ' 
    '   Blank Lines: 12 (20.34%)
    '     File Size: 1.75 KB


    '     Class V2reportsCheckM
    ' 
    '         Properties: CheckmMarkerSet, CheckmMarkerSetRank, CheckmSpeciesTaxId, CheckmVersion, Completeness
    '                     CompletenessPercentile, Contamination
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsCheckM.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsCheckM
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsCheckM

        ''' <summary>
        ''' checkm_marker_set 属性
        ''' </summary>
        <Field("checkm_marker_set")>
        Public Property CheckmMarkerSet As String

        ''' <summary>
        ''' checkm_species_tax_id 属性
        ''' </summary>
        <Field("checkm_species_tax_id")>
        Public Property CheckmSpeciesTaxId As Integer?

        ''' <summary>
        ''' checkm_marker_set_rank 属性
        ''' </summary>
        <Field("checkm_marker_set_rank")>
        Public Property CheckmMarkerSetRank As String

        ''' <summary>
        ''' checkm_version 属性
        ''' </summary>
        <Field("checkm_version")>
        Public Property CheckmVersion As String

        ''' <summary>
        ''' completeness 属性
        ''' </summary>
        <Field("completeness")>
        Public Property Completeness As Single?

        ''' <summary>
        ''' contamination 属性
        ''' </summary>
        <Field("contamination")>
        Public Property Contamination As Single?

        ''' <summary>
        ''' completeness_percentile 属性
        ''' </summary>
        <Field("completeness_percentile")>
        Public Property CompletenessPercentile As Single?

    End Class

End Namespace


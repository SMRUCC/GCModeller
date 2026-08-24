#Region "Microsoft.VisualBasic::30d33b6e240b583f878621d878469c49, ncbi_datasets\Models\V2reportsProtein.vb"

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

    '   Total Lines: 53
    '    Code Lines: 19 (35.85%)
    ' Comment Lines: 23 (43.40%)
    '    - Xml Docs: 78.26%
    ' 
    '   Blank Lines: 11 (20.75%)
    '     File Size: 1.48 KB


    '     Class V2reportsProtein
    ' 
    '         Properties: AccessionVersion, EnsemblProtein, IsoformName, Length, MaturePeptides
    '                     Name
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsProtein.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProtein
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProtein

        ''' <summary>
        ''' accession_version 属性
        ''' </summary>
        <Field("accession_version")>
        Public Property AccessionVersion As String

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <Field("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' isoform_name 属性
        ''' </summary>
        <Field("isoform_name")>
        Public Property IsoformName As String

        ''' <summary>
        ''' ensembl_protein 属性
        ''' </summary>
        <Field("ensembl_protein")>
        Public Property EnsemblProtein As String

        ''' <summary>
        ''' mature_peptides 属性
        ''' </summary>
        <Field("mature_peptides")>
        Public Property MaturePeptides As List(Of Object)

    End Class

End Namespace


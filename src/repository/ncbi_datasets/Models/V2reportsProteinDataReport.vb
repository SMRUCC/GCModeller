#Region "Microsoft.VisualBasic::8e1c7eb2486880f05f0798e98528c5db, ncbi_datasets\Models\V2reportsProteinDataReport.vb"

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

    '   Total Lines: 71
    '    Code Lines: 25 (35.21%)
    ' Comment Lines: 32 (45.07%)
    '    - Xml Docs: 84.38%
    ' 
    '   Blank Lines: 14 (19.72%)
    '     File Size: 2.04 KB


    '     Class V2reportsProteinDataReport
    ' 
    '         Properties: Accession, ConservedDomains, Description, FunctionalSites, GeneId
    '                     IdenticalProteinGroup, Length, ProteinFamilies, TaxId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsProteinDataReport.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsProteinDataReport
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsProteinDataReport

        ''' <summary>
        ''' accession 属性
        ''' </summary>
        <Field("accession")>
        Public Property Accession As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' length 属性
        ''' </summary>
        <Field("length")>
        Public Property Length As Integer?

        ''' <summary>
        ''' gene_id 属性
        ''' </summary>
        <Field("gene_id")>
        Public Property GeneId As Integer?

        ''' <summary>
        ''' identical_protein_group 属性
        ''' </summary>
        <Field("identical_protein_group")>
        Public Property IdenticalProteinGroup As Integer?

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' conserved_domains 属性
        ''' </summary>
        <Field("conserved_domains")>
        Public Property ConservedDomains As List(Of Object)

        ''' <summary>
        ''' functional_sites 属性
        ''' </summary>
        <Field("functional_sites")>
        Public Property FunctionalSites As List(Of Object)

        ''' <summary>
        ''' protein_families 属性
        ''' </summary>
        <Field("protein_families")>
        Public Property ProteinFamilies As List(Of Object)

    End Class

End Namespace


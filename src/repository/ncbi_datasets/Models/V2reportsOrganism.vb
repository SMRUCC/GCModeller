#Region "Microsoft.VisualBasic::8af662e786042583543eb6e539cec8d9, ncbi_datasets\Models\V2reportsOrganism.vb"

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
    '     File Size: 1.81 KB


    '     Class V2reportsOrganism
    ' 
    '         Properties: CommonName, InfraspecificNames, Lineage, OrganismName, PangolinClassification
    '                     SciName, Strain, TaxId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsOrganism.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsOrganism
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsOrganism

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' sci_name 属性
        ''' </summary>
        <Field("sci_name")>
        Public Property SciName As String

        ''' <summary>
        ''' organism_name 属性
        ''' </summary>
        <Field("organism_name")>
        Public Property OrganismName As String

        ''' <summary>
        ''' common_name 属性
        ''' </summary>
        <Field("common_name")>
        Public Property CommonName As String

        ''' <summary>
        ''' lineage 属性
        ''' </summary>
        <Field("lineage")>
        Public Property Lineage As List(Of Object)

        ''' <summary>
        ''' strain 属性
        ''' </summary>
        <Field("strain")>
        Public Property Strain As String

        ''' <summary>
        ''' pangolin_classification 属性
        ''' </summary>
        <Field("pangolin_classification")>
        Public Property PangolinClassification As String

        ''' <summary>
        ''' infraspecific_names 属性
        ''' </summary>
        <Field("infraspecific_names")>
        Public Property InfraspecificNames As Object

    End Class

End Namespace


#Region "Microsoft.VisualBasic::d1a957dabcdd1af14a04a59519c89e93, ncbi_datasets\Models\V2reportsClassification.vb"

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

    '   Total Lines: 83
    '    Code Lines: 29 (34.94%)
    ' Comment Lines: 38 (45.78%)
    '    - Xml Docs: 86.84%
    ' 
    '   Blank Lines: 16 (19.28%)
    '     File Size: 2.16 KB


    '     Class V2reportsClassification
    ' 
    '         Properties: AcellularRoot, Class, Domain, Family, Genus
    '                     Kingdom, Order, Phylum, Realm, Species
    '                     Superkingdom
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsClassification.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsClassification
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsClassification

        ''' <summary>
        ''' superkingdom 属性
        ''' </summary>
        <Field("superkingdom")>
        Public Property Superkingdom As Object

        ''' <summary>
        ''' kingdom 属性
        ''' </summary>
        <Field("kingdom")>
        Public Property Kingdom As Object

        ''' <summary>
        ''' phylum 属性
        ''' </summary>
        <Field("phylum")>
        Public Property Phylum As Object

        ''' <summary>
        ''' class 属性
        ''' </summary>
        <Field("class")>
        Public Property Class As Object

        ''' <summary>
        ''' order 属性
        ''' </summary>
        <Field("order")>
        Public Property Order As Object

        ''' <summary>
        ''' family 属性
        ''' </summary>
        <Field("family")>
        Public Property Family As Object

        ''' <summary>
        ''' genus 属性
        ''' </summary>
        <Field("genus")>
        Public Property Genus As Object

        ''' <summary>
        ''' species 属性
        ''' </summary>
        <Field("species")>
        Public Property Species As Object

        ''' <summary>
        ''' domain 属性
        ''' </summary>
        <Field("domain")>
        Public Property Domain As Object

        ''' <summary>
        ''' realm 属性
        ''' </summary>
        <Field("realm")>
        Public Property Realm As Object

        ''' <summary>
        ''' acellular_root 属性
        ''' </summary>
        <Field("acellular_root")>
        Public Property AcellularRoot As Object

    End Class

End Namespace


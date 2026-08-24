#Region "Microsoft.VisualBasic::bec5d1a778d3251df607aef0568f52d0, ncbi_datasets\Models\V2reportsBiocollectionInstitution.vb"

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
    '     File Size: 1.46 KB


    '     Class V2reportsBiocollectionInstitution
    ' 
    '         Properties: Address, BioCollections, Comments, Country, Name
    '                     Url
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsBiocollectionInstitution.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsBiocollectionInstitution
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsBiocollectionInstitution

        ''' <summary>
        ''' name 属性
        ''' </summary>
        <Field("name")>
        Public Property Name As String

        ''' <summary>
        ''' country 属性
        ''' </summary>
        <Field("country")>
        Public Property Country As String

        ''' <summary>
        ''' address 属性
        ''' </summary>
        <Field("address")>
        Public Property Address As String

        ''' <summary>
        ''' url 属性
        ''' </summary>
        <Field("url")>
        Public Property Url As String

        ''' <summary>
        ''' comments 属性
        ''' </summary>
        <Field("comments")>
        Public Property Comments As String

        ''' <summary>
        ''' bio_collections 属性
        ''' </summary>
        <Field("bio_collections")>
        Public Property BioCollections As List(Of Object)

    End Class

End Namespace


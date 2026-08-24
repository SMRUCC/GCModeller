#Region "Microsoft.VisualBasic::a6986c1cee727cd1849d840c291314b2, ncbi_datasets\Models\V2reportsInfraspecificNames.vb"

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
    '     File Size: 1.41 KB


    '     Class V2reportsInfraspecificNames
    ' 
    '         Properties: Breed, Cultivar, Ecotype, Isolate, Sex
    '                     Strain
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsInfraspecificNames.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsInfraspecificNames
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsInfraspecificNames

        ''' <summary>
        ''' breed 属性
        ''' </summary>
        <Field("breed")>
        Public Property Breed As String

        ''' <summary>
        ''' cultivar 属性
        ''' </summary>
        <Field("cultivar")>
        Public Property Cultivar As String

        ''' <summary>
        ''' ecotype 属性
        ''' </summary>
        <Field("ecotype")>
        Public Property Ecotype As String

        ''' <summary>
        ''' isolate 属性
        ''' </summary>
        <Field("isolate")>
        Public Property Isolate As String

        ''' <summary>
        ''' sex 属性
        ''' </summary>
        <Field("sex")>
        Public Property Sex As String

        ''' <summary>
        ''' strain 属性
        ''' </summary>
        <Field("strain")>
        Public Property Strain As String

    End Class

End Namespace


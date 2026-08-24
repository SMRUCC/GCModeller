#Region "Microsoft.VisualBasic::50d7048a29e2a2c24f9a3b1c549cd921, ncbi_datasets\Models\Ncbiprotddv2QueryStructureDefinition.vb"

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
    '     File Size: 1.75 KB


    '     Class Ncbiprotddv2QueryStructureDefinition
    ' 
    '         Properties: ChainId, Description, DomainNumber, From, MmdbId
    '                     PdbId, Sdid, To
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' Ncbiprotddv2QueryStructureDefinition.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2QueryStructureDefinition
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2QueryStructureDefinition

        ''' <summary>
        ''' sdid 属性
        ''' </summary>
        <Field("sdid")>
        Public Property Sdid As Integer?

        ''' <summary>
        ''' mmdb_id 属性
        ''' </summary>
        <Field("mmdb_id")>
        Public Property MmdbId As Integer?

        ''' <summary>
        ''' pdb_id 属性
        ''' </summary>
        <Field("pdb_id")>
        Public Property PdbId As String

        ''' <summary>
        ''' description 属性
        ''' </summary>
        <Field("description")>
        Public Property Description As String

        ''' <summary>
        ''' chain_id 属性
        ''' </summary>
        <Field("chain_id")>
        Public Property ChainId As String

        ''' <summary>
        ''' domain_number 属性
        ''' </summary>
        <Field("domain_number")>
        Public Property DomainNumber As Integer?

        ''' <summary>
        ''' from 属性
        ''' </summary>
        <Field("from")>
        Public Property From As Integer?

        ''' <summary>
        ''' to 属性
        ''' </summary>
        <Field("to")>
        Public Property To As Integer?

    End Class

End Namespace


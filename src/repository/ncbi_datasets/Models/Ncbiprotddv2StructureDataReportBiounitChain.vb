#Region "Microsoft.VisualBasic::675110f5ae3d47ded9bce68381571618, ncbi_datasets\Models\Ncbiprotddv2StructureDataReportBiounitChain.vb"

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

    '   Total Lines: 47
    '    Code Lines: 17 (36.17%)
    ' Comment Lines: 20 (42.55%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 10 (21.28%)
    '     File Size: 1.33 KB


    '     Class Ncbiprotddv2StructureDataReportBiounitChain
    ' 
    '         Properties: ChainId, Kind, MoleculeGroup, Sdid, TaxId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' Ncbiprotddv2StructureDataReportBiounitChain.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2StructureDataReportBiounitChain
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2StructureDataReportBiounitChain

        ''' <summary>
        ''' chain_id 属性
        ''' </summary>
        <Field("chain_id")>
        Public Property ChainId As String

        ''' <summary>
        ''' tax_id 属性
        ''' </summary>
        <Field("tax_id")>
        Public Property TaxId As Integer?

        ''' <summary>
        ''' kind 属性
        ''' </summary>
        <Field("kind")>
        Public Property Kind As Object

        ''' <summary>
        ''' molecule_group 属性
        ''' </summary>
        <Field("molecule_group")>
        Public Property MoleculeGroup As Integer?

        ''' <summary>
        ''' sdid 属性
        ''' </summary>
        <Field("sdid")>
        Public Property Sdid As Integer?

    End Class

End Namespace


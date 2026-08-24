#Region "Microsoft.VisualBasic::9dd71ac0eee7df352fe9d8865a218dec, ncbi_datasets\Models\Ncbiprotddv2StructureDataReportLigandChain.vb"

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

    '   Total Lines: 41
    '    Code Lines: 15 (36.59%)
    ' Comment Lines: 17 (41.46%)
    '    - Xml Docs: 70.59%
    ' 
    '   Blank Lines: 9 (21.95%)
    '     File Size: 1.15 KB


    '     Class Ncbiprotddv2StructureDataReportLigandChain
    ' 
    '         Properties: ChainId, Kind, Sdid, Sid
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' Ncbiprotddv2StructureDataReportLigandChain.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2StructureDataReportLigandChain
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2StructureDataReportLigandChain

        ''' <summary>
        ''' chain_id 属性
        ''' </summary>
        <Field("chain_id")>
        Public Property ChainId As String

        ''' <summary>
        ''' kind 属性
        ''' </summary>
        <Field("kind")>
        Public Property Kind As Object

        ''' <summary>
        ''' sid 属性
        ''' </summary>
        <Field("sid")>
        Public Property Sid As Integer?

        ''' <summary>
        ''' sdid 属性
        ''' </summary>
        <Field("sdid")>
        Public Property Sdid As Integer?

    End Class

End Namespace


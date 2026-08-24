#Region "Microsoft.VisualBasic::c6e58f420ccfac6c78514b7fe1d26e53, ncbi_datasets\Models\Ncbiprotddv2StructureDataReportExperiment.vb"

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

    '   Total Lines: 29
    '    Code Lines: 11 (37.93%)
    ' Comment Lines: 11 (37.93%)
    '    - Xml Docs: 54.55%
    ' 
    '   Blank Lines: 7 (24.14%)
    '     File Size: 921 B


    '     Class Ncbiprotddv2StructureDataReportExperiment
    ' 
    '         Properties: ExperimentalMethod, Resolution
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' Ncbiprotddv2StructureDataReportExperiment.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: ncbiprotddv2StructureDataReportExperiment
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class Ncbiprotddv2StructureDataReportExperiment

        ''' <summary>
        ''' experimental_method 属性
        ''' </summary>
        <Field("experimental_method")>
        Public Property ExperimentalMethod As String

        ''' <summary>
        ''' resolution 属性
        ''' </summary>
        <Field("resolution")>
        Public Property Resolution As Single?

    End Class

End Namespace


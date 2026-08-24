#Region "Microsoft.VisualBasic::1a641db393c2bd2ed9cf127668bdc91e, ncbi_datasets\Models\V2VersionReply.vb"

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
    '     File Size: 1.11 KB


    '     Class V2VersionReply
    ' 
    '         Properties: MajorVer, MinorVer, PatchVer, Version
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2VersionReply.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2VersionReply
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2VersionReply

        ''' <summary>
        ''' version 属性
        ''' </summary>
        <Field("version")>
        Public Property Version As String

        ''' <summary>
        ''' major_ver 属性
        ''' </summary>
        <Field("major_ver")>
        Public Property MajorVer As Integer?

        ''' <summary>
        ''' minor_ver 属性
        ''' </summary>
        <Field("minor_ver")>
        Public Property MinorVer As Integer?

        ''' <summary>
        ''' patch_ver 属性
        ''' </summary>
        <Field("patch_ver")>
        Public Property PatchVer As Integer?

    End Class

End Namespace


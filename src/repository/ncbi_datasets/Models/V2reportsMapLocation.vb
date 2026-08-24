#Region "Microsoft.VisualBasic::ca93a0bc3fbc7e65a855dd74c1d15b64, ncbi_datasets\Models\V2reportsMapLocation.vb"

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
    '     File Size: 820 B


    '     Class V2reportsMapLocation
    ' 
    '         Properties: MapType, MapValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' V2reportsMapLocation.vb
' 自动生成的模型类 - 基于 OpenAPI 3.0.1 规范
' 源 Schema: v2reportsMapLocation
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace ncbi_datasets.Models

    Public Class V2reportsMapLocation

        ''' <summary>
        ''' map_type 属性
        ''' </summary>
        <Field("map_type")>
        Public Property MapType As Object

        ''' <summary>
        ''' map_value 属性
        ''' </summary>
        <Field("map_value")>
        Public Property MapValue As String

    End Class

End Namespace


#Region "Microsoft.VisualBasic::76fe58e03bf8c700eb4bff66c548b43e, visualize\Cytoscape\Cytoscape.Automation\Models.vb"

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

    ' Class NetworkReference
    ' 
    '     Properties: networkSUID, source
    ' 
    ' Class FileReference
    ' 
    '     Properties: ndex_uuid, source_location, source_method
    ' 
    ' Enum formats
    ' 
    '     cx, egdeList, json
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region


' [{"source":"http://localhost:8887/tmp0000b/upload.json","networkSUID":[445]}]"

Public Class NetworkReference
    Public Property source As String
    Public Property networkSUID As String
End Class

''' <summary>
''' local file reference for cytoscape automation
''' </summary>
Public Class FileReference
    Public Property source_location As String
    Public Property source_method As String = "GET"
    Public Property ndex_uuid As String = "12345"
End Class


Public Enum formats
    ''' <summary>
    ''' SIF format
    ''' </summary>
    egdeList
    ''' <summary>
    ''' cx format
    ''' </summary>
    cx
    ''' <summary>
    ''' cytoscape.js format
    ''' </summary>
    json
End Enum

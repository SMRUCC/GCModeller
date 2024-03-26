#Region "Microsoft.VisualBasic::6b229f0be9baa7c4b7571e53dab42b5f, data\Reactome\WebServices\RESTfulAPI.vb"

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

'     Class APIQueryBuilder
' 
' 
' 
' 
' /********************************************************************************/

#End Region

'http://reactomews.oicr.on.ca:8080/ReactomeRESTfulAPI/ReactomeRESTFulAPI.html

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace RESTfulAPI

    Public Module APIQueryBuilder

        Public Function GetPathway(id As String) As PathwayData
            Dim url As String = $"https://reactome.org/ContentService/data/query/{id}"
            Dim json_str As String = url.GET
            Dim data As PathwayData = json_str.LoadJSON(Of PathwayData)

            Return data
        End Function
    End Module

    Public Class PathwayData

    End Class
End Namespace

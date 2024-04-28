#Region "Microsoft.VisualBasic::e76345c86c0e52ff32f75f1be07f20e6, G:/GCModeller/src/GCModeller/data/Reactome//WebServices/RESTfulAPI.vb"

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

    '   Total Lines: 73
    '    Code Lines: 48
    ' Comment Lines: 5
    '   Blank Lines: 20
    '     File Size: 2.25 KB


    '     Module APIQueryBuilder
    ' 
    '         Function: GetPathway
    ' 
    '     Class ReactomeObject
    ' 
    '         Properties: className, dbId, displayName, schemaClass, summation
    ' 
    '         Function: TryGetDescription
    ' 
    '     Class PathwayData
    ' 
    '         Properties: compartment, isInDisease, isInferred, name, releaseDate
    '                     speciesName, stId, stIdVersion
    ' 
    '     Class compartment
    ' 
    '         Properties: accession, databaseName, definition, name, url
    ' 
    '     Class summation
    ' 
    '         Properties: text
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'http://reactomews.oicr.on.ca:8080/ReactomeRESTfulAPI/ReactomeRESTFulAPI.html

Imports Microsoft.VisualBasic.Linq
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

    Public MustInherit Class ReactomeObject

        Public Property dbId As String
        Public Property displayName As String
        Public Property className As String
        Public Property schemaClass As String
        Public Property summation As summation()

        ''' <summary>
        ''' try to get description text from <see cref="summation"/>.
        ''' </summary>
        ''' <returns></returns>
        Public Function TryGetDescription() As String
            Dim sb As New List(Of String)

            For Each desc As summation In summation.SafeQuery
                If Not desc.text Is Nothing Then
                    Call sb.Add(desc.text)
                End If
            Next

            Return sb.JoinBy(vbCrLf)
        End Function

    End Class

    Public Class PathwayData : Inherits ReactomeObject

        Public Property stId As String
        Public Property stIdVersion As String
        Public Property isInDisease As Boolean
        Public Property isInferred As Boolean
        Public Property name As String()
        Public Property releaseDate As String
        Public Property speciesName As String
        Public Property compartment As compartment()

    End Class

    Public Class compartment : Inherits ReactomeObject

        Public Property accession As String
        Public Property databaseName As String
        Public Property definition As String
        Public Property name As String
        Public Property url As String

    End Class

    Public Class summation : Inherits ReactomeObject

        Public Property text As String

    End Class
End Namespace

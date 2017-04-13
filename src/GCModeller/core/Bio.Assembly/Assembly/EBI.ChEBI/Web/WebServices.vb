#Region "Microsoft.VisualBasic::def317aa43e8b1b5a01c5f4999681394, ..\core\Bio.Assembly\Assembly\EBI.ChEBI\WebServices.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.FileIO

Namespace Assembly.EBI.ChEBI.WebServices

    ''' <summary>
    ''' The main aim of ChEBI Web Services is to provide programmatic access 
    ''' to the ChEBI database in order to aid our users in integrating ChEBI 
    ''' into their applications. Web Services is an integration technology. 
    ''' To ensure software from various sources work well together, this 
    ''' technology is built on open standards such as Simple Object Access 
    ''' Protocol (SOAP), a messaging protocol for transporting information, 
    ''' Web Services Description Language (WSDL), a standard method of describing 
    ''' Web Services and their capabilities. For the transport layer itself, 
    ''' Web Services utilise most of the commonly available network protocols, 
    ''' especially Hypertext Transfer Protocol (HTTP).
    '''
    ''' ChEBI provides SOAP access To its database. If you just wish To obtain 
    ''' light weight ontology objects you can use the Ontology Lookup Service 
    ''' As alternative Web Services.
    ''' 
    ''' > https://www.ebi.ac.uk/chebi/webServices.do
    ''' </summary>
    Public Module WebServices

        ''' <summary>
        ''' Retrieves the complete entity including synonyms, database links and chemical structures, using the ChEBI identifier.
        ''' </summary>
        ''' <param name="chebiId$"></param>
        ''' <returns></returns>
        Public Function GetCompleteEntity(chebiId$) As ChEBIEntity()
            Dim url$ = $"http://www.ebi.ac.uk/webservices/chebi/2.0/test/getCompleteEntity?chebiId={chebiId}"
            Dim xml$ = url.GET
            Dim out As ChEBIEntity() = REST.ParsingRESTData(xml)
            Return out
        End Function

        ''' <summary>
        ''' 查询在线数据或者读取本地的缓存数据
        ''' </summary>
        ''' <param name="chebiID$"></param>
        ''' <param name="localCache$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function QueryChEBI(chebiID$, localCache$) As ChEBIEntity()
            Dim path = localCache.TheFile($"{chebiID}.XML", SearchOption.SearchAllSubDirectories)
            If path Is Nothing Then
                path = localCache & $"/{chebiID}.XML"
            End If
            If path.FileExists(ZERO_Nonexists:=True) Then
                Return path.LoadXml(Of ChEBIEntity())
            Else
                Dim data = GetCompleteEntity(chebiID)
                Call data.GetXml.SaveTo(path)
                Call Thread.Sleep(2000)
                Return data
            End If
        End Function
    End Module
End Namespace

#Region "Microsoft.VisualBasic::8c3edeff98a6595cb3ca708b98b07391, GCModeller\data\SABIO-RK\docuRESTfulWeb\docuRESTfulWeb.vb"

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

    '   Total Lines: 52
    '    Code Lines: 28
    ' Comment Lines: 17
    '   Blank Lines: 7
    '     File Size: 2.01 KB


    ' Module docuRESTfulWeb
    ' 
    '     Function: getModelById, searchKineticLaws, searchKineticLawsRawXml
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports sbXML = SMRUCC.genomics.Model.SBML.Level3.XmlFile(Of SMRUCC.genomics.Data.SABIORK.SBML.SBMLReaction)

Public Module docuRESTfulWeb

    ''' <summary>
    ''' Get a single kinetic law entry by SABIO entry ID
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    Public Function getModelById(id As String,
                                 Optional level As Integer = 2,
                                 Optional version As Integer = 3,
                                 Optional annotation As String = "identifier") As SabiorkSBML

        Dim url = SabiorkSBML.URL_SABIORK_KINETIC_LAWS_QUERY & id
        Dim sbml As String = url.GET

        Return sbml.LoadXml(Of SabiorkSBML)
    End Function

    ''' <summary>
    ''' Search for SABIO kinetic law entries by a query string
    ''' </summary>
    ''' <param name="q"></param>
    ''' <returns></returns>
    Public Function searchKineticLawsRawXml(q As Dictionary(Of QueryFields, String), Optional cache$ = "./") As String
        Static api As New Dictionary(Of String, ModelQuery)

        ' do file data query
        Return api.ComputeIfAbsent(
            key:=cache,
            lazyValue:=Function()
                           Return New ModelQuery(cache)
                       End Function) _
                  .QueryCacheText(q)
    End Function

    ''' <summary>
    ''' Search for SABIO kinetic law entries by a query string
    ''' </summary>
    ''' <param name="q"></param>
    ''' <returns></returns>
    Public Function searchKineticLaws(q As Dictionary(Of QueryFields, String), Optional cache$ = "./") As sbXML
        Static api As New Dictionary(Of String, ModelQuery)

        ' do file data query
        Return api.ComputeIfAbsent(
            key:=cache,
            lazyValue:=Function()
                           Return New ModelQuery(cache)
                       End Function) _
                  .Query(Of sbXML)(q)
    End Function
End Module

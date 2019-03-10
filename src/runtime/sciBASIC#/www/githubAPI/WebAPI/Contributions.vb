﻿#Region "Microsoft.VisualBasic::a5c1f767348c6c2e5278a432f87c6c64, www\githubAPI\WebAPI\Contributions.vb"

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

    '     Module Contributions
    ' 
    '         Function: GetUserContributions
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml

Namespace WebAPI

    Public Module Contributions

        ''' <summary>
        ''' https://github.com/users/xieguigang/contributions
        ''' </summary>
        ''' <param name="userName$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function GetUserContributions(userName$) As Dictionary(Of Date, Integer)
            Dim url$ = $"https://github.com/users/{userName}/contributions"
            Dim svg$ = url.GET
            Dim xml As New XmlDocument
            Call xml.LoadXml("<?xml version=""1.0"" encoding=""utf-8""?>" & vbCrLf & svg)
            Dim g As XmlNodeList = xml _
                .SelectSingleNode("svg") _
                .SelectSingleNode("g") _
                .SelectNodes("g")
            Dim contributions As New Dictionary(Of Date, Integer)

            For Each week As XmlNode In g
                Dim days = week.SelectNodes("rect")

                For Each day As XmlNode In days
                    Dim date$ = day.Attributes.GetNamedItem("data-date").InnerText
                    Dim count = day.Attributes.GetNamedItem("data-count").InnerText
                    contributions(DateTime.Parse([date])) = CInt(count)
                Next
            Next

            Return contributions
        End Function
    End Module
End Namespace

﻿#Region "Microsoft.VisualBasic::b05bbfcdc9026671bc697308d229a5a6, data\RegulonDatabase\Regprecise\WebServices\WebParser\Operon\Operon.vb"

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

    '   Total Lines: 63
    '    Code Lines: 47 (74.60%)
    ' Comment Lines: 4 (6.35%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 12 (19.05%)
    '     File Size: 2.08 KB


    '     Class Operon
    ' 
    '         Properties: ID, members, note
    ' 
    '         Function: getCollection, getSize, PageParser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Regprecise

    ''' <summary>
    ''' Operon that regulated in a regulon
    ''' </summary>
    ''' 
    <XmlType("operon")> Public Class Operon : Inherits ListOf(Of RegulatedGene)

        <XmlAttribute("id")>
        Public Property ID As String

        <XmlElement("member")>
        Public Property members As RegulatedGene()

        <XmlText>
        Public Property note As String

        Public Overrides Function ToString() As String
            With members _
                .Where(Function(g)
                           Return Not g.name.StringEmpty
                       End Function) _
                .Select(Function(g) g.name) _
                .ToArray

                If Not .IsNullOrEmpty Then
                    Return .GetJson
                Else
                    Return members _
                        .Select(Function(g) g.locusId) _
                        .ToArray _
                        .GetJson
                End If
            End With
        End Function

        Public Shared Function PageParser(url As String, Optional cache$ = "./.regprecise/operons/") As Operon()
            Static query As New Dictionary(Of String, OperonQuery)

            Dim webApi As OperonQuery = query.ComputeIfAbsent(
                key:=cache,
                lazyValue:=Function()
                               Return New OperonQuery(cache,,)
                           End Function)

            Return webApi.Query(Of Operon())(url, ".html")
        End Function

        Protected Overrides Function getSize() As Integer
            Return members.Length
        End Function

        Protected Overrides Function getCollection() As IEnumerable(Of RegulatedGene)
            Return members
        End Function
    End Class
End Namespace

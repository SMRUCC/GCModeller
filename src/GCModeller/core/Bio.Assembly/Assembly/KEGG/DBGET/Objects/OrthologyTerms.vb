﻿#Region "Microsoft.VisualBasic::f08657d6bb658699655a40ce1de490ad, core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\OrthologyTerms.vb"

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

    '   Total Lines: 60
    '    Code Lines: 43
    ' Comment Lines: 8
    '   Blank Lines: 9
    '     File Size: 2.05 KB


    '     Class OrthologyTerms
    ' 
    '         Properties: EntityList, Terms
    ' 
    '         Function: FromTerms, getCollection, getSize, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports XmlProperty = Microsoft.VisualBasic.Text.Xml.Models.Property

Namespace Assembly.KEGG.DBGET.bGetObject

    <XmlType("Orthology-terms", [Namespace]:=OrthologyTerms.Xmlns)>
    Public Class OrthologyTerms : Inherits ListOf(Of XmlProperty)

        Public Const Xmlns$ = "http://GCModeller.org/core/KEGG/Model/OrthologyTerm.xsd"

        ''' <summary>
        ''' A collection of term id in array <see cref="Terms"/>
        ''' </summary>
        ''' <returns></returns>
        <XmlIgnore>
        Public ReadOnly Property EntityList As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return Terms.SafeQuery.Keys
            End Get
        End Property

        ''' <summary>
        ''' The KO terms?
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("terms")>
        Public Property Terms As XmlProperty()

        Public Overrides Function ToString() As String
            Return EntityList.GetJson
        End Function

        Public Shared Function FromTerms(terms As IEnumerable(Of NamedValue)) As OrthologyTerms
            Return New OrthologyTerms With {
                .Terms = terms _
                    .SafeQuery _
                    .Select(Function(t) New XmlProperty(t)) _
                    .ToArray
            }
        End Function

        Protected Overrides Function getSize() As Integer
            If Terms.IsNullOrEmpty Then
                Return 0
            Else
                Return Terms.Length
            End If
        End Function

        Protected Overrides Function getCollection() As IEnumerable(Of XmlProperty)
            Return Terms.SafeQuery
        End Function
    End Class
End Namespace

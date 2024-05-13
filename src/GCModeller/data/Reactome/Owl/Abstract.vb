#Region "Microsoft.VisualBasic::356d83a37ce7208aa00ad50dc8e89be3, data\Reactome\Owl\Abstract.vb"

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

    '   Total Lines: 53
    '    Code Lines: 44
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 1.63 KB


    '     Class ResourceElement
    ' 
    '         Properties: ResourceId
    ' 
    '         Function: ToString
    ' 
    '     Class Node
    ' 
    '         Properties: Comments, DataSource, Xref
    ' 
    '     Class RDFresource
    ' 
    '         Properties: ResourceId
    ' 
    '         Function: GetResourceId, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace OwlDocument.Abstract

    Public MustInherit Class ResourceElement
        Implements INamedValue

        <XmlAttribute("ID")>
        Public Property ResourceId As String Implements INamedValue.Key

        Public Overrides Function ToString() As String
            Return ResourceId
        End Function
    End Class

    Public MustInherit Class Node : Inherits ResourceElement
        <XmlElement("xref")>
        Public Property Xref As RDFresource()
        <XmlElement("dataSource")>
        Public Property DataSource As RDFresource
        <XmlElement("comment")>
        Public Property Comments As String()
    End Class

    <XmlType("resource")>
    Public Class RDFresource
        <XmlAttribute("resource")> Public Property ResourceId As String
            Get
                Return _ResourceId
            End Get
            Set(value As String)
                _ResourceId = value
                If _ResourceId.First = "#"c Then
                    __ResourceId = Mid(value, 2)
                Else
                    __ResourceId = value
                End If
            End Set
        End Property

        Dim __ResourceId As String
        Dim _ResourceId As String

        Public Function GetResourceId() As String
            Return __ResourceId
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("resource:= '{0}'", ResourceId)
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::51c9ea5f2b193774d17471479f80437c, ..\GCModeller\data\Reactome\Owl\Xref.vb"

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

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Data.Reactome.OwlDocument.Abstract

Namespace OwlDocument.XrefNodes

    Public MustInherit Class Xref : Inherits ResourceElement
        Public Property db As String
        Public Property id As String
    End Class

    Public Class UnificationXref : Inherits Xref
        Public Property comment As String
        Public Property idVersion As String
    End Class

    Public Class PublicationXref : Inherits Xref
        Public Property year As String
        Public Property title As String
        <XmlElement> Public Property author As String()
        Public Property source As String
    End Class

    Public Class RelationshipXref : Inherits Xref
        Public Property comment As String
        Public Property relationshipType As RDFresource
    End Class
End Namespace

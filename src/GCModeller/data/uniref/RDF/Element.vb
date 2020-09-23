#Region "Microsoft.VisualBasic::9f7e052ae5e7b99761d272d378d49819, data\uniref\RDF\Element.vb"

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

    ' Class Element
    ' 
    '     Properties: commonTaxon, identity, label, length, member
    '                 memberOf, mnemonic, modified, organism, representativeFor
    '                 reviewed, seedFor, sequenceFor, someMembersClassifiedWith, type
    '                 value
    ' 
    ' Class UniRefRDF
    ' 
    '     Properties: data
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

Public Class Element : Inherits Description

    <XmlElement> Public Property member As Resource()
    <XmlElement> Public Property someMembersClassifiedWith As Resource()
    <XmlElement> Public Property commonTaxon As Resource
    <XmlElement> Public Property sequenceFor As Resource()
    <XmlElement> Public Property memberOf As Resource()

    Public Property modified As DataValue
    Public Property identity As DataValue

    Public Property label As String
    Public Property type As Resource
    Public Property length As DataValue
    Public Property value As String

    Public Property seedFor As Resource
    Public Property representativeFor As Resource
    Public Property reviewed As DataValue
    Public Property mnemonic As String
    Public Property organism As Resource

End Class

Public Class UniRefRDF : Inherits RDF(Of Element)

    <XmlElement("Description", [Namespace]:=RDFEntity.XmlnsNamespace)>
    Public Property data As Element()
End Class

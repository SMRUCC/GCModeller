#Region "Microsoft.VisualBasic::7152eef05000ec86f7c70d1b1412c900, GCModeller.Framework.Kernel_Driver\DataServices\ModelBase\Property.vb"

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

    '     Class [Property]
    ' 
    '         Properties: [CompiledDate], Authors, Comment, DBLinks, Emails
    '                     GUID, Name, Publications, Reversion, SpecieId
    '                     Title, URLs
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

Namespace LDM

    <XmlType("GCML_Document_PropertyValue", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements.ComponentsModel/")>
    Public Class [Property]

        <XmlAttribute> Public Property Name As String
        <XmlElement> Public Property Authors As List(Of String)
        <XmlElement> Public Property Comment As String
        <XmlAttribute> Public Property [CompiledDate] As String
        <XmlElement> Public Property SpecieId As String
        <XmlElement> Public Property Title As String
        <XmlElement> Public Property Emails As List(Of String)
        <XmlAttribute> Public Property Reversion As Integer
        <XmlElement("I_GUID", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements.ComponentsModel/")>
        Public Property GUID As String
        <XmlElement> Public Property Publications As List(Of String)
        <XmlElement> Public Property URLs As List(Of String)
        <XmlElement("DB-xRefLinks", Namespace:="http://code.google.com/p/genome-in-code/GCMarkupLanguage/GCML_Documents.XmlElements.ComponentsModel/")>
        Public Property DBLinks As String()

        Sub New()
            Authors = New List(Of String) From {My.Computer.Name}
            CompiledDate = Now.ToString
            Emails = New List(Of String)
            GUID = System.Guid.NewGuid.ToString
            Publications = New List(Of String)
            URLs = New List(Of String) From {"http://code.google.com/p/genome-in-code"}
            DBLinks = New String() {}
        End Sub

        Public Overrides Function ToString() As String
            Return SpecieId
        End Function
    End Class
End Namespace

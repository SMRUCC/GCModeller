#Region "Microsoft.VisualBasic::cdc6ffaa2a967edaa0d599f2e4ccdb3e, data\Xfam\Pfam\PfamFamily.vb"

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

    '     Class PfamFamily
    ' 
    '         Properties: Entries, EntryCount, Release, ReleaseDate
    ' 
    '     Class PfamObject
    ' 
    '         Properties: Description, Name
    ' 
    '         Function: ToString
    ' 
    '     Class Entry
    ' 
    '         Properties: AccID, AdditionalFields, Authors, Dates, ID
    '                     Xrefs
    ' 
    '     Class [Date]
    ' 
    '         Properties: Type, value
    ' 
    '         Function: ToString
    ' 
    '     Class Field
    ' 
    '         Properties: Name, value
    ' 
    '         Function: ToString
    ' 
    '     Class CrossRef
    ' 
    '         Properties: DbKey, DbName
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace SiteSearch

    ''' <summary>
    ''' ftp://ftp.ebi.ac.uk/pub/databases/Pfam/sitesearch/PfamFamily.xml.gz
    ''' </summary>
    ''' 
    <XmlType("database")>
    Public Class PfamFamily : Inherits PfamObject
        <XmlElement("release")> Public Property Release As String
        <XmlElement("release_date")> Public Property ReleaseDate As String
        <XmlElement("entry_count")> Public Property EntryCount As Integer
        <XmlArray("entries")> Public Property Entries As Entry()
    End Class

    Public MustInherit Class PfamObject
        <XmlElement("name")> Public Property Name As String
        <XmlElement("description")> Public Property Description As String

        Public Overrides Function ToString() As String
            Return $"{Name}:  {Description}"
        End Function
    End Class

    <XmlType("entry")>
    Public Class Entry : Inherits PfamObject
        <XmlAttribute("id")> Public Property ID As String
        <XmlAttribute("acc")> Public Property AccID As String
        <XmlAttribute("authors")> Public Property Authors As String
        <XmlArray("dates")> Public Property Dates As [Date]()
        <XmlArray("additional_fields")> Public Property AdditionalFields As Field()
        <XmlArray("cross_references")> Public Property Xrefs As CrossRef()
    End Class

    <XmlType("date")>
    Public Class [Date]
        <XmlAttribute("type")> Public Property Type As String
        <XmlAttribute("value")> Public Property value As String

        Public Overrides Function ToString() As String
            Return $"({Type})  {value}"
        End Function
    End Class

    <XmlType("field")>
    Public Class Field
        <XmlAttribute("name")> Public Property Name As String
        <XmlText> Public Property value As String

        Public Overrides Function ToString() As String
            Return $"{Name} = ""{value}"""
        End Function
    End Class

    <XmlType("ref")>
    Public Class CrossRef
        <XmlAttribute("dbname")> Public Property DbName As String
        <XmlAttribute("dbkey")> Public Property DbKey As String

        Public Overrides Function ToString() As String
            Return $"{DbName} => {DbKey}"
        End Function
    End Class
End Namespace

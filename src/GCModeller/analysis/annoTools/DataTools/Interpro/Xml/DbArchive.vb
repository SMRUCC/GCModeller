#Region "Microsoft.VisualBasic::c66b3c66480d9406e65fa67358db2c3c, ..\GCModeller\analysis\annoTools\DataTools\Interpro\Xml\DbArchive.vb"

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

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel

Namespace Interpro.Xml

    <XmlType("interprodb")>
    Public Class DbArchive : Inherits ITextFile

        Public Property release As DbInfo()
        <XmlElement> Public Property interpro As Interpro()

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.GetXml.SaveTo(getPath(FilePath), getEncoding(Encoding))
        End Function
    End Class

    <XmlType("dbinfo")>
    Public Class DbInfo
        <XmlAttribute> Public Property dbname As String
        <XmlAttribute> Public Property entry_count As Integer
        <XmlAttribute> Public Property file_date As String
        <XmlAttribute> Public Property version As String

        Public Overrides Function ToString() As String
            Return $"[{version}] {dbname}"
        End Function
    End Class

    <XmlType("publication")>
    Public Class Publication
        <XmlAttribute> Public Property id As String
        Public Property author_list As String
        Public Property title As String
        Public Property db_xref As DbXref
        Public Property journal As String
        Public Property location As Location
        Public Property year As String

        Public Overrides Function ToString() As String
            Return $"{author_list}, ""{title}"", {journal}, {location.ToString}, {year}"
        End Function
    End Class

    <XmlType("location")>
    Public Class Location
        <XmlAttribute> Public Property issue As String
        <XmlAttribute> Public Property pages As String
        <XmlAttribute> Public Property volume As String

        Public Overrides Function ToString() As String
            Return $"({volume}){issue}: {pages}"
        End Function
    End Class

    <XmlType("db_xref")>
    Public Class DbXref
        <XmlAttribute> Public Property db As String
        <XmlAttribute> Public Property dbkey As String
        <XmlAttribute> Public Property protein_count As Integer
        <XmlAttribute> Public Property name As String

        Public Overrides Function ToString() As String
            Return $"{db}: {dbkey}"
        End Function
    End Class

    <XmlType("rel_ref")>
    Public Class RelRef
        <XmlAttribute> Public Property ipr_ref As String

        Public Overrides Function ToString() As String
            Return ipr_ref
        End Function
    End Class

    <XmlType("taxon_data")>
    Public Class TaxonData
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property proteins_count As Integer

        Public Overrides Function ToString() As String
            Return $"{name}; //{proteins_count}"
        End Function
    End Class

    <XmlType("interpro")>
    Public Class Interpro
        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property protein_count As Integer
        <XmlAttribute> Public Property short_name As String
        <XmlAttribute> Public Property type As String
        Public Property name As String
        Public Property abstract As String
        Public Property pub_list As Publication()
        Public Property parent_list As RelRef()
        Public Property contains As RelRef()
        Public Property found_in As RelRef()
        Public Property member_list As DbXref()
        Public Property external_doc_list As DbXref()
        Public Property structure_db_links As DbXref()
        Public Property taxonomy_distribution As TaxonData()
        Public Property sec_list As SecAcc()

        Public Overrides Function ToString() As String
            Return $"[{type}] {short_name}   {name}"
        End Function
    End Class


    Public Class SecAcc
        <XmlAttribute> Public Property acc As String

        Public Overrides Function ToString() As String
            Return acc
        End Function
    End Class
End Namespace

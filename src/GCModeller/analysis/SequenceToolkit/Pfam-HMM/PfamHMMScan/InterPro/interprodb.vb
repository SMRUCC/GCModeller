#Region "Microsoft.VisualBasic::0749efa4d373e17b938e423f45bcd172, DataTools\Interpro\Xml\DbArchive.vb"

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

'     Class DbArchive
' 
'         Properties: interpro, release
' 
'         Function: Save
' 
'     Class DbInfo
' 
'         Properties: dbname, entry_count, file_date, version
' 
'         Function: ToString
' 
'     Class Publication
' 
'         Properties: author_list, db_xref, id, journal, location
'                     title, year
' 
'         Function: ToString
' 
'     Class Location
' 
'         Properties: issue, pages, volume
' 
'         Function: ToString
' 
'     Class DbXref
' 
'         Properties: db, dbkey, name, protein_count
' 
'         Function: ToString
' 
'     Class RelRef
' 
'         Properties: ipr_ref
' 
'         Function: ToString
' 
'     Class TaxonData
' 
'         Properties: name, proteins_count
' 
'         Function: ToString
' 
'     Class Interpro
' 
'         Properties: abstract, contains, external_doc_list, found_in, id
'                     member_list, name, parent_list, protein_count, pub_list
'                     sec_list, short_name, structure_db_links, taxonomy_distribution, type
' 
'         Function: ToString
' 
'     Class SecAcc
' 
'         Properties: acc
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization

Namespace Interpro.Xml

    ''' <summary>
    ''' ftp://ftp.ebi.ac.uk/pub/databases/interpro/current_release/
    ''' </summary>
    <XmlType("interprodb")>
    Public Class interprodb

        Public Property release As dbinfo()
        <XmlElement> Public Property interpro As Interpro()

        Public Function Save(FilePath As String, Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.GetXml.SaveTo(FilePath, Encoding)
        End Function
    End Class

    <XmlType("dbinfo")>
    Public Class dbinfo

        <XmlAttribute> Public Property dbname As String
        <XmlAttribute> Public Property entry_count As Integer
        <XmlAttribute> Public Property file_date As String
        <XmlAttribute> Public Property version As String

        Public Overrides Function ToString() As String
            Return $"[{version}] {dbname}"
        End Function
    End Class
End Namespace

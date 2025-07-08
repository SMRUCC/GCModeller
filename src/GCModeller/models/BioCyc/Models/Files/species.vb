#Region "Microsoft.VisualBasic::91782d04573526314f1ad1a00215a00f, core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\Specie.vb"

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

'   Total Lines: 95
'    Code Lines: 49 (51.58%)
' Comment Lines: 30 (31.58%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 16 (16.84%)
'     File Size: 6.40 KB


'     Class Specie
' 
'         Properties: GeneticCode, Genome, MitochndrialGeneticCode, NCBITaxonomyId, PGDBAuthors
'                     PGDBCopyright, PGDBFooterCitation, PGDBHomePage, PGDBLastConsistencyCheck, PGDBName
'                     PGDBTIER, PGDBUniqueId, Rank, StrainName, Table
' 
'         Function: GetAttributeList, GetFullName, ToFileStream
' 
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

''' <summary>
''' the organism taxonomy species information
''' </summary>
''' 
<Xref("species.dat")>
Public Class species : Inherits Model

    Public Shared ReadOnly AttributeList As String() = {
        "UNIQUE-ID", "TYPES", "COMMON-NAME", "BIOMASS-TEMPLATES", "CITATIONS", "COMMENT",
        "COMMENT-INTERNAL", "CONTACT-EMAIL", "CREDITS", "DATA-SOURCE", "DBLINKS",
        "DOCUMENTATION", "GENETIC-CODE", "GENOME", "HIDE-SLOT?", "INSTANCE-NAME-TEMPLATE",
        "MEMBER-SORT-FN", "MITOCHONDRIAL-GENETIC-CODE", "PATHOLOGIC-NAME-MATCHER-EVIDENCE",
        "PATHOLOGIC-PWY-EVIDENCE", "PGDB-AUTHORS", "PGDB-COPYRIGHT",
        "PGDB-FOOTER-CITATION", "PGDB-HOME-PAGE", "PGDB-LAST-CONSISTENCY-CHECK",
        "PGDB-NAME", "PGDB-TIER", "PGDB-UNIQUE-ID", "RANK", "SEQUENCE-SOURCE",
        "STRAIN-NAME", "SUBSPECIES-NAME", "SYNONYMS", "UPDATE-STATS"}

    <AttributeField("GENETIC-CODE")> <XmlAttribute> Public Property GeneticCode As String
    <AttributeField("GENOME")> <XmlAttribute> Public Property Genome As String
    <AttributeField("MITOCHONDRIAL-GENETIC-CODE")> <XmlAttribute> Public Property MitochndrialGeneticCode As String
    <AttributeField("PGDB-AUTHORS")> <XmlArray> Public Property PGDBAuthors As String()
    <AttributeField("PGDB-COPYRIGHT")> <XmlElement> Public Property PGDBCopyright As String
    <AttributeField("PGDB-FOOTER-CITATION")> <XmlElement> Public Property PGDBFooterCitation As String
    <AttributeField("PGDB-LAST-CONSISTENCY-CHECK")> <XmlAttribute> Public Property PGDBLastConsistencyCheck As String
    <AttributeField("PGDB-NAME")> <XmlAttribute> Public Property PGDBName As String
    <AttributeField("PGDB-TIER")> <XmlAttribute> Public Property PGDBTIER As String
    <AttributeField("PGDB-UNIQUE-ID")> <XmlAttribute> Public Property PGDBUniqueId As String
    <AttributeField("RANK")> <XmlAttribute> Public Property Rank As String
    <AttributeField("STRAIN-NAME")> <XmlAttribute> Public Property StrainName As String

    ''' <summary>
    ''' uint ncbi taxonomy id
    ''' </summary>
    ''' <returns></returns>
    <AttributeField("NCBI-TAXONOMY-ID")> <XmlAttribute> Public Property NCBITaxonomyId As String
    <AttributeField("PGDB-HOME-PAGE")> Public Property PGDBHomePage As String

    Public Function GetFullName() As String
        Return commonName & " " & StrainName
    End Function

End Class
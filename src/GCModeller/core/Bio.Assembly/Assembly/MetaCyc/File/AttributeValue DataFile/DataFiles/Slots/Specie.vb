#Region "Microsoft.VisualBasic::df3e07d3d43e93429b0cea3e62595584, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\Specie.vb"

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
    '    Code Lines: 49
    ' Comment Lines: 30
    '   Blank Lines: 16
    '     File Size: 6.31 KB


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

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports System.Reflection
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    Public Class Specie : Inherits MetaCyc.File.DataFiles.Slots.Object

        Public Shared ReadOnly AttributeList As String() = {
            "UNIQUE-ID", "TYPES", "COMMON-NAME", "BIOMASS-TEMPLATES", "CITATIONS", "COMMENT",
            "COMMENT-INTERNAL", "CONTACT-EMAIL", "CREDITS", "DATA-SOURCE", "DBLINKS",
            "DOCUMENTATION", "GENETIC-CODE", "GENOME", "HIDE-SLOT?", "INSTANCE-NAME-TEMPLATE",
            "MEMBER-SORT-FN", "MITOCHONDRIAL-GENETIC-CODE", "PATHOLOGIC-NAME-MATCHER-EVIDENCE",
            "PATHOLOGIC-PWY-EVIDENCE", "PGDB-AUTHORS", "PGDB-COPYRIGHT",
            "PGDB-FOOTER-CITATION", "PGDB-HOME-PAGE", "PGDB-LAST-CONSISTENCY-CHECK",
            "PGDB-NAME", "PGDB-TIER", "PGDB-UNIQUE-ID", "RANK", "SEQUENCE-SOURCE",
            "STRAIN-NAME", "SUBSPECIES-NAME", "SYNONYMS", "UPDATE-STATS"}

        <MetaCycField()> <XmlAttribute> Public Property GeneticCode As String
        <MetaCycField()> <XmlAttribute> Public Property Genome As String
        <MetaCycField()> <XmlAttribute> Public Property MitochndrialGeneticCode As String
        <MetaCycField(Name:="PGDB-AUTHORS", Type:=MetaCycField.Types.TStr)> <XmlArray> Public Property PGDBAuthors As List(Of String)
        <MetaCycField(Name:="PGDB-COPYRIGHT")> <XmlElement> Public Property PGDBCopyright As String
        <MetaCycField(Name:="PGDB-FOOTER-CITATION")> <XmlElement> Public Property PGDBFooterCitation As String
        <MetaCycField(Name:="PGDB-LAST-CONSISTENCY-CHECK")> <XmlAttribute> Public Property PGDBLastConsistencyCheck As String
        <MetaCycField(Name:="PGDB-NAME")> <XmlAttribute> Public Property PGDBName As String
        <MetaCycField(Name:="PGDB-TIER")> <XmlAttribute> Public Property PGDBTIER As String
        <MetaCycField(Name:="PGDB-UNIQUE-ID")> <XmlAttribute> Public Property PGDBUniqueId As String
        <MetaCycField()> <XmlAttribute> Public Property Rank As String
        <MetaCycField()> <XmlAttribute> Public Property StrainName As String
        <MetaCycField(Name:="NCBI-TAXONOMY-ID")> <XmlAttribute> Public Property NCBITaxonomyId As String
        <MetaCycField(Name:="PGDB-HOME-PAGE")> Public Property PGDBHomePage As String

        Public Function GetFullName() As String
            Return CommonName & " " & StrainName
        End Function

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.species
            End Get
        End Property

        Friend Function GetAttributeList() As String()
            Return (From s As String In Specie.AttributeList Select s Order By Len(s) Descending).ToArray
        End Function

        Public Function ToFileStream() As String
            Dim ItemProperties As PropertyInfo() = New PropertyInfo() {}, FieldAttributes As MetaCycField() = New MetaCycField() {}
            Dim sBuilder As StringBuilder = New StringBuilder(1024 * 1024)

            Call MetaCyc.File.DataFiles.Reflection.FileStream.GetMetaCycField(Of Specie)(ItemProperties, FieldAttributes)

            Return MetaCyc.File.DataFiles.Reflection.FileStream.Generate(Me, ItemProperties, FieldAttributes)
        End Function

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As Specie
        '    Dim Specie As Specie = New Specie

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of MetaCyc.File.DataFiles.Slots.Specie) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Specie.GetAttributeList, e), Specie)

        '    Specie.PGDBAuthors = StringQuery(Specie.Object, "PGDB-AUTHORS")
        '    If Specie.Object.ContainsKey("MITOCHONDRIAL-GENETIC-CODE") Then Specie.MitochndrialGeneticCode = Specie.Object("MITOCHONDRIAL-GENETIC-CODE") Else Specie.MitochndrialGeneticCode = ""
        '    If Specie.Object.ContainsKey("GENETIC-CODE") Then Specie.GeneticCode = Specie.Object("GENETIC-CODE") Else Specie.GeneticCode = ""
        '    If Specie.Object.ContainsKey("PGDB-COPYRIGHT") Then Specie.PGDBCopyright = Specie.Object("PGDB-COPYRIGHT") Else Specie.PGDBCopyright = ""
        '    If Specie.Object.ContainsKey("PGDB-FOOTER-CITATION") Then Specie.PGDBFooterCitation = Specie.Object("PGDB-FOOTER-CITATION") Else Specie.PGDBFooterCitation = ""
        '    If Specie.Object.ContainsKey("PGDB-LAST-CONSISTENCY-CHECK") Then Specie.PGDBLastConsistencyCheck = Specie.Object("PGDB-LAST-CONSISTENCY-CHECK") Else Specie.PGDBLastConsistencyCheck = ""
        '    If Specie.Object.ContainsKey("PGDB-NAME") Then Specie.PGDBName = Specie.Object("PGDB-NAME") Else Specie.PGDBName = ""
        '    If Specie.Object.ContainsKey("PGDB-TIER") Then Specie.PGDBTIER = Specie.Object("PGDB-TIER") Else Specie.PGDBTIER = ""
        '    If Specie.Object.ContainsKey("PGDB-UNIQUE-ID") Then Specie.PGDBUniqueId = Specie.Object("PGDB-UNIQUE-ID") Else Specie.PGDBUniqueId = ""
        '    If Specie.Object.ContainsKey("RANK") Then Specie.Rank = Specie.Object("RANK") Else Specie.Rank = ""
        '    If Specie.Object.ContainsKey("STRAIN-NAME") Then Specie.StrainName = Specie.Object("STRAIN-NAME") Else Specie.StrainName = ""
        '    If Specie.Object.ContainsKey("GENOME") Then Specie.Genome = Specie.Object("GENOME") Else Specie.Genome = ""
        '    If Specie.Object.ContainsKey("NCBI-TAXONOMY-ID") Then Specie.NCBITaxonomyId = Specie.Object("NCBI-TAXONOMY-ID") Else Specie.NCBITaxonomyId = ""

        '    Return Specie
        'End Operator

        'Public Shared Shadows Widening Operator CType(e As SMRUCC.genomics.Assembly.MetaCyc.File.AttributeValue) As Specie
        '    Dim Query As Generic.IEnumerable(Of MetaCyc.File.DataFiles.Slots.Specie) =
        '        From c As MetaCyc.File.AttributeValue.Object
        '        In e.Objects
        '        Select CType(c, MetaCyc.File.DataFiles.Slots.Specie)
        '    Return Query.First
        'End Operator

        'Public Shared Shadows Widening Operator CType(Path As String) As Specie
        '    Dim File As MetaCyc.File.AttributeValue = Path
        '    Return CType(File, Specie)
        'End Operator
    End Class
End Namespace

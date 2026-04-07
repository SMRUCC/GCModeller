#Region "Microsoft.VisualBasic::dfaf9cc6e42627a3b21346a0afa2e9d4, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\InterPro\Elements.vb"

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

    '   Total Lines: 69
    '    Code Lines: 56 (81.16%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 13 (18.84%)
    '     File Size: 2.18 KB


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
    '     Class db_xref
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
    '     Class SecAcc
    ' 
    '         Properties: acc
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace InterPro.Xml

    <XmlType("publication")>
    Public Class Publication
        <XmlAttribute> Public Property id As String
        Public Property author_list As String
        Public Property title As String
        Public Property db_xref As db_xref
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
    Public Class db_xref
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

    Public Class SecAcc
        <XmlAttribute> Public Property acc As String

        Public Overrides Function ToString() As String
            Return acc
        End Function
    End Class
End Namespace

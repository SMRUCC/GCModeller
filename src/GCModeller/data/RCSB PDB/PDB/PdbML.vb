#Region "Microsoft.VisualBasic::202a4796340c8b9c86f10912624a2d62, ..\GCModeller\data\RCSB PDB\PDB\PdbML.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports Microsoft.VisualBasic.DocumentFormat
Imports Microsoft.VisualBasic.DocumentFormat.RDF.Serialization

Namespace PDBML

    ''' <summary>
    ''' PDBML/XML File Format
    ''' The Protein Data Bank Markup Language (PDBML) provides a representation of PDB data in XML format. The description of this format is provided in XML schema of the PDB Exchange Data Dictionary. This schema is produced by direct translation of the mmCIF format PDB Exchange Data Dictionary. Other data dictionaries used by the PDB have been electronically translated into XML/XSD schemas.
    ''' 
    ''' Further information and related resources are available at http://pdbml.pdb.org/.
    ''' </summary>
    ''' <remarks></remarks>
    <RDFNamespaceImports("PDBx", "http://pdbml.pdb.org/schema/pdbx-v40.xsd")>
    <XmlRoot("datablock", DataType:="PDBx:datablock", Namespace:="http://pdbml.pdb.org/schema/pdbx-v40.xsd")>
    Public Class PdbML

        <XmlAttribute> Public Property datablockName As String
        <RDFElement("atom_siteCategory")> Public Property AtomSiteCategory As Elements.AtomSiteCategory
        <XmlElement("entity_polyCategory")> Public Property entityPolyCategory As entity_polyCategory

        <XmlType("entity_polyCategory", Namespace:="http://pdbml.pdb.org/schema/pdbx-v40.xsd", IncludeInSchema:=True)>
        Public Class entity_polyCategory

            <XmlElement("entity_poly", Namespace:="http://pdbml.pdb.org/schema/pdbx-v40.xsd")> Public Property entity_polyes As entity_poly()

            Public Class entity_poly
                <XmlAttribute("entity_id")> Public Property entity_id As Integer
                ' <PDBx:nstd_linkage>no</PDBx:nstd_linkage>
                ' <PDBx:nstd_monomer>no</PDBx:nstd_monomer>
                Public Property pdbx_seq_one_letter_code As String
                Public Property pdbx_seq_one_letter_code_can As String
                Public Property pdbx_strand_id As String
                Public Property type As String
            End Class
        End Class
    End Class

    Namespace Elements

        <RDF.Serialization.RDFType("atom_siteCategory")>
        Public Class AtomSiteCategory

            <RDF.Serialization.RDFType("atom_site")>
            Public Class AtomSite
                <XmlAttribute> Public Property id As String
                <RDF.Serialization.RDFElement("B_iso_or_equiv")> Public Property B_iso_or_equiv As String
                <RDF.Serialization.RDFElement("Cartn_x")> Public Property Cartn_x As String
                <RDF.Serialization.RDFElement("Cartn_y")> Public Property Cartn_y As String
                <RDF.Serialization.RDFElement("Cartn_z")> Public Property Cartn_z As String
                <RDF.Serialization.RDFElement("auth_asym_id")> Public Property auth_asym_id As String
                <RDF.Serialization.RDFElement("auth_atom_id")> Public Property auth_atom_id As String
                <RDF.Serialization.RDFElement("auth_comp_id")> Public Property auth_comp_id As String
                <RDF.Serialization.RDFElement("auth_seq_id")> Public Property auth_seq_id As String
                <RDF.Serialization.RDFElement("group_PDB")> Public Property group_PDB As String
                '<RDF.Serialization.RDFElement("label_alt_id")> xsi:nil="true"/>
                <RDF.Serialization.RDFElement("label_asym_id")> Public Property label_asym_id As String
                <RDF.Serialization.RDFElement("label_atom_id")> Public Property label_atom_id As String
                <RDF.Serialization.RDFElement("label_comp_id")> Public Property label_comp_id As String
                <RDF.Serialization.RDFElement("label_entity_id")> Public Property label_entity_id As String
                <RDF.Serialization.RDFElement("label_seq_id")> Public Property label_seq_id As String
                <RDF.Serialization.RDFElement("occupancy")> Public Property occupancy As String
                <RDF.Serialization.RDFElement("pdbx_PDB_model_num")> Public Property pdbx_PDB_model_num As String
                <RDF.Serialization.RDFElement("type_symbol")> Public Property type_symbol As String
            End Class
        End Class
    End Namespace
End Namespace

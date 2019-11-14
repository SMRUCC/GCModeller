#Region "Microsoft.VisualBasic::301a49a6309f1e1c950f56a5b6ee9331, data\RCSB PDB\PDB\PdbML.vb"

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

    '     Class PdbML
    ' 
    '         Properties: AtomSiteCategory, datablockName, entityPolyCategory
    '         Class entity_polyCategory
    ' 
    '             Properties: entity_polyes
    '             Class entity_poly
    ' 
    '                 Properties: pdbx_seq_one_letter_code, pdbx_seq_one_letter_code_can, pdbx_strand_id, type
    ' 
    ' 
    ' 
    ' 
    ' 
    '     Class PDBx
    ' 
    '         Properties: id
    ' 
    '         Function: ToString
    ' 
    '     Class PDBxEntity
    ' 
    '         Properties: entity_id
    ' 
    '         Function: ToString
    ' 
    '         Class Site
    ' 
    '             Properties: auth_asym_id, auth_atom_id, auth_comp_id, auth_seq_id, B_iso_or_equiv
    '                         Cartn_x, Cartn_y, Cartn_z, group_PDB, label_asym_id
    '                         label_atom_id, label_comp_id, label_entity_id, label_seq_id, occupancy
    '                         pdbx_PDB_model_num, type_symbol
    ' 
    '         Class anisotrop
    ' 
    '             Properties: pdbx_auth_asym_id, pdbx_auth_atom_id, pdbx_auth_comp_id, pdbx_auth_seq_id, pdbx_label_asym_id
    '                         pdbx_label_atom_id, pdbx_label_comp_id, pdbx_label_seq_id, type_symbol, U11
    '                         U12, U13, U22, U23, U33
    ' 
    '         Class sites
    ' 
    '             Properties: fract_transf_matrix11, fract_transf_matrix12, fract_transf_matrix13, fract_transf_matrix21, fract_transf_matrix22
    '                         fract_transf_matrix23, fract_transf_matrix31, fract_transf_matrix32, fract_transf_matrix33, fract_transf_vector1
    '                         fract_transf_vector2, fract_transf_vector3
    ' 
    '         Class type
    ' 
    '             Properties: symbol
    ' 
    '         Class typeCategory
    ' 
    '             Properties: atom_types
    ' 
    '         Class sitesCategory
    ' 
    '             Properties: atom_sites
    ' 
    '         Class siteCategory
    ' 
    '             Properties: atom_sites
    ' 
    '         Class anisotropCategory
    ' 
    '             Properties: atom_site_anisotrop
    ' 
    '         Class authorCategory
    ' 
    '             Properties: authors
    ' 
    '         Class author
    ' 
    '             Properties: name, pdbx_ordinal
    ' 
    '         Class conformCategory
    ' 
    '             Properties: conforms
    ' 
    '         Class conform
    ' 
    '             Properties: dict_location, dict_name, dict_version
    ' 
    '             Function: ToString
    ' 
    '     Class cellCategory
    ' 
    '         Properties: cells
    ' 
    '     Class cell
    ' 
    '         Properties: angle_alpha, angle_beta, angle_gamma, length_a, length_b
    '                     length_c, Z_PDB
    ' 
    '         Class compCategory
    ' 
    '             Properties: chem_comp
    ' 
    '         Class comp
    ' 
    '             Properties: formula, formula_weight, mon_nstd_flag, name, type
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace PDBML

    ''' <summary>
    ''' ##### PDBML/XML File Format
    ''' 
    ''' The ``Protein Data Bank Markup Language (PDBML)`` provides a representation of PDB data in XML 
    ''' format. 
    ''' The description of this format is provided in XML schema of the PDB Exchange Data Dictionary. 
    ''' This schema is produced by direct translation of the mmCIF format PDB Exchange Data Dictionary. 
    ''' Other data dictionaries used by the PDB have been electronically translated into XML/XSD schemas.
    ''' 
    ''' Further information and related resources are available at http://pdbml.pdb.org/.
    ''' </summary>
    ''' <remarks></remarks>
    <XmlRoot("datablock", DataType:="datablock", Namespace:="http://pdbml.pdb.org/schema/pdbx-v40.xsd")>
    Public Class PdbML

        <XmlAttribute> Public Property datablockName As String
        <XmlElement("atom_siteCategory")> Public Property AtomSiteCategory As atom.siteCategory
        <XmlElement("entity_polyCategory")> Public Property entityPolyCategory As entity_polyCategory

        <XmlType("entity_polyCategory", Namespace:="http://pdbml.pdb.org/schema/pdbx-v40.xsd", IncludeInSchema:=True)>
        Public Class entity_polyCategory

            <XmlElement("entity_poly", Namespace:="http://pdbml.pdb.org/schema/pdbx-v40.xsd")> Public Property entity_polyes As entity_poly()

            Public Class entity_poly : Inherits PDBxEntity

                ' <PDBx:nstd_linkage>no</PDBx:nstd_linkage>
                ' <PDBx:nstd_monomer>no</PDBx:nstd_monomer>
                Public Property pdbx_seq_one_letter_code As String
                Public Property pdbx_seq_one_letter_code_can As String
                Public Property pdbx_strand_id As String
                Public Property type As String
            End Class
        End Class
    End Class

    Public MustInherit Class PDBx

        <XmlAttribute> Public Property id As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public MustInherit Class PDBxEntity

        <XmlAttribute("entity_id")> Public Property entity_id As Integer

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Namespace atom

        <XmlType("atom_site")> Public Class Site : Inherits PDBx

            <XmlElement("B_iso_or_equiv")> Public Property B_iso_or_equiv As String
            <XmlElement("Cartn_x")> Public Property Cartn_x As String
            <XmlElement("Cartn_y")> Public Property Cartn_y As String
            <XmlElement("Cartn_z")> Public Property Cartn_z As String
            <XmlElement("auth_asym_id")> Public Property auth_asym_id As String
            <XmlElement("auth_atom_id")> Public Property auth_atom_id As String
            <XmlElement("auth_comp_id")> Public Property auth_comp_id As String
            <XmlElement("auth_seq_id")> Public Property auth_seq_id As String
            <XmlElement("group_PDB")> Public Property group_PDB As String
            '<XmlElement("label_alt_id")> xsi:nil="true"/>
            <XmlElement("label_asym_id")> Public Property label_asym_id As String
            <XmlElement("label_atom_id")> Public Property label_atom_id As String
            <XmlElement("label_comp_id")> Public Property label_comp_id As String
            <XmlElement("label_entity_id")> Public Property label_entity_id As String
            <XmlElement("label_seq_id")> Public Property label_seq_id As String
            <XmlElement("occupancy")> Public Property occupancy As String
            <XmlElement("pdbx_PDB_model_num")> Public Property pdbx_PDB_model_num As String
            <XmlElement("type_symbol")> Public Property type_symbol As String
        End Class

        <XmlType("atom_site_anisotrop")> Public Class anisotrop : Inherits PDBx

            Public Property U11 As String
            Public Property U12 As String
            Public Property U13 As String
            Public Property U22 As String
            Public Property U23 As String
            Public Property U33 As String
            Public Property pdbx_auth_asym_id As String
            Public Property pdbx_auth_atom_id As String
            Public Property pdbx_auth_comp_id As String
            Public Property pdbx_auth_seq_id As String
            ' <PDBx:pdbx_label_alt_id xsi : nil="true" />
            Public Property pdbx_label_asym_id As String
            Public Property pdbx_label_atom_id As String
            Public Property pdbx_label_comp_id As String
            Public Property pdbx_label_seq_id As String
            Public Property type_symbol As String
        End Class

        <XmlType("atom_sites")> Public Class sites : Inherits PDBxEntity
            Public Property fract_transf_matrix11 As String
            Public Property fract_transf_matrix12 As String
            Public Property fract_transf_matrix13 As String
            Public Property fract_transf_matrix21 As String
            Public Property fract_transf_matrix22 As String
            Public Property fract_transf_matrix23 As String
            Public Property fract_transf_matrix31 As String
            Public Property fract_transf_matrix32 As String
            Public Property fract_transf_matrix33 As String
            Public Property fract_transf_vector1 As String
            Public Property fract_transf_vector2 As String
            Public Property fract_transf_vector3 As String
        End Class

        <XmlType("atom_type")> Public Class type

            <XmlAttribute> Public Property symbol As String
        End Class

        <XmlType("atom_typeCategory")> Public Class typeCategory

            <XmlElement("atom_type")> Public Property atom_types As type()
        End Class

        <XmlType("atom_sitesCategory")> Public Class sitesCategory
            <XmlElement("atom_sites")> Public Property atom_sites As sites()
        End Class

        <XmlType("atom_siteCategory")> Public Class siteCategory

            <XmlElement("atom_site")>
            Public Property atom_sites As atom.Site()
        End Class

        <XmlType("atom_site_anisotropCategory")> Public Class anisotropCategory

            <XmlElement("atom_site_anisotrop")>
            Public Property atom_site_anisotrop As anisotrop()
        End Class
    End Namespace

    Namespace audit

        <XmlType("audit_authorCategory")> Public Class authorCategory
            <XmlElement("audit_author")> Public Property authors As author()
        End Class

        <XmlType("audit_author")> Public Class author

            <XmlAttribute> Public Property pdbx_ordinal As Integer
            Public Property name As String
        End Class

        <XmlType("audit_conformCategory")> Public Class conformCategory
            <XmlElement("audit_conform")> Public Property conforms As conform()
        End Class

        <XmlType("audit_conform")> Public Class conform
            <XmlAttribute> Public Property dict_name As String
            <XmlAttribute> Public Property dict_version As String
            Public Property dict_location As String

            Public Overrides Function ToString() As String
                Return Me.GetJson
            End Function
        End Class

    End Namespace

    <XmlType("cellCategory")> Public Class cellCategory
        <XmlElement("cell")> Public Property cells As cell()
    End Class

    <XmlType(NameOf(cell))> Public Class cell : Inherits PDBxEntity
        Public Property Z_PDB As String
        Public Property angle_alpha As String
        Public Property angle_beta As String
        Public Property angle_gamma As String
        Public Property length_a As String
        Public Property length_b As String
        Public Property length_c As String
    End Class

    Namespace chem

        <XmlType("chem_compCategory")> Public Class compCategory

            <XmlElement("chem_comp")> Public Property chem_comp As comp()
        End Class

        <XmlType("chem_comp")> Public Class comp : Inherits PDBx
            Public Property formula As String
            Public Property formula_weight As String
            Public Property mon_nstd_flag As String
            Public Property name As String
            Public Property type As String
        End Class

    End Namespace

End Namespace

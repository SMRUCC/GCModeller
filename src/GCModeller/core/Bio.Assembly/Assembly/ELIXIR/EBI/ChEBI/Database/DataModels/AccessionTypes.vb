#Region "Microsoft.VisualBasic::973760c0c0f88c9d14d40e70de5cb5bd, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\EBI\ChEBI\Database\DataModels\AccessionTypes.vb"

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

    '   Total Lines: 161
    '    Code Lines: 43
    ' Comment Lines: 114
    '   Blank Lines: 4
    '     File Size: 5.33 KB


    '     Enum AccessionTypes
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports SMRUCC.genomics.Assembly.ELIXIR.EBI.ChEBI.XML

Namespace Assembly.ELIXIR.EBI.ChEBI

    ''' <summary>
    ''' Chebi accession types, table key name in <see cref="RegistryNumbers"/>
    ''' </summary>
    Public Enum AccessionTypes As Byte

        ''' <summary>
        ''' KEGG COMPOUND accession
        ''' </summary>
        <Description("KEGG COMPOUND accession")> KEGG_Compound
        ''' <summary>
        ''' CAS Registry Number
        ''' </summary>
        <Description("CAS Registry Number")> CAS_Registry
        ''' <summary>
        ''' UM-BBD compID
        ''' </summary>
        <Description("UM-BBD compID")> UM_BBD_compID
        ''' <summary>
        ''' Beilstein Registry Number
        ''' </summary>
        <Description("Beilstein Registry Number")> Beilstein_Registry
        ''' <summary>
        ''' LIPID MAPS instance accession
        ''' </summary>
        <Description("LIPID MAPS instance accession")> LIPID_MAPS_instance
        ''' <summary>
        ''' KEGG DRUG accession
        ''' </summary>
        <Description("KEGG DRUG accession")> KEGG_Drug
        ''' <summary>
        ''' Gmelin Registry Number
        ''' </summary>
        <Description("Gmelin Registry Number")> Gmelin_Registry
        ''' <summary>
        ''' LIPID MAPS class accession
        ''' </summary>
        <Description("LIPID MAPS class accession")> LIPID_MAPS_class
        ''' <summary>
        ''' PDBeChem accession
        ''' </summary>
        <Description("PDBeChem accession")> PDBeChem
        ''' <summary>
        ''' KEGG GLYCAN accession
        ''' </summary>
        <Description("KEGG GLYCAN accession")> KEGG_Glycan
        ''' <summary>
        ''' COMe accession
        ''' </summary>
        <Description("COMe accession")> COMe
        ''' <summary>
        ''' MolBase accession
        ''' </summary>
        <Description("MolBase accession")> MolBase
        ''' <summary>
        ''' RESID accession
        ''' </summary>
        <Description("RESID accession")> RESID
        ''' <summary>
        ''' Reaxys Registry Number
        ''' </summary>
        <Description("Reaxys Registry Number")> Reaxys_Registry
        ''' <summary>
        ''' PDB accession
        ''' </summary>
        <Description("PDB accession")> PDB
        ''' <summary>
        ''' DrugBank accession
        ''' </summary>
        <Description("DrugBank accession")> DrugBank
        ''' <summary>
        ''' Patent accession
        ''' </summary>
        <Description("Patent accession")> Patent
        ''' <summary>
        ''' WebElements accession
        ''' </summary>
        <Description("WebElements accession")> WebElements
        ''' <summary>
        ''' PubMed citation
        ''' </summary>
        <Description("PubMed citation")> PubMed
        ''' <summary>
        ''' Wikipedia accession
        ''' </summary>
        <Description("Wikipedia accession")> Wikipedia
        ''' <summary>
        ''' MetaCyc accession
        ''' </summary>
        <Description("MetaCyc accession")> MetaCyc
        ''' <summary>
        ''' HMDB accession
        ''' </summary>
        <Description("HMDB accession")> HMDB
        ''' <summary>
        ''' Agricola citation
        ''' </summary>
        <Description("Agricola citation")> Agricola_citation
        ''' <summary>
        ''' Chinese Abstracts citation
        ''' </summary>
        <Description("Chinese Abstracts citation")> Chinese_Abstracts_citation
        ''' <summary>
        ''' ChemIDplus accession
        ''' </summary>
        <Description("ChemIDplus accession")> ChemIDplus
        ''' <summary>
        ''' Chemspider accession
        ''' </summary>
        <Description("Chemspider accession")> Chemspider
        ''' <summary>
        ''' PubMed Central citation
        ''' </summary>
        <Description("PubMed Central citation")> PubMed_Central
        ''' <summary>
        ''' CiteXplore citation
        ''' </summary>
        <Description("CiteXplore citation")> CiteXplore
        ''' <summary>
        ''' KNApSAcK accession
        ''' </summary>
        <Description("KNApSAcK accession")> KNApSAcK
        ''' <summary>
        ''' YMDB accession
        ''' </summary>
        <Description("YMDB accession")> YMDB
        ''' <summary>
        ''' ECMDB accession
        ''' </summary>
        <Description("ECMDB accession")> ECMDB
        ''' <summary>
        ''' SMID accession
        ''' </summary>
        <Description("SMID accession")> SMID
        ''' <summary>
        ''' Pesticides accession
        ''' </summary>
        <Description("Pesticides accession")> Pesticides
        ''' <summary>
        ''' LINCS accession
        ''' </summary>
        <Description("LINCS accession")> LINCS
        ''' <summary>
        ''' Pubchem accession
        ''' </summary>
        <Description("Pubchem accession")> Pubchem
        ''' <summary>
        ''' ETH
        ''' </summary>
        <Description("ETH")> ETH
        ''' <summary>
        ''' Drug Central accession
        ''' </summary>
        <Description("Drug Central accession")> Drug_Central

    End Enum
End Namespace

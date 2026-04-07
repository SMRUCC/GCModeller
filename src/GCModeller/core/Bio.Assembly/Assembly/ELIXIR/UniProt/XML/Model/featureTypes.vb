#Region "Microsoft.VisualBasic::833646bbeda552d289669aad714d74ad, core\Bio.Assembly\Assembly\ELIXIR\UniProt\XML\Model\featureTypes.vb"

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

    '   Total Lines: 54
    '    Code Lines: 44 (81.48%)
    ' Comment Lines: 6 (11.11%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (7.41%)
    '     File Size: 2.72 KB


    '     Enum featureTypes
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

Namespace Assembly.Uniprot.XML

    ''' <summary>
    ''' Describes the type of a sequence annotation.
    ''' Equivalent to the flat file FT feature keys, but using full terms instead 
    ''' of acronyms. The string value enumeration of <see cref="feature.type"/>
    ''' property.
    ''' </summary>
    Public Enum featureTypes

        <Description("active site")> active_site
        <Description("binding site")> binding_site
        <Description("calcium-binding region")> calcium_binding_region
        <Description("chain")> chain
        <Description("coiled-coil region")> coiled_coil_region
        <Description("compositionally biased region")> compositionally_biased_region
        <Description("cross-link")> cross_link
        <Description("disulfide bond")> disulfide_bond
        <Description("DNA-binding region")> DNA_binding_region
        <Description("domain")> domain
        <Description("glycosylation site")> glycosylation_site
        <Description("helix")> helix
        <Description("initiator methionine")> initiator_methionine
        <Description("lipid moiety-binding region")> lipid_moiety_binding_region
        <Description("metal ion-binding site")> metal_ion_binding_site
        <Description("modified residue")> modified_residue
        <Description("mutagenesis site")> mutagenesis_site
        <Description("non-consecutive residues")> non_consecutive_residues
        <Description("non-terminal residue")> non_terminal_residue
        <Description("nucleotide phosphate-binding region")> nucleotide_phosphate_binding_region
        <Description("peptide")> peptide
        <Description("propeptide")> propeptide
        <Description("region of interest")> region_of_interest
        <Description("repeat")> repeat
        <Description("non-standard amino acid")> non_standard_amino_acid
        <Description("sequence conflict")> sequence_conflict
        <Description("sequence variant")> sequence_variant
        <Description("short sequence motif")> short_sequence_motif
        <Description("signal peptide")> signal_peptide
        <Description("site")> site
        <Description("splice variant")> splice_variant
        <Description("strand")> strand
        <Description("topological domain")> topological_domain
        <Description("transit peptide")> transit_peptide
        <Description("transmembrane region")> transmembrane_region
        <Description("turn")> turn
        <Description("unsure residue")> unsure_residue
        <Description("zinc finger region")> zinc_finger_region
        <Description("intramembrane region")> intramembrane_region

    End Enum
End Namespace

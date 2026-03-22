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
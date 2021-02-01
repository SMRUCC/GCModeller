#Region "Microsoft.VisualBasic::79313d9a33f19e50b4046cbe217cf845, core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\Features\Qualifier.vb"

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

    '     Enum FeatureQualifiers
    ' 
    '         [function], [partial], anticodon, bound_moiety, citation
    '         codon, codon_start, cons_splice, db_xref, direction
    '         EC_number, evidence, frequency, gene, inference
    '         label, locus_tag, map, mod_base, mol_type
    '         note, number, organism, phenotype, product
    '         protein_id, pseudo, rpt_family, rpt_type, rpt_unit
    '         standard_name, transl_except, transl_table, translation, type
    '         usedin
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    ''' <summary>
    ''' Qualifiers provide additional information about features. They take
    ''' the form of a slash (/) followed by a qualifier name and, if
    ''' applicable, an equal sign (=) and a qualifier value. Feature
    ''' qualifiers begin at column 22
    ''' </summary>
    ''' <remarks>请注意，由于是直接使用ToString方法进行查询键值的获取的，所以请不要修改这些枚举对象的大小写</remarks>
    Public Enum FeatureQualifiers
        ''' <summary>
        ''' Location of the anticodon of tRNA and the amino acid for which it codes
        ''' </summary>
        ''' <remarks></remarks>
        anticodon
        ''' <summary>
        ''' Moiety bound
        ''' </summary>
        ''' <remarks></remarks>
        bound_moiety
        ''' <summary>
        ''' Reference to a citation providing the claim of or evidence for a feature
        ''' </summary>
        ''' <remarks></remarks>
        citation
        ''' <summary>
        ''' Specifies a codon that is different from any found in the reference genetic code
        ''' </summary>
        ''' <remarks></remarks>
        codon
        ''' <summary>
        ''' Indicates the first base of the first complete codon in a CDS (as 1 or 2 or 3)
        ''' </summary>
        ''' <remarks></remarks>
        codon_start
        ''' <summary>
        ''' Identifies intron splice sites that do not conform to the 5'-GT... AG-3' splice site consensus
        ''' </summary>
        ''' <remarks></remarks>
        cons_splice
        ''' <summary>
        ''' A database cross-reference; pointer to related information in another database. A description of all cross-references can be found at: http://www.ncbi.nlm.nih.gov/collab/db_xref.html
        ''' </summary>
        ''' <remarks></remarks>
        db_xref
        ''' <summary>
        ''' Direction of DNA replication
        ''' </summary>
        ''' <remarks></remarks>
        direction
        ''' <summary>
        ''' Enzyme Commission number for the enzyme product of the	sequence
        ''' </summary>
        ''' <remarks></remarks>
        EC_number
        ''' <summary>
        ''' Value indicating the nature of supporting evidence
        ''' </summary>
        ''' <remarks></remarks>
        evidence
        ''' <summary>
        ''' Frequency of the occurrence of a feature
        ''' </summary>
        ''' <remarks></remarks>
        frequency
        ''' <summary>
        ''' Function attributed to a sequence
        ''' </summary>
        ''' <remarks></remarks>
        [function]
        ''' <summary>
        ''' Symbol of the gene corresponding to a sequence region (usable with all features)
        ''' </summary>
        ''' <remarks></remarks>
        gene
        inference
        ''' <summary>
        ''' A label used to permanently identify a feature
        ''' </summary>
        ''' <remarks></remarks>
        label
        locus_tag
        ''' <summary>
        ''' Map position of the feature in free-format text
        ''' </summary>
        ''' <remarks></remarks>
        map
        ''' <summary>
        ''' Abbreviation for a modified nucleotide base
        ''' </summary>
        ''' <remarks></remarks>
        mod_base
        mol_type
        ''' <summary>
        ''' Any comment or additional information
        ''' </summary>
        ''' <remarks></remarks>
        note
        ''' <summary>
        ''' A number indicating the order of genetic elements (e.g., exons or introns) in the 5 to 3 direction
        ''' </summary>
        ''' <remarks></remarks>
        number
        ''' <summary>
        ''' Name of the organism that is the source of the	sequence data in the record. 
        ''' </summary>
        ''' <remarks></remarks>
        organism
        ''' <summary>
        ''' Differentiates between complete regions and partial ones
        ''' </summary>
        ''' <remarks></remarks>
        [partial]
        ''' <summary>
        ''' Phenotype conferred by the feature
        ''' </summary>
        ''' <remarks></remarks>
        phenotype
        ''' <summary>
        ''' Name of a product encoded by a coding region (CDS) feature
        ''' </summary>
        ''' <remarks></remarks>
        product
        ''' <summary>
        ''' Indicates that this feature is a non-functional version of the element named by the feature key
        ''' </summary>
        ''' <remarks></remarks>
        pseudo
        ''' <summary>
        ''' Type of repeated sequence; Alu or Kpn, for example
        ''' </summary>
        ''' <remarks></remarks>
        rpt_family
        ''' <summary>
        ''' Organization of repeated sequence
        ''' </summary>
        ''' <remarks></remarks>
        rpt_type
        ''' <summary>
        ''' Identity of repeat unit that constitutes a repeat_region
        ''' </summary>
        ''' <remarks></remarks>
        rpt_unit
        ''' <summary>
        ''' Accepted standard name for this feature
        ''' </summary>
        ''' <remarks></remarks>
        standard_name
        ''' <summary>
        ''' Translational exception: single codon, the translation of which does not conform to the reference genetic code
        ''' </summary>
        ''' <remarks></remarks>
        transl_except
        ''' <summary>
        ''' Amino acid translation of a coding region
        ''' </summary>
        ''' <remarks></remarks>
        translation
        ''' <summary>
        ''' Name of a strain if different from that in the SOURCE field
        ''' </summary>
        ''' <remarks></remarks>
        type
        ''' <summary>
        ''' Indicates that feature is used in a compound feature in another entry
        ''' </summary>
        ''' <remarks></remarks>
        usedin
        transl_table
        protein_id
    End Enum
End Namespace

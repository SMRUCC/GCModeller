#Region "Microsoft.VisualBasic::5408af664be520353b0ccdee561f643a, GCModeller\core\Bio.Assembly\Assembly\ELIXIR\UniProt\Web\ID_types.vb"

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

    '   Total Lines: 429
    '    Code Lines: 132
    ' Comment Lines: 295
    '   Blank Lines: 2
    '     File Size: 9.65 KB


    '     Enum ID_types
    ' 
    '         ALLERGOME_ID, ARACHNOSERVER_ID, BIOCYC_ID, BIOGRID_ID, BIOMUTA_ID
    '         CCDS_ID, CGD, CHEMBL_ID, CHITARS_ID, CLEANEX_ID
    '         COLLECTF_ID, CONOSERVER_ID, DICTYBASE_ID, DIP_ID, DISPROT_ID
    '         DMDM_ID, DNASU_ID, DRUGBANK_ID, ECHOBASE_ID, ECOGENE_ID
    '         EGGNOG_ID, EMBL, EMBL_ID, ENSEMBL_ID, ENSEMBL_PRO_ID
    '         ENSEMBL_TRS_ID, ENSEMBLGENOME_ID, ENSEMBLGENOME_PRO_ID, ENSEMBLGENOME_TRS_ID, ESTHER_ID
    '         EUHCVDB_ID, EUPATHDB_ID, FLYBASE_ID, GENECARDS_ID, GENEDB_ID
    '         GENENAME, GENEREVIEWS_ID, GENETREE_ID, GENEWIKI_ID, GENOMERNAI_ID
    '         GUIDETOPHARMACOLOGY_ID, H_INVDB_ID, HGNC_ID, HOGENOM_ID, HOVERGEN_ID
    '         HPA_ID, KEGG_ID, KO_ID, LEGIOLIST_ID, LEPROMA_ID
    '         MAIZEGDB_ID, MEROPS_ID, MGI_ID, MIM_ID, MINT_ID
    '         MYCOCLAP_ID, NEXTPROT_ID, NF100, NF50, NF90
    '         OMA_ID, ORPHANET_ID, ORTHODB_ID, P_ENTREZGENEID, P_GI
    '         P_REFSEQ_AC, PATRIC_ID, PDB_ID, PEROXIBASE_ID, PHARMGKB_ID
    '         PIR, POMBASE_ID, PSEUDOCAP_ID, REACTOME_ID, REBASE_ID
    '         REFSEQ_NT_ID, RGD_ID, SGD_ID, STRING_ID, SWISSLIPIDS_ID
    '         TAIR_ID, TCDB_ID, TREEFAM_ID, TUBERCULIST_ID, UCSC_ID
    '         UNIGENE_ID, UNIPATHWAY_ID, UPARC, VECTORBASE_ID, WBPARASITE_ID
    '         WORLD_2DPAGE_ID, WORMBASE_ID, WORMBASE_PRO_ID, WORMBASE_TRS_ID, XENBASE_ID
    '         ZFIN_ID
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace Assembly.Uniprot.Web

    Public Enum ID_types
#Region "UniProt"
        ''' <summary>
        ''' UniProtKB AC/ID
        ''' </summary>
        <Description("ACC,ID")> ACC_ID
        ''' <summary>
        ''' UniParc
        ''' </summary>
        UPARC
        ''' <summary>
        ''' UniRef50
        ''' </summary>
        NF50
        ''' <summary>
        ''' UniRef90
        ''' </summary>
        NF90
        ''' <summary>
        ''' UniRef100
        ''' </summary>
        NF100
        ''' <summary>
        ''' Gene name
        ''' </summary>
        GENENAME
#End Region
#Region "Sequence databases"
        ''' <summary>
        ''' EMBL/GenBank/DDBJ
        ''' </summary>
        EMBL_ID
        ''' <summary>
        ''' EMBL/GenBank/DDBJ CDS
        ''' </summary>
        EMBL
        ''' <summary>
        ''' Entrez Gene (GeneID)
        ''' </summary>
        P_ENTREZGENEID
        ''' <summary>
        ''' GI number
        ''' </summary>
        P_GI
        ''' <summary>
        ''' PIR
        ''' </summary>
        PIR
        ''' <summary>
        ''' RefSeq Nucleotide
        ''' </summary>
        REFSEQ_NT_ID
        ''' <summary>
        ''' RefSeq Protein
        ''' </summary>
        P_REFSEQ_AC
        ''' <summary>
        ''' UniGene
        ''' </summary>
        UNIGENE_ID
#End Region
#Region "3D structure databases"
        ''' <summary>
        ''' PDB
        ''' </summary>
        PDB_ID
        ''' <summary>
        ''' DisProt
        ''' </summary>
        DISPROT_ID
#End Region
#Region "Protein-protein interaction databases"
        ''' <summary>
        ''' BioGrid
        ''' </summary>
        BIOGRID_ID
        ''' <summary>
        ''' DIP
        ''' </summary>
        DIP_ID
        ''' <summary>
        ''' MINT
        ''' </summary>
        MINT_ID
        ''' <summary>
        ''' String
        ''' </summary>
        STRING_ID
#End Region
#Region "Chemistry"
        ''' <summary>
        ''' ChEMBL
        ''' </summary>
        CHEMBL_ID
        ''' <summary>
        ''' DrugBank
        ''' </summary>
        DRUGBANK_ID
        ''' <summary>
        ''' GuidetoPHARMACOLOGY
        ''' </summary>
        GUIDETOPHARMACOLOGY_ID
        ''' <summary>
        ''' SwissLipids
        ''' </summary>
        SWISSLIPIDS_ID
#End Region
#Region "Protein family/group databases"
        ''' <summary>
        ''' Allergome
        ''' </summary>
        ALLERGOME_ID
        ''' <summary>
        ''' ESTHER
        ''' </summary>
        ESTHER_ID
        ''' <summary>
        ''' MEROPS
        ''' </summary>
        MEROPS_ID
        ''' <summary>
        ''' mycoCLAP
        ''' </summary>
        MYCOCLAP_ID
        ''' <summary>
        ''' PeroxiBase
        ''' </summary>
        PEROXIBASE_ID
        ''' <summary>
        ''' REBASE
        ''' </summary>
        REBASE_ID
        ''' <summary>
        ''' TCDB
        ''' </summary>
        TCDB_ID
#End Region
#Region "Polymorphism and mutation databases"
        ''' <summary>
        ''' BioMuta
        ''' </summary>
        BIOMUTA_ID
        ''' <summary>
        ''' DMDM
        ''' </summary>
        DMDM_ID
#End Region
#Region "2D gel databases"
        ''' <summary>
        ''' World-2DPAGE
        ''' </summary>
        WORLD_2DPAGE_ID
#End Region
#Region "Protocols and materials databases"
        ''' <summary>
        ''' DNASU
        ''' </summary>
        DNASU_ID
#End Region
#Region "Genome annotation databases"
        ''' <summary>
        ''' Ensembl
        ''' </summary>
        ENSEMBL_ID
        ''' <summary>
        ''' Ensembl Protein
        ''' </summary>
        ENSEMBL_PRO_ID
        ''' <summary>
        ''' Ensembl Transcript
        ''' </summary>
        ENSEMBL_TRS_ID
        ''' <summary>
        ''' Ensembl Genomes
        ''' </summary>
        ENSEMBLGENOME_ID
        ''' <summary>
        ''' Ensembl Genomes Protein
        ''' </summary>
        ENSEMBLGENOME_PRO_ID
        ''' <summary>
        ''' Ensembl Genomes Transcript
        ''' </summary>
        ENSEMBLGENOME_TRS_ID
        ''' <summary>
        ''' GeneDB
        ''' </summary>
        GENEDB_ID
        '''' <summary>
        '''' GeneID (Entrez Gene)
        '''' </summary>
        'P_ENTREZGENEID
        ''' <summary>
        ''' KEGG
        ''' </summary>
        KEGG_ID
        ''' <summary>
        ''' PATRIC
        ''' </summary>
        PATRIC_ID
        ''' <summary>
        ''' UCSC
        ''' </summary>
        UCSC_ID
        ''' <summary>
        ''' VectorBase
        ''' </summary>
        VECTORBASE_ID
        ''' <summary>
        ''' WBParaSite
        ''' </summary>
        WBPARASITE_ID
#End Region
#Region "Organism-specific databases"
        ''' <summary>
        ''' ArachnoServer
        ''' </summary>
        ARACHNOSERVER_ID
        ''' <summary>
        ''' CCDS
        ''' </summary>
        CCDS_ID
        ''' <summary>
        ''' CGD
        ''' </summary>
        CGD
        ''' <summary>
        ''' ConoServer
        ''' </summary>
        CONOSERVER_ID
        ''' <summary>
        ''' dictyBase
        ''' </summary>
        DICTYBASE_ID
        ''' <summary>
        ''' EchoBASE
        ''' </summary>
        ECHOBASE_ID
        ''' <summary>
        ''' EcoGene
        ''' </summary>
        ECOGENE_ID
        ''' <summary>
        ''' euHCVdb
        ''' </summary>
        EUHCVDB_ID
        ''' <summary>
        ''' EuPathDB
        ''' </summary>
        EUPATHDB_ID
        ''' <summary>
        ''' FlyBase
        ''' </summary>
        FLYBASE_ID
        ''' <summary>
        ''' GeneCards
        ''' </summary>
        GENECARDS_ID
        ''' <summary>
        ''' GeneReviews
        ''' </summary>
        GENEREVIEWS_ID
        ''' <summary>
        ''' H-InvDB
        ''' </summary>
        H_INVDB_ID
        ''' <summary>
        ''' HGNC
        ''' </summary>
        HGNC_ID
        ''' <summary>
        ''' HPA
        ''' </summary>
        HPA_ID
        ''' <summary>
        ''' LegioList
        ''' </summary>
        LEGIOLIST_ID
        ''' <summary>
        ''' Leproma
        ''' </summary>
        LEPROMA_ID
        ''' <summary>
        ''' MaizeGDB
        ''' </summary>
        MAIZEGDB_ID
        ''' <summary>
        ''' MGI
        ''' </summary>
        MGI_ID
        ''' <summary>
        ''' MIM
        ''' </summary>
        MIM_ID
        ''' <summary>
        ''' neXtProt
        ''' </summary>
        NEXTPROT_ID
        ''' <summary>
        ''' Orphanet
        ''' </summary>
        ORPHANET_ID
        ''' <summary>
        ''' PharmGKB
        ''' </summary>
        PHARMGKB_ID
        ''' <summary>
        ''' PomBase
        ''' </summary>
        POMBASE_ID
        ''' <summary>
        ''' PseudoCAP
        ''' </summary>
        PSEUDOCAP_ID
        ''' <summary>
        ''' RGD
        ''' </summary>
        RGD_ID
        ''' <summary>
        ''' SGD
        ''' </summary>
        SGD_ID
        ''' <summary>
        ''' TAIR
        ''' </summary>
        TAIR_ID
        ''' <summary>
        ''' TubercuList
        ''' </summary>
        TUBERCULIST_ID
        ''' <summary>
        ''' WormBase
        ''' </summary>
        WORMBASE_ID
        ''' <summary>
        ''' WormBase Protein
        ''' </summary>
        WORMBASE_PRO_ID
        ''' <summary>
        ''' WormBase Transcript
        ''' </summary>
        WORMBASE_TRS_ID
        ''' <summary>
        ''' Xenbase
        ''' </summary>
        XENBASE_ID
        ''' <summary>
        ''' ZFIN
        ''' </summary>
        ZFIN_ID
#End Region
#Region "Phylogenomic databases"
        ''' <summary>
        ''' eggNOG
        ''' </summary>
        EGGNOG_ID
        ''' <summary>
        ''' GeneTree
        ''' </summary>
        GENETREE_ID
        ''' <summary>
        ''' HOGENOM
        ''' </summary>
        HOGENOM_ID
        ''' <summary>
        ''' HOVERGEN
        ''' </summary>
        HOVERGEN_ID
        ''' <summary>
        ''' KO
        ''' </summary>
        KO_ID
        ''' <summary>
        ''' OMA
        ''' </summary>
        OMA_ID
        ''' <summary>
        ''' OrthoDB
        ''' </summary>
        ORTHODB_ID
        ''' <summary>
        ''' TreeFam
        ''' </summary>
        TREEFAM_ID
#End Region
#Region "Enzyme and pathway databases"
        ''' <summary>
        ''' BioCyc
        ''' </summary>
        BIOCYC_ID
        ''' <summary>
        ''' Reactome
        ''' </summary>
        REACTOME_ID
        ''' <summary>
        ''' UniPathway
        ''' </summary>
        UNIPATHWAY_ID
#End Region
#Region "Gene expression databases"
        ''' <summary>
        ''' CleanEx
        ''' </summary>
        CLEANEX_ID
        ''' <summary>
        ''' CollecTF
        ''' </summary>
        COLLECTF_ID
#End Region
#Region "Other"
        ''' <summary>
        ''' ChiTaRS
        ''' </summary>
        CHITARS_ID
        ''' <summary>
        ''' GeneWiki
        ''' </summary>
        GENEWIKI_ID
        ''' <summary>
        ''' GenomeRNAi
        ''' </summary>
        GENOMERNAI_ID
#End Region
    End Enum
End Namespace

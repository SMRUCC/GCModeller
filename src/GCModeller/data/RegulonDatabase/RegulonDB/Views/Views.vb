#Region "Microsoft.VisualBasic::91cc972005dca87f01a630c3574e82f0, data\RegulonDatabase\RegulonDB\Views\Views.vb"

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

    '     Class Index
    ' 
    '         Properties: value
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    '     Class AttenuatorsPrediction
    ' 
    '         Properties: AntiAntiterminatorEnergy, AntiAntiterminatorSequence, AntiterminatorEnergy, AntiterminatorSequence, AttenuatorType
    '                     RegulatedGene, TerminatorEnergy, TerminatorSequence
    ' 
    '     Class BindingSitePredictionSet
    ' 
    '         Properties: AbsoluteLeft, AbsoluteRight, gi, Method, NameTF
    '                     Sequence, Strand
    ' 
    '     Class OperonPredictionSetDistances
    ' 
    '         Properties: Identifiers, Operonname, OperonStrand
    ' 
    '     Class PromoterPredictionSigma24Set
    ' 
    '         Properties: _10Box, _10Box_Lend, _10Box_Rend, _35Box, _35Box_Lend
    '                     _35Box_Rend, Evidence, EvidenceType, Gene, Homology_Level
    '                     Lend, Pos_1, Promoter_Name, Pvalue, Rend
    '                     Score, Sequence, Sigma, Signicance_Level, Spacer
    '                     Strand
    ' 
    '     Class RiboswitchesPrediction
    ' 
    '         Properties: GENE_NAME, RFAM_DESCRIPTION, RFAM_ID, RFAM_POSLEFT, RFAM_POSRIGHT
    '                     RFAM_SCORE, RFAM_SEQUENCE, RFAM_STRAND, RFAM_TYPE
    ' 
    '     Class TF_predicted
    ' 
    '         Properties: Bnumber, EvolutionaryFamily, GeneName, GI, MotifPosition
    '                     MotifPosition2, PFAMDescriptionMOTIF, PFAMDescriptionMOTIF2, PFAMID, Score
    '                     Status, UniProtID
    ' 
    '     Class TFBSs_predictions
    ' 
    '         Properties: Endsequence, ft_name, ft_type, ln_Pval, Pval
    '                     rank, rank_pm, seq_id, sig, start
    '                     strand, weight, Wmax, Wmin
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace RegulonDB.Views

    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class Index : Inherits Attribute

        Public ReadOnly Property value As Integer

        Sub New(index As Integer)
            value = index
        End Sub

        Public Overrides Function ToString() As String
            Return $"# {value}"
        End Function
    End Class

    ''' <summary>
    ''' AttenuatorsPrediction.txt
    ''' 
    ''' # Reference:
    ''' # "Merino Enrique., Yanofsky Charles. 2005 Transcription attenuation:
    ''' #  a highly conserved regulatory strategy used by bacteria. Trends Genet 21(5):260-4"
    ''' # Columns:
    ''' # 1) Regulated gene
    ''' # 2) Attenuator type
    ''' # 3) Terminator energy
    ''' # 4) Terminator sequence
    ''' # 5) Antiterminator energy
    ''' # 6) Antiterminator sequence
    ''' # 7) Anti-Antiterminator energy
    ''' # 8) Anti-Antiterminator sequence
    ''' # -------------------------------------------------------------------------------
    ''' </summary>
    Public Class AttenuatorsPrediction
        <Index(0)> Public Property RegulatedGene As String
        <Index(1)> Public Property AttenuatorType As String
        <Index(2)> Public Property TerminatorEnergy As String
        <Index(3)> Public Property TerminatorSequence As String
        <Index(4)> Public Property AntiterminatorEnergy As String
        <Index(5)> Public Property AntiterminatorSequence As String
        <Index(6)> Public Property AntiAntiterminatorEnergy As String
        <Index(7)> Public Property AntiAntiterminatorSequence As String
    End Class

    ''' <summary>
    ''' BindingSitePredictionSet.txt
    ''' 
    ''' # Citation
    ''' #
    ''' # User Is committed To cite properly the use Of source this data.
    ''' # Hernández, M., González, A., Espinosa, V., Vasconcelos, AT., Collado-Vides, J. (2004) Complementing computationally predicted regulatory sites In Tractor_DB Using a pattern matching approach. In Silico Biology., 5, 0020.
    ''' # _________________________________________________________________________________
    ''' #
    ''' # Columns:
    ''' #   (1) gi
    ''' #   (2) Name	TF
    ''' #   (3) Strand
    ''' #   (4) Absolute Left
    ''' #   (5) Absolute Right
    ''' #   (6) Sequence
    ''' #   (7) Method
    ''' #
    ''' </summary>
    Public Class BindingSitePredictionSet
        <Index(0)> Public Property gi As String
        <Index(1)> Public Property NameTF As String
        <Index(2)> Public Property Strand As String
        <Index(3)> Public Property AbsoluteLeft As String
        <Index(4)> Public Property AbsoluteRight As String
        <Index(5)> Public Property Sequence As String
        <Index(6)> Public Property Method As String
    End Class

    ''' <summary>
    ''' OperonPredictionSetDistances.txt,
    ''' OperonPredictionSetDistancesRiley.txt
    ''' 
    ''' # Reference: Moreno-Hagelsieb G. and Collado-Vides J. (2002) "Operon Conservation from
    ''' # the Point Of View Of Escherichia coli, And Inference Of Functional
    ''' # Interdependence Of Gene Products from Genome Context" In Silico Biology 2:
    ''' # 87-95 PMID : 12066843 http://regulondb.ccg.unam.mx/html/Publications.jsp#Related_papers
    ''' #
    ''' # 1) Operon name
    ''' # 2) Operon Strand.
    ''' # 3) Identifiers Of genes (GI) that belong To an operon (separated by ",")
    ''' #
    ''' </summary>
    Public Class OperonPredictionSetDistances
        <Index(0)> Public Property Operonname As String
        <Index(1)> Public Property OperonStrand As String
        ''' <summary>
        ''' Identifiers Of genes (GI) that belong To an operon (separated by ",")
        ''' </summary>
        ''' <returns></returns>
        <Index(2)> Public Property Identifiers As String
    End Class

    ''' <summary>
    ''' PromoterPredictionSigma24Set.txt, 
    ''' PromoterPredictionSigma28Set.txt,
    ''' PromoterPredictionSigma32Set.txt,
    ''' PromoterPredictionSigma38Set.txt,
    ''' PromoterPredictionSigma54Set.txt,
    ''' PromoterPredictionSigma70Set.txt
    ''' 
    ''' ;Computational Prediction of Promoters in the Escherichia coli genome.
    ''' ;Author: Araceli M. Huerta (amhuerta@ccg.unam.mx)
    ''' ;Pleace cite: A.M. Huerta, J. Collado-Vides, J Mol Biol. 333:261-78 (2003).
    ''' ;File Title: Sigma70, Sigma24, Sigma28, Sigma32, Sigma38, &amp; Sigma54 promoter predictions in E. coli K12.
    ''' ;Predictions were done in E. coli K12 from the NCBI version: NC_000913.2 (13-FEB-2011)
    ''' ;Predictions were calibrated using RegulonDB Version 7.0 (http://regulondb.ccg.unam.mx),
    ''' ;that does correspond to Ecocyc version 14.5
    ''' ;The data would be distributed following the Licence: http://regulondb.ccg.unam.mx/menu/download/full_version/license_regulondb_registration.jsp
    ''' ;
    ''' ;The data contain 4342 Sigma70 promoter predictions: lowest score = 1.334(P - Value() = 0.000850233109777606), highest score = 6.726 (P-value = 1.44124966379965E-06)
    ''' ;The data contain 3964 Sigma38 promoter predictions: lowest score = 2.48(P - Value() = 0.0139817831594055), highest score = 7.31 (P-value = 1.34034288573999E-05)
    ''' ;The data contain 752 Sigma32 promoter predictions: lowest score = 1.865(P - Value() = 0.000248516827317573), highest score = 10.953 (P-value = 6.30082606126306E-08)
    ''' ;The data contain 524 Sigma24 promoter predictions: lowest score = -3.81(P - Value() = 0.000106219802845229), highest score = 9.705 (P-value = 1.01563147242684E-06)
    ''' ;The data contain 212 Sigma28 promoter predictions: lowest score = 2.996(P - Value() = 0.0000887222371802212), highest score = 14.789 (P-value = 6.00904502872066E-09)
    ''' ;The data contain 79 Sigma54 promoter predictions: lowest score = 0.48(P - Value() = 0.00000358052901567964), highest score = 12.405 (P-value = 1.83692991299296E-07)
    ''' ;Promoter Predictions can be read in http://www.ccg.unam.mx/Computational_Genomics/PromoterTools
    ''' ;---------------------------------------------------------------------------------------
    ''' ;(1)Lend	(2)Rend	(3)Strand	(4)Gene	(5)Promoter_Name	(6)Sigma	(7)-35Box_Lend	(8)-35Box_Rend	(9)-35Box	(10)Spacer	(11)-10Box_Lend	(12)-10Box_Rend	(13)-10Box	(14)Pos_1	(15)Score	(16)Homology_Level	(17)P-value	(18)Signicance_Level	(19)Evidence	(20)EvidenceType	(21)Sequence
    ''' </summary>
    Public Class PromoterPredictionSigma24Set
        <Index(1)> Public Property Lend As String
        <Index(2)> Public Property Rend As String
        <Index(3)> Public Property Strand As String
        <Index(4)> Public Property Gene As String
        <Index(5)> Public Property Promoter_Name As String
        <Index(6)> Public Property Sigma As String
        <Column("-35Box_Lend")> <Index(7)> Public Property _35Box_Lend As String
        <Column("-35Box_Rend")> <Index(8)> Public Property _35Box_Rend As String
        <Column("-35Box")> <Index(9)> Public Property _35Box As String
        <Index(10)> Public Property Spacer As String
        <Column("-10Box_Lend")> <Index(11)> Public Property _10Box_Lend As String
        <Column("-10Box_Rend")> <Index(12)> Public Property _10Box_Rend As String
        <Column("-10Box")> <Index(13)> Public Property _10Box As String
        <Index(14)> Public Property Pos_1 As String
        <Index(15)> Public Property Score As String
        <Index(16)> Public Property Homology_Level As String
        <Column("P-value")> <Index(17)> Public Property Pvalue As String
        <Index(18)> Public Property Signicance_Level As String
        <Index(19)> Public Property Evidence As String
        <Index(20)> Public Property EvidenceType As String
        <Index(21)> Public Property Sequence As String
    End Class

    ''' <summary>
    ''' RiboswitchesPrediction.txt
    ''' 
    ''' # Reference:
    ''' #  "Griffiths-Jones Sam., Moxon Simon., Marshall Mhairi., Khanna Ajay., Eddy Sean R., Bateman Alex. 2005 Rfam: annotating
    ''' # non-coding RNAs In complete genomes. Nucleic Acids Res 33:D121-4"
    ''' #
    ''' # Table name: RFAM
    ''' # (1)    RFAM_ID
    ''' # (2)    GENE_NAME
    ''' # (3)    RFAM_TYPE
    ''' # (4)    RFAM_DESCRIPTION
    ''' # (5)    RFAM_SCORE
    ''' # (6)    RFAM_STRAND
    ''' # (7)    RFAM_POSLEFT
    ''' # (8)    RFAM_POSRIGHT
    ''' # (9)    RFAM_SEQUENCE
    ''' </summary>
    Public Class RiboswitchesPrediction
        <Index(1)> Public Property RFAM_ID As String
        <Index(2)> Public Property GENE_NAME As String
        <Index(3)> Public Property RFAM_TYPE As String
        <Index(4)> Public Property RFAM_DESCRIPTION As String
        <Index(5)> Public Property RFAM_SCORE As String
        <Index(6)> Public Property RFAM_STRAND As String
        <Index(7)> Public Property RFAM_POSLEFT As String
        <Index(8)> Public Property RFAM_POSRIGHT As String
        <Index(9)> Public Property RFAM_SEQUENCE As String
    End Class

    ''' <summary>
    ''' TF_predicted.txt
    ''' 
    ''' # 
    ''' #  		Transcription Factor Prediction Dataset  Version 2.0
    ''' #
    ''' #  184 TFs experimentally characterized And deposited In RegulonDB  were used As 
    ''' # seeds In BLASTP searches against the complete proteome Of E. coli. E-values ≤ 1E-6 
    ''' # And a coverage Of 70% were considered. In addition, TFs specifically associated To 
    ''' # E. coli K12 And deposited In DBD database, HAMAP, Superfamily DB And PFAM 
    ''' # databases were retrieved.
    ''' #
    ''' # Reference:
    ''' #
    ''' # The functional landscape bound To the transcription factors Of Escherichia coli K-12.
    ''' # Pérez-Rueda E, Tenorio-Salgado S, Huerta-Saquero A, Balderas-Martínez YI, Moreno-Hagelsieb G.
    ''' # Comput Biol Chem. 2015 Jun 6;58:93-103. doi: 10.1016/j.compbiolchem.2015.06.002.
    ''' # PMID: 26094112 
    ''' # _____________________________________________________________________________
    ''' # Column Description:
    ''' #  (1) Gene Name
    ''' #  (2) Bnumber (Locus)
    ''' #  (3) UniProt ID
    ''' #  (4) GI
    ''' #  (5) Evolutionary Family
    ''' #  (6) Status [Prediction, Experimental characterized]
    ''' #  (7) Score
    ''' #  (8) PFAM ID
    ''' #  (9) PFAM Description MOTIF
    ''' # (10) Motif Position
    ''' # (11) PFAM Description MOTIF
    ''' # (12) Motif Position
    ''' #
    ''' </summary>
    Public Class TF_predicted
        <Index(1)> Public Property GeneName As String
        <Column("Bnumber(Locus)")> <Index(2)> Public Property Bnumber As String
        <Index(3)> Public Property UniProtID As String
        <Index(4)> Public Property GI As String
        <Index(5)> Public Property EvolutionaryFamily As String
        <Column("Status [Prediction, Experimental characterized]")> <Index(6)> Public Property Status As String
        <Index(7)> Public Property Score As String
        <Index(8)> Public Property PFAMID As String
        <Index(9)> Public Property PFAMDescriptionMOTIF As String
        <Index(10)> Public Property MotifPosition As String
        <Index(11)> Public Property PFAMDescriptionMOTIF2 As String
        <Index(12)> Public Property MotifPosition2 As String
    End Class

    ''' <summary>
    ''' TFBSs_predictions_v3.0.txt
    ''' 
    ''' # Title: Computationally predicted transcription factor binding sites (TFBSs) using the evaluated weight matrix v3.0
    ''' #
    ''' #
    ''' # Description Of the dataset:
    ''' #
    ''' # Computationally predicted transcription factor binding sites (TFBSs) In
    ''' # upstream regions Of the Escherichia coli K-12 genome, based On version 8.8
    ''' # Of RegulonDB.
    ''' # We scanned all upstream regions Of every Single gene, from +50 To -400 Or
    ''' # from +50 To the closest upstream ORF, whatever happens first.
    ''' #
    ''' #
    ''' # Citation:
    ''' #
    ''' # Dataset provided And maintained by RegulonDB (PUBMED: #21051347) from the
    ''' # original source published In: Medina-Rivera et al. Theoretical And
    ''' # empirical quality assessment Of transcription factor-binding motifs. Nucleic Acids
    ''' # Research (2011) vol. 39 (3) pp. 808-824 (PubMed: 20923783)
    ''' #
    ''' # Columns:
    ''' # 1. - map name (eg: gene name),
    ''' # 2. - feature type (site, ORF),
    ''' # 3. - identifier(ex: GATA_box, Abf1_site)
    ''' # 4. - strand (D For Direct, R For Reverse),
    ''' # 5. - start position (may be negative)
    ''' # 6. - End position (may be negative)
    ''' # 7. - (Optional) sequence (ex: AAGATAAGCG)
    ''' # 8. - (Optional) The weight Of a sequence segment
    ''' # 9.- Pval. The significance Of the weight associated To Each site
    ''' # 10.- ln_Pval
    ''' # 11.- Significance. sig = -log(P-value)
    ''' #
    ''' # seq_id	ft_type	ft_name	strand	start	End	sequence	weight	Pval	ln_Pval	sig	Wmin	Wmax	rank	rank_pm
    ''' </summary>
    Public Class TFBSs_predictions
        <Index(0)> Public Property seq_id As String
        <Index(1)> Public Property ft_type As String
        <Index(2)> Public Property ft_name As String
        <Index(3)> Public Property strand As String
        <Index(4)> Public Property start As String
        <Index(5)> Public Property Endsequence As String
        <Index(6)> Public Property weight As String
        <Index(7)> Public Property Pval As String
        <Index(8)> Public Property ln_Pval As String
        <Index(9)> Public Property sig As String
        <Index(10)> Public Property Wmin As String
        <Index(11)> Public Property Wmax As String
        <Index(12)> Public Property rank As String
        <Index(13)> Public Property rank_pm As String
    End Class
End Namespace

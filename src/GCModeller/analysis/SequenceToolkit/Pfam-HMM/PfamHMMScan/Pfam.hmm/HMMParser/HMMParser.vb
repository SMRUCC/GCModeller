#Region "Microsoft.VisualBasic::f03996d0b4a7716c64a6440fe38255f7, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\Pfam.hmm\HMMParser\HMMParser.vb"

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

    ' Class HMMParser
    ' 
    '     Properties: [DATE], ACC, ALPH, BM, CKSUM
    '                 COM, CONS, CS, DESC, EFFN
    '                 GA, HMM, LENG, MAP, MAXL
    '                 MM, NAME, NC, NSEQ, RF
    '                 SM, STATS, TC
    ' 
    ' Structure STATS
    ' 
    '     Properties: FORWARD, MSV, VITERBI
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' HMMER3/f [3.1b2 | February 2015]
''' </summary>
Public Class HMMParser

    ''' <summary>
    ''' Model name; &lt;s> is a single word containing no spaces or tabs. The name is normally picked up
    ''' from the #=GF ID line from a Stockholm alignment file. If this Is Not present, the name Is created
    ''' from the name Of the alignment file by removing any file type suffix. For example, an otherwise
    ''' nameless HMM built from the alignment file rrm.slx would be named rrm. Mandatory.
    ''' </summary>
    ''' <returns></returns>
    Public Property NAME As String
    ''' <summary>
    ''' Accession number; &lt;s> is a one-word accession number. This is picked up from the #=GF AC
    ''' line in a Stockholm format alignment. Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property ACC As String
    ''' <summary>
    ''' Description line; &lt;s> is a one-line free text description. This is picked up from the #=GF DE line
    ''' in a Stockholm alignment file. Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property DESC As String
    ''' <summary>
    ''' Model length; &lt;d>, a positive nonzero integer, is the number of match states in the model.
    ''' Mandatory.
    ''' </summary>
    ''' <returns></returns>
    Public Property LENG As Integer
    ''' <summary>
    ''' Max instance length; &lt;d>, a positive nonzero integer, is the upper bound on the length at which
    ''' And instance of the model Is expected to be found. Used only by nhmmer And nhmmscan. Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property MAXL As Integer
    ''' <summary>
    ''' Symbol alphabet type. For biosequence analysis models, &lt;s> is amino, DNA, or RNA (case insensitive).
    ''' There are also other accepted alphabets For purposes beyond biosequence analysis,
    ''' including coins, dice, and custom. This determines the symbol alphabet And the size Of the symbol
    ''' emission probability distributions. If amino, the alphabet size K Is Set To 20 And the symbol
    ''' alphabet to “ACDEFGHIKLMNPQRSTVWY” (alphabetic order); if DNA, the alphabet size K Is set
    ''' to 4 And the symbol alphabet to “ACGT”; if RNA, the alphabet size K Is set to 4 And the symbol
    ''' alphabet to “ACGU”. Mandatory.
    ''' </summary>
    ''' <returns></returns>
    Public Property ALPH As String
    ''' <summary>
    ''' Reference annotation flag; &lt;s> is either no or yes (case insensitive). If yes, the reference annotation
    ''' character field For Each match state In the main model (see below) Is valid; If no, these
    ''' characters are ignored. Reference column annotation Is picked up from a Stockholm alignment
    ''' file's #=GC RF line. It is propagated to alignment outputs, and also may optionally be used to define
    ''' consensus match columns In profile HMM construction. Optional; assumed To be no If Not
    ''' present.
    ''' </summary>
    ''' <returns></returns>
    Public Property RF As String
    ''' <summary>
    ''' Model masked flag; &lt;s> is either no or yes (case insensitive). If yes, the model mask annotation
    ''' character field For Each match state In the main model (see below) Is valid; If no, these characters
    ''' are ignored. Indicates that the profile model was created such that emission probabilities at masked
    ''' positions are Set To match the background frequency, rather than being Set based On observed
    ''' frequencies in the alignment. Position-specific insertion And deletion rates are Not altered, even in
    ''' masked regions. Optional; assumed To be no If Not present.
    ''' </summary>
    ''' <returns></returns>
    Public Property MM As String
    ''' <summary>
    ''' Consensus residue annotation flag; &lt;s> is either no or yes (case insensitive). If yes, the consensus
    ''' residue field For Each match state In the main model (see below) Is valid. If no, these characters
    ''' are ignored. Consensus residue annotation Is determined When models are built. For models Of
    ''' Single sequences, the consensus Is the same As the query sequence. For models Of multiple alignments,
    ''' the consensus Is the maximum likelihood residue at Each position. Upper Case indicates
    ''' that the model's emission probability for the consensus residue is an arbitrary threshold (0.5 for
    ''' protein models, 0.9 for DNA/RNA models).
    ''' </summary>
    ''' <returns></returns>
    Public Property CONS As String
    ''' <summary>
    ''' Consensus structure annotation flag; &lt;s> is either no or yes (case insensitive). If yes, the consensus
    ''' Structure character field For Each match state In the main model (see below) Is valid; If no
    ''' these characters are ignored. Consensus Structure annotation Is picked up from a Stockholm file's
    ''' #=GC SS_cons line, And propagated to alignment displays. Optional; assumed to be no if Not
    ''' present.
    ''' </summary>
    ''' <returns></returns>
    Public Property CS As String
    ''' <summary>
    ''' Map annotation flag; &lt;s> is either no or yes (case insensitive). If set to yes, the map annotation
    ''' field in the main model (see below) Is valid; if no, that field will be ignored. The HMM/alignment map
    ''' annotates each match state with the index of the alignment column from which it came. It can be
    ''' used for quickly mapping any subsequent HMM alignment back to the original multiple alignment,
    ''' via the model. Optional; assumed To be no If Not present.
    ''' </summary>
    ''' <returns></returns>
    Public Property MAP As String
    ''' <summary>
    ''' Date the model was constructed; &lt;s> is a free text date string. This field is only used for logging
    ''' purposes.y Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property [DATE] As String
    ''' <summary>
    ''' Command line log; &lt;n> counts command line numbers, and &lt;s> is a one-line command.
    ''' There may be more than one COM line per save file, Each numbered starting from n = 1. These
    ''' lines record every HMMER command that modified the save file. This helps us reproducibly And
    ''' automatically log how Pfam models have been constructed, For example. Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property COM As String
    ''' <summary>
    ''' Sequence number; &lt;d> is a nonzero positive integer, the number of sequences that the HMM
    ''' was trained On. This field Is only used For logging purposes. Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property NSEQ As Integer
    ''' <summary>
    ''' Effective sequence number; &lt;f> is a nonzero positive real, the effective total number of sequences
    ''' determined by hmmbuild during sequence weighting, For combining observed counts With
    ''' Dirichlet prior information In parameterizing the model. This field Is only used For logging purposes.
    ''' Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property EFFN As Double
    ''' <summary>
    ''' Training alignment checksum; &lt;d> is a nonnegative unsigned 32-bit integer. This number is
    ''' calculated from the training sequence data, And used In conjunction With the alignment map information
    ''' to verify that a given alignment Is indeed the alignment that the map Is for. Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property CKSUM As Long
    ''' <summary>
    ''' Pfam gathering thresholds GA1 and GA2. See Pfam documentation of GA lines. Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property GA As Double()
    ''' <summary>
    ''' Pfam trusted cutoffs TC1 and TC2. See Pfam documentation of TC lines. Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property TC As Double()
    ''' <summary>
    ''' Pfam noise cutoffs NC1 and NC2. See Pfam documentation of NC lines. Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property NC As Double()
    Public Property BM As String
    Public Property SM As String
    ''' <summary>
    ''' Statistical parameters needed for E-value calculations. &lt;s1> is the model’s
    ''' alignment mode configuration: currently only LOCAL Is recognized. &lt;s2> Is the name Of the score
    ''' distribution: currently MSV, VITERBI, and FORWARD are recognized. &lt;f1> And &lt;f2> are two realvalued
    ''' parameters controlling location And slope Of Each distribution, respectively; And For
    ''' Gumbel distributions For MSV And Viterbi scores, And And For exponential tails For Forward
    ''' scores. values must be positive. All three lines Or none of them must be present: when all three
    ''' are present, the model Is considered To be calibrated For E-value statistics. Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property STATS As STATS
    ''' <summary>
    ''' The first line in the main model section may be an optional line starting with COMPO: these are
    ''' the model's overall average match state emission probabilities, which are used as a background
    ''' residue composition In the “filter null” model. The K fields On this line are log probabilities For Each
    ''' residue in the appropriate biosequence alphabet's order. Optional.
    ''' </summary>
    ''' <returns></returns>
    Public Property HMM As HMM
End Class

''' <summary>
''' Statistical parameters needed for E-value calculations. &lt;s1> is the model’s
''' alignment mode configuration: currently only LOCAL Is recognized. &lt;s2> Is the name Of the score
''' distribution: currently MSV, VITERBI, and FORWARD are recognized. &lt;f1> And &lt;f2> are two realvalued
''' parameters controlling location And slope Of Each distribution, respectively; And For
''' Gumbel distributions For MSV And Viterbi scores, And And For exponential tails For Forward
''' scores. values must be positive. All three lines Or none of them must be present: when all three
''' are present, the model Is considered To be calibrated For E-value statistics. Optional.
''' </summary>
Public Structure STATS
    Public Property MSV As Double()
    Public Property VITERBI As Double()
    Public Property FORWARD As Double()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure

#Region "Microsoft.VisualBasic::8d039edda112c7f2660eecf9072269e3, analysis\Motifs\PrimerDesigner\Restriction_enzyme\Enzyme.vb"

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

    '   Total Lines: 165
    '    Code Lines: 60
    ' Comment Lines: 90
    '   Blank Lines: 15
    '     File Size: 8.59 KB


    '     Class Enzyme
    ' 
    '         Properties: Cut, Enzyme, Isoschizomers, PDB, Recognition
    '                     Source
    ' 
    '         Function: GetCutSite, ToString
    ' 
    '     Class Recognition
    ' 
    '         Properties: Forwards, Reversed
    ' 
    '         Function: ToString
    ' 
    '     Class Cut
    ' 
    '         Properties: CutSite1, CutSite2, IsSingle, Reversed
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Restriction_enzyme

    ''' <summary>
    ''' A restriction enzyme or restriction endonuclease is a special type of biological macromolecule that functions as part of the "immune system" in bacteria.
    ''' One special kind of restriction enzymes is the class of "homing endonucleases", these being present in all three domains of life,
    ''' although their function seems to be very different from one domain to another.
    ''' The classical restriction enzymes cut up, And hence render harmless, any unknown (non-cellular) DNA that enters a bacterial cell As a result Of a viral infection.
    ''' They recognize a specific DNA sequence, usually Short (3 To 8 bp), And cut it, producing either blunt Or overhung ends, either at Or nearby the recognition site.
    ''' Restriction enzymes are quite variable In the Short DNA sequences they recognize.
    ''' An organism often has several different enzymes, Each specific To a distinct Short DNA sequence.[1]
    ''' </summary>
    Public Class Enzyme

        ''' <summary>
        ''' Accepted name of the molecule, according to the internationally adopted nomenclature[2][3], and bibliographical references.
        ''' (Further reading: see the section "Nomenclature" in the article "Restriction enzyme".)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Enzyme As String
        ''' <summary>
        ''' Code used to identify the structure of a protein in the PDB database of protein structures.
        ''' The 3D atomic structure of a protein provides highly valuable information to understand the intimate details of its
        ''' mechanism of action[4][5].
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property PDB As String
        ''' <summary>
        ''' Organism that naturally produces the enzyme.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Source As String
        ''' <summary>
        ''' Sequence of DNA recognized by the enzyme and to which it specifically binds.
        ''' </summary>
        ''' <returns></returns>
        Public Property Recognition As Recognition
        ''' <summary>
        ''' Cutting site and DNA products of the cut. The recognition sequence and the cutting site usually match,
        ''' but sometimes the cutting site can be dozens of nucleotides away from the recognition site[6][7].
        ''' (对所识别的位点<see cref="Recognition"/>的剪切的模式)
        ''' </summary>
        ''' <returns></returns>
        Public Property Cut As Cut()
        ''' <summary>
        ''' Isoschizomers are pairs of restriction enzymes specific to the same recognition sequence. For example, SphI (CGTAC/G) and BbuI (CGTAC/G) are isoschizomers of each other.
        ''' The first enzyme discovered which recognizes a given sequence is known as the prototype; all subsequently identified enzymes that recognize that sequence are isoschizomers.
        ''' Isoschizomers are isolated from different strains of bacteria and therefore may require different reaction conditions.
        ''' An enzyme that recognizes the same sequence but cuts it differently Is a neoschizomer. Neoschizomers are a specific type (subset) Of isoschizomer.
        ''' For example, SmaI (CCC/GGG) And XmaI (C/CCGGG) are neoschizomers Of Each other.
        ''' An enzyme that recognizes a slightly different sequence, but produces the same ends Is an isocaudomer.
        ''' In some cases, only one out of a pair of isoschizomers can recognize both the methylated as well as unmethylated forms of restriction sites.
        ''' In contrast, the other restriction enzyme can recognize only the unmethylated form of the restriction site.
        ''' This property of some isoschizomers allows identification of methylation state of the restriction site while isolating it from a bacterial strain.
        ''' For example, the restriction enzymes HpaII And MspI are isoschizomers, as they both recognize the sequence 5'-CCGG-3' when it is unmethylated.
        ''' But when the second C of the sequence is methylated, only MspI can recognize it while HpaII cannot.
        ''' </summary>
        ''' <returns></returns>
        Public Property Isoschizomers As String()

        Public Function GetCutSite(strand As Strands) As Cut
            If strand = Strands.Forward Then
                Return Cut.Where(Function(c) Not c.Reversed).FirstOrDefault
            Else
                Return Cut.Where(Function(c) c.Reversed).FirstOrDefault
            End If
        End Function

        Public Overrides Function ToString() As String
            Return Enzyme & ": " & Recognition.ToString
        End Function
    End Class

    ''' <summary>
    ''' Recognition sequence
    '''
    ''' The recognition sequence, sometimes also referred to as recognition site, 
    ''' of any DNA-binding protein motif that exhibits binding specificity, refers 
    ''' to the DNA sequence (or subset thereof), to which the domain is specific. 
    ''' Recognition sequences are palindromes .
    ''' 
    ''' The transcription factor Sp1 For example, binds the sequences 
    ''' 5'-(G/T)GGGCGG(G/A)(G/A)(C/T)-3', where (G/T) indicates that the domain 
    ''' will bind a guanine or thymine at this position.
    ''' 
    ''' The restriction endonuclease PstI recognizes, binds, And cleaves the sequence 5'-CTGCAG-3'.
    ''' However, a recognition sequence refers to a different aspect from that of recognition 
    ''' site. A given recognition sequence can occur one Or more times, Or Not at all on a 
    ''' specific DNA fragment. A recognition site Is specified by the position of the site. 
    ''' For example, there are two PstI recognition site in the following DNA sequence fragment, 
    ''' start at base 9 And 31 respectively. A recognition sequence Is a specific sequence, 
    ''' usually very short (less than 10 bases). Depending on the degree of specificity of 
    ''' the protein, a DNA-binding protein can bind to more than one specific sequence. For PstI,
    ''' which has a single sequence specificity, it Is 5'-CTGCAG-3'. It is always the same whether 
    ''' at the first recognition site or the second in the following example sequence. For Sp1,
    ''' which has multiple (16) sequence specificity as shown above, the two recognition sites 
    ''' in the following example sequence fragment are at 18 and 32, and their respective recognition
    ''' sequences are 5'-GGGGCGGAGC-3' and 5'-TGGGCGGAAC-3'.
    ''' 
    ''' 5'-AACGTTAGCTGCAGTCGGGGCGGAGCTAGGCTGCAGGAATTGGGCGGAACCT-3'
    ''' </summary>
    Public Class Recognition

        <XmlAttribute> Public Property Forwards As String
        <XmlAttribute> Public Property Reversed As String

        Default Public ReadOnly Property GetSite(strand As Strands) As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return If(strand = Strands.Forward, Forwards, Reversed)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Forwards
        End Function
    End Class

    ''' <summary>
    ''' 
    ''' ```
    ''' <see cref="CutSite1"/> | <see cref="CutSite2"/>
    ''' ```
    ''' 
    ''' or 
    ''' 
    ''' ```
    ''' <see cref="CutSite1"/>
    ''' ```
    ''' 
    ''' 对所识别的位点<see cref="Recognition"/>的剪切的模式
    ''' </summary>
    Public Class Cut

        <XmlAttribute> Public Property Reversed As Boolean
        <XmlAttribute> Public Property CutSite1 As String
        <XmlAttribute> Public Property CutSite2 As String

        <XmlIgnore>
        Public ReadOnly Property IsSingle As Boolean
            Get
                Return CutSite2.StringEmpty
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim cutSite As String

            If IsSingle Then
                cutSite = CutSite1
            Else
                cutSite = CutSite1 & " " & CutSite2
            End If

            If Reversed Then
                Return $"3' ---{cutSite}--- 5'"
            Else
                Return $"5' ---{cutSite}--- 3'"
            End If
        End Function
    End Class
End Namespace

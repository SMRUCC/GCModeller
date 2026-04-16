#Region "Microsoft.VisualBasic::3fc29851d21350021506e01c733e5ccc, engine\Model\Cellular\RNATypes.vb"

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

    '   Total Lines: 79
    '    Code Lines: 17 (21.52%)
    ' Comment Lines: 49 (62.03%)
    '    - Xml Docs: 97.96%
    ' 
    '   Blank Lines: 13 (16.46%)
    '     File Size: 3.44 KB


    '     Enum RNATypes
    ' 
    '         AntisenseRNAs, hnRNA, micsRNA, miRNAs, ribosomalRNA
    '         Riboswitches, snoRNAs, sRNAs, tmRNA, tRNA
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

Namespace Cellular

    ''' <summary>
    ''' Enumerates the data types of the RNA molecules
    ''' </summary>
    ''' <remarks>
    ''' The exact types and abundance of RNA molecules can vary between different species of prokaryotic bacteria, 
    ''' but generally, the three main types—mRNA, rRNA, and tRNA—are universally present. The presence and function 
    ''' of other types of RNA in prokaryotes are an area of ongoing research.
    ''' </remarks>
    Public Enum RNATypes As Byte

        ''' <summary>
        ''' Messenger RNA (mRNA): Carries the genetic code from DNA to the ribosome, where it is translated into a protein. Could be translate to protein pipetide
        ''' </summary>
        ''' <remarks>
        ''' default type of the RNA molecule
        ''' </remarks>
        mRNA = 0
        ''' <summary>
        ''' Transfer RNA (tRNA): Brings the correct amino acid to the ribosome in accordance with the codon sequence of the mRNA during translation. 
        ''' Helper rna molecule for the protein transaltion
        ''' </summary>
        tRNA

        ''' <summary>
        ''' Ribosomal RNA (rRNA): Forms the structural and functional components of ribosomes, which are the molecular machines that catalyze protein synthesis. 
        ''' In prokaryotes, the most common rRNAs are <strong>16S, 23S, and 5S</strong>.
        ''' </summary>
        <Description("rRNA")>
        ribosomalRNA

        ' In addition to these main types, prokaryotes may also have other types of RNA, including:

        ''' <summary>
        ''' Heterogeneous nuclear RNA (hnRNA): This term is more commonly used in eukaryotes, but prokaryotes also have precursor RNAs 
        ''' that may serve similar roles before being processed into mature mRNA.
        ''' </summary>
        hnRNA

        ''' <summary>
        ''' Small regulatory RNAs (sRNAs): These are non-coding RNAs that can regulate gene expression by interacting with mRNA or other RNAs, 
        ''' often by base-pairing with target sequences.
        ''' </summary>
        sRNAs

        ''' <summary>
        ''' Riboswitches: These are structured regions within some mRNA molecules that can bind small molecules and regulate gene expression by controlling translation or RNA stability.
        ''' </summary>
        Riboswitches

        ''' <summary>
        ''' tmRNA (transfer-messenger RNA): In prokaryotes, tmRNA tags unfinished proteins for degradation and helps to rescue ribosomes that would otherwise be trapped on truncated mRNAs.
        ''' </summary>
        tmRNA

        ''' <summary>
        ''' Small nucleolar RNAs (snoRNAs): Though more common in eukaryotes, some prokaryotes have snoRNAs that guide chemical modifications of rRNA.
        ''' </summary>
        snoRNAs

        ''' <summary>
        ''' MicroRNAs (miRNAs): These are typically associated with eukaryotes, but recent studies have suggested that prokaryotes may also possess small RNAs with similar functions.
        ''' </summary>
        miRNAs

        ''' <summary>
        ''' Antisense RNAs: These can base-pair with sense mRNA and regulate its expression, stability, or translation
        ''' </summary>
        AntisenseRNAs

        ''' <summary>
        ''' 其他类型的RNA
        ''' </summary>
        micsRNA
    End Enum
End Namespace

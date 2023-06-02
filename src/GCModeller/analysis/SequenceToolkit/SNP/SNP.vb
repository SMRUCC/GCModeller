#Region "Microsoft.VisualBasic::4390af87de7fa23466762ebeab219102, GCModeller\analysis\SequenceToolkit\SNP\SNP.vb"

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

'   Total Lines: 111
'    Code Lines: 64
' Comment Lines: 38
'   Blank Lines: 9
'     File Size: 3.48 KB


' Class SNP
' 
'     Properties: AminoAcidChange, CDS, CDSInterval, CDSPosition, Change
'                 CodonChange, CodonNumber, Coverage, gene, Intervals
'                 Left, Length, Name, note, PolymorphismType
'                 PositionWithinCodon, product, protein_id, ProteinEffect, ReferenceFrequency
'                 ReferenceNucleotides, Right, SignificantStrandBias, Site, StrandBias
'                 TotalLength, VariantFrequency, VariantNucleotides, VariantRawFrequency, VariantSequences
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.ComponentModel.Loci

''' <summary>
''' SNP site
''' </summary>
''' <remarks>
''' ###### 2016-11-30
''' Column names corrected as the list:
''' 
''' + Min (original sequence)	
''' + Max (original sequence)	
''' + Change	
''' + Coverage	
''' + Polymorphism Type	
''' + Reference Nucleotide(s)	
''' + Variant Frequency	
''' + Variant Nucleotide(s)	
''' + Amino Acid Change	
''' + CDS	
''' + CDS Codon Number	
''' + CDS Interval	
''' + CDS Position	
''' + CDS Position Within Codon	
''' + Codon Change	
''' + gene	
''' + note	
''' + product	
''' + Protein Effect	
''' + protein_id	
''' + Variant Raw Frequency	
''' + Variant Sequences
''' </remarks>
Public Class SNP
    Implements IMotifSite
    Implements IMotifScoredSite

    <Column("Sequence Name")>
    Public Property Name As String
    <Column("Min (original sequence)")> Public Property Left As Integer
    <Column("Max (original sequence)")> Public Property Right As Integer
    Public Property Length As Integer
    Public Property Change As String
    Public Property Coverage As Double Implements IMotifScoredSite.Score

    <Column("# Intervals")>
    Public Property Intervals As Integer
    <Column("Length (with gaps)")>
    Public Property TotalLength As Integer
    <Column("Polymorphism Type")>
    Public Property PolymorphismType As String Implements IMotifSite.family
    <Column("Variant Nucleotide(s)")>
    Public Property VariantNucleotides As String

    Public Property StrandBias As Double
    <Column("Strand-Bias >50% P-value")>
    Public Property SignificantStrandBias As String
    <Column("Variant Frequency")>
    Public Property VariantFrequency As Double
    <Column("Variant Raw Frequency")>
    Public Property VariantRawFrequency As Double
    <Collection("Variant Sequences", ", ")>
    Public Property VariantSequences As String()

    Public Property ReferenceFrequency As Double
    <Column("Reference Nucleotide(s)")>
    Public Property ReferenceNucleotides As String

    Public Property gene As String
    Public Property note As String
    Public Property product As String
    <Column("Protein Effect")>
    Public Property ProteinEffect As String
    Public Property protein_id As String Implements IMotifSite.name

    ''' <summary>
    ''' 这个SNP位点可能引起的氨基酸序列上面的残基的变化
    ''' </summary>
    ''' <returns></returns>
    <Column("Amino Acid Change")>
    Public Property AminoAcidChange As String
    Public Property CDS As String
    <Column("CDS Codon Number")>
    Public Property CodonNumber As String
    <Column("CDS Position")>
    Public Property CDSPosition As String
    <Column("CDS Position Within Codon")>
    Public Property PositionWithinCodon As String
    <Column("CDS Interval")>
    Public Property CDSInterval As String

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    <Column("Codon Change")>
    Public Property CodonChange As String

    Private Property Site As Location Implements IMotifSite.site
        Get
            Return New Location(Left, Right)
        End Get
        Set(value As Location)
            With value
                Left = .left
                Right = .right
            End With
        End Set
    End Property
End Class

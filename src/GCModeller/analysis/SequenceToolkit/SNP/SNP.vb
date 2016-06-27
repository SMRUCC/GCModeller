Imports System.Data.Linq.Mapping
Imports Microsoft.VisualBasic.Language

''' <summary>
''' SNP site
''' </summary>
Public Class SNP : Inherits ClassObject

    <Column(Name:="Sequence Name")> Public Property Name As String
    Public Property Left As Integer
    Public Property Right As Integer
    Public Property Length As Integer
    <Column(Name:="# Intervals")> Public Property Intervals As Integer
    <Column(Name:="Length (with gaps)")> Public Property TotalLength As Integer
    Public Property PolymorphismType As String
    <Column(Name:="Variant Nucleotide(s)")> Public Property VariantNucleotides As String
    Public Property Coverage As Double
    Public Property StrandBias As Double
    <Column(Name:="Strand-Bias >50% P-value")> Public Property SignificantStrandBias
    Public Property VariantFrequency As Double
    Public Property VariantRawFrequency As Double
    Public Property VariantSequences As String
    Public Property Change As String
    Public Property ReferenceFrequency As Double
    Public Property ReferenceNucleotides As String
    ''' <summary>
    ''' 这个SNP位点可能引起的氨基酸序列上面的残基的变化
    ''' </summary>
    ''' <returns></returns>
    Public Property AminoAcidChange As String
    Public Property CDS As String
    <Column(Name:="CDS Codon Number")> Public Property CodonNumber As String
    Public Property CDSPosition As String
    Public Property PositionWithinCodon As String

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property CodonChange As String

End Class

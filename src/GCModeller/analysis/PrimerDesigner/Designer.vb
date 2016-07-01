Imports System.Runtime.CompilerServices

''' <summary>
''' Wu, J. S., et al. (2004). "Primer design using genetic algorithm." Bioinformatics 20(11): 1710-1717.
''' 
'''	MOTIVATION: Before performing a polymerase chain reaction experiment, a pair of primers to clip 
''' the target DNA subsequence is required. However, this is a tedious task as too many constraints 
''' need to be satisfied. Various kinds of approaches for designing a primer have been proposed in 
''' the last few decades, but most of them do not have restriction sites on the designed primers and 
''' do not satisfy the specificity constraint. RESULTS: The proposed algorithm imitates nature's 
''' process of evolution and genetic operations on chromosomes in order to achieve optimal solutions, 
''' and is a best fit for DNA behavior. Experimental results indicate that the proposed algorithm can 
''' find a pair of primers that not only obeys the design properties but also has a specific 
''' restriction site and specificity. Gel electrophoresis verifies that the proposed method really 
''' can clip out the target sequence. AVAILABILITY: A public version of the software is available on 
''' request from the authors.
''' </summary>
''' <remarks></remarks>
Public Module Designer

    Public Function Search(minL As Integer, maxL As Integer, Optional sense_rs As String = "", Optional antisense_rs As String = "") As Primer()
        Throw New NotImplementedException
    End Function

    Public Function Ratings(Primer As Primer, Profiles As SearchProfile) As Double
        Throw New NotImplementedException
    End Function
End Module

Public Class SearchProfile
    Public Property MinLength As Integer
    Public Property MaxLength As Integer
    Public Property SenseRestricted As String
    Public Property AntisenseRestricted As String
    Public Property MinGC As Double
    Public Property MaxGC As Double
    Public Property MinTm As Double
    Public Property MaxTm As Double
    Public Property DeltaTm As Double
    Public Property DeltaGC As Double
End Class

Public Class Primer

    Public ReadOnly Property GCDifference As Double
        Get
            Return Math.Abs(ForwardSequence.GC - ReversedSequence.GC)
        End Get
    End Property

    Public ReadOnly Property TmDifference As Double
        Get
            Return Math.Abs(ForwardSequence.Tm - ReversedSequence.Tm)
        End Get
    End Property

    Public ReadOnly Property Product As SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
        Get
            Return New SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation With {.Left = Reversed.Right, .Right = Forward.Left}
        End Get
    End Property

    Public Property Forward As SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
    Public Property Reversed As SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation

    Public Property SenseRestrictedSite As String
    Public Property AntisenseRestrictedSite As String

    ''' <summary>
    ''' Fe - Fs
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Alpha As Integer
        Get
            Return Forward.Ends - Forward.Start
        End Get
    End Property

    ''' <summary>
    ''' Rs - Fe
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Beta As Integer
        Get
            Return Reversed.Start - Reversed.Ends
        End Get
    End Property

    ''' <summary>
    ''' Re - Rs
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Gamma As Integer
        Get
            Return Reversed.Ends - Reversed.Start
        End Get
    End Property

    Dim _Template As SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader
    Dim _ReversedTemplate As SMRUCC.genomics.SequenceModel.NucleotideModels.SegmentReader

    Sub New(Template As SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid)
        _Template = New SequenceModel.NucleotideModels.SegmentReader(Template)
        _ReversedTemplate = New SequenceModel.NucleotideModels.SegmentReader(Template.Complement)
    End Sub

    Public ReadOnly Property ForwardSequence As SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid
        Get
            Return New SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid(SenseRestrictedSite & _Template.TryParse(Forward.Start, Forward.Ends))
        End Get
    End Property

    Public ReadOnly Property ReversedSequence As SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid
        Get
            Return New SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid(AntisenseRestrictedSite & _ReversedTemplate.TryParse(Reversed.Start, Reversed.Ends))
        End Get
    End Property
End Class
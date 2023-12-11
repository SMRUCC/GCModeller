Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

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

    Public ReadOnly Property Product As NucleotideLocation
        Get
            Return New NucleotideLocation With {
                .Left = Reversed.right,
                .Right = Forward.left
            }
        End Get
    End Property

    Public Property Forward As NucleotideLocation
    Public Property Reversed As NucleotideLocation

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

    Dim _Template As IPolymerSequenceModel
    Dim _ReversedTemplate As IPolymerSequenceModel

    Sub New(Template As NucleicAcid)
        _Template = Template
        _ReversedTemplate = Template.Complement
    End Sub

    Public ReadOnly Property ForwardSequence As NucleicAcid
        Get
            Return New NucleicAcid(SenseRestrictedSite & _Template.CutSequenceLinear(Forward.Start, Forward.Ends))
        End Get
    End Property

    Public ReadOnly Property ReversedSequence As NucleicAcid
        Get
            Return New NucleicAcid(AntisenseRestrictedSite & _ReversedTemplate.CutSequenceLinear(Reversed.Start, Reversed.Ends))
        End Get
    End Property
End Class

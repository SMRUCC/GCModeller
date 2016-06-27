Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Motif
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.Patterns
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace SequenceLogo

    ''' <summary>
    ''' Abstract model for a residue site in a motif sequence fragment.
    ''' </summary>
    Public Interface ILogoResidue : Inherits IPatternSite
        ''' <summary>
        ''' The information of this site can give us.
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Bits As Double
    End Interface

    ''' <summary>
    ''' Drawing model for the sequence logo visualization.
    ''' </summary>
    Public Class DrawingModel : Inherits ClassObject

        ''' <summary>
        ''' The motif model is consist of a sequence of residue sites.
        ''' </summary>
        ''' <returns></returns>
        Public Property Residues As Residue()
        Public Property En As Double
        ''' <summary>
        ''' This drawing display title.
        ''' </summary>
        ''' <returns></returns>
        Public Property ModelsId As String

        Public Overrides Function ToString() As String
            Return ModelsId & " --> " & Me.GetJson
        End Function

        Public ReadOnly Property Alphabets As Integer
            Get
                Return Residues(Scan0).Alphabets.Length
            End Get
        End Property

        ''' <summary>
        ''' Creates the residue model in amino acid profiles
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Shared Function AAResidue(x As ILogoResidue) As Residue
            Dim Residue As Residue = New Residue With {
                .Alphabets = ColorSchema.AA.ToArray(
                    Function(r) New Alphabet With {
                        .Alphabet = r,
                        .RelativeFrequency = x(r)}),
                .Bits = x.Bits
            }

            Return Residue
        End Function

        ''' <summary>
        ''' Creates the residue model for the nucleotide sequence motif model.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Shared Function NTResidue(x As ILogoResidue) As Residue
            Dim Residue As Residue = New Residue With {
                .Alphabets = {
                    New Alphabet With {.Alphabet = "A"c, .RelativeFrequency = x("A"c)},
                    New Alphabet With {.Alphabet = "T"c, .RelativeFrequency = x("T"c)},
                    New Alphabet With {.Alphabet = "G"c, .RelativeFrequency = x("G"c)},
                    New Alphabet With {.Alphabet = "C"c, .RelativeFrequency = x("C"c)}
                },
                .Bits = x.Bits
            }

            Return Residue
        End Function

        ''' <summary>
        ''' ## Get information content profile from PWM
        ''' </summary>
        ''' <param name="pwm"></param>
        ''' <returns></returns>
        Public Shared Function pwm2ic(pwm As DrawingModel) As Double()
            Dim npos As Integer = pwm.Residues.First.Alphabets.Length
            Dim ic As Double() = New Double(npos - 1) {}
            For i As Integer = 0 To npos - 1
                Dim idx As Integer = i
                ic(i) = 2 + Sum(pwm.Residues.ToArray(Of Double)(
                                Function(x) If(x.Alphabets(idx).RelativeFrequency > 0,
                                x.Alphabets(idx).RelativeFrequency * Math.Log(x.Alphabets(idx).RelativeFrequency, 2),
                                0)))
            Next

            Return ic
        End Function
    End Class
End Namespace
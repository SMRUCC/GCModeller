Imports System.Text.RegularExpressions

Namespace Assembly.NCBI.GenBank.TabularFormat.FastaObjects

    Public Class Fasta : Inherits LANS.SystemsBiology.SequenceModel.FASTA.FastaToken

        Dim _UniqueId As String

        Public ReadOnly Property UniqueId As String
            Get
                Return _UniqueId
            End Get
        End Property

        Protected Sub New()
        End Sub

        Public Shared Function CreateObject(UniqueId As String, Fasta As SequenceModel.FASTA.FastaToken) As Fasta
            Return New Fasta With {
                ._UniqueId = UniqueId,
                .Attributes = Fasta.Attributes,
                .SequenceData = Fasta.SequenceData
            }
        End Function
    End Class

    ''' <summary>
    ''' *.fna 基因组序列
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GenomeSequence : Inherits SequenceModel.FASTA.FastaToken

        Public ReadOnly Property GI As String
        Public ReadOnly Property LocusID As String
        Public ReadOnly Property Description As String

        Sub New(Fasta As SequenceModel.FASTA.FastaToken)
            _GI = Fasta.Attributes(1)
            _LocusID = Regex.Replace(Fasta.Attributes(3), "\.\d+", "")
            _Description = Fasta.Attributes(4)
            Attributes = Fasta.Attributes
            Me.SequenceData = Fasta.SequenceData
        End Sub

        Public Function SaveBriefData(Path As String, Optional encoding As System.Text.Encoding = Nothing) As Boolean
            Dim Fasta As New SequenceModel.FASTA.FastaToken With {
                .SequenceData = SequenceData,
                .Attributes = New String() {LocusID}
            }
            Return Fasta.SaveTo(Path, encoding)
        End Function
    End Class

    ''' <summary>
    ''' *.ffn 基因序列
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneObject : Inherits SequenceModel.FASTA.FastaToken

        Public ReadOnly Property GI As String
        Public ReadOnly Property Locus As String
        Public ReadOnly Property Location As ComponentModel.Loci.Location
        Public ReadOnly Property Description As String

        Sub New(Fasta As SequenceModel.FASTA.FastaToken)
            _Gi = Fasta.Attributes(1)
            _Locus = Fasta.Attributes(3)
            _Description = Fasta.Attributes.Last
            Attributes = Fasta.Attributes
            SequenceData = Fasta.SequenceData
            Dim sLoci As String = Mid(Regex.Match(Description, ":\d+-\d+").Value, 2)
            Dim Tokens As String() = Strings.Split(sLoci, ":")
            _Location = New ComponentModel.Loci.Location(Val(Tokens.First), Val(Tokens.Last))
        End Sub
    End Class

    ''' <summary>
    ''' *.faa 蛋白质序列
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Protein : Inherits SequenceModel.FASTA.FastaToken

        Public ReadOnly Property GI As String
        Public ReadOnly Property Locus As String
        Public ReadOnly Property Description As String

        Sub New(Fasta As SequenceModel.FASTA.FastaToken)
            _Gi = Fasta.Attributes(1)
            _Locus = Fasta.Attributes(3)
            _Description = Fasta.Attributes.Last
            Attributes = Fasta.Attributes
            SequenceData = Fasta.SequenceData
        End Sub
    End Class
End Namespace
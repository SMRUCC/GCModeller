Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Bac_sRNA.org

    Public Class Sequence : Inherits Contig
        Implements I_PolymerSequenceModel

        Public ReadOnly Property UniqueId As String
        Public ReadOnly Property Specie As String
        Public ReadOnly Property Name As String

        Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData
            Get
                Return __raw.SequenceData
            End Get
            Set(value As String)
                __raw.SequenceData = value
            End Set
        End Property

        Dim __raw As FastaToken

        Sub New()
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sid"></param>
        ''' <param name="org"></param>
        ''' <param name="name"></param>
        ''' <param name="seq">The fasta sequence data</param>
        ''' <param name="loci"></param>
        Sub New(sid As String, org As String, name As String, seq As String, loci As NucleotideLocation)
            _UniqueId = sid
            _Specie = org
            _Name = name
            _MappingLocation = loci
            __raw = New FastaToken({UniqueId,
                                   Specie,
                                   name,
                                   loci.Left,
                                   loci.Right,
                                   loci.Strand.ToString}, seq)
        End Sub

        Sub New(atts As String())
            Call Me.New(atts(Scan0),
                        atts(1),
                        atts(2), "",
                        New NucleotideLocation(Scripting.CTypeDynamic(Of Integer)(atts(3)),
                                               Scripting.CTypeDynamic(Of Integer)(atts(4)),
                                               atts(5).GetStrand))
        End Sub

        Public Shared Function [CType](fa As FastaToken) As Sequence
            Return New Sequence With {
                ._UniqueId = fa.Attributes(Scan0),
                ._Specie = fa.Attributes(1),
                ._Name = fa.Attributes(2),
                .__raw = fa
            }
        End Function

        Public Overrides Function ToString() As String
            Return __raw.GetJson
        End Function

        Public Function ToFasta() As FastaToken
            Return New FastaToken(__raw.Attributes, __raw.SequenceData)
        End Function

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation With {
                .Left = __raw.Attributes(3),
                .Right = __raw.Attributes(4),
                .Strand = LociAPI.GetStrand(__raw.Attributes(5))
            }
        End Function
    End Class
End Namespace
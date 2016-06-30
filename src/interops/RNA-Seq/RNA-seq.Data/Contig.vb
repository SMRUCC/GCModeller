Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation
Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.FASTA
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.BOW.DocumentFormat.SAM.DocumentElements
Imports Microsoft.VisualBasic

Public Class Contig : Inherits NucleotideModels.Contig
    Implements IAbstractFastaToken

    Public Property FLAGS As String()
    Public Property Location As NucleotideLocation
        Get
            Return Me._MappingLocation
        End Get
        Set(value As NucleotideLocation)
            Me._MappingLocation = value
        End Set
    End Property

    Public ReadOnly Property Title As String Implements IAbstractFastaToken.Title
        Get
            Return Me.ToString
        End Get
    End Property

    Public Property Attributes As String() Implements IAbstractFastaToken.Attributes
    Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

    Public Function ToFastaToken() As FastaToken
        Return New FastaToken With {
            .SequenceData = SequenceData,
            .Attributes = {Location.ToString, String.Join(" / ", FLAGS)}
        }
    End Function

    ''' <summary>
    ''' 所装配出来的位置和方向有关
    ''' </summary>
    ''' <param name="Reads"></param>
    ''' <param name="Reversed">实际的方向</param>
    ''' <returns></returns>
    Public Shared Function AssemblingForward(Reads As List(Of AlignmentReads), Reversed As Boolean) As Contig
        '由于顺序是已经从小到大排好序了的，所以在这里直接进行装配
        Dim AssembledRead = New Contig With {
            .Location = New NucleotideLocation(Reads(0).POS, Reads.Last.PNEXT - 1, If(Not Reversed, Strands.Forward, Strands.Reverse)),
            .SequenceData = String.Join("", (From Read As AlignmentReads
                                             In Reads
                                             Select Read.SequenceData).ToArray),
            .FLAGS = (From read In Reads Select read.GetBitFLAGSDescriptions).ToArray
        }
        Return AssembledRead
    End Function

    ''' <summary>
    ''' 不明白在bitwiseFLAG里面已经标注了Reverse方向了，为什么还是有些Reads会是正向的
    ''' bwa标注出来的位置和ncbi上面的blast的位置好像不一致？？？
    ''' </summary>
    ''' <param name="Reads">位置从大到小的</param>
    ''' <returns></returns>
    Public Shared Function AssemblingReversed(Reads As List(Of AlignmentReads), Reversed As Boolean) As Contig
        '由于顺序是已经从大到小排好序了的，所以在这里直接进行反向装配
        Dim AssembledRead = New Contig With {
            .Location = New NucleotideLocation(Reads(0).POS, Reads.Last.PNEXT + 1, If(Not Reversed, Strands.Forward, Strands.Reverse)),
            .FLAGS = (From read In Reads Select read.GetBitFLAGSDescriptions).ToArray
        }
        Call Reads.Reverse()
        AssembledRead.SequenceData = String.Join("", (From Read As AlignmentReads
                                                      In Reads
                                                      Select Read.SequenceData).ToArray)
        Return AssembledRead
    End Function

    Protected Overrides Function __getMappingLoci() As NucleotideLocation
        Return _MappingLocation
    End Function
End Class

Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Patterns.Clustal
Imports Microsoft.VisualBasic.Linq.Extensions

Public Class Signature : Implements IAbstractFastaToken

    Public Property PfamString As Sanger.Pfam.PfamString.PfamString
    ''' <summary>
    ''' 多序列比对的出现概率
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property Level As Double

    Public ReadOnly Property Title As String Implements IAbstractFastaToken.Title
        Get
            Return PfamString.ProteinId
        End Get
    End Property

    Public Property Attributes As String() Implements IAbstractFastaToken.Attributes
        Get
            Return {PfamString.ProteinId}
        End Get
        Set(value As String())
            PfamString.ProteinId = value.JoinBy(" ")
        End Set
    End Property

    <XmlText>
    Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

    Public Overrides Function ToString() As String
        Return PfamString.ToString
    End Function

    Public Function ToFasta() As SequenceModel.FASTA.FastaToken
        Return New FastaToken(Me)
    End Function

    Public Shared Function CreateObject(SRChain As SR(), Name As String) As Signature
        Return New Signature With {
            .PfamString = GetPfamString(SRChain, Name),
            .SequenceData = New String(SRChain.ToArray(Function(x) x.Residue))
        }
    End Function

    Public Shared Function CreateObject(chain As SRChain) As Signature
        Return CreateObject(chain.lstSR, chain.Name)
    End Function
End Class
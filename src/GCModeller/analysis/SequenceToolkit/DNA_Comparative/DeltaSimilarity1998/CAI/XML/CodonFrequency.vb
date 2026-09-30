Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace DeltaSimilarity1998.CAI.XML

    ''' <summary>
    ''' One codon w-weight entry of the CAI table: 
    ''' (codon triple, weight value).
    ''' </summary>
    Public Structure CodonBias

        <XmlAttribute> Public Property Codon As DNA()
        <XmlAttribute> Public Property Bias As Double

        ''' <summary>
        ''' </summary>
        ''' <param name="codon$">三联体密码子字符串</param>
        ''' <param name="bias#">w(codon) weight value</param>
        Sub New(codon$, bias#)
            Me.Bias = bias
            Me.Codon = codon _
                .Select(AddressOf Conversion.CharEnums) _
                .ToArray
        End Sub

        Public ReadOnly Property CodonString As String
            Get
                Return NucleotideModels.NucleicAcid.ToString(Codon)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace

Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Topologically.SimilarityMatches

    Public Class ReversedLociMatchedResult : Inherits LociMatchedResult

        Public Property ReversedMatched As String

        Public Shared Function GenerateFromBase(Loci As LociMatchedResult) As ReversedLociMatchedResult
            Dim RevLoci As ReversedLociMatchedResult = New ReversedLociMatchedResult
            RevLoci.Location = Loci.Location
            RevLoci.Similarity = Loci.Similarity
            RevLoci.Matched = Loci.Loci
            RevLoci.Loci = NucleicAcid.Complement(New String(Loci.Loci.ToArray.Reverse.ToArray))
            RevLoci.ReversedMatched = Loci.Matched

            Return RevLoci
        End Function
    End Class

    Public Class LociMatchedResult

        ''' <summary>
        ''' 原来的序列
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Loci As String
        ''' <summary>
        ''' 模糊匹配上的序列
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Matched As String
        ''' <summary>
        ''' 相似度
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Similarity As Double
        Public Property Location As Integer()
    End Class
End Namespace
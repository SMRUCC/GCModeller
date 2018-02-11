Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace ContextModel.Promoter

    Public Module Extensions

        ''' <summary>
        ''' Read from <see cref="PrefixLength"/> members.
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetPrefixLengths() As IEnumerable(Of Integer)
            Return From L In GetType(PrefixLength).GetEnumValues Select CInt(L)
        End Function

        ''' <summary>
        ''' 解析出所有基因前面的序列片段
        ''' </summary>
        ''' <param name="context"></param>
        ''' <param name="nt"></param>
        ''' <param name="length%"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ParseUpstreamByLength(context As PTT, nt As IPolymerSequenceModel, length%) As Dictionary(Of String, FastaSeq)
            Dim genes = context.GeneObjects
            Dim parser = From gene As GeneBrief
                         In genes.AsParallel
                         Let upstream = gene.GetUpstreamSeq(nt, length)
                         Select gene.Synonym,
                             promoter = upstream
            Dim table = parser.ToDictionary(Function(g) g.Synonym, Function(g) g.promoter)
            Return table
        End Function

        ''' <summary>
        ''' Get upstream nt sequence in a specific length for target gene.
        ''' (在这个函数之中，位点的计算的时候会有一个碱基的偏移量是因为为了不将起始密码子ATG之中的A包含在结果序列之中)
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="nt"></param>
        ''' <param name="len%"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function GetUpstreamSeq(gene As GeneBrief, nt As IPolymerSequenceModel, len%) As FastaSeq
            Dim loci As NucleotideLocation = gene.Location

            With loci.Normalization()
                If .Strand = Strands.Forward Then
                    ' 正向序列是上游，无需额外处理
                    loci = New NucleotideLocation(.Left - len, .Left - 1)
                Else
                    ' 反向序列是下游，需要额外小心
                    loci = New NucleotideLocation(.Right + 1, .Right + len, ComplementStrand:=True)
                End If
            End With

            Dim site As SimpleSegment = nt.CutSequenceCircular(loci)
            Dim attrs$() = gene.headers(site)
            Dim promoter As New FastaSeq With {
                .Headers = attrs,
                .SequenceData = site.SequenceData
            }

            Return promoter
        End Function

        <Extension>
        Private Function headers(gene As GeneBrief, site As SimpleSegment) As String()
            If gene.Product.StringEmpty Then
                Return {gene.Synonym & " " & site.ID}
            Else
                Return {gene.Synonym & " " & site.ID, gene.Product}
            End If
        End Function
    End Module
End Namespace
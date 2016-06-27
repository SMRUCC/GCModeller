Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.SequenceModel.FASTA

Namespace SequenceModel.Patterns.Clustal

    ''' <summary>
    ''' Semi-Residue(半残基，出现概率不是百分之百的)
    ''' </summary>
    Public Class SR

        ''' <summary>
        ''' 在多重比对之中的序列上面的位置
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Index As Integer
        ''' <summary>
        ''' 残基符号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Residue As Char
        ''' <summary>
        ''' 出现的频率
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Frq As Double
        ''' <summary>
        ''' 区块的序列号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Block As String

        Public Overrides Function ToString() As String
            Return Residue
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="aln"></param>
        ''' <param name="block">生成类似于domain的参数所需求的，假若这个值太低，则计算MPAlignment的时候会得分很低，除非特别应用请不要修改这个数值</param>
        ''' <param name="levels"></param>
        ''' <returns></returns>
        Public Shared Function FromAlign(aln As IEnumerable(Of FastaToken), Optional block As Double = 1.0R, Optional levels As Integer = 10) As SRChain()
            If aln.IsNullOrEmpty Then
                Return New SRChain() {}
            Else
                Try
                    Return __createSafly(aln, block, levels)
                Catch ex As Exception
                    Call App.LogException(ex)
                    Return New SRChain() {}
                End Try
            End If
        End Function

        Private Shared Function __createSafly(aln As IEnumerable(Of FastaToken), block As Double, levels As Integer) As SRChain()
            Dim chMAT = aln.ToArray(Function(x) x.SequenceData.ToArray).ToArray
            Dim width As Integer = chMAT.Length
            Dim chain As SR()() = (From idx As Integer
                                   In chMAT(Scan0).Sequence
                                   Select __getSite(chMAT, idx, width, levels)).ToArray
            Dim lstChain As SRChain() = __allocBlock(chain, block).ToArray(
                Function(x, idx) New SRChain With {
                    .lstSR = x,
                    .Name = idx})
            Return lstChain
        End Function

        ''' <summary>
        ''' 做了一次矩阵转置
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="cutoff"></param>
        ''' <returns></returns>
        Private Shared Function __allocBlock(source As SR()(), cutoff As Double) As SR()()
            Dim chains As SR()() = source.First.Length.ToArray(Function(row) source.ToArray(Function(x) x(row)))
            Dim LQuery As SR()() = chains.ToArray(Function(x) __allocBlock(x, cutoff))
            Return LQuery
        End Function

        Private Shared Function __allocBlock(source As SR(), cutoff As Double) As SR()
            Dim block As Integer = 0

            For i As Integer = 0 To source.Length - 1
                If source(i).Residue <> "-"c AndAlso source(i).Frq >= cutoff Then
                    source(i).Block = block
                    block += 1
                Else
                    source(i).Block = "*"
                End If
            Next

            Return source
        End Function

        ''' <summary>
        ''' 返回前10的位点
        ''' 位点不足的话会使用最后一个位点进行补充
        ''' 
        ''' 这里是构建特征序列的核心算法部分
        ''' </summary>
        ''' <param name="chMAT">频率矩阵</param>
        ''' <param name="index"></param>
        ''' <param name="width"></param>
        ''' <returns></returns>
        Private Shared Function __getSite(chMAT As Char()(), index As Integer, width As Integer, levels As Integer) As SR()
            Dim nList As Char() = chMAT.ToArray(Function(x) x(index))
            Dim ng = (From ch As Char In nList Select ch Group ch By ch Into Count).ToArray
            Dim orders = (From ch In ng Where ch.ch <> "-"c Select ch Order By ch.Count Descending).ToArray

            If orders.Length = 0 Then
                Return levels.ToArray(
                    Function(x) New SR With {
                        .Residue = "-"c,
                        .Frq = 1,
                        .Index = index})
            Else
                Dim lst = orders.Take(10).ToArray(Function(x) New KeyValuePair(Of Char, Integer)(x.ch, x.Count))
                Dim block As New List(Of KeyValuePair(Of Char, Integer))
                Dim p As Integer = 0
                Dim pp As Double = 1   ' 上限
                Dim d As Double = 1 / levels

                For i As Integer = 0 To levels - 1
                    pp -= d ' 下限

                    If p = lst.Count Then
                        p -= 1
                    End If

                    If lst(p).Value / width >= pp Then      ' 至少要达到下限
                        Call block.Add(lst(p))
                        Call p.MoveNext
                    Else ' 使用上一个残基来替代
                        If p = 0 Then
                            Call block.Add(New KeyValuePair(Of Char, Integer)("-"c, CInt(pp * levels)))
                        Else
                            Call block.Add(lst(p - 1))
                        End If
                    End If
                Next

                Return block.ToArray(Function(x) New SR With {
                    .Residue = x.Key,
                    .Frq = x.Value / width,
                    .Index = index
                })
            End If
        End Function
    End Class
End Namespace
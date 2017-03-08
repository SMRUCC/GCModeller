Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace VCF

    Public Module VCFStream

        ''' <summary>
        ''' 使用迭代器加载一个很大的vcf数据文件
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <param name="meta"></param>
        ''' <returns></returns>
        Public Function LoadStream(path$, Optional ByRef meta As MetaData = Nothing) As IEnumerable(Of SNPVcf)
            Dim n% = 0
            meta = MetaData.ParseMeta(file:=path, n:=n)
            Return path.__parserInternal(count:=n)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <param name="count%">所需要跳过的元数据的行数目</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Private Iterator Function __parserInternal(path$, count%) As IEnumerable(Of SNPVcf)

            For Each line$ In path.IterateAllLines.Skip(count)
                Yield VCFStream.LineParser(line)
            Next
        End Function

        ''' <summary>
        ''' 将vcf文件之中的某一行字符串解析为SNP位点数据
        ''' </summary>
        ''' <param name="line$"></param>
        ''' <returns></returns>
        Public Function LineParser(line$, seqTitles$()) As SNPVcf
            Dim t$() = line.Split(ASCII.TAB)
            Dim i As int = Scan0

            Return New SNPVcf With {
                .CHROM = t(++i),
                .POS = t(++i),
                .ID = t(++i),
                .REF = t(++i),
                .ALT = t(++i),
                .QUAL = t(++i),
                .FILTER = t(++i),
                .INFO = t(++i),
                .FORMAT = t(++i),
                .Sequences = seqTitles.__seq(t.Skip(i).ToArray)
            }
        End Function

        <Extension>
        Private Function __seq(seqs$(), t$()) As Dictionary(Of String, String)

        End Function
    End Module
End Namespace
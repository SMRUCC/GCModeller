
Imports System.IO
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Module PTTFileReader

        ''' <summary>
        ''' 出错不会被处理，而<see cref="PTT.Load(String, Boolean)"/>函数则会处理错误，返回Nothing
        ''' </summary>
        ''' <returns></returns>
        Public Function Read(path As String, Optional FillBlankName As Boolean = False) As PTT
            Dim lines As String() = File.ReadAllLines(path)
            Dim PTT As New PTT With {
                .Title = lines(0)
            }

            lines = (From s As String In lines.Skip(3) Where Not String.IsNullOrWhiteSpace(s) Select s).ToArray
            Dim Genes As GeneBrief() = New GeneBrief(lines.Length - 1) {}
            For i As Integer = 0 To lines.Length - 1
                Dim strLine As String = lines(i)
                Genes(i) = GeneBrief.DocumentParser(strLine, FillBlankName)
            Next
            PTT.GeneObjects = Genes
            Dim strTemp As String = r.Match(PTT.Title, " - \d+\.\.\d+").Value
            PTT.Size = Val(Strings.Split(strTemp, "..").Last)
            PTT.Title = PTT.Title.Replace(strTemp, "")

            Return PTT
        End Function
    End Module
End Namespace
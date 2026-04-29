
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Namespace MSA.Tabular

    Public Module Reader

        <Extension>
        Public Iterator Function Read(s As Stream) As IEnumerable(Of Stockholm)

        End Function

        Public Function Parser(source As String, schema As PropertyInfo()) As Stockholm
            Dim Tokens As String() = source.LineTokens
            Dim fields As Dictionary(Of String, String) =
            __fieldsParser((From line As String In Tokens
                            Where Not String.IsNullOrWhiteSpace(line) AndAlso
                                line.First = "#"c
                            Select line).ToArray)
            Dim aln = (From line As String In Tokens
                       Where Not String.IsNullOrWhiteSpace(line) AndAlso
                       line.First <> "#"c
                       Let sp As String() = line.Trim.Split
                       Let id As String = sp.First
                       Let seq As String = sp.Last
                       Let fa As SequenceModel.FASTA.FastaSeq =
                       New SequenceModel.FASTA.FastaSeq With {
                          .Headers = {id},
                          .SequenceData = seq
                       }
                       Select fa).ToArray
            Dim family As New Stockholm With {
            .Alignments = New SequenceModel.FASTA.FastaFile(aln)
        }

            For Each prop As PropertyInfo In schema
                Call prop.SetValue(family, fields.TryGetValue(prop.Name))
            Next

            Return family
        End Function

        Public Function ParseSchema() As PropertyInfo()
            Dim typeInfo As Type = GetType(Stockholm)
            Dim props As PropertyInfo() = typeInfo.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            Dim stringType As Type = GetType(String)
            props = (From prop As PropertyInfo In props
                     Where prop.CanWrite AndAlso
                     prop.PropertyType.Equals(stringType)
                     Select prop).ToArray
            Return props
        End Function

        Private Function __fieldsParser(lines As String()) As Dictionary(Of String, String)
            Dim LQuery = (From x As String In lines
                          Let tokens As String() = Strings.Split(x, "   ")
                          Let name As String = tokens.First.Split.Last
                          Let value As String = tokens.Last
                          Select name, value
                          Group By name Into Group) _
                         .ToDictionary(Function(x) x.name,
                                       Function(x) x.Group.Select(Function(xx) xx.value).JoinBy(" "))
            Return LQuery
        End Function

        Public Function DatabaseParser(path As String) As Dictionary(Of String, Stockholm)
            Dim inText As String = FileIO.FileSystem.ReadAllText(path)
            Dim datas As String() = Regex.Split(inText, "^[/][/]$", RegexOptions.Multiline)
            Dim schema As PropertyInfo() = Stockholm.ParseSchema
            Dim LQuery = (From x As String In datas.AsParallel
                          Let Rfam As Stockholm = Stockholm.Parser(x, schema)
                          Where Not String.IsNullOrEmpty(Rfam.AC)
                          Select Rfam).ToDictionary(Function(x) x.AC)
            Return LQuery
        End Function
    End Module
End Namespace
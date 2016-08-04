Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports RDotNET.Extensions.Bioinformatics
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API.base
Imports SMRUCC.genomics.Analysis.Metagenome

Partial Module CLI

    <ExportAPI("/calcFst", Usage:="/calcFst /in <in.csv> [/out <out.Csv>]")>
    Public Function calcFst(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".calcFst.Csv")
        Dim pop = [in].LoadCsv(Of GenotypeDetails)
        Dim df As DocumentStream.File = pop.ExpandLocis
        Dim name As String = "myFreq"
        Dim types As New Dictionary(Of String, Type) From {
            {polysat.Genomes, GetType(Integer)}
        }

        For Each ns As String In df.First.Skip(2)
            Call types.Add(ns, GetType(Double))  ' data.frame里面的这一列是频数，Double
        Next

        Call df.Columns.Skip(1).JoinColumns _
               .PushAsDataFrame(name,
                                types:=types,
                                rowNames:=df.Columns.First.Skip(1))   ' 向R之中推送数据

        require("polysat")

        Dim result = polysat.calcFst(name, NULL, NULL)

    End Function
End Module


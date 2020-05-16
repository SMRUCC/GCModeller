Imports System.IO
Imports System.Runtime.CompilerServices

Public Module ReportTextBuilder

    <Extension>
    Public Sub BuildSearchReport(result As IEnumerable(Of GeneReport), report As TextWriter)
        For Each gene As GeneReport In result
            Call gene.BuildSearchReport(report)
        Next
    End Sub

    <Extension>
    Public Sub BuildSearchReport(gene As GeneReport, report As TextWriter)
        Call report.WriteLine(">" & gene.locus_tag)
        Call report.WriteLine(" Length of sequence -            " & gene.length)
        Call report.WriteLine(" Threshold for promoters -       " & gene.threshold)
        Call report.WriteLine(" Number of predicted promoters - " & gene.tfBindingSites.Length)
        Call report.WriteLine($" Promoter Pos:     {gene.promoterPos} LDF-  {gene.promoterPosLDF}")

        For Each com In gene.components
            Call report.WriteLine(com.ToString)
        Next

        Call report.WriteLine()
        Call report.WriteLine(" Oligonucleotides from known TF binding sites:")
        Call report.WriteLine()

        Call report.WriteLine($" For promoter at     {gene.promoterPos}:")

        For Each site In gene.tfBindingSites
            Call report.WriteLine($"        {site.regulator}:  {site.oligonucleotides} at position      {site.position} Score -  {site.score}")
        Next
    End Sub

End Module

Imports SMRUCC.genomics.AnalysisTools.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.DatabaseServices.Regprecise
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic

Partial Module CLI

    <ExportAPI("/Gets.Sites.Genes",
               Usage:="/Gets.Sites.Genes /in <tf.bbh.csv> /sites <motiflogs.csv> [/out <out.csv>]")>
    Public Function GetSites(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim sites As String = args("/sites")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & $"-{sites.BaseName}.csv")
        Dim bbh As IEnumerable(Of BBHIndex) = [in].LoadCsv(Of BBHIndex)
        Dim motifLogs As IEnumerable(Of MotifLog) = sites.LoadCsv(Of MotifLog)
        Dim Xmls As IEnumerable(Of String) = FileIO.FileSystem.GetFiles(GCModeller.FileSystem.GetRepositoryRoot & "/RegpreciseDownloads/", FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
        Dim MotifLDMs = (From log As BacteriaGenome
                         In Xmls.Select(AddressOf LoadXml(Of BacteriaGenome))
                         Where Not log.Regulons Is Nothing AndAlso
                             Not log.Regulons.Regulators.IsNullOrEmpty
                         Select log)
        Dim result As New List(Of MotifLog)
        Dim bbhhash = BBHIndex.BuildHitsHash(bbh, True)
        Dim motifsHash = (From x As MotifLog
                          In motifLogs
                          Select x
                          Group x By x.Regulog Into Group) _
                               .ToDictionary(Function(x) x.Regulog,
                                             Function(x) x.Group.ToArray)
        Dim logsHash As New List(Of Regulator)

        For Each regulog As BacteriaGenome In MotifLDMs
            For Each TF As Regulator In regulog.Regulons.Regulators
                If TF.Type <> Regulator.Types.TF Then
                    Continue For
                End If
                If Not bbhhash.ContainsKey(TF.Regulator.Key) Then
                    Continue For
                End If
                If Not motifsHash.ContainsKey(TF.Regulog.Key) Then
                    Continue For
                End If

#If DEBUG Then
                logsHash += TF
#End If

                Dim maps As String() = bbhhash(TF.Regulator.Key)
                Dim sitesFound As MotifLog() = motifsHash(TF.Regulog.Key)

                For Each site In sitesFound
                    site.tag = String.Join("; ", maps)
                Next

                result += sitesFound

                Call Console.Write(".")
            Next
        Next

#If DEBUG Then
        Dim test = (From x As Regulator
                    In logsHash
                    Select x
                    Group x By x.Regulog.Key Into Group) _
                         .ToDictionary(Function(x) x.Key,
                                       Function(x) x.Group.ToArray)
        result = result.OrderBy(Function(x) x.ID).ToList
#End If
        Return result.SaveTo(out)
    End Function
End Module
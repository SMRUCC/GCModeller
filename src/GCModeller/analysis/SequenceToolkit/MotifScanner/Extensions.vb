Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Extensions

    <Extension>
    Public Function CreateModel(pwm As Probability) As MotifPWM
        Dim alphabets As Char() = New Char() {"A"c, "C"c, "G"c, "T"c}
        Dim n As Integer = 100  ' normalized as 100 sequence input
        Dim E As Double = Probability.E(nsize:=100)

        Return New MotifPWM With {
            .name = pwm.name,
            .note = pwm.ToString,
            .pwm = pwm.region _
                .Select(Function(r)
                            Dim hi As Double = Probability.HI(r)

                            Return New ResidueSite(r, alphabets) With {
                                .bits = Probability.CalculatesBits(hi, E, NtMol:=alphabets.Length = 4)
                            }
                        End Function) _
                .ToArray,
            .alphabets = alphabets
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="db"></param>
    ''' <param name="regions"></param>
    ''' <param name="n_threads"></param>
    ''' <param name="identities_cutoff"></param>
    ''' <param name="minW"></param>
    ''' <param name="top"></param>
    ''' <param name="permutation"></param>
    ''' <param name="workflowMode">
    ''' parallel will works in different mode
    ''' </param>
    ''' <param name="progress"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ScanSites(db As PWMDatabase, regions As IEnumerable(Of FastaSeq),
                              Optional n_threads As Integer = 8,
                              Optional identities_cutoff As Double = 0.8,
                              Optional minW As Double = 0.85,
                              Optional top As Integer = 3,
                              Optional permutation As Integer = 2500,
                              Optional workflowMode As Boolean = False,
                              Optional progress As Action(Of String) = Nothing) As Dictionary(Of String, MotifMatch())

        Dim parallelOptions As New ParallelOptions With {
            .MaxDegreeOfParallelism = n_threads
        }

        If workflowMode Then
            Return db.ScanSiteWorkflowMode(regions, parallelOptions,
                                           progress:=If(progress, New Action(Of String)(AddressOf VBDebugger.debug)),
                                           identities_cutoff:=identities_cutoff,
                                           minW:=minW,
                                           top:=top,
                                           permutation:=permutation)
        Else
            Return db.ScanSiteUIMode(regions, parallelOptions, progress,
                                     identities_cutoff:=identities_cutoff,
                                     minW:=minW,
                                     top:=top,
                                     permutation:=permutation)
        End If
    End Function

    <Extension>
    Private Function ScanSiteWorkflowMode(db As PWMDatabase, regions As IEnumerable(Of FastaSeq),
                                          parallelOptions As ParallelOptions,
                                          progress As Action(Of String),
                                          Optional identities_cutoff As Double = 0.8,
                                          Optional minW As Double = 0.85,
                                          Optional top As Integer = 3,
                                          Optional permutation As Integer = 2500) As Dictionary(Of String, MotifMatch())

        Dim pwm As Dictionary(Of String, Probability()) = db.LoadMotifs()
        Dim tfbsList As New Dictionary(Of String, MotifMatch())
        Dim i As i32 = 1
        Dim allFamily As String() = pwm.Keys.ToArray
        Dim tss As FastaSeq() = regions.SafeQuery.ToArray

        Call System.Threading.Tasks.Parallel.For(
                fromInclusive:=0,
                toExclusive:=tss.Length,
                parallelOptions,
                body:=Sub(j)
                          Dim region As FastaSeq = tss(j)
                          Dim list As New List(Of MotifMatch)

                          For Each family As String In allFamily
                              Call list.AddRange(region.ScanRegion(family, pwm(family),
                                                                   identities_cutoff:=identities_cutoff,
                                                                   minW:=minW,
                                                                   top:=top,
                                                                   permutation:=permutation))
                          Next

                          SyncLock tfbsList
                              Call tfbsList.Add(region.Title, (From site As MotifMatch
                                                               In list
                                                               Where Not site Is Nothing).ToArray)
                              Call progress($"search TFBS for {region.Title} ... {++i}/{tss.Length}")
                          End SyncLock
                      End Sub
        )

        Return tfbsList
    End Function

    <Extension>
    Public Iterator Function ScanRegion(region As FastaSeq, family As String, pwm As Probability(),
                                        identities_cutoff As Double,
                                        minW As Double,
                                        top As Integer,
                                        permutation As Integer) As IEnumerable(Of MotifMatch)

        For Each model As Probability In pwm
            For Each site As MotifMatch In model.ScanSites(region, 0.8,
                                                           minW:=minW,
                                                           identities:=identities_cutoff,
                                                           top:=top,
                                                           permutation:=permutation)
                If Not site Is Nothing Then
                    site.seeds = {family, model.name}
                    Yield site
                End If
            Next
        Next
    End Function

    <Extension>
    Private Function ScanSiteUIMode(db As PWMDatabase, regions As IEnumerable(Of FastaSeq),
                                    parallelOptions As ParallelOptions,
                                    progress As Action(Of String),
                                    Optional identities_cutoff As Double = 0.8,
                                    Optional minW As Double = 0.85,
                                    Optional top As Integer = 3,
                                    Optional permutation As Integer = 2500) As Dictionary(Of String, MotifMatch())

        Dim pwm As Dictionary(Of String, Probability()) = db.LoadMotifs()
        Dim tfbsList As New Dictionary(Of String, MotifMatch())
        Dim i As i32 = 1
        Dim allFamily As String() = pwm.Keys.ToArray
        Dim tss As FastaSeq() = regions.SafeQuery.ToArray
        Dim bar As Tqdm.ProgressBar = Nothing

        If App.EnableTqdm AndAlso progress Is Nothing Then
            progress = Sub(label) bar.SetLabel(label)
        Else
            progress = If(progress, New Action(Of String)(AddressOf VBDebugger.EchoLine))
        End If

        For Each region As FastaSeq In Tqdm.Wrap(tss, bar:=bar, wrap_console:=App.EnableTqdm)
            Dim list As New List(Of MotifMatch)

            Call progress($"search TFBS for {region.Title} ... {++i}/{tss.Length}")
            Call System.Threading.Tasks.Parallel.For(
                fromInclusive:=0,
                toExclusive:=allFamily.Length,
                parallelOptions,
                body:=Sub(j)
                          Dim result As MotifMatch() = region.ScanRegion(allFamily(j), pwm(allFamily(j)),
                                                                         identities_cutoff:=identities_cutoff,
                                                                         minW:=minW,
                                                                         top:=top,
                                                                         permutation:=permutation).ToArray
                          SyncLock list
                              Call list.AddRange(result)
                          End SyncLock
                      End Sub)

            Call tfbsList.Add(region.Title, (From site As MotifMatch
                                             In list
                                             Where Not site Is Nothing).ToArray)
        Next

        Return tfbsList
    End Function

End Module

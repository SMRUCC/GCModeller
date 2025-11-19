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

    <Extension>
    Public Function ScanSites(db As PWMDatabase, regions As IEnumerable(Of FastaSeq),
                              Optional n_threads As Integer = 8,
                              Optional identities_cutoff As Double = 0.8,
                              Optional minW As Double = 0.85,
                              Optional top As Integer = 3,
                              Optional permutation As Integer = 2500,
                              Optional progress As Action(Of String) = Nothing) As Dictionary(Of String, MotifMatch())

        Dim pwm As Dictionary(Of String, Probability()) = db.LoadMotifs()
        Dim tfbsList As New Dictionary(Of String, MotifMatch())
        Dim i As i32 = 1
        Dim parallelOptions As New ParallelOptions With {
            .MaxDegreeOfParallelism = n_threads
        }
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
                          Dim family As String = allFamily(j)

                          For Each model As Probability In pwm(family)
                              For Each site As MotifMatch In model.ScanSites(region, 0.8,
                                                                             minW:=minW,
                                                                             identities:=identities_cutoff,
                                                                             top:=top,
                                                                             permutation:=permutation)
                                  If Not site Is Nothing Then
                                      site.seeds = {family, model.name}
                                      list.Add(site)
                                  End If
                              Next
                          Next
                      End Sub)

            Call tfbsList.Add(region.Title, (From site As MotifMatch
                                             In list
                                             Where Not site Is Nothing).ToArray)
        Next

        Return tfbsList
    End Function

End Module

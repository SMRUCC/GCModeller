Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Interops.NBCR
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("motif_tool")>
Module MotifsTool

    Sub Main()
        Call Converts.makeDataframe.addHandler(GetType(Probability), AddressOf pwm_table)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function pwm_table(pwm As Probability, args As list, env As Environment) As dataframe
        Dim alphabets As String() = pwm.region _
            .Select(Function(r) r.frequency.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim tbl As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        For Each c As String In alphabets
            Call tbl.add(c, From r As Residue
                            In pwm.region
                            Select r.frequency.TryGetValue(c))
        Next

        Return tbl
    End Function

    ''' <summary>
    ''' read meme motif text file
    ''' </summary>
    ''' <param name="file">file path to the meme motif text file(*.meme)</param>
    ''' <returns></returns>
    <ExportAPI("read_meme")>
    Public Function read_meme(file As String) As MotifPWM()
        Return MEME_Suite.ParsePWMFile(file).ToArray
    End Function

    <ExportAPI("save_meme")>
    Public Function save_meme(motif As Probability, file As String) As Object
        Return motif.SaveToMeme(file)
    End Function

    <ExportAPI("open_meme_dir")>
    Public Function open_meme_dir(dir As String) As MEMEMotifRepository
        Return New MEMEMotifRepository(dir)
    End Function

End Module

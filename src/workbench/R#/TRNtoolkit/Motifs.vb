Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Interops.NBCR

<Package("motif_tool")>
Module MotifsTool

    ''' <summary>
    ''' read meme motif text file
    ''' </summary>
    ''' <param name="file">file path to the meme motif text file(*.meme)</param>
    ''' <returns></returns>
    <ExportAPI("read_meme")>
    Public Function read_meme(file As String) As MotifPWM()
        Return MEME_Suite.ParsePWMFile(file).ToArray
    End Function

End Module

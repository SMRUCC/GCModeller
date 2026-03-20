Imports System
Imports HMMER3

Module Program
    Sub Main(args As String())
        Call parserTest()
        HMMER3.Examples.Example1_BasicUsage()
    End Sub

    Sub parserTest()
        Dim list = KOFamScan.ParseTable("G:\GCModeller\src\GCModeller\analysis\SequenceToolkit\Pfam-HMM\kofamscan.txt".OpenReadonly).ToArray

        Pause()
    End Sub
End Module

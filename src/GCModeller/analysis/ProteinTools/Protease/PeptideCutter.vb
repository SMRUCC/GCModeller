Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.SeqFeature
Imports r = System.Text.RegularExpressions.Regex

Public Module PeptideCutter

    <Extension>
    Public Iterator Function RunTest(seq As IPolymerSequenceModel, proteases As IEnumerable(Of CleavageRule)) As IEnumerable(Of Site)
        Dim windows = seq.SequenceData _
            .ToUpper _
            .Select(Function(c) c.ToString) _
            .SlideWindows(slideWindowSize:=6)

        For Each enzyme As CleavageRule In proteases
            Dim rule As SeqValue(Of String)() = enzyme _
                .GetRules _
                .SeqIterator _
                .ToArray
            Dim match As Boolean

            For Each window As SlideWindow(Of String) In windows
                match = True

                For Each index As SeqValue(Of String) In rule
                    If Not r.Match(window(index), index.value).Success Then
                        match = False
                        Exit For
                    End If
                Next

                If match Then
                    Yield New Site With {
                        .Name = enzyme.Protease,
                        .Left = window.Left + 1
                    }
                End If
            Next
        Next
    End Function
End Module

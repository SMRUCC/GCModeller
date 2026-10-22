Imports System
Imports SMRUCC.genomics.Analysis.Circuits.Enumeration

Module Program
    Sub Main(args As String())
        Console.WriteLine("total    = {0}", TopologyEnumerator.CountAll())
        Console.WriteLine("connected= {0}", TopologyEnumerator.CountConnected())

        For n As Integer = 0 To 9
            Console.WriteLine("links<={0} : {1}", n, TopologyEnumerator.CountUpTo(n))
        Next

        Dim hist As Integer() = TopologyEnumerator.LinkCountHistogram(TopologyEnumerator.EnumerateConnected())

        For i As Integer = 0 To hist.Length - 1
            Console.WriteLine("  link {0}: {1}", i, hist(i))
        Next
    End Sub
End Module

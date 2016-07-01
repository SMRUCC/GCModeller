Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Module Program

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine)
        ''For Each file In FileIO.FileSystem.GetFiles("E:\xan\", FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
        ''    Call ExtractCognateTCS(file)
        ''Next

        'Dim data = SequenceAssembler.initialize({".\sk.fas", ".\rr.fas"})
        'Dim networklayout = BeliefNetwork.GenerateNetwork(data)
        'Dim network = New BeliefNetwork

        'Call networklayout.GetXml.Save("x:\network_test.xml")
        'Call network.LoadData(networklayout)

        'Dim pr = network.GetBelief(
        '    {SMRUCC.genomics.SequenceModel.FASTA.File.Read("./sk.txt").First.Sequence, ""},
        '    {"", SMRUCC.genomics.Assembly.FASTAFile.Read("./rr.txt").First.Sequence})

        'Console.WriteLine("The cross-talk possibility is {0}", pr)
        'Console.ReadLine()
    End Function

    Public Sub ExtractCognateTCS(GenomeData As DocumentStream.File)
        Dim SKPath As String = FileIO.FileSystem.GetParentPath(GenomeData.FilePath) & "/Sk.fsa"
        Dim RRPath As String = FileIO.FileSystem.GetParentPath(GenomeData.FilePath) & "/RR.fsa"

        Dim Sk As FASTA.FastaFile = FASTA.FastaFile.Read(SKPath, False)
        Dim RR As FASTA.FastaFile = FASTA.FastaFile.Read(RRPath, False)

        Dim SkPfams As String() = New String() {"PF00512", "PF07536", "PF07568", "PF07730", "PF06580", "PF01627"}
        Dim RRPfam As String = "PF00072"

        For i As Integer = 0 To GenomeData.Count - 1
            Dim row = GenomeData(i)
            Dim Pfams As String = row(3)
            For Each skPfam As String In SkPfams
                If Pfams.Contains(skPfam) Then
                    Dim nxt = GenomeData(i + 1)
                    If nxt.Column(3).Contains(RRPfam) Then '找到一对Cognate双组份系统
                        Call Sk.Add(New FASTA.FastaToken With {.SequenceData = row(4), .Attributes = New String() {row.First, row(2)}})
                        Call RR.Add(New FASTA.FastaToken With {.SequenceData = nxt(4), .Attributes = New String() {nxt.First, nxt(2)}})
                    End If
                End If
            Next
        Next

        Call Sk.Save(SKPath)
        Call RR.Save(RRPath)
    End Sub
End Module

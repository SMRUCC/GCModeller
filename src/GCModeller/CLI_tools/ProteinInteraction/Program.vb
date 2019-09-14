#Region "Microsoft.VisualBasic::7298b021de63c412b7f7e78d2ccc8683, CLI_tools\ProteinInteraction\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module Program
    ' 
    '     Function: Main
    ' 
    '     Sub: ExtractCognateTCS
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.Data.csv

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

    Public Sub ExtractCognateTCS(GenomeData As IO.File, EXPORT$)
        Dim SKPath As String = $"{EXPORT}/Sk.fsa"
        Dim RRPath As String = $"{EXPORT}/RR.fsa"

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
                        Call Sk.Add(New FASTA.FastaSeq With {.SequenceData = row(4), .Headers = New String() {row.First, row(2)}})
                        Call RR.Add(New FASTA.FastaSeq With {.SequenceData = nxt(4), .Headers = New String() {nxt.First, nxt(2)}})
                    End If
                End If
            Next
        Next

        Call Sk.Save(SKPath)
        Call RR.Save(RRPath)
    End Sub
End Module

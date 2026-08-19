Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure.ProteinStructure

Module Program
    Sub Main(args As String())
        Dim fasta = "G:\cell-render\data\ec_numbers.fasta"
        Dim sb As New StringBuilder

        ' two artificial families: family A shares a motif block, family B another
        Dim aMotif = "ACDEFGHIKL"   ' common 9-mer repeatedly
        Dim bMotif = "MNPQRSTVWY"   ' common 9-mer repeatedly

        For i As Integer = 1 To 20
            ' family A: built around aMotif
            Dim seqA = "M" & aMotif & "G" & aMotif & "K" & aMotif & "D" & If(i Mod 2 = 0, "V", "L")
            sb.AppendLine(">protA_" & i)
            sb.AppendLine(seqA)
        Next

        For i As Integer = 1 To 20
            ' family B: built around bMotif
            Dim seqB = "S" & bMotif & "T" & bMotif & "R" & bMotif & "N" & If(i Mod 2 = 0, "Q", "E")
            sb.AppendLine(">protB_" & i)
            sb.AppendLine(seqB)
        Next

        File.WriteAllText(fasta, sb.ToString)

        Dim clust As New ProteinFamilyClustering With {
            .k = 5,
            .topN = 500,
            .svdDims = 9,
            .knnK = 6,
            .similarityCutoff = 0.0
        }

        Dim result = clust.Run(fasta)

        Console.WriteLine("sequenceCount = " & result.sequenceNames.Length)
        Console.WriteLine("familyCount   = " & result.familyCount)
        Console.WriteLine("svdDims       = " & result.svdDims)
        Console.WriteLine("knnEdges      = " & result.knnEdges.Length)

        ' show how many distinct family ids appear among protA vs protB
        Dim aFams = result.sequenceNames _
            .Where(Function(n) n.StartsWith("protA_")) _
            .Select(Function(n, idx) result.familyAssignments(idx)) _
            .Distinct _
            .Count
        Dim bFams = result.sequenceNames _
            .Where(Function(n) n.StartsWith("protB_")) _
            .Select(Function(n, idx) result.familyAssignments(idx)) _
            .Distinct _
            .Count

        Console.WriteLine("protA distinct families = " & aFams)
        Console.WriteLine("protB distinct families = " & bFams)

        For Each fam In result.families
            Console.WriteLine("  " & fam.ToString())
        Next

        File.Delete(fasta)
        Console.WriteLine("SMOKE TEST OK")
    End Sub
End Module

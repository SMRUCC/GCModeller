#Region "Microsoft.VisualBasic::5fda704e90e13c86e6ae7a5ffba904af, analysis\ProteinTools\ProteinMatrix\test\Program.vb"

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


    ' Code Statistics:

    '   Total Lines: 187
    '    Code Lines: 134 (71.66%)
    ' Comment Lines: 18 (9.63%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 35 (18.72%)
    '     File Size: 7.74 KB


    ' Module Program
    ' 
    '     Sub: InMemorySmokeTest, Main, RunCluster, StreamingSmokeTest
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.Model.MotifGraph.ProteinStructure.FamilyCluster

Module Program
    Sub Main(args As String())
        ' Linclust 算法演示(内存构造数据,无需外部文件)
        ' Call LinclustDemo.Run()

        ' CDHit 聚类 + FamilyExports/SequenceCluster 导出演示(内存构造数据,无需外部文件)
        ' 默认导出到 Z:/cdhit_exports,可在调用处指定 outputDir
        Call CDHitDemo.Run(outputDir:="Z:/cdhit_exports")

        ' 以下为 FamilyCluster 流式聚类测试,依赖外部数据文件,默认不运行:
        ' Call RunCluster()
    End Sub

    Sub RunCluster()
        Dim clust As New ProteinFamilyClustering With {
            .k = 4,
            .topN = 10000,
            .svdDims = 9,
            .knnK = 6,
            .similarityCutoff = 0.0
        }

        Dim result = clust.RunStreaming("G:\cell-render\data\ec_numbers.fasta", workDir:="Z:/demo")

        Console.WriteLine("[stream] sequenceCount = " & result.sequenceNames.Length)
        Console.WriteLine("[stream] familyCount   = " & result.familyCount)
        Console.WriteLine("[stream] svdDims       = " & result.svdDims)

        ' the big intermediate products must be present on disk and streamable
        Dim svdRows = result.StreamSvd.Count
        Dim knnEdges = result.StreamKnnEdges.Count
        Console.WriteLine("[stream] svd rows on disk = " & svdRows)
        Console.WriteLine("[stream] knn edges on disk = " & knnEdges)

        ' every family must have a reference sequence chosen by MSA
        Dim famWithRef = result.families.Count(Function(f) f.reference IsNot Nothing)
        Console.WriteLine("[stream] families with reference = " & famWithRef & " / " & result.families.Length)

        ' family separation check: protA and protB should land in disjoint family sets.
        ' build a name -> index lookup so the assignment is read at the correct position.
        Dim nameIndex = result.sequenceNames _
            .Select(Function(n, i) (n, i)) _
            .ToDictionary(Function(x) x.n, Function(x) x.i)
    End Sub

    Sub InMemorySmokeTest()
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

    Sub StreamingSmokeTest()
        Dim fasta = "Z:\ec_numbers.fasta"
        Dim workDir = "Z:\stream_out"
        Dim sb As New StringBuilder

        Dim aMotif = "ACDEFGHIKL"
        Dim bMotif = "MNPQRSTVWY"

        For i As Integer = 1 To 20
            Dim seqA = "M" & aMotif & "G" & aMotif & "K" & aMotif & "D" & If(i Mod 2 = 0, "V", "L")
            sb.AppendLine(">protA_" & i)
            sb.AppendLine(seqA)
        Next

        For i As Integer = 1 To 20
            Dim seqB = "S" & bMotif & "T" & bMotif & "R" & bMotif & "N" & If(i Mod 2 = 0, "Q", "E")
            sb.AppendLine(">protB_" & i)
            sb.AppendLine(seqB)
        Next

        File.WriteAllText(fasta, sb.ToString)

        If Directory.Exists(workDir) Then
            Directory.Delete(workDir, recursive:=True)
        End If

        Dim clust As New ProteinFamilyClustering With {
            .k = 5,
            .topN = 500,
            .svdDims = 9,
            .knnK = 6,
            .similarityCutoff = 0.0
        }

        Dim result = clust.RunStreaming(fasta, workDir)

        Console.WriteLine("[stream] sequenceCount = " & result.sequenceNames.Length)
        Console.WriteLine("[stream] familyCount   = " & result.familyCount)
        Console.WriteLine("[stream] svdDims       = " & result.svdDims)

        ' the big intermediate products must be present on disk and streamable
        Dim svdRows = result.StreamSvd.Count
        Dim knnEdges = result.StreamKnnEdges.Count
        Console.WriteLine("[stream] svd rows on disk = " & svdRows)
        Console.WriteLine("[stream] knn edges on disk = " & knnEdges)

        ' every family must have a reference sequence chosen by MSA
        Dim famWithRef = result.families.Count(Function(f) f.reference IsNot Nothing)
        Console.WriteLine("[stream] families with reference = " & famWithRef & " / " & result.families.Length)

        ' family separation check: protA and protB should land in disjoint family sets.
        ' build a name -> index lookup so the assignment is read at the correct position.
        Dim nameIndex = result.sequenceNames _
            .Select(Function(n, i) (n, i)) _
            .ToDictionary(Function(x) x.n, Function(x) x.i)
        Dim aFams = result.sequenceNames _
            .Where(Function(n) n.StartsWith("protA_")) _
            .Select(Function(n) result.familyAssignments(nameIndex(n))) _
            .Distinct _
            .ToArray
        Dim bFams = result.sequenceNames _
            .Where(Function(n) n.StartsWith("protB_")) _
            .Select(Function(n) result.familyAssignments(nameIndex(n))) _
            .Distinct _
            .ToArray
        Dim overlap = aFams.Intersect(bFams).Count
        Console.WriteLine("[stream] protA families = " & aFams.Length & ", protB families = " & bFams.Length & ", overlap = " & overlap)

        If result.sequenceNames.Length <> 40 OrElse svdRows <> 40 OrElse knnEdges = 0 OrElse famWithRef <> result.families.Length OrElse overlap > 0 Then
            Throw New Exception("[stream] smoke test assertion failed")
        End If

        File.Delete(fasta)
        Console.WriteLine("STREAMING SMOKE TEST OK")
    End Sub
End Module


#Region "Microsoft.VisualBasic::ed3bdfffcc55ceb588cf192ed2bedeb6, analysis\Metagenome\Metagenome\Kmers\KmerBackground.vb"

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

    '   Total Lines: 86
    '    Code Lines: 65 (75.58%)
    ' Comment Lines: 1 (1.16%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 20 (23.26%)
    '     File Size: 3.57 KB


    '     Class KmerBackground
    ' 
    '         Properties: KmerDistributions, Prior, speciesKmerCounts
    ' 
    '         Function: Load
    ' 
    '         Sub: FlushCache, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Kmers

    Public Class KmerBackground

        Public Property Prior As Dictionary(Of Integer, Double)
        Public Property KmerDistributions As KmerMemory(Of Dictionary(Of Integer, Double))
        Public Property speciesKmerCounts As Dictionary(Of Integer, ULong)

        Public Sub Save(dir As String)
            Call dir.MakeDir

            Using csv As New StreamWriter($"{dir}/kmer_dist.csv".Open(FileMode.OpenOrCreate, doClear:=True), Encoding.ASCII)
                Call csv.WriteLine("kmer,ncbi_taxid,frequency")

                For Each kmerdata In KmerDistributions
                    Dim kmer As String = kmerdata.Key

                    For Each q In kmerdata.Value
                        Call csv.WriteLine({kmer, q.Key, q.Value}.JoinBy(","))
                    Next
                Next

                Call csv.Flush()
            End Using

            Call Prior.GetJson.SaveTo($"{dir}/bayes_priors.json")
            Call speciesKmerCounts.GetJson.SaveTo($"{dir}/species_kmer.json")
        End Sub

        Public Shared Function Load(dir As String, Optional cache_size As Integer = 1000) As KmerBackground
            Dim bayes As Dictionary(Of Integer, Double) = $"{dir}/bayes_priors.json".LoadJsonFile(Of Dictionary(Of Integer, Double))
            Dim species As Dictionary(Of Integer, ULong) = $"{dir}/species_kmer.json".LoadJsonFile(Of Dictionary(Of Integer, ULong))
            Dim kmerDist As New KmerMemory(Of Dictionary(Of Integer, Double))

            Using csv As New StreamReader($"{dir}/kmer_dist.csv".Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True), Encoding.ASCII)
                Dim line As Value(Of String) = ""
                Dim cache As New Dictionary(Of String, Dictionary(Of Integer, Double))

                ' skip of the csv header line
                Call csv.ReadLine()

                Do While (line = csv.ReadLine) IsNot Nothing
                    Dim tokens As String() = CStr(line).Split(","c)
                    Dim taxid As Integer = Integer.Parse(tokens(1))
                    Dim q As Double = Double.Parse(tokens(2))

                    If cache.Count > cache_size Then
                        Call FlushCache(kmerDist, cache)
                    End If

                    If cache.ContainsKey(tokens(0)) Then
                        cache(tokens(0)).Add(taxid, q)
                    Else
                        cache.Add(tokens(0), New Dictionary(Of Integer, Double) From {{taxid, q}})
                    End If
                Loop

                Call FlushCache(kmerDist, cache)
            End Using

            Return New KmerBackground With {
                .KmerDistributions = kmerDist,
                .Prior = bayes,
                .speciesKmerCounts = species
            }
        End Function

        Private Shared Sub FlushCache(ByRef kmerDist As KmerMemory(Of Dictionary(Of Integer, Double)), ByRef cache As Dictionary(Of String, Dictionary(Of Integer, Double)))
            For Each kmer In cache
                If Not kmerDist.HashKmer(kmer.Key) Then
                    Call kmerDist.Add(kmer.Key, cache(kmer.Key))
                Else
                    Call kmerDist(kmer.Key).AddRange(cache(kmer.Key))
                End If
            Next

            Call cache.Clear()
        End Sub

    End Class
End Namespace

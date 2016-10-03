#Region "Microsoft.VisualBasic::837f693de48c9188270369a44a42fddf, ..\GCModeller\analysis\RNA-Seq\TSSsTools\API.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.SequenceModel

Public Module API

    <ExportAPI("/Reads.Db.Dump", Usage:="/Reads.Db.Dump /in <reads.count.csv> [/out <out.dat>]")>
    Public Function CreateDb(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile & ".Reads.Count.dat")
        Dim counts = inFile.LoadCsv(Of ReadsCount)
        Return ReadsCount.WriteDb(counts, out).CLICode
    End Function

    <ExportAPI("/Reads.Count", Usage:="/Reads.Count /in <mappings.csv> /ref <ref.fasta> [/out <out.csv>]")>
    Public Function Count(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".ReadsCount.Csv")
        Dim ref As New SMRUCC.genomics.SequenceModel.FASTA.FastaToken(args("/ref"))
        Dim mappings As New DocumentStream.Linq.DataStream(inFile)  ' 读取测序数据的mapping结果
        Dim readsCount As ___readsCount = New ___readsCount(ref)

        Call $"Start write dictionary data....".__DEBUG_ECHO
        Call mappings.ForEachBlock(Of BlastnMapping)(AddressOf readsCount.ForEachBuild)

        Return readsCount.readsCount.SaveTo(out)
    End Function

    Private Class ___readsCount

        Public readsCount As ReadsCount()

        Sub New(ref As I_PolymerSequenceModel)
            Call $"Create reads count from references, length {ref.SequenceData.Length} characters....".__DEBUG_ECHO

            Dim chars As Char() = ref.SequenceData.ToArray
            Call $"Chars array of the reference sequence created!".__DEBUG_ECHO

            Dim index As Long() = chars.LongSeq
            Call $"Character indexed...".__DEBUG_ECHO

            readsCount = (From i As Long In index.AsParallel
                          Select pos = New ReadsCount With {
                              .Index = i + 1,
                              .NT = chars(i)
                          }
                          Order By pos.Index Ascending).ToArray

            Call $"Reads dictionary created!".__DEBUG_ECHO
        End Sub

        Public Sub ForEachBuild(source As BlastnMapping())
            Dim LQuery = (From x In source Select loci = DirectCast(x.MappingLocation.Normalization, ComponentModel.Loci.NucleotideLocation)).ToArray

            For Each r As ComponentModel.Loci.NucleotideLocation In LQuery
                If r.Strand = Strands.Forward Then
                    For i As Long = r.Left To r.Right
                        readsCount(i - 1).ReadsPlus += 1
                    Next

                    readsCount(r.Left - 1).SharedPlus += 1
                Else
                    For i As Long = r.Left To r.Right
                        readsCount(i - 1).ReadsMinus += 1
                    Next

                    readsCount(r.Right - 1).SharedMinus += 1
                End If
            Next
        End Sub
    End Class
End Module

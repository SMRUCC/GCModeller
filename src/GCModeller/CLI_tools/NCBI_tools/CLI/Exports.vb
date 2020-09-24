#Region "Microsoft.VisualBasic::5f31f7af8e88c48665f10e406763e73b, CLI_tools\NCBI_tools\CLI\Exports.vb"

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

    ' Module CLI
    ' 
    '     Function: FilterExports
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Filter.Exports",
               Info:="String similarity match of the fasta title with given terms for search and export by taxonomy.",
               Usage:="/Filter.Exports /in <nt.fasta> /tax <taxonomy_DIR> /gi2taxid <gi2taxid.txt> /words <list.txt> [/out <out.DIR>]")>
    <Group(CLIGrouping.ExportTools)>
    <Group(CLIGrouping.GITools)>
    Public Function FilterExports(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim tax As New NcbiTaxonomyTree(args("/tax"))
        Dim gi2taxid = Taxonomy.AcquireAuto(args("/gi2taxid"))
        Dim words As String = args("/words")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "." & words.BaseName & ".EXPORT/")
        Dim wordList As String() = words.IterateAllLines.Select(AddressOf Trim).ToArray
        Dim m As New Value(Of DistResult)

        Const deli As String = "AAAAAAAAAAAA"

        For Each fa In New StreamIterator([in]).ReadStream
            Dim gi As Integer = CInt(Val(Regex.Match(fa.Title, "gi\|\d+").Value.Split("|"c).Last))

            If gi2taxid.ContainsKey(gi) Then
                Dim taxid As Integer = gi2taxid(gi)
                Dim taxon = tax.GetAscendantsWithRanksAndNames(taxid, True)
                Dim tree = TaxonomyNode.Taxonomy(taxon, deli).NormalizePathString.Replace(deli, "/")
                Dim name As String = fa.Headers.ElementAtOrDefault(4, "").Trim
                Dim write As Boolean = False

                Call fa.Headers.Add(TaxonomyNode.Taxonomy(taxon,))

                For Each s As String In wordList
                    If InStr(name, s, CompareMethod.Text) > 0 OrElse
                        ((Not (m = LevenshteinDistance.ComputeDistance(s, name)) Is Nothing) AndAlso
                        s.Split.Length - m.Value.NumMatches < 3) Then

                        Dim path As String = out & "/" & tree & "/" & s & ".fasta"
                        Call fa.GenerateDocument(-1, False).SaveTo(path, Encoding.ASCII, append:=True)
                        write = True
                        Call fa.Title.__DEBUG_ECHO

                        Exit For
                    End If
                Next

                If Not write Then
                    Dim hash = TaxonomyNode.RankTable(taxon)
                    Dim sp As String = hash.TryGetValue("species", [default]:=Nothing)

                    If Not String.IsNullOrEmpty(sp) Then

                        For Each s As String In wordList
                            If InStr(s, sp, CompareMethod.Text) > 0 OrElse
                                InStr(sp, s, CompareMethod.Text) > 0 OrElse
                                ((Not (m = LevenshteinDistance.ComputeDistance(s, sp)) Is Nothing) AndAlso
                                s.Split.Length - m.Value.NumMatches < 3) Then

                                Dim path As String = out & "/" & tree & "/" & s & ".fasta"
                                Call fa.GenerateDocument(-1, False).SaveTo(path, Encoding.ASCII, append:=True)
                                write = True
                                Call fa.Title.__DEBUG_ECHO

                                Exit For
                            End If
                        Next
                    End If
                End If
            End If
        Next

        Return 0
    End Function
End Module

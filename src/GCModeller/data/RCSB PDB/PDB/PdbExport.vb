#Region "Microsoft.VisualBasic::53ca133a3017def80a7b8268f2f45f2c, data\RCSB PDB\PDB\PdbExport.vb"

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

    ' Module PdbExport
    ' 
    '     Function: __generateItem, AssemblyProteinComplexes, ExportSequence, GetByKeyword, LoadBhCsv
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.Polypeptides

''' <summary>
''' PDB File Format
''' The Protein Data Bank (PDB) format provides a standard representation for macromolecular structure data derived from
''' X-ray diffraction and NMR studies. This representation was created in the 1970's and a large amount of software 
''' using it has been written.
''' 
''' Documentation describing the PDB file format is available from the wwPDB at http://www.wwpdb.org/docs.html.
''' 
''' Historical copies of the PDB file format from 1992*	and 1996* are available.
''' 
''' * PDF documents require Acrobat Reader
''' </summary>
''' <remarks></remarks>
Public Module PdbExport

    Public Function ExportSequence(pdbFile As String) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
        Dim chunkBuffer As String() = IO.File.ReadAllLines(pdbFile)

        Dim Sequence = GetByKeyword(chunkBuffer, "SEQRES")
        Dim Chains As String() = (From item In Sequence.AsParallel Let id = item(1) Select id Distinct Order By id Ascending).ToArray
        Dim FASTA As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq() =
            New SMRUCC.genomics.SequenceModel.FASTA.FastaSeq(Chains.Count - 1) {}
        Dim Definitions = GetByKeyword(chunkBuffer, "DBREF")

        For i As Integer = 0 To Chains.Count - 1
            Dim ChainId As String = Chains(i)
            Dim Segments = (From item In Sequence.AsParallel Where String.Equals(ChainId, item(1)) Select item Order By Val(item(0)) Ascending).ToArray
            Dim seqBuilder As StringBuilder = New StringBuilder(1024)

            For Each segment In Segments
                For Each item As String In segment.Skip(3)
                    Call seqBuilder.Append(Polypeptide.Abbreviate(item))
                Next
            Next

            Dim Def = (From item In Definitions Where String.Equals(item(1), ChainId) Select item).First
            Dim FsaObject As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq =
                New SequenceModel.FASTA.FastaSeq

            FsaObject.SequenceData = seqBuilder.ToString
            FsaObject.Headers = New String() {Def(0), Def(1), Def(4), Def(5), Def(6)}
            FASTA(i) = FsaObject
        Next

        Return FASTA
    End Function

    Private Function GetByKeyword(chunkBuffer As String(), keyword As String) As String()()
        Dim LQuery = (From strLine As String In chunkBuffer Let Tokens As String() = strLine.Split
                      Let KeywordToken As String = Tokens.First
                      Where String.Equals(keyword, KeywordToken)
                      Select (From strItem As String In Tokens.Skip(1).ToArray Where Not String.IsNullOrEmpty(strItem) Select strItem).ToArray).ToArray
        Return LQuery
    End Function

    Public Iterator Function AssemblyProteinComplexes(bhPairs As KeyValuePair(Of String, PdbItem)(), PdbAssemblies As IEnumerable(Of PdbComplexesAssembly)) As IEnumerable(Of AssemblyComplex)
        For Each Entry In PdbAssemblies '每一个Entry相当于一个蛋白质复合物
            Dim LQuery = (From item In bhPairs.AsParallel Where String.Equals(item.Value.PdbId, Entry.PdbId) Select item).ToArray

            If Not LQuery.IsNullOrEmpty Then
                Dim TargetProtein = (From item In LQuery Where Entry.Equals(item.Value) Select item.Key Distinct Order By Key Ascending).ToArray '筛选出与Entry相同的蛋白质
                Dim ChainIdLQuery = (From itr In Entry.InteractionChainId Select (From item In LQuery Where String.Equals(item.Value.ChainId, itr) Select item.Key).ToArray).ToArray '获取每一个ChainId所对应的蛋白质
                Dim CheckLQuery = (From item In ChainIdLQuery Where item.IsNullOrEmpty Select 1).Count > 0

                If CheckLQuery Then '检查数据的完整性
                    '该记录不是完整的记录，则忽略忽略掉
                    Continue For
                End If

                Dim TempChunk As New List(Of String()) From {TargetProtein}
                Call TempChunk.AddRange(ChainIdLQuery)

                Dim ProteinComplexesAssembly = TempChunk.ToArray.AllCombinations '利用Entry里面的记录在Lquery里面进行筛选，使用组合的方式进行组装蛋白质

                For Each item In ProteinComplexesAssembly
                    Dim Row As New AssemblyComplex With {.UnitCounts = item.Count}
                    Dim ProteinId As String() = (From strData As String In item.IteratesALL Select strData Order By strData Ascending).ToArray

                    Row.AssemblyComponents = ProteinId

                    Yield Row
                Next
            End If
        Next
    End Function
End Module

Public Class AssemblyComplex

    Public Property UnitCounts As Integer
    Public Property AssemblyComponents As String()

End Class
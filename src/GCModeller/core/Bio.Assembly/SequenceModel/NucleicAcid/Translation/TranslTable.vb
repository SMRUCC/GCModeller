#Region "Microsoft.VisualBasic::0368959bcb7a11d36737a7be8577cff6, Bio.Assembly\SequenceModel\NucleicAcid\Translation\TranslTable.vb"

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

    '     Class TranslTable
    ' 
    '         Properties: CodenTable, InitCodons, StopCodons, TranslTable
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: __checkDirection, __parseHash, __parseTable, __split, __trimForce
    '                   CreateFrom, Find, GetEnumerator, GetTable, IEnumerable_GetEnumerator
    '                   IsInitCoden, (+2 Overloads) IsStopCoden, IsStopCodon, ToCodonCollection, ToString
    '                   (+2 Overloads) Translate
    ' 
    '         Sub: __initProfiles
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid
Imports SMRUCC.genomics.SequenceModel.Polypeptides
Imports SMRUCC.genomics.SequenceModel.Polypeptides.Polypeptides
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Conversion

Namespace SequenceModel.NucleotideModels.Translation

    ''' <summary>
    ''' Compiled by Andrzej (Anjay) Elzanowski and Jim Ostell at National Center for Biotechnology Information (NCBI), Bethesda, Maryland, U.S.A.
    ''' 
    ''' NCBI takes great care To ensure that the translation For Each coding sequence (CDS) present In GenBank records Is correct. 
    ''' Central To this effort Is careful checking On the taxonomy Of Each record And assignment Of the correct genetic code 
    ''' (shown As a /transl_table qualifier On the CDS In the flat files) For Each organism And record. This page summarizes And references this work.
    ''' 
    ''' The synopsis presented below Is based primarily On the reviews by Osawa et al. (1992) And Jukes And Osawa (1993). 
    ''' Listed In square brackets [] (under Systematic Range) are tentative assignments Of a particular code based On 
    ''' sequence homology And/Or phylogenetic relationships.
    ''' 
    ''' The print-form ASN.1 version Of this document, which includes all the genetic codes outlined below, Is also available here. 
    ''' Detailed information On codon usage can be found at the Codon Usage Database.
    ''' 
    ''' GenBank format by historical convention displays mRNA sequences Using the DNA alphabet. 
    ''' Thus, For the convenience Of people reading GenBank records, the genetic code tables shown here use T instead Of U.
    ''' 
    ''' The following genetic codes are described here:
    ''' 
    ''' 1. The Standard Code
    ''' 2. The Vertebrate Mitochondrial Code
    ''' 3. The Yeast Mitochondrial Code
    ''' 4. The Mold, Protozoan, And Coelenterate Mitochondrial Code And the Mycoplasma/Spiroplasma Code
    ''' 5. The Invertebrate Mitochondrial Code
    ''' 6. The Ciliate, Dasycladacean And Hexamita Nuclear Code
    ''' 9. The Echinoderm And Flatworm Mitochondrial Code
    ''' 10. The Euplotid Nuclear Code
    ''' 11. The Bacterial, Archaeal And Plant Plastid Code
    ''' 12. The Alternative Yeast Nuclear Code
    ''' 13. The Ascidian Mitochondrial Code
    ''' 14. The Alternative Flatworm Mitochondrial Code
    ''' 16. Chlorophycean Mitochondrial Code
    ''' 21. Trematode Mitochondrial Code
    ''' 22. Scenedesmus obliquus Mitochondrial Code
    ''' 23. Thraustochytrium Mitochondrial Code
    ''' 24. Pterobranchia Mitochondrial Code
    ''' 25. Candidate Division SR1 And Gracilibacteria Code
    ''' 
    ''' > http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25
    ''' </summary>
    Public Class TranslTable : Implements Generic.IEnumerable(Of KeyValuePair(Of Integer, AminoAcid))

        ''' <summary>
        ''' 遗传密码子表（哈希表）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CodenTable As IReadOnlyDictionary(Of Integer, AminoAcid)
        ''' <summary>
        ''' transl_table=<see cref="Transltable"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TranslTable As Integer

        Public ReadOnly Property InitCodons As Integer()
        Public ReadOnly Property StopCodons As Integer()

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Table">资源文件里面的字典数据或者读取自外部文件的数据</param>
        Sub New(Table As String)
            Dim transl_table = __parseTable(Table.LineTokens, _TranslTable)
            Call __initProfiles(transl_table)
        End Sub

        Sub New(index As Integer, transl_table As Dictionary(Of Codon, AminoAcid))
            TranslTable = index
            Call __initProfiles(transl_table)
        End Sub

        ''' <summary>
        ''' 生成起始密码子和终止密码子
        ''' </summary>
        Private Sub __initProfiles(transl_table As Dictionary(Of Codon, AminoAcid))
            _StopCodons = (From codon In transl_table Where codon.Key.IsStopCodon Select codon.Key.TranslHash).ToArray
            _InitCodons = (From codon In transl_table Where codon.Key.IsInitCodon Select codon.Key.TranslHash).ToArray
            _CodenTable = (From codon As KeyValuePair(Of Codon, AminoAcid)
                           In transl_table
                           Where Not codon.Key.IsStopCodon
                           Select codon) _
                               .ToDictionary(Function(codon) codon.Key.TranslHash,
                                             Function(codon) codon.Value)
        End Sub

        Private Shared Function __parseTable(Tokens As String(), ByRef transl_table As Integer) As Dictionary(Of Codon, AminoAcid)
            Dim index As String = Tokens(Scan0)
            Tokens = (From token As String
                      In Tokens.Skip(1)
                      Let ss As String = Trim(token)
                      Where Not String.IsNullOrEmpty(ss)
                      Select ss).ToArray
            Dim dict As Dictionary(Of Codon, AminoAcid) = __parseHash(Tokens)
            transl_table = Scripting.CTypeDynamic(Of Integer)(index.Split("="c).Last)

            Return dict
        End Function

        Private Shared Function __parseHash(tokens As String()) As Dictionary(Of Codon, AminoAcid)
            Dim MAT = tokens.Select(Function(token As String) Regex.Split(token, "\s+")).ToArray
            Dim Codons = MAT.Select(Function(line) __split(line)).Unlist
            Dim LQuery = (From Token As String() In Codons Select code = New Codon(Token), AA = Token(1).First).ToArray
            Dim hash = LQuery.ToDictionary(Function(obj) obj.code, Function(obj) SequenceModel.Polypeptides.ToEnums(obj.AA))
            Return hash
        End Function

        Private Shared Function __split(line As String()) As String()()
            Dim withInits As Boolean = line.Length = 13

            If withInits Then

                Dim source As New List(Of String)(line)
                Dim Tokens As New List(Of String())
                Dim list As New List(Of String)

                Do While source.Count > 0
                    Call list.Add(source.Take(3).ToArray)
                    Call source.RemoveRange(0, 3)

                    If source.Count > 0 Then
                        If String.Equals(source(Scan0), "i") Then ' 起始密码子
                            Call list.Add("i")
                            Call source.RemoveAt(Scan0)
                        End If
                    End If

                    Call Tokens.Add(list.ToArray)
                    Call list.Clear()
                Loop

                Return Tokens.ToArray
            Else
                Dim Tokens = line.Split(3)
                Return Tokens
            End If
        End Function

        ''' <summary>
        ''' 判断某一个密码子是否为终止密码子
        ''' </summary>
        ''' <param name="hash">该密码子的哈希值</param>
        ''' <returns>这个密码子是否为一个终止密码</returns>
        ''' <remarks></remarks>
        Public Function IsStopCoden(hash As Integer) As Boolean
            Return Array.IndexOf(StopCodons, hash) > -1
        End Function

        Public Function IsStopCoden(Coden As NucleotideModels.Translation.Codon) As Boolean
            Dim hash As Integer = Coden.TranslHash
            Return Array.IndexOf(StopCodons, hash) > -1
        End Function

        ''' <summary>
        ''' 将一条核酸链翻译为蛋白质序列
        ''' </summary>
        ''' <param name="NucleicAcid"></param>
        ''' <param name="force">强制程序跳过终止密码子</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Translate(NucleicAcid As String, force As Boolean) As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)

            NucleicAcid = __checkDirection(NucleicAcid)

            For i As Integer = 1 To Len(NucleicAcid) Step 3
                Dim Tokens As Char() = Mid(NucleicAcid, i, 3)

                If Tokens.Length = 3 Then
                    Dim hash As Integer = Translation.TranslTable.Find(Tokens(0), Tokens(1), Tokens(2))

                    If IsStopCoden(hash) Then
                        If force Then
                            Call sBuilder.Append(ForceStopCoden)
                        Else
                            Exit For
                        End If
                    Else
                        Dim aa As AminoAcid = CodenTable(hash)
                        Dim ch As Char = Polypeptides.Polypeptides.ToChar(aa)

                        Call sBuilder.Append(ch)
                    End If
                End If
            Next

            Dim prot As String = sBuilder.ToString

            If force Then
                Return __trimForce(prot)
            Else
                Return prot
            End If
        End Function

        ''' <summary>
        ''' 三个字母所表示的三联体密码子
        ''' </summary>
        ''' <param name="coden"></param>
        ''' <returns></returns>
        Public Function IsStopCodon(coden As String) As Boolean
            If coden.Length = 3 Then
                Dim hash As Integer = Translation.TranslTable.Find(coden(0), coden(1), coden(2))
                Return IsStopCoden(hash)
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' 三个字母所表示的三联体密码子
        ''' </summary>
        ''' <param name="coden"></param>
        ''' <returns></returns>
        Public Function IsInitCoden(coden As String) As Boolean
            If coden.Length = 3 Then
                Dim hash As Integer = Translation.TranslTable.Find(coden(0), coden(1), coden(2))
                Return Array.IndexOf(InitCodons, hash) > -1
            Else
                Return False
            End If
        End Function

        Private Function __checkDirection(sequence As String) As String
            Dim First As String = Mid(sequence, 1, 3)
            Dim last As String = Mid(sequence, Len(sequence) - 3)

            If IsInitCoden(First) Then  ' 正常的序列
                Return sequence
            End If

            Dim lastAsInit As String = New String(last.Reverse.ToArray)
            If IsInitCoden(lastAsInit) Then  ' 方向可能颠倒了
                Call $"This sequence is possibly in reverse direction...".__DEBUG_ECHO
                Return New String(sequence.Reverse.ToArray)
            End If

            First = NucleicAcid.Complement(First)
            If IsInitCoden(First) Then  ' 互补的序列
                Call $"This sequence is possibly in complement strand...".__DEBUG_ECHO
                Return NucleicAcid.Complement(sequence)
            End If

            lastAsInit = NucleicAcid.Complement(lastAsInit)
            If IsInitCoden(lastAsInit) Then  ' 方向可能颠倒了
                Call $"This sequence is possibly in reverse&complement strand...".__DEBUG_ECHO
                Return New String(NucleicAcid.Complement(sequence).Reverse.ToArray)
            End If

            Return sequence ' 实在判断不出来了，只能够硬着头皮翻译下去了 
        End Function

        Private Function __trimForce(prot As String) As String
            If prot.Last = ForceStopCoden Then
                prot = Mid(prot, 1, Len(prot) - 1)
            End If
            Return prot
        End Function

        Const ForceStopCoden As Char = "-"c

        Public Function Translate(SequenceData As NucleicAcid, force As Boolean) As String
            Return Translate(SequenceData.SequenceData, force)
        End Function

        ''' <summary>
        ''' 没有终止密码子，非翻译用途的
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ToCodonCollection(SequenceData As NucleicAcid) As Codon()
            Dim Codons = SequenceData.ToArray.CreateSlideWindows(3, offset:=3)
            Dim AA As Codon() =
                LinqAPI.Exec(Of Codon) <= From Codon As SlideWindow(Of DNA)
                                          In Codons
                                          Let aac As Codon = New Codon With {
                                              .X = Codon.Items(0),
                                              .Y = Codon.Items(1),
                                              .Z = Codon.Items(2)
                                          }
                                          Select aac
            AA = (From codon As Codon
                  In AA
                  Where Array.IndexOf(StopCodons, codon.TranslHash) = -1 ' 由于使用无参数的构造函数构造出来的密码子对象是没有启动和终止的信息的，所以使用当前的翻译表的终止密码表来判断
                  Select codon).ToArray
            Return AA
        End Function

        ''' <summary>
        ''' Available index value was described at http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/index.cgi?chapter=tgencodes#SG25
        ''' </summary>
        ''' <param name="index"></param>
        ''' <returns></returns>
        Public Shared Function GetTable(index As Integer) As TranslTable
            Return _tables(index)
        End Function

        Protected Shared ReadOnly _tables As Dictionary(Of Integer, TranslTable) =
            New Dictionary(Of Integer, TranslTable) From {
 _
            {1, New TranslTable(My.Resources.transl_table_1)},
            {2, New TranslTable(My.Resources.transl_table_2)},
            {3, New TranslTable(My.Resources.transl_table_3)},
            {4, New TranslTable(My.Resources.transl_table_4)},
            {5, New TranslTable(My.Resources.transl_table_5)},
            {6, New TranslTable(My.Resources.transl_table_6)},
            {9, New TranslTable(My.Resources.transl_table_9)},
            {10, New TranslTable(My.Resources.transl_table_10)},
            {11, New TranslTable(My.Resources.transl_table_11)},
            {12, New TranslTable(My.Resources.transl_table_12)},
            {13, New TranslTable(My.Resources.transl_table_13)},
            {14, New TranslTable(My.Resources.transl_table_14)},
            {16, New TranslTable(My.Resources.transl_table_16)},
            {21, New TranslTable(My.Resources.transl_table_21)},
            {22, New TranslTable(My.Resources.transl_table_22)},
            {23, New TranslTable(My.Resources.transl_table_23)},
            {24, New TranslTable(My.Resources.transl_table_24)},
            {25, New TranslTable(My.Resources.transl_table_25)}
        }

        Public Shared Function Find(r1 As Char, r2 As Char, r3 As Char) As Integer
            Return Codon.CalTranslHash(NucleotideConvert(r1),
                                       NucleotideConvert(r2),
                                       NucleotideConvert(r3))
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of Integer, AminoAcid)) _
            Implements IEnumerable(Of KeyValuePair(Of Integer, AminoAcid)).GetEnumerator
            For Each codon In Me.CodenTable
                Yield codon
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function

        Public Shared Function CreateFrom(hashTable As String) As TranslTable
            Dim transl_table As Integer, hashTokens As String() = hashTable.LineTokens
            Dim dict As Dictionary(Of Codon, AminoAcid) =
                __parseTable(hashTokens, transl_table)
            Dim table As TranslTable = New TranslTable(transl_table, dict)
            Return table
        End Function

        Public Overrides Function ToString() As String
            Return $"transl_table={TranslTable}"
        End Function
    End Class
End Namespace

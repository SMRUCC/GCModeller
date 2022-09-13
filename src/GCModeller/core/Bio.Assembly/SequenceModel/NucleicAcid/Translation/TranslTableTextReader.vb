#Region "Microsoft.VisualBasic::0e5e7af68ce2087ae06b8e4bd906be2c, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Translation\TranslTableTextReader.vb"

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

    '   Total Lines: 96
    '    Code Lines: 77
    ' Comment Lines: 4
    '   Blank Lines: 15
    '     File Size: 4.34 KB


    '     Module TranslTableTextReader
    ' 
    '         Function: doParseTable, parseHashValues, splitMatrix
    ' 
    '         Sub: doInitProfiles
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Conversion
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid
Imports SMRUCC.genomics.SequenceModel.Polypeptides
Imports SMRUCC.genomics.SequenceModel.Polypeptides.Polypeptide

Namespace SequenceModel.NucleotideModels.Translation

    Module TranslTableTextReader

        ''' <summary>
        ''' 生成起始密码子和终止密码子
        ''' </summary>
        Friend Sub doInitProfiles(transl_table As Dictionary(Of Codon, AminoAcid),
                                  ByRef stopCodons As Integer(),
                                  ByRef initCodons As Integer(),
                                  ByRef codenTable As IReadOnlyDictionary(Of Integer, AminoAcid))

            stopCodons = (From codon In transl_table Where codon.Key.IsStopCodon Select codon.Key.TranslHashCode).ToArray
            initCodons = (From codon In transl_table Where codon.Key.IsInitCodon Select codon.Key.TranslHashCode).ToArray
            codenTable = (From codon As KeyValuePair(Of Codon, AminoAcid)
                          In transl_table
                          Where Not codon.Key.IsStopCodon
                          Select codon) _
                               .ToDictionary(Function(codon) codon.Key.TranslHashCode,
                                             Function(codon)
                                                 Return codon.Value
                                             End Function)
        End Sub

        <Extension>
        Friend Function doParseTable(tokens As String(), ByRef transl_table As Integer) As Dictionary(Of Codon, AminoAcid)
            Dim index As String = tokens(Scan0)
            Dim hashTable As Dictionary(Of Codon, AminoAcid)

            tokens = (From token As String
                      In tokens.Skip(1)
                      Let ss As String = Strings.Trim(token)
                      Where Not String.IsNullOrEmpty(ss)
                      Select ss).ToArray
            hashTable = parseHashValues(tokens)
            transl_table = Scripting.CTypeDynamic(Of Integer)(index.Split("="c).Last)

            Return hashTable
        End Function

        Private Function parseHashValues(tokens As String()) As Dictionary(Of Codon, AminoAcid)
            Dim matrix As String()() = tokens _
                .Select(Function(token As String)
                            Return Regex.Split(token, "\s+")
                        End Function) _
                .ToArray
            Dim codons = matrix.Select(Function(line) splitMatrix(line)).Unlist
            Dim LQuery = (From Token As String() In codons Select code = New Codon(Token), AA = Token(1).First).ToArray
            Dim hashTable = LQuery.ToDictionary(Function(obj) obj.code,
                                                Function(obj)
                                                    Return SequenceModel.Polypeptides.ToEnums(obj.AA)
                                                End Function)
            Return hashTable
        End Function

        Private Function splitMatrix(line As String()) As String()()
            Dim withInits As Boolean = (line.Length = 13)

            If Not withInits Then
                Return line.Split(3)
            End If

            Dim source As New List(Of String)(line)
            Dim tokens As New List(Of String())
            Dim list As New List(Of String)

            Do While source.Count > 0
                Call list.Add(source.Take(3).ToArray)
                Call source.RemoveRange(0, 3)

                If source.Count > 0 Then
                    ' 起始密码子
                    If String.Equals(source(Scan0), "i") Then
                        Call list.Add("i")
                        Call source.RemoveAt(Scan0)
                    End If
                End If

                Call tokens.Add(list.ToArray)
                Call list.Clear()
            Loop

            Return tokens.ToArray
        End Function
    End Module
End Namespace

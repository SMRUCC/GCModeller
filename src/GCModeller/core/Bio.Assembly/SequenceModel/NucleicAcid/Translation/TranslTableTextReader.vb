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
        Friend Sub doInitProfiles(transl_table As Dictionary(Of Codon, AminoAcid), ByRef StopCodons As Integer(), ByRef InitCodons As Integer(), ByRef CodenTable As IReadOnlyDictionary(Of Integer, AminoAcid))
            StopCodons = (From codon In transl_table Where codon.Key.IsStopCodon Select codon.Key.TranslHash).ToArray
            InitCodons = (From codon In transl_table Where codon.Key.IsInitCodon Select codon.Key.TranslHash).ToArray
            CodenTable = (From codon As KeyValuePair(Of Codon, AminoAcid)
                          In transl_table
                          Where Not codon.Key.IsStopCodon
                          Select codon) _
                               .ToDictionary(Function(codon) codon.Key.TranslHash,
                                             Function(codon)
                                                 Return codon.Value
                                             End Function)
        End Sub

        <Extension>
        Friend Function doParseTable(Tokens As String(), ByRef transl_table As Integer) As Dictionary(Of Codon, AminoAcid)
            Dim index As String = Tokens(Scan0)
            Tokens = (From token As String
                      In Tokens.Skip(1)
                      Let ss As String = Trim(token)
                      Where Not String.IsNullOrEmpty(ss)
                      Select ss).ToArray
            Dim dict As Dictionary(Of Codon, AminoAcid) = parseHashValues(Tokens)
            transl_table = Scripting.CTypeDynamic(Of Integer)(index.Split("="c).Last)

            Return dict
        End Function

        Private Function parseHashValues(tokens As String()) As Dictionary(Of Codon, AminoAcid)
            Dim MAT = tokens.Select(Function(token As String) Regex.Split(token, "\s+")).ToArray
            Dim Codons = MAT.Select(Function(line) splitMatrix(line)).Unlist
            Dim LQuery = (From Token As String() In Codons Select code = New Codon(Token), AA = Token(1).First).ToArray
            Dim hash = LQuery.ToDictionary(Function(obj) obj.code,
                                           Function(obj)
                                               Return SequenceModel.Polypeptides.ToEnums(obj.AA)
                                           End Function)
            Return hash
        End Function

        Private Function splitMatrix(line As String()) As String()()
            Dim withInits As Boolean = line.Length = 13

            If withInits Then

                Dim source As New List(Of String)(line)
                Dim Tokens As New List(Of String())
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

                    Call Tokens.Add(list.ToArray)
                    Call list.Clear()
                Loop

                Return Tokens.ToArray
            Else
                Dim Tokens = line.Split(3)
                Return Tokens
            End If
        End Function
    End Module
End Namespace
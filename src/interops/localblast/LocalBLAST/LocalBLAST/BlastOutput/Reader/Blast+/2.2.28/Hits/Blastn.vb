Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Class BlastnHit : Inherits SubjectHit

        Dim _strand As String

        Public Property Strand As String
            Get
                Return _strand
            End Get
            Set(value As String)
                _strand = value

                If String.IsNullOrEmpty(value) Then
                    Me._queryStrand = Strands.Unknown
                    Me._referenceStrand = Strands.Unknown
                    Return
                End If

                Dim Tokens As String() = value.Split("/"c)
                Me._queryStrand = GetStrand(Tokens(Scan0))
                Me._referenceStrand = GetStrand(Tokens(1))
            End Set
        End Property

        Dim _queryStrand As Strands
        Dim _referenceStrand As Strands

        Public Overrides ReadOnly Property QueryLocation As Location
            Get
                Return New SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation(MyBase.QueryLocation, _queryStrand)
            End Get
        End Property

        Public Overrides ReadOnly Property SubjectLocation As Location
            Get
                Return New SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation(MyBase.SubjectLocation, _referenceStrand)
            End Get
        End Property

        Const scoreFLAG As String = " Score ="

        Public Shared Function hitParser(str As String) As BlastnHit()
            If InStr(str, NO_HITS_FOUND) Then
                Return New BlastnHit() {}
            End If

            Dim Tokens As String() = Regex.Split(str, "^>", RegexOptions.Multiline).Skip(1).ToArray
            Dim LQuery As BlastnHit() = (From s As String In Tokens Select BlastnTryParse(s)).ToArray.MatrixToVector
            Return LQuery
        End Function

        Private Shared Function BlastnTryParse(Text As String) As BlastnHit()
            Dim Tokens As String() = Regex.Split(Text, "^\s*Score\s*=", RegexOptions.Multiline)
            Dim Name As String = Strings.Split(Tokens.First, "Length=").First.TrimA
            Dim hitLen As Double = Text.Match("Length=\d+").RegexParseDouble
            Dim LQuery = (From s As String
                          In Tokens.Skip(1)
                          Select BlastnHit.__blastnTryParse(scoreFLAG & s, Name, hitLen)).ToArray
            Return LQuery
        End Function

        Private Shared Function __blastnTryParse(str As String, Name As String, len As Double) As BlastnHit
            Dim blastnHit As BlastnHit = New BlastnHit With {
                .Score = LocalBLAST.BLASTOutput.ComponentModel.Score.TryParse(Of LocalBLAST.BLASTOutput.ComponentModel.Score)(str),
                .Name = Name,
                .Length = len
            }

            Dim strHsp As String() = (From Match As Match
                                      In Regex.Matches(str, PAIRWISE, RegexOptions.Singleline + RegexOptions.IgnoreCase)
                                      Select Match.Value).ToArray
            blastnHit.Hsp = ParseHitSegments(strHsp)
            blastnHit.Strand = Regex.Match(str, "^\s*Strand=.+?$", RegexOptions.Multiline).Value.Replace("Strand=", "").Trim

            Return blastnHit
        End Function
    End Class

End Namespace
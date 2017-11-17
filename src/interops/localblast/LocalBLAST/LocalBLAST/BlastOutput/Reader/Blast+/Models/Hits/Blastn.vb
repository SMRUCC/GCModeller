#Region "Microsoft.VisualBasic::06e70fa7e21fdf8bec20690ad6ee1cfd, ..\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.2.28\Hits\Blastn.vb"

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

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel

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
                    Me._referStrand = Strands.Unknown
                    Return
                End If

                Dim Tokens As String() = value.Split("/"c)
                Me._queryStrand = GetStrand(Tokens(Scan0))
                Me._referStrand = GetStrand(Tokens(1))
            End Set
        End Property

        Dim _queryStrand As Strands
        ''' <summary>
        ''' 参考链的方向
        ''' </summary>
        Dim _referStrand As Strands

        Public Overrides ReadOnly Property QueryLocation As Location
            Get
                Return New NucleotideLocation(MyBase.QueryLocation, _queryStrand)
            End Get
        End Property

        Public Overrides ReadOnly Property SubjectLocation As Location
            Get
                Return New NucleotideLocation(MyBase.SubjectLocation, _referStrand)
            End Get
        End Property

        Const scoreFLAG As String = " Score ="

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        ''' <remarks>非并行化的，所以会保持原有的顺序</remarks>
        Public Shared Function hitParser(str As String) As BlastnHit()
            If InStr(str, NO_HITS_FOUND) Then
                Return New BlastnHit() {}
            End If

            Dim Tokens As String() = Regex _
                .Split(str, "^>", RegexOptions.Multiline) _
                .Skip(1) _
                .ToArray  ' 分段
            Dim LQuery As BlastnHit() = Tokens _
                .Select(AddressOf BlastnTryParse) _
                .ToVector

            Return LQuery
        End Function

        Private Shared Function BlastnTryParse(Text As String) As BlastnHit()
            Dim Tokens As String() = Regex.Split(Text, "^\s*Score\s*=", RegexOptions.Multiline)
            Dim Name As String = Strings.Split(Tokens.First, "Length=").First.TrimNewLine
            Dim hitLen As Double = Text.Match("Length=\d+").RegexParseDouble
            Dim LQuery As BlastnHit() = LinqAPI.Exec(Of BlastnHit) <=
 _
                From s As String
                In Tokens.Skip(1)
                Select BlastnHit.__blastnTryParse(scoreFLAG & s, Name, hitLen)

            Return LQuery
        End Function

        Private Shared Function __blastnTryParse(str As String, Name As String, len As Double) As BlastnHit
            Dim blastnHit As New BlastnHit With {
                .Score = Score.TryParse(Of Score)(str),
                .Name = Name,
                .Length = len
            }

            Dim strHsp As String() = Regex.Matches(str, PAIRWISE, RegexICSng).ToArray
            blastnHit.Hsp = ParseHitSegments(strHsp)
            blastnHit.Strand = Regex.Match(str, "^\s*Strand=.+?$", RegexOptions.Multiline).Value.Replace("Strand=", "").Trim

            Return blastnHit
        End Function
    End Class
End Namespace

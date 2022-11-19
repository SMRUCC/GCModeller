#Region "Microsoft.VisualBasic::a69c39d7c47124a1a0291256f891d6d0, GCModeller\core\Bio.Assembly\Assembly\KEGG\Archives\SequenceDump.vb"

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

    '   Total Lines: 176
    '    Code Lines: 143
    ' Comment Lines: 6
    '   Blank Lines: 27
    '     File Size: 7.40 KB


    '     Class SequenceDump
    ' 
    '         Properties: CommonName, Description, LocusId, SpeciesId
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __altParser, __familyParser, __trimBraket, Create, KEGGFamily
    '                   TitleParser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Assembly.KEGG.Archives

    Public NotInheritable Class SequenceDump : Inherits FastaSeq

        Public Property SpeciesId As String
        Public Property LocusId As String
        Public Property CommonName As String
        Public Property Description As String

        Protected Friend Sub New()
        End Sub

        Public Shared Function Create(Fasta As FASTA.FastaSeq) As KEGG.Archives.SequenceDump
            Dim Description As SequenceDump = New SequenceDump
            Dim KEGGDescription As String = Fasta.Headers.First

            Dim strTmp As String = KEGGDescription.Split.First
            Dim TokensTmp As String() = strTmp.Split(CChar(":"))
            Dim p As Integer
            Description.SpeciesId = TokensTmp.First
            Description.LocusId = TokensTmp.Last
            p = Len(String.Format("{0}:{1} ", Description.SpeciesId, Description.LocusId)) + 1
            Description.CommonName = Mid(KEGGDescription, p).Split(CChar(";")).First
            Description.Description = Mid(KEGGDescription, 2 + p + Len(Description.CommonName)).Trim
            Description.SequenceData = Fasta.SequenceData
            Description.Headers = Fasta.Headers

            Return Description
        End Function

        Public Overrides Function ToString() As String
            Return $"{SpeciesId}:{LocusId} {CommonName}; {Description}"
        End Function

        ''' <summary>
        ''' 需要兼容KEGG和Regprecise数据库  _(:зゝ∠)_
        ''' </summary>
        ''' <param name="title"></param>
        ''' <returns></returns>
        Public Shared Function TitleParser(title As String) As KeyValuePair(Of String, String)
            Dim locusId As String = title.Split.First.Split("|"c).First
            Dim def As String = Mid(title, Len(locusId) + 1)
            def = def.Split("|"c).Last
            Return New KeyValuePair(Of String, String)(locusId, def)
        End Function

        Private Shared Function __trimBraket(s As String) As String
            If s.First = "("c Then
                If s.Last = ")"c Then
                    Return s
                Else
                    Return Mid(s, 2)
                End If
            Else
                Return s
            End If
        End Function

        Private Shared Function __altParser(def As String, [default] As String) As String
            Dim fam As String() = (From m As Match
                                   In Regex.Matches(def, "(repressor|activator|regulator)\s\S+", RegexOptions.IgnoreCase)
                                   Let s As String = m.Value.Split.Last
                                   Where Not String.Equals(s, "of", StringComparison.OrdinalIgnoreCase) AndAlso
                                       Not String.Equals(s, "(A)", StringComparison.OrdinalIgnoreCase) AndAlso
                                       Not String.Equals(s, "(N)", StringComparison.OrdinalIgnoreCase) AndAlso
                                       Not (s.First = "("c AndAlso s.Last <> ")"c) AndAlso
                                       Not Regex.Match(s, "EC[:]\d+\.").Success AndAlso
                                       Not String.Equals(s, "(partial)", StringComparison.OrdinalIgnoreCase) AndAlso
                                       Not String.Equals(s, "protein", StringComparison.OrdinalIgnoreCase)
                                   Select s).ToArray

            If Not fam.IsNullOrEmpty Then
                For Each s As String In fam
                    If Char.IsUpper(s.First) Then
                        Return s
                    End If
                Next

                Return fam.First
            End If

            Dim Tokens As String() = def _
                .Split _
                .Select(Function(s) Regex.Replace(s, "[;,]", "").Trim) _
                .ToArray
            def = [default]
            Dim LQuery = (From t As String In Tokens Where String.Equals(def, t, StringComparison.OrdinalIgnoreCase) Select t).FirstOrDefault
            If String.IsNullOrEmpty(LQuery) Then
                ' 实在找不到了
                def = "*" & def
            End If
            Return def
        End Function

        Public Shared Function KEGGFamily(title As String, Optional [default] As String = "") As String
            If title Is Nothing Then
                Return [default]
            End If

            Dim def As String = Regex.Replace(title, "[;,]", " ") '.Replace("-", "/")
            def = Regex.Replace(def, "hydrolase", "", RegexOptions.IgnoreCase)

            Dim fam As String() = (From m As Match In Regex.Matches(def, "\S+\s+family", RegexOptions.IgnoreCase) Select __trimBraket(m.Value.Split.First)).ToArray
            def = If(fam.IsNullOrEmpty, __altParser(def, [default]), __familyParser(fam, def, [default]))

            If InStr(def, "-") > 0 AndAlso Not String.Equals(def.Split("-"c).Last, "like", StringComparison.OrdinalIgnoreCase) Then
                def = def.Replace("-", "/")
            End If
            If String.Equals(def, "/") Then
                def = [default]
            End If

            For Each s As String In ___q
                If String.Equals(s, def) Then
                    def = [default]
                    Return def
                End If
            Next

            If String.IsNullOrWhiteSpace(def) OrElse def.Last = "."c Then
                def = [default]
            End If

            Return def
        End Function

        Shared ReadOnly ___q As String() = {"and",
"for",
"involved",
"Like/protein",
"phosphoribosyltransferase",
"transcription",
"with",
"cone"}

        Private Shared Function __familyParser(fam As String(), def As String, [default] As String) As String
            Dim LQuery As String = (From t As String In fam
                                    Where Not t.First = "("c AndAlso
                                        Not String.Equals(t, "helix", StringComparison.OrdinalIgnoreCase) AndAlso
                                        Not String.Equals(t, "regulator", StringComparison.OrdinalIgnoreCase) AndAlso
                                        Not String.Equals(t, "repressor", StringComparison.OrdinalIgnoreCase)
                                    Select t).FirstOrDefault
            If String.IsNullOrEmpty(LQuery) Then
                def = fam.First
                If String.Equals(def, "helix", StringComparison.OrdinalIgnoreCase) Then
                    def = "Winged Helix"
                End If
                If String.Equals(def, "regulator", StringComparison.OrdinalIgnoreCase) OrElse
                    String.Equals(def, "repressor", StringComparison.OrdinalIgnoreCase) Then
                    def = [default]
                End If
            Else
                def = LQuery
            End If

            If String.IsNullOrEmpty(def) Then
                Return def
            End If

            If def.First = "("c Then
                def = Mid(def, 2)
            End If

            If String.Equals(def, "70") OrElse String.Equals(def, "54") OrElse String.Equals(def, "38") Then
                def = "Sigma" & def
            End If

            Return def
        End Function
    End Class
End Namespace

Imports System.Text

Namespace SequenceModel

    Public Module BioSequenceValidator

        ' DNA：4 种碱基 + IUPAC 简并碱基
        Private ReadOnly DnaChars As New HashSet(Of Char)("ACGTRYSWKMBDHVN".ToCharArray())

        ' RNA：以 U 取代 T，简并碱基同上
        Private ReadOnly RnaChars As New HashSet(Of Char)("ACGURYSWKMBDHVN".ToCharArray())

        ' 蛋白质：20 种标准氨基酸单字母码
        '         + B(Asx/Asn-Asp)、Z(Glx/Gln-Glu)、X(未知)、U(硒代半胱氨酸)、O(吡咯赖氨酸)
        Private ReadOnly ProteinChars As New HashSet(Of Char)("ACDEFGHIKLMNPQRSTVWYBXZUO".ToCharArray())

        ''' <summary>
        ''' 综合判断：返回字符串所属的序列类型。
        ''' 匹配顺序为 DNA → RNA → 蛋白质（核苷酸字母是氨基酸字母的子集，
        ''' 纯核苷酸序列优先判为核酸，避免被误判成蛋白质）。
        ''' </summary>
        Public Function IdentifySequence(sequence As String) As SeqTypes
            Dim s As String = Clean(sequence)
            If s Is Nothing Then Return SeqTypes.Unknown

            If MatchesAll(s, DnaChars) Then Return SeqTypes.DNA
            If MatchesAll(s, RnaChars) Then Return SeqTypes.RNA
            If MatchesAll(s, ProteinChars) Then Return SeqTypes.Protein

            Return SeqTypes.Unknown
        End Function

        ''' <summary>是否为合法的 DNA 序列</summary>
        Public Function IsValidDna(sequence As String) As Boolean
            Dim s As String = Clean(sequence)
            Return s IsNot Nothing AndAlso MatchesAll(s, DnaChars)
        End Function

        ''' <summary>是否为合法的 RNA 序列</summary>
        Public Function IsValidRna(sequence As String) As Boolean
            Dim s As String = Clean(sequence)
            Return s IsNot Nothing AndAlso MatchesAll(s, RnaChars)
        End Function

        ''' <summary>是否为合法的蛋白质序列</summary>
        Public Function IsValidProtein(sequence As String) As Boolean
            Dim s As String = Clean(sequence)
            Return s IsNot Nothing AndAlso MatchesAll(s, ProteinChars)
        End Function

        ''' <summary>只要属于三种序列之一即返回 True</summary>
        Public Function IsValidBioSequence(sequence As String) As Boolean
            Return IdentifySequence(sequence) <> SeqTypes.Unknown
        End Function

        ' 去掉所有空白字符并转为大写；空串返回 Nothing
        Private Function Clean(sequence As String) As String
            If String.IsNullOrWhiteSpace(sequence) Then Return Nothing

            Dim sb As New StringBuilder(sequence.Length)
            For Each c As Char In sequence
                If Not Char.IsWhiteSpace(c) Then sb.Append(Char.ToUpperInvariant(c))
            Next

            If sb.Length = 0 Then Return Nothing
            Return sb.ToString()
        End Function

        ' 逐字符检查是否全部在合法字符集内
        Private Function MatchesAll(s As String, validChars As HashSet(Of Char)) As Boolean
            For Each c As Char In s
                If Not validChars.Contains(c) Then Return False
            Next
            Return True
        End Function

    End Module
End Namespace
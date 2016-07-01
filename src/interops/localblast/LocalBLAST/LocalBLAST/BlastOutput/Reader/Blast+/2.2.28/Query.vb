Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Extensions
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel

Namespace LocalBLAST.BLASTOutput.BlastPlus

    Public Class Query

        <XmlAttribute> Public Property QueryName As String
        ''' <summary>
        ''' Protein length of <see cref="QueryName">the target query protein</see>.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property QueryLength As Integer
        <XmlAttribute> Public Property EffectiveSearchSpace As Long
        <XmlElement> Public Property p As Parameter
        <XmlElement> Public Property Gapped As Parameter

        <XmlArray> Public Property SubjectHits As SubjectHit()

        Public Overrides Function ToString() As String
            Return QueryName
        End Function

        Public Function GetBestHit(Optional coverage As Double = 0.5, Optional identities As Double = 0.15) As SubjectHit
            If SubjectHits.IsNullOrEmpty Then Return Nothing

            Dim LQuery = (From hit As SubjectHit
                          In SubjectHits
                          Where hit.LengthQuery / QueryLength >= coverage AndAlso
                                  hit.Score.Identities.Value >= identities
                          Select hit
                          Order By hit.Score.RawScore Descending).FirstOrDefault
            Return LQuery
        End Function

        ''' <summary>
        ''' 导出所有符合条件的最佳匹配
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetBesthits(coverage As Double, identities As Double) As SubjectHit()
            If SubjectHits.IsNullOrEmpty Then Return Nothing
            Dim LQuery = (From hit As SubjectHit
                          In SubjectHits
                          Where hit.LengthQuery / QueryLength >= coverage AndAlso
                              hit.Score.Identities.Value >= identities
                          Select hit
                          Order By hit.Score.RawScore Descending).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' <see cref="ReaderTypes.BLASTP"/>
        ''' </summary>
        ''' <param name="strText"></param>
        ''' <returns></returns>
        Public Shared Function TryParse(strText As String) As Query
            Dim Query As Query = New Query With {
                .QueryName = GetQueryName(strText),
                .QueryLength = GetQueryLength(strText)
            }
            Query.SubjectHits = SubjectHit.GetItems(strText)

            Dim TEMP = Parameter.TryParseBlastPlusParameters(strText)
            Query.p = TEMP(0)
            Query.Gapped = TEMP(1)
            Query.EffectiveSearchSpace = Val(Regex.Match(strText, "Effective search space used: \d+", RegexOptions.Singleline).Value.Match("\d+"))

            Return Query
        End Function

        ''' <summary>
        ''' <see cref="ReaderTypes.BLASTN"/>
        ''' </summary>
        ''' <param name="strText"></param>
        ''' <returns></returns>
        Public Shared Function BlastnOutputParser(strText As String) As Query
            Dim Query As Query = New Query With {
                .QueryName = GetQueryName(strText),
                .QueryLength = GetQueryLength(strText)
            }
            Dim hitsBuffer As BlastnHit() = BlastnHit.hitParser(strText)

            Query.SubjectHits = Constrain(Of SubjectHit, BlastnHit)(hitsBuffer)

            Dim TEMP = Parameter.TryParseBlastPlusBlastn(strText)
            Query.p = TEMP(0)
            Query.Gapped = TEMP(1)
            Query.EffectiveSearchSpace = Val(Regex.Match(strText, "Effective search space used: \d+", RegexOptions.Singleline).Value.Match("\d+"))

            Return Query
        End Function

        Private Shared Function GetQueryLength(text As String) As Integer
            Return Val(Mid(Regex.Match(text, "Length=\d+").Value, 8))
        End Function

        Private Shared Function GetQueryName(text As String) As String
            Dim QueryName As String = Mid(Regex.Match(text, "Query= .+?Length", RegexOptions.Singleline).Value, 8).Trim
            If Len(QueryName) > 8 Then
                QueryName = Mid(QueryName, 1, Len(QueryName) - 8).TrimA
            Else
                Call $"This query name value is not valid!{vbCrLf}""{QueryName}""".__DEBUG_ECHO
                QueryName = Regex.Replace(QueryName, "Length$", "", RegexOptions.IgnoreCase)
            End If

            Return QueryName
        End Function
    End Class
End Namespace
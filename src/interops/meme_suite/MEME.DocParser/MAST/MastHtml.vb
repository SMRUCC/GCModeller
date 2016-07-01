Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

#Const DEBUG = 0

Namespace DocumentFormat.MAST.HTML

    Public Class MASTHtml
        <XmlElement> Public Property MatchedSites As MatchedSite()
    End Class

    Public Class MatchedSite : Implements IAbstractFastaToken

        <XmlAttribute> Public Property SequenceId As String Implements FASTA.IAbstractFastaToken.Title
        <XmlAttribute> Public Property Strand As String
        <XmlAttribute> Public Property EValue As Double
        <XmlAttribute> Public Property MotifId As Integer
        <XmlAttribute> Public Property PValue As Double
        <XmlAttribute> Public Property Starts As Integer
        <XmlAttribute> Public Property Ends As Integer

        Private Property Attributes As String() Implements FASTA.IAbstractFastaToken.Attributes
            Get
                Return {
                    $"{NameOf(Strand)}:={Strand}",
                    $"{NameOf(EValue)}:={EValue}",
                    $"{NameOf(MotifId)}:={MotifId}",
                    $"{NameOf(PValue)}:={PValue}",
                    $"{NameOf(Starts)}:={Starts}",
                    $"{NameOf(Ends)}:={Ends}"
                }
            End Get
            Set(value As String())
                ' DO NOTHING
            End Set
        End Property
        Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

        Public Overrides Function ToString() As String
            Return $"{SequenceId} <--> Motif_{MotifId}:  E-value:={EValue}, #{Starts},{Ends}"
        End Function

        Public Shared Function TryParse(strData As String) As MatchedSite()
            Dim Tokens As String() = (From match As Match In Regex.Matches(strData, "<td>[^<]+</td>", RegexOptions.Singleline) Select match.Value).ToArray
            Dim SequenceId As String = Tokens(0).GetValue, EValue = Tokens(1).GetValue
            Dim Matches = (From m As Match In Regex.Matches(strData, "title=""[^""]+?""") Select m.Value).ToArray

            If Matches.Length < 2 Then
                Return Nothing
            End If

            Dim LQuery = (From strValue As String In Matches.Skip(1)
                          Let MotifId = Val(Regex.Match(strValue, "Motif \d+").Value.Split.Last)
                          Let PValue = Val(Mid(strValue, InStr(strValue, "p-value: ") + 9))
                          Let starts As String = Regex.Match(strValue, "starts:\s+\d+").Value.Split.Last
                          Let ends As String = Regex.Match(strValue, "ends:\s+\d+").Value.Split.Last
                          Select New MatchedSite With {
                              .PValue = PValue,
                              .MotifId = MotifId,
                              .SequenceId = SequenceId,
                              .EValue = EValue,
                              .Starts = CInt(starts),
                              .Ends = CInt(ends)}).ToArray
            Return LQuery
        End Function
    End Class

End Namespace
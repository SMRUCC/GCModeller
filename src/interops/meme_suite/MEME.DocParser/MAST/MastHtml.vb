#Region "Microsoft.VisualBasic::018db23cb7092fdcff9bde2588c20e4c, ..\interops\meme_suite\MEME.DocParser\MAST\MastHtml.vb"

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
Imports System.Xml.Serialization
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

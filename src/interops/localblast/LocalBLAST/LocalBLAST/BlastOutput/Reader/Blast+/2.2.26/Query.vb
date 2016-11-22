#Region "Microsoft.VisualBasic::08b0feac97b2abfa51ba7a806099cead, ..\interops\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.2.26\Query.vb"

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

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace LocalBLAST.BLASTOutput.BlastPlus.v226

    Public Class Query

        <XmlElement> Public Query, Subject As NamedValue(Of Integer)
        <XmlAttribute> Public EffectiveSearchSpace As Long

        <XmlElement> Public p, Gapped As LocalBLAST.BLASTOutput.ComponentModel.Parameter
        <XmlArray> Public Hits As Segment()

        Public Overrides Function ToString() As String
            If Hits Is Nothing OrElse Hits.Count = 0 Then
                Return Legacy.Query.HITS_NOT_FOUND
            Else
                Return String.Format("{0} <--> {1} ({2} hit segments.)", Query.ToString, Subject.ToString, Hits.Count)
            End If
        End Function

        Public Const QUERY_HEADER As String = "Query= .+?Length[=]\d+"
        Public Const SUBJECT_HEADER As String = "Subject= .+?Length[=]\d+"

        Public Shared Function TryParse(Text As String) As Query
            Dim Query As Query = New Query With {
                .Query = ParseHead(Text, QUERY_HEADER, "Query= "),
                .Subject = ParseHead(Text, SUBJECT_HEADER, "Subject= ")
            }
            Dim TEMP = LocalBLAST.BLASTOutput.ComponentModel.Parameter.TryParseBlastPlusParameters(Text)
            Query.p = TEMP(0)
            Query.Gapped = TEMP(1)
            Query.Hits = Segment.TryParse(Text)

            Return Query
        End Function

        Private Shared Function ParseHead(Text As String, Regx As String, Replaced As String) As NamedValue(Of Integer)
            Dim Head As String = Regex.Match(Text, Regx, RegexOptions.Singleline).Value
            Dim Tokens = Head.Split(Chr(10))
            Dim NameBuilder As StringBuilder = New StringBuilder(128)
            Dim Length% = Tokens.Last.Match("\d+").RegexParseDouble
            For i As Integer = 0 To Tokens.Count - 2
                Call NameBuilder.Append(Tokens(i))
            Next
            Call NameBuilder.Replace(Replaced, "")

            Return New NamedValue(Of Integer) With {
                .Name = Trim(NameBuilder.ToString),
                .Value = Length
            }
        End Function

    End Class
End Namespace

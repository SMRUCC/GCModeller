#Region "Microsoft.VisualBasic::d631b2365fb7533fffecf4c8d299c49d, GCModeller\core\Bio.Annotation\GFF\Document\FeatureParser.vb"

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

    '   Total Lines: 125
    '    Code Lines: 79
    ' Comment Lines: 33
    '   Blank Lines: 13
    '     File Size: 4.99 KB


    '     Module FeatureParser
    ' 
    '         Function: attributeTokens, CreateObject, CreateObjectGff3, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.Loci

Namespace Assembly.NCBI.GenBank.TabularFormat.GFF

    Public Module FeatureParser

        ''' <summary>
        ''' 生成gff文件之中的一行的基因组特性位点的数据
        ''' </summary>
        ''' <returns></returns>
        Public Function ToString(x As Feature) As String
            Dim attrs As String() = x.attributes _
                .Select(Function(token)
                            Return $"{token.Key}={token.Value.CLIToken}"
                        End Function) _
                .ToArray
            Dim tokens As String() = {
                x.seqname,
                x.source,
                x.feature,
                CStr(x.start), CStr(x.ends),
                x.score, x.strand.GetBriefCode, x.frame,
                attrs.JoinBy(";")
            }
            Dim line As String = String.Join(vbTab, tokens)
            Return line
        End Function

        ''' <summary>
        ''' ```
        ''' Fields are: &lt;seqname> &lt;source> &lt;feature> &lt;start> &lt;end> &lt;score> &lt;strand> &lt;frame> [attributes] [comments]
        ''' ```
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="version">gff1, gff2, gff3之间的差异是由于本属性值的列的读取方式的差异而产生的</param>
        ''' <returns></returns>
        Public Function CreateObject(data$, version%) As Feature
            Dim t As String() = data.Split(ASCII.TAB)
            Dim feature As New Feature
            Dim i As i32 = Scan0

            ' Fields are: <seqname> <source> <feature> <start> <end> <score> <strand> <frame> [attributes] [comments]

            With feature
                .seqname = t(++i)
                .source = t(++i)
                .feature = t(++i)
                .Left = CLng(Val(t(++i)))
                .Right = CLng(Val(t(++i)))
                .score = t(++i)
                .strand = GetStrand(t(++i))
                .frame = t(++i)
            End With

            '在这里开始读取可选的列数据
            Dim attrValue As String = If(t.Length > i, t(++i), "")

            If Not String.IsNullOrEmpty(attrValue) Then
                Select Case version
                    Case 1
                    Case 2
                    Case 3 : feature.attributes = CreateObjectGff3(attrValue)
                    Case Else
                        ' DO_NOTHING
                End Select
            End If

            Return feature
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' gi|66571684|gb|CP000050.1|	RefSeq	Coding gene	42	1370	.	+	.	name=dnaA;product="chromosome replication initiator DnaA"
        ''' </remarks>
        Private Function CreateObjectGff3(data As String) As Dictionary(Of String, String)
            Dim tokens As String() = attributeTokens(Line:=data)
            Dim LQuery = (From Token As String In tokens
                          Let p As Integer = InStr(Token, "=")
                          Let Name As String = Mid(Token, 1, p - 1),
                              Value As String = Mid(Token, p + 1)
                          Select Name, Value).ToArray
            Dim attrs = LQuery.ToDictionary(Function(obj) obj.Name.ToLower,   ' Key已经被转换为小写了
                                            Function(obj) If(Len(obj.Value) > 2 AndAlso
                                                            obj.Value.First = """"c AndAlso
                                                            obj.Value.Last = """"c, Mid(obj.Value, 2, Len(obj.Value) - 2), obj.Value))
            Return attrs
        End Function

        ''' <summary>
        ''' A regex expression string that use for split the line text.
        ''' </summary>
        ''' <remarks></remarks>
        Const SplitRegxExpression As String = "[" & vbTab & ";](?=(?:[^""]|""[^""]*"")*$)"

        ''' <summary>
        ''' Row parsing into column tokens
        ''' </summary>
        ''' <param name="Line"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function attributeTokens(Line As String) As String()
            If String.IsNullOrEmpty(Line) Then
                Return Nothing
            End If

            Dim Row = Regex.Split(Line, SplitRegxExpression)
            For i As Integer = 0 To Row.Length - 1
                If Not String.IsNullOrEmpty(Row(i)) Then
                    If Row(i).First = """"c AndAlso Row(i).Last = """"c Then
                        Row(i) = Mid(Row(i), 2, Len(Row(i)) - 2)
                    End If
                End If
            Next
            Return Row
        End Function
    End Module
End Namespace

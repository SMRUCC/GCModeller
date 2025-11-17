#Region "Microsoft.VisualBasic::e550815267d9d5a29300dff0a24514c5, meme_suite\MEME.DocParser\MEME\HtmlParser\LDM\SiteInfo.vb"

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

    '     Class SiteInfo
    ' 
    '         Properties: Ends, Name, Pvalue, Start
    ' 
    '         Function: ToString, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace DocumentFormat.MEME.HTML

    ''' <summary>
    ''' 向某一个目标Motif所做出贡献的序列对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SiteInfo
        ''' <summary>
        ''' Site name，该目标序列的Fasta文件的文件头
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Overridable Property Name As String
        <XmlAttribute> Public Overridable Property Pvalue As Double
        ''' <summary>
        ''' 在整条序列之中的起始位置
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Overridable Property Start As Long
        <XmlAttribute> Public Overridable Property Ends As Long

        Private Const FLAG_PVALUE As String = "p-value: "
        Private Const FLAG_STARTS As String = "starts: "
        Private Const FLAG_ENDS As String = "ends: "

        Public Overrides Function ToString() As String
            Return String.Format("{0}: p-value: {1}", Name, Pvalue)
        End Function

        Protected Friend Shared Function TryParse(strData As String) As SiteInfo
            Dim Site As SiteInfo = New SiteInfo
            Site.Name = Regex.Match(strData, "<td>.+?</td>").Value.GetValue

            Dim strTemp As String = Regex.Match(strData, "title="".+""").Value

            'p-value: 7.81e-09    starts: 67    ends: 94 
            Site.Pvalue = Val(Mid(strTemp, InStr(strTemp, FLAG_PVALUE) + Len(FLAG_PVALUE)))
            Site.Start = Val(Mid(strTemp, InStr(strTemp, FLAG_STARTS) + Len(FLAG_STARTS)))
            Site.Ends = Val(Mid(strTemp, InStr(strTemp, FLAG_ENDS) + Len(FLAG_ENDS)))

            Return Site
        End Function
    End Class

End Namespace

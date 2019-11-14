#Region "Microsoft.VisualBasic::b223f0a386571b1661886ec6ca00d515, core\Bio.Assembly\Assembly\NCBI\Database\CDD\BLAST\DomnArch.vb"

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

    '     Class DomnArch
    ' 
    '         Function: Load, Parse, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic

Namespace Assembly.NCBI.CDD.Blastp

    ''' <summary>
    ''' The blast+ program alignment output log file analysis module.
    ''' (blast+程序的序列比对日志输出文件的分析模块)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DomnArch

        <XmlAttribute> Public Protein As String
        <XmlElement> Public Domains As Domain()

        Public Const REGX_SPLIT As String = "^ Score =.+?$"

        ''' <summary>
        ''' Save the domain architecture model into a xml file.
        ''' (将本蛋白质的结构域模型保存为一个XML文件)
        ''' </summary>
        ''' <param name="XmlFile"></param>
        ''' <remarks></remarks>
        Public Function Save(XmlFile As String) As Boolean
            Return Me.GetXml.SaveTo(XmlFile)
        End Function

        ''' <summary>
        ''' Parsing a blastp output log file and get the protein domains.
        ''' (通过分析一个BLASTP分析得到的报告文件而获取一个蛋白质的结构数据)
        ''' </summary>
        ''' <param name="LogFile">The path of the target log file.(目标日志文件的文件路径)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Parse(LogFile As String) As DomnArch
            Dim Text As String = FileIO.FileSystem.ReadAllText(LogFile)
            Dim Tokens As List(Of String) = Regex.Split(Text, "^>", RegexOptions.Multiline).AsList
            Dim DomainList As New List(Of Domain)

            If Tokens.Count = 1 Then Return Nothing 'No domains

            Call Tokens.RemoveAt(Scan0)  'Remove the useless reference header 

            For Each Token As String In Tokens
                Dim DomainId As String = Token.Split(CChar("|")).First
                Dim lstScore As List(Of String) =
                    Regex.Split(Token, REGX_SPLIT, RegexOptions.Multiline).AsList

                Call lstScore.RemoveAt(Scan0)

                For Each s As String In lstScore
                    Dim LQuery As String() = (From Line As String
                                              In s.Split(CChar(vbCr)).Skip(2).ToArray
                                              Where InStr(Line, "Query") = 2
                                              Select Line).ToArray
                    Dim Domain As New Domain With {
                        .ID = DomainId
                    }

                    Domain.Left = Val(Regex.Match(LQuery.First, "\d+").Value)
                    Domain.Right = Val(LQuery.Last.Split.Last)

                    Call DomainList.Add(Domain)
                Next
            Next

            Dim LOrderQuery = From e In DomainList Select e Distinct Order By e.Left

            Return New DomnArch With {
                .Domains = LOrderQuery.ToArray,
                .Protein = LogFile.Replace("\", "/").Split(CChar("/")).Last.Split(CChar(".")).First
            }
        End Function

        ''' <summary>
        ''' Restore a domain architecture model from a xml file.
        ''' (从一个XML文件之中读取一个蛋白质结构域模型)
        ''' </summary>
        ''' <param name="XmlFile"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Load(XmlFile As String) As DomnArch
            Return XmlFile.LoadXml(Of DomnArch)
        End Function

        ''' <summary>
        ''' Read the out put log file from a specific file.
        ''' (从日个指定的文件之中读取输出日志，进而进行下一步的分析)
        ''' </summary>
        ''' <param name="File">The path of the log file.(目标日志文件的文件路径)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Widening Operator CType(File As String) As DomnArch
            Return DomnArch.Parse(File)
        End Operator
    End Class
End Namespace

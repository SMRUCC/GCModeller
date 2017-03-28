#Region "Microsoft.VisualBasic::ebf48fa57a1f56f52eb9dd88833db05f, ..\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\FeatureBriefs\PTT\DbAPI.vb"

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
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Module DbAPI

        Public ReadOnly Property PTTs As String() = {"*.asn", "*.faa", "*.ffn", "*.fna", "*.frn", "*.gbk", "*.gff", "*.ptt", "*.rnt", "*.rpt", "*.val"}

        ''' <summary>
        ''' 用于在加载PTT数据库的时候计算出有多少个基因组和质粒的数据在当前的数据库文件夹之中
        ''' </summary>
        ''' <param name="DIR"></param>
        ''' <returns></returns>
        Public Function GetDbEntries(DIR As String) As String()
            Dim gbs As IEnumerable(Of String) = ls - l - wildcards("*.gb", "*.gbk") <= DIR
            If gbs.IsNullOrEmpty Then
                gbs = ls - l - wildcards("*.PTT", "*.ptt") <= DIR
            End If
            Dim locus As String() = gbs.ToArray(Function(x) x.BaseName).Distinct.ToArray
            Return locus
        End Function

        ''' <summary>
        ''' 为了保持一些兼容性
        ''' </summary>
        ''' <param name="DIR"></param>
        ''' <param name="locus"></param>
        ''' <returns></returns>
        Public Function GetGb(DIR As String, locus As String) As String
            Dim gbs As IEnumerable(Of String) = FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.gb", "*.gbk")
            Dim locusId As String = locus & "(\.\d+)*"
            Dim found As String = LinqAPI.DefaultFirst(Of String) <=
                From x As String
                In gbs
                Let name As String = x.BaseName
                Where Regex.Match(name, locusId, RegexOptions.IgnoreCase).Success
                Select x

            Return found
        End Function

        Public Function GetEntryList(DIR As String) As PTTEntry()
            Dim locus As String() = GetDbEntries(DIR)
            Return locus.ToArray(Function(x) PTTEntry.NewEntry(DIR, x))
        End Function
    End Module

    ''' <summary>
    ''' 数据库文件的列表
    ''' </summary>
    Public Structure PTTEntry
        Dim asn As String
        Dim faa As String
        Dim ffn As String
        Dim fna As String
        Dim frn As String
        Dim gbk As String
        Dim gff As String
        Dim ptt As String
        Dim rnt As String
        Dim rpt As String
        Dim val As String

        Public ReadOnly Property DIR As String
            Get
                If gbk.FileExists Then Return gbk.ParentPath
                If ptt.FileExists Then Return ptt.ParentPath
                If fna.FileExists Then Return fna.ParentPath

                Throw New NullReferenceException
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function NewEntry(DIR As String, locus As String) As PTTEntry
            Return New PTTEntry With {
 _
                .asn = $"{DIR}/{locus}.asn",
                .faa = $"{DIR}/{locus}.faa",
                .ffn = $"{DIR}/{locus}.ffn",
                .fna = $"{DIR}/{locus}.fna",
                .frn = $"{DIR}/{locus}.frn",
                .gbk = DbAPI.GetGb(DIR, locus),
                .gff = $"{DIR}/{locus}.gff",
                .ptt = $"{DIR}/{locus}.ptt",
                .rnt = $"{DIR}/{locus}.rnt",
                .rpt = $"{DIR}/{locus}.rpt",
                .val = $"{DIR}/{locus}.val"
            }
        End Function
    End Structure
End Namespace

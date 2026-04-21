#Region "Microsoft.VisualBasic::ae5280739a8d72d96b0d3cf69299195c, core\Bio.Assembly\ComponentModel\Locus\LocusExtensions.vb"

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

'   Total Lines: 215
'    Code Lines: 130 (60.47%)
' Comment Lines: 60 (27.91%)
'    - Xml Docs: 93.33%
' 
'   Blank Lines: 25 (11.63%)
'     File Size: 8.91 KB


'     Module LocusExtensions
' 
'         Function: (+2 Overloads) Equals, GetRelationship, GetStrand, MergeJoins, NCBIstyle
'                   TryParse, tryParseInternal
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports r = System.Text.RegularExpressions.Regex
Imports std = System.Math

Namespace ComponentModel.Loci

    ''' <summary>
    ''' Methods for some nucleotide utility.
    ''' </summary>
    <HideModuleName>
    Public Module LocusExtensions

        ''' <summary>
        ''' 直接合并相邻的一个位点集合到一个新的更加长的位点
        ''' </summary>
        ''' <typeparam name="TLocation"></typeparam>
        ''' <param name="grouped">
        ''' 其实在这里是直接将最小的左端和最大的右端合并构成一个更大范围的location
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function MergeJoins(Of TLocation As Location)(grouped As IEnumerable(Of TLocation)) As TLocation
            Dim Ralign As TLocation() = LinqAPI.Exec(Of TLocation) <=
                From l As TLocation
                In grouped
                Select l
                Order By l.right Descending
            Dim Lalign As TLocation() = LinqAPI.Exec(Of TLocation) <=
                From l As TLocation
                In grouped
                Select l
                Order By l.left Ascending

            Dim clone As TLocation = grouped.First.Clone
            clone.left = Lalign.First.left
            clone.right = Ralign.Last.right
            Return clone
        End Function

        ''' <summary>
        ''' Gets the location relationship of two loci segments.
        ''' </summary>
        ''' <param name="lcl">
        ''' 在计算之前请先调用<see cref="Location.Normalization()"/>方法来修正
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' (判断获取两个位点片段之间的位置关系，请注意，这个函数只依靠左右位置来判断关系，
        ''' 假若对核酸链的方向有要求在调用本函数之前请确保二者在同一条链之上)
        ''' </remarks>
        '''
        <ExportAPI("Get.Relationship")>
        <Extension>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetRelationship(site As Location, lcl As Location) As SegmentRelationships
            Return GetNormalizedSiteRelationship(site.Normalization, lcl.Normalization)
        End Function

        Private Function GetNormalizedSiteRelationship(normSite As Location, normLcl As Location) As SegmentRelationships
            Dim sLeft = normSite.left
            Dim sRight = normSite.right
            Dim lLeft = normLcl.left
            Dim lRight = normLcl.right

            ' 1. 完全相等
            If lLeft = sLeft AndAlso lRight = sRight Then
                Return SegmentRelationships.Equals
            End If

            ' 2. 完全在上游 (包含边界相接的情况，例如 lcl=[1,10], site=[10,20] 视为上游)
            ' 注意：如果是0-based半开区间(BED格式)，请保持原来的 < ；如果是1-based闭区间，请使用 <=
            If lRight <= sLeft Then
                Return SegmentRelationships.UpStream
            End If

            ' 3. 完全在下游 (包含边界相接的情况)
            If lLeft >= sRight Then
                Return SegmentRelationships.DownStream
            End If

            ' 4. 严格包含在内 (不包含端点对齐的情况，端点对齐归入Overlap)
            If lLeft > sLeft AndAlso lRight < sRight Then
                Return SegmentRelationships.Inside
            End If

            ' 5. 严格覆盖 (不包含端点对齐的情况，端点对齐归入Overlap)
            If lLeft < sLeft AndAlso lRight > sRight Then
                Return SegmentRelationships.Cover
            End If

            ' 6. 上游重叠 / 左侧重叠 (lcl从site的左侧开始重叠)
            ' 此时必然有 lLeft <= sLeft (含左对齐) 且 lRight <= sRight 且存在交集
            If lLeft <= sLeft AndAlso lRight <= sRight Then
                Return SegmentRelationships.UpStreamOverlap
            End If

            ' 7. 下游重叠 / 右侧重叠 (lcl从site的右侧开始重叠)
            ' 此时必然有 lLeft >= sLeft (含右对齐) 且 lRight >= sRight 且存在交集
            If lLeft >= sLeft AndAlso lRight >= sRight Then
                Return SegmentRelationships.DownStreamOverlap
            End If

            ' 理论上只要输入是合法的区间(left <= right)，以上条件必定命中一个
            Return SegmentRelationships.Blank
        End Function

        ''' <summary>
        ''' Gets the location relationship of two loci segments.
        ''' </summary>
        ''' <param name="lcl">
        ''' 在计算之前请先调用<see cref="Location.Normalization()"/>方法来修正
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' (判断获取两个位点片段之间的位置关系，请注意，这个函数只依靠左右位置来判断关系，
        ''' 假若对核酸链的方向有要求在调用本函数之前请确保二者在同一条链之上)
        ''' </remarks>
        '''
        <ExportAPI("Get.Relationship")>
        <Extension>
        Public Function GetRelationship(site As NucleotideLocation, lcl As NucleotideLocation) As SegmentRelationships
            Dim normSite = site.Normalization
            Dim normLcl = lcl.Normalization

            If site.Strand <> lcl.Strand Then
                If normLcl.left > normSite.left AndAlso normLcl.right < normSite.right Then
                    Return SegmentRelationships.InnerAntiSense
                End If
            End If

            Return GetNormalizedSiteRelationship(DirectCast(normSite, Location), DirectCast(normLcl, Location))
        End Function

        ''' <summary>
        ''' Convert the string value type nucleotide strand information description data 
        ''' into a <see cref="Strands"/> enumerate data.
        ''' </summary>
        ''' <param name="str">从文本文件之中所读取出来关于链方向的字符串描述数据</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Get.Strands"),
            Extension>
        Public Function GetStrand(str As String) As Strands
            If String.IsNullOrEmpty(str) Then
                Return Strands.Unknown
            End If

            If str.First = "("c AndAlso str.Last = ")"c Then
                str = Mid(str, 2, Len(str) - 2)
            End If

            Select Case str.ToLower
                Case "+", "forward", "plus", "#forward", "direct", "#+", "1", "f"
                    Return Strands.Forward
                Case "-", "reverse", "minus", "complement", "#reverse", "#-", "-1", "r"
                    Return Strands.Reverse
                Case Else
                    Return Strands.Unknown
            End Select
        End Function

        ''' <summary>
        ''' Try parse NCBI sequence dump location/<see cref="NucleotideLocation.ToString()"/> dump location.
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Loci.Parser")>
        Public Function TryParse(loci As String) As NucleotideLocation
            If loci.StringEmpty Then
                Return Nothing
            ElseIf InStr(loci, " ==> ") > 0 OrElse InStr(loci, "~") > 0 Then
                Return tryParseInternal(loci)
            End If

            Const complement$ = "complement\([^)]+\)"

            Dim isComplement As Boolean = r.Match(loci, complement, RegexOptions.IgnoreCase).Success
            Dim s As Strands = Strands.Forward Or Strands.Reverse.When(isComplement)
            Dim pos%() = LinqAPI.Exec(Of Integer) _
                                                  _
                () <= From match As Match
                      In Regex.Matches(loci, "\d+")
                      Let n As Integer = CInt(Val(match.Value))
                      Select n
                      Order By n Ascending

            Dim nuclLoci As New NucleotideLocation With {
                .left = pos(0),
                .right = pos(1),
                .Strand = s,
                .tagStr = loci
            }

            Return nuclLoci
        End Function

        <Extension>
        Public Function NCBIstyle(loci As NucleotideLocation) As String
            Dim tag$ = $"{loci.left}..{loci.right}"

            If loci.Strand = Strands.Reverse Then
                tag = $"complement({tag})"
            End If

            Return tag
        End Function

        ''' <summary>
        ''' ```
        ''' 388739 ==> 389772 #Forward
        ''' 388739~389772#Forward
        ''' ```
        ''' </summary>
        ''' <param name="input"></param>
        ''' <returns></returns>
        Private Function tryParseInternal(input As String) As NucleotideLocation
            Dim t As String() = input.Split(" "c, "#"c, "~"c).Where(Function(si) Not si.StringEmpty).ToArray
            Dim left As Integer = CInt(Val(t(0)))
            Dim right As Integer = CInt(Val(t(2)))
            Dim strand As Strands = GetStrand(t(3))

            Return New NucleotideLocation(left, right, strand).Normalization
        End Function

        ''' <summary>
        ''' 这个函数在<see cref="Equals"/>函数的基础之上还添加了对链方向的判断
        ''' </summary>
        ''' <param name="site"></param>
        ''' <param name="loci"></param>
        ''' <param name="allowedOffset"></param>
        ''' <returns></returns>
        <ExportAPI("Loci.Equals")>
        Public Function Equals(site As NucleotideLocation, loci As NucleotideLocation, Optional allowedOffset% = 10) As Boolean
            If loci.Strand <> site.Strand Then
                ' 链的方向不一样，不能相等
                Return False
            Else
                Return Equals(DirectCast(site, Location), DirectCast(loci, Location), allowedOffset)
            End If
        End Function

        ''' <summary>
        ''' One site is reference to the same location as another loci site it does in a given offset range?
        ''' </summary>
        ''' <param name="site">A loci site</param>
        ''' <param name="loci">Another loci site</param>
        ''' <param name="allowedOffset"></param>
        ''' <returns></returns>
        <ExportAPI("Loci.Equals")>
        Public Function Equals(site As Location, loci As Location, Optional allowedOffset As Integer = 10) As Boolean
            Dim loci1 As Integer() = {site.left, site.right}
            Dim loci2 As Integer() = {loci.left, loci.right}

            If allowedOffset = 0 Then
                Return loci1.Min = loci2.Min AndAlso loci1.Max = loci2.Max
            Else
                Return std.Abs(loci1.Min - loci2.Min) <= allowedOffset AndAlso
                       std.Abs(loci1.Max - loci2.Max) <= allowedOffset
            End If
        End Function
    End Module
End Namespace

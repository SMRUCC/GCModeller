#Region "Microsoft.VisualBasic::3503e92f460f1908613cfaa634dba434, GCModeller\core\Bio.Assembly\ComponentModel\Locus\LociSites\TagSite.vb"

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

    '   Total Lines: 86
    '    Code Lines: 52
    ' Comment Lines: 19
    '   Blank Lines: 15
    '     File Size: 3.35 KB


    '     Interface ITagSite
    ' 
    '         Properties: Distance, tag
    ' 
    '     Structure TagSite
    ' 
    '         Properties: contig, Distance, tag
    ' 
    '         Function: ToString
    ' 
    '     Module TagSiteExtensions
    ' 
    '         Function: __innerGroup, Groups
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports stdNum = System.Math

Namespace ComponentModel.Loci.Abstract

    ''' <summary>
    ''' This loci site have a tag
    ''' </summary>
    Public Interface ITagSite

        ''' <summary>
        ''' This property is usually be the locus_tag attribute
        ''' </summary>
        ''' <returns></returns>
        Property tag As String
        ''' <summary>
        ''' The distance of current loci site to the site that tagged by <see cref="tag"/> value.
        ''' (当前的这个位点对象距离<see cref="tag"/>所标记的位点的距离)
        ''' </summary>
        ''' <returns></returns>
        Property Distance As Integer
    End Interface

    Public Structure TagSite(Of T As IContig)
        Implements ITagSite

        Public Property contig As T

        Public Property Distance As Integer Implements ITagSite.Distance
        Public Property tag As String Implements ITagSite.tag

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure

    Public Module TagSiteExtensions

        ''' <summary>
        ''' 对位点进行分组操作
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source"></param>
        ''' <param name="offset"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function Groups(Of T As ITagSite)(source As IEnumerable(Of T), Optional offset As Integer = 10) As IEnumerable(Of GroupResult(Of T, String))
            Dim Grouping = (From x As T In source Select x Group x By x.tag Into Group)  ' 首先按照位点的tag标记进行分组
            Dim locis As IEnumerable(Of GroupResult(Of T, String)) =
                (From x
                 In Grouping'.AsParallel
                 Select x.Group.__innerGroup(offset, tag:=x.tag)).IteratesALL

            For Each x As GroupResult(Of T, String) In locis
                Yield x
            Next
        End Function

        <Extension> Private Function __innerGroup(Of T As ITagSite)(source As IEnumerable(Of T),
                                                     offset As Integer,
                                                     tag As String) As GroupResult(Of T, String)()

            Dim locis = (From x As T In source Select x Group x By x.Distance Into Group)
            locis = (From x In locis Select x Order By x.Distance Ascending).ToArray

            Dim result As New List(Of GroupResult(Of T, String))(
                New GroupResult(Of T, String)($"{tag}:{locis.First.Distance}", locis.First.Group))
            Dim last As Integer = locis.First.Distance

            For Each x In locis.Skip(1)
                If stdNum.Abs(x.Distance - last) <= offset Then
                    result.Last.Add(x.Group)
                Else
                    last = x.Distance
                    result += New GroupResult(Of T, String)($"{tag}:{x.Distance}", x.Group)
                End If
            Next

            Return result
        End Function
    End Module
End Namespace

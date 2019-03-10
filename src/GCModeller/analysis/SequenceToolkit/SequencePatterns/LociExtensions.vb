﻿#Region "Microsoft.VisualBasic::bc8599830050eee6cd332e6f77adcdd9, analysis\SequenceToolkit\SequencePatterns\LociExtensions.vb"

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

    ' Module LociExtensions
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: __ip, __pl, __revp, __revpcsv, __rps
    '               __rpscsv, ConvertsAuto, MirrorsLoci, (+4 Overloads) ToLoci, (+6 Overloads) ToLocis
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

''' <summary>
''' 将序列特征的搜索结果转换为<see cref="SimpleSegment"/>对象类型
''' </summary>
Public Module LociExtensions

    <Extension>
    Public Function ToLoci(x As PalindromeLoci) As SimpleSegment
        Return New SimpleSegment With {
            .Strand = x.MappingLocation.Strand.GetBriefCode,
            .Start = x.MappingLocation.Left,
            .Ends = x.MappingLocation.Right,
            .SequenceData = x.Palindrome
        }
    End Function

    ''' <summary>
    ''' --->&lt;---
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <Extension>
    Public Function MirrorsLoci(x As PalindromeLoci) As SimpleSegment
        Return New SimpleSegment With {
            .Strand = x.MappingLocation.Strand.GetBriefCode,
            .Start = x.MappingLocation.Left,
            .Ends = x.MappingLocation.Right,
            .SequenceData = x.Loci & x.Palindrome
        }
    End Function

    ''' <summary>
    ''' 对于简单的重复序列而言，正向链上面的重复片段，例如AAGTCT在反向链上面就是AGACTT，总是可以找得到对应的，所以在这里只需要记录下正向链的数据就好了
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="start"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ToLoci(x As Topologically.Repeats, start As Integer) As SimpleSegment
        Return New SimpleSegment With {
            .Start = start,
            .Ends = start + x.Length,
            .SequenceData = x.SequenceData,
            .Strand = "+"
        }
    End Function

    <Extension>
    Public Function ToLoci(x As RevRepeats, start As Integer) As SimpleSegment
        Return New SimpleSegment With {
            .Start = start,
            .Ends = start + x.Length,
            .SequenceData = x.SequenceData,
            .Strand = "+"
        }
    End Function

    <Extension>
    Public Function ToLoci(x As Topologically.ImperfectPalindrome) As SimpleSegment
        Dim id As String = ""

        If Not x.Data Is Nothing Then
            If x.Data.ContainsKey("seq") Then
                id = x.Data("seq") & "-"
            End If
        Else

        End If

        id = id & $"{x.Left},{x.Paloci}"

        Return New SimpleSegment With {
            .Start = x.MappingLocation.Left,
            .Ends = x.MappingLocation.Right,
            .Strand = x.MappingLocation.Strand.GetBriefCode,
            .SequenceData = x.Palindrome,
            .ID = id
        }
    End Function

    <Extension>
    Public Function ToLocis(x As IEnumerable(Of PalindromeLoci)) As SimpleSegment()
        Return x.Select(AddressOf ToLoci).ToArray
    End Function

    <Extension>
    Public Function ToLocis(x As IEnumerable(Of Topologically.Repeats)) As SimpleSegment()
        Return LinqAPI.Exec(Of SimpleSegment) <=
            From loci As Repeats
            In x
            Select From n As Integer
                   In loci.Locations
                   Select loci.ToLoci(n)
    End Function

    <Extension>
    Public Function ToLocis(x As IEnumerable(Of RevRepeats)) As SimpleSegment()
        Return LinqAPI.Exec(Of SimpleSegment) <=
            From loci As RevRepeats
            In x
            Select From n As Integer
                   In loci.Locations
                   Select loci.ToLoci(n)
    End Function

    <Extension>
    Public Function ToLocis(x As IEnumerable(Of Topologically.ImperfectPalindrome)) As SimpleSegment()
        Return x.Select(AddressOf ToLoci).ToArray
    End Function

    ''' <summary>
    ''' 自动根据类型转换为位点数据
    ''' </summary>
    ''' <param name="df"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ConvertsAuto(df As IO.File) As SimpleSegment()
        Dim types As Type() = {
            GetType(ImperfectPalindrome),
            GetType(RevRepeatsView),
            GetType(RevRepeats),
            GetType(RepeatsView),
            GetType(Repeats),
            GetType(PalindromeLoci)
        }
        Dim type As Type = df.GetType(types)
        Dim handler As Func(Of IO.File, SimpleSegment()) = __types(type)
        Dim result As SimpleSegment() = handler(df)

        Return result
    End Function

    Private Function __ip(df As IO.File) As SimpleSegment()
        Return df.AsDataSource(Of ImperfectPalindrome).ToLocis
    End Function

    Private Function __revp(df As IO.File) As SimpleSegment()
        Return df.AsDataSource(Of RevRepeats).ToLocis
    End Function

    Private Function __rps(df As IO.File) As SimpleSegment()
        Return df.AsDataSource(Of Repeats).ToLocis
    End Function

    Private Function __revpcsv(df As IO.File) As SimpleSegment()
        Return df.AsDataSource(Of RevRepeatsView).ToLocis
    End Function

    Private Function __rpscsv(df As IO.File) As SimpleSegment()
        Return df.AsDataSource(Of RepeatsView).ToLocis
    End Function

    <Extension>
    Public Function ToLocis(locis As IEnumerable(Of RepeatsView)) As SimpleSegment()
        Return locis.Select(Function(l) l.ToLoci).ToArray
    End Function

    <Extension>
    Public Function ToLocis(locis As IEnumerable(Of RevRepeatsView)) As SimpleSegment()
        Return locis.Select(Function(l) l.ToLoci).ToArray
    End Function

    Private Function __pl(df As IO.File) As SimpleSegment()
        Return df.AsDataSource(Of PalindromeLoci).ToLocis
    End Function

    ReadOnly __types As IReadOnlyDictionary(Of Type, Func(Of IO.File, SimpleSegment()))

    Sub New()
        Dim hash As New Dictionary(Of Type, Func(Of IO.File, SimpleSegment()))

        Call hash.Add(GetType(ImperfectPalindrome), AddressOf __ip)
        Call hash.Add(GetType(RevRepeats), AddressOf __revp)
        Call hash.Add(GetType(Repeats), AddressOf __rps)
        Call hash.Add(GetType(PalindromeLoci), AddressOf __pl)
        Call hash.Add(GetType(RepeatsView), AddressOf __rpscsv)
        Call hash.Add(GetType(RevRepeatsView), AddressOf __revpcsv)

        __types = hash
    End Sub
End Module

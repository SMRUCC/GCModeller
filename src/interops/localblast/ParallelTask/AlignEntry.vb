#Region "Microsoft.VisualBasic::3c8b651d8fd51bec0a632822c976c70a, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\BatchParallel\AlignEntry.vb"

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

'     Class AlignEntry
' 
'         Properties: FilePath, IsPairMatch
' 
'         Function: BiDirEquals, (+2 Overloads) SelectEquals, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

''' <summary>
''' blast结果文件的路径，在这里面包含有query和subject的来源的信息
''' </summary>
Public Class AlignEntry : Inherits I_BlastQueryHit

    ''' <summary>
    ''' The file path of the blast output log data or the csv data file.(日志文件或者Csv数据文件)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FilePath As String

    Public ReadOnly Property IsPairMatch As Boolean
        Get
            Return InStr(FilePath, VennDataBuilder.QUERY_LINKS_SUBJECT) > 0
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return String.Format("{0} <---> {1};   //{2}", QueryName, HitName, FilePath)
    End Function

    ''' <summary>
    ''' Is this paired of the blast output data is the bidirectional besthit result.(判断当前的这两个数据文件对象是否为双向比对结果)
    ''' </summary>
    ''' <param name="Entry"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function BiDirEquals(Entry As AlignEntry) As Boolean
        Return String.Equals(Entry.HitName, QueryName) AndAlso
            String.Equals(Entry.QueryName, HitName)
    End Function

    ''' <summary>
    ''' 选取当前对象的反向比对对象
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SelectEquals(data As IEnumerable(Of AlignEntry)) As AlignEntry
        Dim LQuery As AlignEntry =
            LinqAPI.DefaultFirst(Of AlignEntry) <= From logEntry As AlignEntry
                                                   In data
                                                   Where String.Equals(logEntry.HitName, QueryName) AndAlso
                                                       String.Equals(logEntry.QueryName, HitName)
                                                   Select logEntry
        Return LQuery
    End Function

    Public Function SelectEquals(Of TAnonymousType As Class)(
                                    data As IEnumerable(Of TAnonymousType),
                                   [Select] As Func(Of TAnonymousType, AlignEntry)) As TAnonymousType

        Dim LQuery As TAnonymousType =
            LinqAPI.DefaultFirst(Of TAnonymousType) <= From x As TAnonymousType
                                                       In data
                                                       Where Me.BiDirEquals([Select](x))
                                                       Select x
        Return LQuery
    End Function
End Class
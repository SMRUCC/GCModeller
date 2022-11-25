#Region "Microsoft.VisualBasic::5d63c6dd6afbf35687aa2126ecdd837c, GCModeller\analysis\SequenceToolkit\SmithWaterman\Extension\HSP.vb"

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

    '   Total Lines: 61
    '    Code Lines: 40
    ' Comment Lines: 11
    '   Blank Lines: 10
    '     File Size: 1.80 KB


    ' Class HSP
    ' 
    '     Properties: LengthHit, LengthQuery, Query, QueryLength, Subject
    '                 SubjectLength
    ' 
    '     Constructor: (+3 Overloads) Sub New
    '     Function: CreateFrom
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Linq
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports stdNum = System.Math

''' <summary>
''' 对<see cref="LocalHSPMatch(Of Char)"/>的XML文件保存文件格式对象
''' </summary>
Public Class HSP : Inherits Match

    Public Property Query As String
    Public Property Subject As String

    <XmlAttribute> Public Property QueryLength As Integer
    <XmlAttribute> Public Property SubjectLength As Integer

    ''' <summary>
    ''' length of the query fragment size
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LengthQuery As Integer
        Get
            Return stdNum.Abs(toA - fromA)
        End Get
    End Property

    ''' <summary>
    ''' length of the hit fragment size
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LengthHit As Integer
        Get
            Return stdNum.Abs(toB - fromB)
        End Get
    End Property

    Sub New()
    End Sub

    Sub New(match As Match)
        Call MyBase.New(match)
    End Sub

    Sub New(local As LocalHSPMatch(Of Char))
        Call MyBase.New(local)

        Query = New String(local.seq1)
        Subject = New String(local.seq2)
        QueryLength = local.QueryLength
        SubjectLength = local.SubjectLength
    End Sub

    Public Shared Function CreateFrom(Of T)(local As LocalHSPMatch(Of T), toChar As Func(Of T, Char)) As HSP
        Return New HSP(local) With {
            .Query = local.seq1.Select(toChar).CharString,
            .Subject = local.seq2.Select(toChar).CharString,
            .QueryLength = local.QueryLength,
            .SubjectLength = local.SubjectLength
        }
    End Function
End Class

#Region "Microsoft.VisualBasic::ab0214583f63e5d7513d93ba22264070, GCModeller\analysis\SequenceToolkit\SequencePatterns\Pattern\SegLoci.vb"

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

    '   Total Lines: 55
    '    Code Lines: 32
    ' Comment Lines: 16
    '   Blank Lines: 7
    '     File Size: 2.14 KB


    '     Class SegLoci
    ' 
    '         Properties: Left, Length, Right, Sequence
    ' 
    '         Function: CreateObject, ToRow, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace Pattern

    ''' <summary>
    ''' 程序所搜索到的一个序列片段
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SegLoci : Implements ILocationComponent

        ''' <summary>
        ''' 序列片段
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Sequence As String
        ''' <summary>
        ''' 该序列片段的长度
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Length As Integer
        ''' <summary>
        ''' 该序列在整条序列当中的左端长度和右端长度
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Left As Integer Implements ILocationComponent.Left
        <XmlAttribute> Public Property Right As Integer Implements ILocationComponent.Right

        Public Function ToRow() As RowObject
            Return New String() {Left, Right, Length, Sequence}
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}; <{1}, {2}>, length:= {3}", Sequence, Left, Right, Length)
        End Function

        Public Shared Function CreateObject(SegmentValue As Match, WholeSeq As String, ByRef startLeft As Long) As SegLoci
            Dim Segment As SegLoci = New SegLoci With {
                .Sequence = SegmentValue.Value,
                .Length = SegmentValue.Length
            }
#If netcore5 = 0 Then
            Segment.Left = Strings.InStr(Start:=startLeft, String1:=WholeSeq, String2:=Segment.Sequence, Compare:=CompareMethod.Text)
#Else
            Segment.Left = Strings.InStr(startLeft, String1:=WholeSeq, String2:=Segment.Sequence, Compare:=CompareMethod.Text)
#End If
            Segment.Right = Segment.Left + Segment.Length - 1
            startLeft = Segment.Left + 1

            Return Segment
        End Function
    End Class
End Namespace

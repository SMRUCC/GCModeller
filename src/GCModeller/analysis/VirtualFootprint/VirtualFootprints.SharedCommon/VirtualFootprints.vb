#Region "Microsoft.VisualBasic::f3867bd292637ce662adf9cc2bc8b865, ..\GCModeller\analysis\VirtualFootprint\VirtualFootprints.SharedCommon\VirtualFootprints.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports LANS.SystemsBiology.ComponentModel.Loci.Abstract
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation

Namespace DocumentFormat

    Public Class VirtualFootprints : Implements ILocationComponent
        Implements ITagSite

        ''' <summary>
        ''' 注释源信息
        ''' </summary>
        ''' <returns></returns>
        Public Property MotifId As String
        Public Property Starts As Integer Implements ILocationComponent.Left
        Public Property Ends As Integer Implements ILocationComponent.Right
        <Column("ORF ID")> Public Overridable Property ORF As String Implements ITagSite.tag
        ' <Column("RNA-Gene?")> Public Property RNAGene As String

        ''' <summary>
        ''' <see cref="MotifId"></see>与<see cref="ORF"></see>的ATG之间的距离
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("ATG-Distance")> Public Property Distance As Integer Implements ITagSite.Distance

        '''' <summary>
        '''' 字符串形式的相对位置描述：请参阅<seealso cref="PredictedRegulationFootprint.MotifLocation"></seealso>
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Property LociDescrib As String

        '''' <summary>
        '''' 对所预测出来的motif位点在本基因对象之上的相对位置的描述
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public ReadOnly Property MotifLocation As LANS.SystemsBiology.ComponentModel.Loci. SegmentRelationships
        '    Get
        '        Return GetLociDescrib(Me.LociDescrib)
        '    End Get
        'End Property

        Public Shared Function GetLociDescrib(s_Descr As String) As SegmentRelationships
            If InStr(s_Descr, "Inside the") > 0 Then
                Return SegmentRelationships.Inside
            ElseIf InStr(s_Descr, "Overlap on up_stream with") > 0 Then
                Return SegmentRelationships.UpStreamOverlap
            ElseIf InStr(s_Descr, "Overlap on down_stream with") > 0 Then
                Return SegmentRelationships.DownStreamOverlap
            ElseIf InStr(s_Descr, "In the downstream of") > 0 Then
                Return SegmentRelationships.DownStream
            ElseIf InStr(s_Descr, "In the promoter") > 0 Then
                Return SegmentRelationships.UpStream
            Else
                Return SegmentRelationships.Blank
            End If
        End Function

        'Public Function ExtractGenes() As String()
        '    If Not String.IsNullOrEmpty(ORF) Then
        '        Return {ORF}
        '    Else
        '        Dim m As String = Regex.Match(LociDescrib, "\[.+?\]").Value
        '        m = Mid(m, 2, Len(m) - 2)
        '        Dim list = Strings.Split(m, "; ")
        '        Return list
        '    End If
        'End Function

        ''' <summary>
        ''' Motif序列片段的长度
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Length As Integer
            Get
                Return Math.Abs(Ends - Starts)
            End Get
        End Property
        Public Property Strand As String
        <Column("ORF.Direct")> Public Property ORFDirection As String
        Public Property Sequence As String

        ''' <summary>
        ''' Motif序列的正则表达式表述模型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Signature As String

        Public Overrides Function ToString() As String
            Return $"{ORF}   Left={Starts};  ATG.Distance={Distance};   // {Sequence()}"
        End Function
    End Class
End Namespace

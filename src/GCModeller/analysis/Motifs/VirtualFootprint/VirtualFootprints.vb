#Region "Microsoft.VisualBasic::7dbeda0a7468b872f6221f6d508f23f6, analysis\Motifs\SharedDataModels\VirtualFootprints.vb"

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

'     Class VirtualFootprints
' 
'         Properties: Distance, Ends, Length, MotifId, ORF
'                     ORFDirection, Sequence, Signature, Starts, Strand
' 
'         Function: GetLociDescrib, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace DocumentFormat

    ''' <summary>
    ''' A motif site feature data model
    ''' </summary>
    Public Class VirtualFootprints : Implements ILocationComponent
        Implements ITagSite
        Implements INucleotideLocation
        Implements INamedValue

        ''' <summary>
        ''' 注释源信息
        ''' </summary>
        ''' <returns></returns>
        Public Property motif_id As String

        Public Property starts As Integer Implements ILocationComponent.left
        Public Property ends As Integer Implements ILocationComponent.right

        ''' <summary>
        ''' target gene that to be regulated
        ''' </summary>
        ''' <returns></returns>
        <Column("ORF ID")>
        Public Overridable Property ORF As String Implements ITagSite.tag, INamedValue.Key

        ''' <summary>
        ''' <see cref="motif_id"></see>与<see cref="ORF"></see>的ATG之间的距离
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("TSS-distance")> Public Property distance As Integer Implements ITagSite.Distance

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
        Public ReadOnly Property length As Integer
            Get
                Return Math.Abs(ends - starts)
            End Get
        End Property

        Public Property strand As Strands Implements INucleotideLocation.Strand

        ''' <summary>
        ''' the matched TFBS site sequence 
        ''' </summary>
        ''' <returns></returns>
        Public Property sequence As String

        ''' <summary>
        ''' Motif序列的正则表达式表述模型
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property signature As String

        Public Overrides Function ToString() As String
            Return $"{ORF}   Left={starts};  ATG.Distance={distance};   // {sequence()}"
        End Function
    End Class
End Namespace

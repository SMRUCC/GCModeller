#Region "Microsoft.VisualBasic::6d433fb656f6aac29ae700b352aad239, data\Xfam\Pfam\MPAlignment\Output\MPAlignmentOutput.vb"

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

    '   Total Lines: 93
    '    Code Lines: 54 (58.06%)
    ' Comment Lines: 27 (29.03%)
    '    - Xml Docs: 96.30%
    ' 
    '   Blank Lines: 12 (12.90%)
    '     File Size: 4.67 KB


    '     Class AlignmentOutput
    ' 
    '         Properties: AlignmentResult, DeltaScore, FullScore, LengthDelta, ProteinQuery
    '                     ProteinSbjct, Score, Similarity
    ' 
    '         Function: FormatAlignmentDomains, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports std = System.Math

Namespace ProteinDomainArchitecture.MPAlignment

    ''' <summary>
    ''' Protein Domain Alignment result.(蛋白质结构域的比对结果输出，从<see cref="ToString"/>函数生成文本文件格式的比对报告)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AlignmentOutput

        ' <XmlElement> Public Const VERSION As String = GetType(AlignmentOutput).Assembly.GetVersion.ToString

        ''' <summary>
        ''' MPScore
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Score As Double
        ''' <summary>
        ''' <see cref="Score">MPScore</see> without any penalty
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute("perfects")> Public Property FullScore As Double

        ''' <summary>
        ''' <see cref="Score"></see>/<see cref="FullScore"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Similarity As Double
            Get
                Return (Score / FullScore)
            End Get
        End Property

        Public Property ProteinQuery As PfamString.PfamString
        Public Property ProteinSbjct As PfamString.PfamString
        Public Property AlignmentResult As DomainAlignment()
        <XmlAttribute("delta")> Public Property LengthDelta As Double
        <XmlAttribute("delta.score")> Public Property DeltaScore As Double

        Private Shared Function FormatAlignmentDomains(data As AlignmentOutput) As String()
            Dim ChunkBuffer As List(Of String) = New List(Of String)
            Dim QueryMaxLength As Integer = If(data.AlignmentResult.IsNullOrEmpty, 0, (From item In data.AlignmentResult Select Len(item.ProteinQueryDomainDs.DomainId)).ToArray.Max) + 8
            Dim SbjctMaxLength As Integer = If(data.AlignmentResult.IsNullOrEmpty, 0, (From item In data.AlignmentResult Select Len(item.ProteinSbjctDomainDs.DomainId)).ToArray.Max) + 8

            If QueryMaxLength <= 8 OrElse SbjctMaxLength <= 8 Then
                Call ChunkBuffer.Add("No Domain can be aligned!")
                Return ChunkBuffer.ToArray
            End If

            Call ChunkBuffer.Add(String.Join(" ", {String.Format("Query{0}", New String(" "c, QueryMaxLength - 5)), String.Format("Subject{0}", New String(" "c, SbjctMaxLength - 7)), "Score"}))
            Call ChunkBuffer.Add(String.Join("+", {String.Format("-----{0}", New String("-"c, QueryMaxLength - 6)), String.Format("-------{0}", New String("-"c, SbjctMaxLength - 8)),
                                                       New String("-", Len(std.E.ToString) + 5)}))
            For Each item In data.AlignmentResult
                Call ChunkBuffer.Add(item.FormatPlantTextOutput(QueryMaxLength, SbjctMaxLength))
            Next

            Return ChunkBuffer.ToArray
        End Function

        ''' <summary>
        ''' 从这里生成文本文件格式的比对报告
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(String.Format("Domain alignment for {0} and {1}" & vbCrLf & vbCrLf, ProteinQuery.ProteinId, ProteinSbjct.ProteinId))
            Call sBuilder.AppendLine("Pfam-string brief information:" & vbCrLf)
            Call sBuilder.AppendLine("Query=")
            Call sBuilder.AppendLine(ProteinQuery.get_PlantTextOutput)
            Call sBuilder.AppendLine("Subject=")
            Call sBuilder.AppendLine(ProteinSbjct.get_PlantTextOutput)
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine(String.Format("Structure Compared Score: {0}", Score))
            Call sBuilder.AppendLine(String.Format("Structure Similarity:     {1}%", Score, Similarity * 100))
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine(String.Join(vbCrLf, AlignmentOutput.FormatAlignmentDomains(Me)))
NO_DOMAIN_: Call sBuilder.AppendLine()
            Call sBuilder.AppendLine("FullScore:   " & FullScore)
            Call sBuilder.AppendLine("LengthDelta: " & LengthDelta)
            Call sBuilder.AppendLine("DeltaScore:  " & DeltaScore)
            Call sBuilder.AppendLine(vbCrLf & New String("-", 120))

            Return sBuilder.ToString
        End Function
    End Class
End Namespace

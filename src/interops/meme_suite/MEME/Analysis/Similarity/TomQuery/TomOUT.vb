#Region "Microsoft.VisualBasic::302e31998ccb7aa931d80aacb7ee8fc2, meme_suite\MEME\Analysis\Similarity\TomQuery\TomOUT.vb"

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

    '     Class TomOUT
    ' 
    '         Properties: Alignment, Query, QueryLength, Subject, SubjectLength
    ' 
    '         Function: ResultView, ToString, Visual
    ' 
    '     Class SW_HSP
    ' 
    '         Properties: FromQ, FromS, Score, ToQ, ToS
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Imaging.BitmapImage

Namespace Analysis.Similarity.TOMQuery

    Public Class TomOUT

        Public Property Query As MotifScans.AnnotationModel
        Public Property Subject As MotifScans.AnnotationModel
        Public Property Alignment As DistResult

        Public ReadOnly Property QueryLength As Integer
            Get
                If Query Is Nothing Then
                    Return 0
                Else
                    Return Query.PWM.Length
                End If
            End Get
        End Property

        Public ReadOnly Property SubjectLength As Integer
            Get
                If Subject Is Nothing Then
                    Return 0
                Else
                    Return Subject.PWM.Length
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Query.ToString}, {Subject.ToString}"
        End Function

        Public Function Visual() As Image
            Return TomVisual.VisualLevEdit(Query, Subject, Alignment, False).CorpBlank(20)
        End Function

        Public Function ResultView() As TomTOm.CompareResult
            Return TomTOm.CreateResult(Query, Subject, Alignment)
        End Function
    End Class

    ''' <summary>
    ''' 使用这个对象进行高分区的绘图
    ''' </summary>
    Public Class SW_HSP : Inherits TomOUT
        <XmlAttribute> Public Property FromQ As Integer
        <XmlAttribute> Public Property FromS As Integer
        <XmlAttribute> Public Property ToQ As Integer
        <XmlAttribute> Public Property ToS As Integer
        <XmlAttribute> Public Property Score As Double

        Public Overrides Function ToString() As String
            Dim q As String = $"{Query.Uid}.[{FromQ}, {ToQ}]"
            Dim s As String = $"{Subject.Uid}.[{FromS}, {ToS}]"
            Return $"{q}, {s}"
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::d1fd8431e8c1d5b8da27e7a96204bb80, engine\IO\GCMarkupLanguage\v2\Xml\Genomics\transcription.vb"

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

    '   Total Lines: 49
    '    Code Lines: 21 (42.86%)
    ' Comment Lines: 18 (36.73%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (20.41%)
    '     File Size: 1.50 KB


    '     Class transcription
    ' 
    '         Properties: biological_process, centralDogma, mode, motif, note
    '                     regulator, targets
    ' 
    '     Class Motif
    ' 
    '         Properties: distance, family, left, right, sequence
    '                     strand
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace v2

    ''' <summary>
    ''' 基因表达转录调控
    ''' </summary>
    Public Class transcription

        <XmlAttribute> Public Property regulator As String

        <XmlAttribute> Public Property mode As String

        ''' <summary>
        ''' 这个调控关系所影响到的中心法则的事件名称
        ''' </summary>
        ''' <returns></returns>
        Public Property centralDogma As String()
        Public Property biological_process As String
        Public Property motif As Motif
        ''' <summary>
        ''' a collection of the genes id inside the regulated operons
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property targets As String()
        Public Property note As String

    End Class

    ''' <summary>
    ''' 调控的motif位点
    ''' </summary>
    Public Class Motif

        <XmlAttribute> Public Property family As String
        <XmlAttribute> Public Property left As Integer
        <XmlAttribute> Public Property right As Integer
        <XmlAttribute> Public Property strand As Char

        ''' <summary>
        ''' 这个motif位点到被调控的基因的ATG位点的最短距离
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("atg-distance")>
        Public Property distance As Integer

        <XmlText> Public Property sequence As String
    End Class
End Namespace

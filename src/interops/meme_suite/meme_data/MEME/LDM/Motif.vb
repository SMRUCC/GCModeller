#Region "Microsoft.VisualBasic::c3d09af4e50168dbbe95c5d64eedb6b5, meme_suite\MEME.DocParser\MEME\LDM\Motif.vb"

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

    '     Class Motif
    ' 
    '         Properties: Evalue, Id, llr, Mcs, NtMolType
    '                     PspMatrix, Signature, Sites, uid, Width
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel

Namespace DocumentFormat.MEME.LDM

    ''' <summary>
    ''' Motif data from the text format output file of the meme program.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Motif

        ''' <summary>
        ''' 进行一些绘图操作的时候可能需要使用到这个属性，一般情况之下这个属性只是用作于一些用户的自定义数据，不太重要
        ''' </summary>
        ''' <returns></returns>
        <XmlElement> Public Property uid As String
        <XmlAttribute> Public Property Id As String
        <XmlAttribute> Public Property Width As Integer
        <XmlAttribute> Public Property llr As Integer
        <XmlAttribute> Public Property Evalue As Double

        ''' <summary>
        ''' Simplified pos.-specific probability matrix
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PspMatrix As MotifPM()
        '<XmlAttribute> Public Property RelativeEntropy As Double

        ''' <summary>
        ''' Multilevel consensus sequence
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Mcs As String()

        ''' <summary>
        ''' 产生这个Motif的序列的集合
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("Motif.Sites", Namespace:="http://code.google.com/p/genome-in-code/meme/motif.matrix")>
        Public Property Sites As Site()
        ''' <summary>
        ''' 使用这个正则表达式来描述当前的这个Motif的特征
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("Motif.Signature")> Public Property Signature As String

        ''' <summary>
        ''' 是否为核酸类型的Motif位点，TRUE为是，FALSE为蛋白质序列的Motif
        ''' </summary>
        ''' <returns></returns>
        Public Property NtMolType As Boolean

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace

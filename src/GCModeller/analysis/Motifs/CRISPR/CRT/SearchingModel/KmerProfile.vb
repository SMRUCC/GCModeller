#Region "Microsoft.VisualBasic::02c12b9a0e9ba68c6e60593ffba4b1ab, analysis\Motifs\CRISPR\CRT\SearchingModel\KmerProfile.vb"

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

    '     Structure KmerProfile
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace SearchingModel

    ''' <summary>
    ''' 重复片段搜索程序的参数设置对象
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure KmerProfile

        ''' <summary>
        ''' Succession of similarly spaced repeats of length k..(CRISPR片段的长度)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Dim k As Integer

        <XmlAttribute> Dim minR, maxR As Integer
        <XmlAttribute> Dim minS, maxS As Integer

        Public Overrides Function ToString() As String
            Return String.Format("[i + minR:={0} + minS:={1} .. i + maxR:={2} + maxS:={3} + k:={3}]", minR, minS, maxR, maxS, k)
        End Function
    End Structure
End Namespace

#Region "Microsoft.VisualBasic::6528b2902bb7154b7e8d054d2101d207, visualize\visualizeTools\ComparativeGenomics\DrawingModel.vb"

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

'     Class DrawingModel
' 
'         Properties: Genome1, Genome2, Links
' 
'     Class GenomeModel
' 
'         Properties: genes, Length, SegmentOffset, Title
' 
'         Function: GetEnumerator, GetEnumerator1, ToString
' 
'     Class GeneLink
' 
'         Properties: annotation, Color, genome1, genome2
' 
'         Function: Equals, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace ComparativeGenomics

    ''' <summary>
    ''' 比较两个基因组的差异的模型
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DrawingModel

        Public Property Genome1 As GenomeModel
        Public Property Genome2 As GenomeModel

        Public Property Links As GeneLink()

    End Class

    ''' <summary>
    ''' 两个基因组之间的相互共同的基因
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GeneLink

        <XmlAttribute> Public Property genome1 As String
        <XmlAttribute> Public Property genome2 As String

        <XmlElement>
        Public Property Color As Color
        <XmlText>
        Public Property annotation As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} === {1};   //{2}", genome1, genome2, annotation)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function Equals(id1 As String, id2 As String) As Boolean
            Return (String.Equals(id1, genome1, StringComparison.OrdinalIgnoreCase) AndAlso String.Equals(id2, genome2, StringComparison.OrdinalIgnoreCase)) OrElse
                (String.Equals(id2, genome1, StringComparison.OrdinalIgnoreCase) AndAlso String.Equals(id1, genome2, StringComparison.OrdinalIgnoreCase))
        End Function
    End Class
End Namespace

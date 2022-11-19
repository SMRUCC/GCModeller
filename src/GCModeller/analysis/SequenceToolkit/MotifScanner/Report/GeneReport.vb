#Region "Microsoft.VisualBasic::82fe61eab25f991d197aaa94f85a80a2, GCModeller\analysis\SequenceToolkit\MotifScanner\Report\GeneReport.vb"

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

    '   Total Lines: 46
    '    Code Lines: 29
    ' Comment Lines: 8
    '   Blank Lines: 9
    '     File Size: 1.54 KB


    ' Class GeneReport
    ' 
    '     Properties: components, length, locus_tag, numOfPromoters, promoterPos
    '                 promoterPosLDF, tfBindingSites, threshold
    ' 
    ' Class TFBindingSite
    ' 
    '     Properties: oligonucleotides, position, regulator, score
    ' 
    '     Function: ToString
    ' 
    ' Class PromoterComponent
    ' 
    '     Properties: oligonucleotides, pos, score, type
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Public Class GeneReport

    ''' <summary>
    ''' the promoter region associated gene id
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute> Public Property locus_tag As String
    ''' <summary>
    ''' the length of the input promoter region
    ''' </summary>
    ''' <returns></returns>
    Public Property length As Integer
    Public Property threshold As Double
    <XmlAttribute> Public Property numOfPromoters As Integer
    Public Property promoterPos As Integer
    Public Property promoterPosLDF As Double
    <XmlElement> Public Property components As PromoterComponent()
    <XmlElement> Public Property tfBindingSites As TFBindingSite()

End Class

Public Class TFBindingSite
    <XmlAttribute> Public Property regulator As String
    <XmlText> Public Property oligonucleotides As String
    <XmlAttribute> Public Property position As Integer
    <XmlAttribute> Public Property score As Double

    Public Overrides Function ToString() As String
        Return $"       {regulator}:  {oligonucleotides} at position      {position} Score -  {score}"
    End Function
End Class

Public Class PromoterComponent

    <XmlAttribute> Public Property type As String
    <XmlAttribute> Public Property pos As Integer
    <XmlText> Public Property oligonucleotides As String
    <XmlAttribute> Public Property score As Double

    Public Overrides Function ToString() As String
        Return $" {type} at pos.     {pos} {oligonucleotides} Score    {score}"
    End Function

End Class

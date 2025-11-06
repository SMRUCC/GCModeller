#Region "Microsoft.VisualBasic::c4a01edb520f15cab7db32449b1ea2f7, localblast\LocalBLAST\Pipeline\Models\Hit.vb"

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

    '   Total Lines: 32
    '    Code Lines: 13 (40.62%)
    ' Comment Lines: 15 (46.88%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (12.50%)
    '     File Size: 1.10 KB


    '     Class Hit
    ' 
    '         Properties: hitName, identities, positive, tag
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Tasks.Models

    ''' <summary>
    ''' 和Query的一个比对结果
    ''' </summary>
    Public Class Hit : Implements INamedValue

        ''' <summary>
        ''' <see cref="HitName"></see>所在的物种
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property tag As String
        ''' <summary>
        ''' 和query蛋白质比对上的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlText>
        Public Property hitName As String Implements INamedValue.Key

        <XmlAttribute> Public Property score As Double
        <XmlAttribute> Public Property identities As Double
        <XmlAttribute> Public Property positive As Double
        <XmlAttribute> Public Property gaps As Double
        <XmlAttribute> Public Property evalue As Double

        Public Overrides Function ToString() As String
            Return $"[{tag}] {hitName} {{identities:= {identities}, positive:= {positive}}}"
        End Function
    End Class
End Namespace

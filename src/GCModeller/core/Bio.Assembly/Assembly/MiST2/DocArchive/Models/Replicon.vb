#Region "Microsoft.VisualBasic::62956066d670104a537b59f8f25713f2, ..\core\Bio.Assembly\Assembly\MiST2\DocArchive\Models\Replicon.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ProteinModel
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.MiST2

    ''' <summary>
    ''' 基因组之中的一个复制子
    ''' </summary>
    Public Class Replicon

        <XmlAttribute> Public Property Accession As String
        <XmlAttribute> Public Property Replicon As String

        ''' <summary>
        ''' Replicon Size (Mbp)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Size As String

        ''' <summary>
        ''' One-component
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OneComponent As Transducin()
        Public Property TwoComponent As TwoComponent
        Public Property Chemotaxis As Transducin()
        ''' <summary>
        ''' Extra Cytoplasmic Function
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("ExtraCytoFunc")> Public Property ECF As Transducin()
        Public Property Other As Transducin()

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}, {2}Mbp", Replicon, Accession, Size)
        End Function

        ''' <summary>
        ''' 获取在本对象中所存储的所有的蛋白质对象
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SignalProteinCollection() As Transducin()
            Dim List As List(Of Transducin) = New List(Of Transducin)
            Call List.AddRange(OneComponent)
            Call List.AddRange(CType(TwoComponent, Assembly.MiST2.Transducin()))
            Call List.AddRange(Chemotaxis)
            Call List.AddRange(ECF)
            Call List.AddRange(Other)

            Return List.ToArray
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::fb878d1fbb3cb7d9743902c4cb4d5635, GCModeller\engine\IO\GCMarkupLanguage\v2\Xml\Genome\TranscriptUnit.vb"

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

    '   Total Lines: 28
    '    Code Lines: 18
    ' Comment Lines: 4
    '   Blank Lines: 6
    '     File Size: 708 B


    '     Class TranscriptUnit
    ' 
    '         Properties: genes, id, numOfGenes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace v2

    Public Class TranscriptUnit

        <XmlAttribute>
        Public Property id As String

        ''' <summary>
        ''' 基因列表，在这个属性之中定义了该基因组之中的所有基因的数据
        ''' </summary>
        ''' <returns></returns>
        Public Property genes As XmlList(Of gene)

        Public ReadOnly Property numOfGenes As Integer
            Get
                If genes Is Nothing Then
                    Return 0
                Else
                    Return genes.size
                End If
            End Get
        End Property

    End Class
End Namespace

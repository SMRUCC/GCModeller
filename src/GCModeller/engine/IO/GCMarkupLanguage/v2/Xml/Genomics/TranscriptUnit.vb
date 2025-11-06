#Region "Microsoft.VisualBasic::71fb7111ab33ae881bd583ce81edeaf3, engine\IO\GCMarkupLanguage\v2\Xml\Genomics\TranscriptUnit.vb"

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

    '   Total Lines: 47
    '    Code Lines: 27 (57.45%)
    ' Comment Lines: 11 (23.40%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (19.15%)
    '     File Size: 1.20 KB


    '     Class TranscriptUnit
    ' 
    '         Properties: genes, id, numOfGenes
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization

Namespace v2

    ''' <summary>
    ''' an operon model is a single transcript unit
    ''' </summary>
    Public Class TranscriptUnit

        <XmlAttribute> Public Property id As String

        ''' <summary>
        ''' the display name of this operon object
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String

        <XmlAttribute> Public Property sites As String()

        ''' <summary>
        ''' the operon gene list
        ''' </summary>
        ''' <returns></returns>
        <XmlElement>
        Public Property genes As gene()

        <XmlElement>
        Public Property note As String

        Public ReadOnly Property numOfGenes As Integer
            Get
                If genes Is Nothing Then
                    Return 0
                Else
                    Return genes.TryCount
                End If
            End Get
        End Property

        Sub New()
        End Sub

        ''' <summary>
        ''' create a transcript unit with single gene inside
        ''' </summary>
        ''' <param name="gene"></param>
        Sub New(gene As gene)
            genes = {gene}
            id = "TU-" & gene.locus_tag
            name = gene.locus_tag
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{id}] {genes.Select(Function(g) g.locus_tag).JoinBy(", ")}"
        End Function

    End Class
End Namespace

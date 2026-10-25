#Region "Microsoft.VisualBasic::e4ba5024584b2cc0a22ed5ba1f60d4c7, annotations\GSEA\PFSNet\PFSNet\Models\GraphEdge\GraphEdge.vb"

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

    '   Total Lines: 53
    '    Code Lines: 30 (56.60%)
    ' Comment Lines: 16 (30.19%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (13.21%)
    '     File Size: 1.97 KB


    '     Class GraphEdge
    ' 
    '         Properties: g1, g2, isSelfLoop, pathwayID
    ' 
    '         Function: LoadData, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language

Namespace DataStructure

    ''' <summary>
    ''' Gene to Gene Interaction.(基因与基因之间的互作关系)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GraphEdge

        <XmlAttribute>
        Public Property pathwayID As String

        ''' <summary>
        ''' The geneID of a gene node in current pathway
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("gene1")> Public Property g1 As String
        ''' <summary>
        ''' Another partner gene node its id in current pathway
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute("gene2")> Public Property g2 As String

        ''' <summary>
        ''' Does <see cref="g1"/> is equals to <see cref="g2"/>?
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property isSelfLoop As Boolean
            Get
                Return String.Equals(g1, g2)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Join(vbTab, pathwayID, g1, g2)
        End Function

        Public Shared Function LoadData(path As String) As GraphEdge()
            Dim edges As New System.Collections.Generic.List(Of GraphEdge)

            For Each raw As String In IO.File.ReadLines(path)
                If String.IsNullOrWhiteSpace(raw) Then
                    Continue For
                End If

                Dim tokens As String() = raw.Split(ControlChars.Tab)

                If tokens.Length < 3 Then
                    Continue For
                End If

                Dim pathway As String = tokens(0).Trim.TrimStart(ChrW(&HFEFF))
                Dim gene1 As String = tokens(1).Trim
                Dim gene2 As String = tokens(2).Trim

                If pathway.Equals("pathway", StringComparison.OrdinalIgnoreCase) AndAlso
                   gene1.Equals("gene1", StringComparison.OrdinalIgnoreCase) AndAlso
                   gene2.Equals("gene2", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                If pathway.Length = 0 OrElse gene1.Length = 0 OrElse gene2.Length = 0 Then
                    Continue For
                End If

                edges.Add(New GraphEdge With {
                    .pathwayID = pathway,
                    .g1 = gene1,
                    .g2 = gene2
                })
            Next

            Return edges.ToArray
        End Function
    End Class
End Namespace

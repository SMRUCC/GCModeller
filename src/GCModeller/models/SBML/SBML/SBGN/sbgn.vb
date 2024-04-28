#Region "Microsoft.VisualBasic::0e03f9b94b60c0c95e6eead8cf84aa87, G:/GCModeller/src/GCModeller/models/SBML/SBML//SBGN/sbgn.vb"

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

    '   Total Lines: 57
    '    Code Lines: 36
    ' Comment Lines: 8
    '   Blank Lines: 13
    '     File Size: 1.82 KB


    '     Class sbgnFile
    ' 
    '         Properties: canvas, map
    ' 
    '         Function: MeasureCanvasSize, ReadXml
    ' 
    '     Class map
    ' 
    '         Properties: arc, glyph, language
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace SBGN

    ''' <summary>
    ''' Systems Biology Graphical Notation
    ''' </summary>
    ''' <remarks>
    ''' Systems Biology Graphical Notation (SBGN) project, an effort to standardise 
    ''' the graphical notation used in maps of biological processes.
    ''' </remarks>
    ''' 
    <XmlRoot("sbgn", [Namespace]:="http://sbgn.org/libsbgn/0.2")>
    <XmlType("sbgn", [Namespace]:="http://sbgn.org/libsbgn/0.2")>
    Public Class sbgnFile

        Public Property map As map

        Public ReadOnly Property canvas As Double()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return MeasureCanvasSize()
            End Get
        End Property

        Private Function MeasureCanvasSize() As Double()
            Dim rects = map.glyph _
                .SafeQuery _
                .Select(Function(gl) CType(gl.bbox, RectangleF)) _
                .JoinIterates(map.arc.SafeQuery.Select(Function(a) a.GetPolygon.GetRectangle)) _
                .ToArray
            Dim maxX = Aggregate r As RectangleF In rects Into Max(r.Right)
            Dim maxY = Aggregate r As RectangleF In rects Into Max(r.Bottom)

            Return {maxX, maxY}
        End Function

        Public Shared Function ReadXml(file As String) As sbgnFile
            Return file.SolveStream.LoadFromXml(GetType(sbgnFile))
        End Function

    End Class

    Public Class map

        <XmlElement("glyph")> Public Property glyph As glyph()
        <XmlElement("arc")> Public Property arc As arc()

        <XmlAttribute>
        Public Property language As String

    End Class

End Namespace

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
                .Select(Function(gl) CType(gl.bbox, RectangleF)) _
                .JoinIterates(map.arc.Select(Function(a) a.GetPolygon.GetRectangle)) _
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
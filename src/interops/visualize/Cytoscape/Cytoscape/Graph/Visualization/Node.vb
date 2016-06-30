Imports System.Drawing

Namespace CytoscapeGraphView

    ''' <summary>
    ''' 一般是绘制圆形
    ''' </summary>
    Public Class Node

        ''' <summary>
        ''' 半径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Radius As Integer
        Public ReadOnly Property Location As Point

        Private Sub New(r As Integer, Location As Point)
            Me.Radius = r
            Me.Location = Location
            Rectangle = New Rectangle(New Point(Location.X - r, Location.Y - r), New Size(r * 2, r * 2))
            Me.ir = r * 1.1
        End Sub

        Sub New(Node As XGMML.Node, xScale As Double, yScale As Double)
            Call Me.New(((xScale + yScale) / 3) * (Node.Graphics.w + Node.Graphics.h) / 2,
                        New Point(Node.Graphics.x * xScale, Node.Graphics.y * yScale))
            Me.NodeModel = Node
        End Sub

        Public ReadOnly Property NodeModel As XGMML.Node

        ''' <summary>
        ''' 在画图的时候的圆形的正方形的绘图区域
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Rectangle As Rectangle

        Public Function OffSet(value As Point) As Node
            Me._Location = New Point(Location.X + value.X, Location.Y + value.Y)
            Me._Rectangle = New Rectangle(New Point(Location.X - Radius, Location.Y - Radius), Rectangle.Size)
            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return $"{Location.ToString} // {NameOf(Radius)}:={Radius}"
        End Function

        ''' <summary>
        ''' 计算当前节点和另外一个节点的夹角的sin(alpha)
        ''' </summary>
        ''' <param name="Node"></param>
        ''' <returns></returns>
        Public Function getSinAlpha(Node As Node) As Double
            Return (Location.Y - Node.Location.Y) / (Math.Sqrt((Location.X - Node.Location.X) ^ 2 + (Location.Y - Node.Location.Y) ^ 2))
        End Function

        Public Function getCosAlpha(Node As Node) As Double
            Return (Node.Location.X - Location.X) / (Math.Sqrt((Location.X - Node.Location.X) ^ 2 + (Location.Y - Node.Location.Y) ^ 2))
        End Function

        ''' <summary>
        ''' Interface .Radius
        ''' </summary>
        Protected ReadOnly ir As Double

        Public Function Point_getInterface(Node As Node) As Point
            Dim we = ir * getCosAlpha(Node)
            Dim he = ir * getSinAlpha(Node)
            Dim e = New Point(Location.X + we, Location.Y - he)
            Return e
        End Function
    End Class
End Namespace
Imports System.Drawing
Imports LANS.SystemsBiology
Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.ComponentModel.Loci.Abstract
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace ChromosomeMap.DrawingModels

    ''' <summary>
    ''' 染色体上面的一个基因的绘图模型
    ''' </summary>
    Public Class SegmentObject : Inherits MapModelCommon
        Implements sIdEnumerable
        Implements IGeneBrief

        ''' <summary>
        ''' 基因号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LocusTag As String Implements ICOGDigest.Identifier
        ''' <summary>
        ''' 基因名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CommonName As String Implements ICOGDigest.COG

        ''' <summary>
        ''' 基因功能注释文字
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Product As String Implements ICOGDigest.Product

        Private Property I_COGEntry_Length As Integer Implements ICOGDigest.Length
        Public Property Location As NucleotideLocation Implements IContig.Location

        Const LocusTagOffset = 20
        Const CommonNameOffset = 15

        Public Overrides Function ToString() As String
            Return String.Format("{0}:  [{1}, {2}]", LocusTag, Left, Right)
        End Function

        Public Shared ReadOnly TextAlignments As Dictionary(Of String, __TextAlignment) =
            New Dictionary(Of String, __TextAlignment) From {
 _
                {"left", AddressOf LeftAligned},
                {"middle", AddressOf MiddleAlignment},
                {"right", AddressOf RightAlignment}
        }

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="segnmentLength"></param>
        ''' <param name="headLength"></param>
        ''' <param name="textLength"></param>
        ''' <param name="p"></param>
        ''' <returns>返回字符串的位置信息</returns>
        ''' <remarks></remarks>
        Public Delegate Function __TextAlignment(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="segnmentLength">基因对象的图形的绘制长度</param>
        ''' <param name="textLength">使用MeasureString获取得到的字符串的绘制长度</param>
        ''' <param name="p">基因对象额绘制坐标</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function LeftAligned(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point
            Return p
        End Function

        Private Shared Function RightAlignment(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point
            p = New Point(p.X + segnmentLength - textLength, p.Y)
            p = InternalCheckRightEndTrimmed(p, textLength, rightEnds)
            Return p
        End Function

        Private Shared Function InternalCheckRightEndTrimmed(p As Point, textLength As Integer, rightEnds As Integer) As Point
            If p.X + textLength > rightEnds Then
                Dim d = p.X + textLength - rightEnds
                d = p.X - d
                p = New Point(d, p.Y)
            End If

            Return p
        End Function

        Private Shared Function MiddleAlignment(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point
            Dim d As Integer = (segnmentLength - textLength) / 2
            p = New Point(d + p.X - headLength, p.Y)
            p = InternalCheckRightEndTrimmed(p, textLength, rightEnds)
            Return p
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Gr"></param>
        ''' <param name="Location">图形的左上角的坐标</param>
        ''' <returns>返回绘制的图形的大小</returns>
        ''' <remarks></remarks>
        Public Function Draw(Gr As System.Drawing.Graphics,
                             Location As System.Drawing.Point,
                             ConvertFactor As Double,
                             RightLimited As Integer, conf As Conf) As Size

            Dim GraphicPath As System.Drawing.Drawing2D.GraphicsPath
            Dim LocusTagLocation As Integer = Location.X
            Dim Font As System.Drawing.Font, size As System.Drawing.SizeF

            Font = conf.LocusTagFont

            Me.ConvertFactor = ConvertFactor

            If Direction < 0 Then
                GraphicPath = CreateBackwardModel(Location, RightLimited)
            ElseIf Direction > 0 Then
                GraphicPath = CreateForwardModel(Location, RightLimited)
            Else
                GraphicPath = CreateNoneDirectionModel(Location, RightLimited)
            End If

            Call Gr.DrawPath(New System.Drawing.Pen(System.Drawing.Brushes.Black, 5), GraphicPath)
            Call Gr.FillPath(Me.Color, GraphicPath)

            size = Gr.MeasureString(LocusTag, Font)

            Dim MaxLength = System.Math.Max(size.Width, Length)

            If size.Width > Length Then
                LocusTagLocation -= 0.5 * Global.System.Math.Abs(Length - size.Width)
            Else
                LocusTagLocation += 0.5 * Global.System.Math.Abs(Length - size.Width)
            End If

            Dim pLocusTagLocation = InternalCheckRightEndTrimmed(New System.Drawing.Point(LocusTagLocation, Location.Y - size.Height - LocusTagOffset), MaxLength, RightLimited)
            Call Gr.DrawString(LocusTag, Font, System.Drawing.Brushes.Black, pLocusTagLocation)

            size = Gr.MeasureString(CommonName, Font)
            MaxLength = System.Math.Max(size.Width, Length)
            LocusTagLocation = Location.X
            If size.Width > Length Then
                LocusTagLocation -= 0.5 * Global.System.Math.Abs(Length - size.Width)
            Else
                LocusTagLocation += 0.5 * Global.System.Math.Abs(Length - size.Width)
            End If
            pLocusTagLocation = New System.Drawing.Point(LocusTagLocation, pLocusTagLocation.Y + Height + 10 + size.Height + LocusTagOffset)
            Call Gr.DrawString(Me.CommonName, conf.LocusTagFont, Brushes.Black, pLocusTagLocation)

            Font = New System.Drawing.Font("Microsoft YaHei", 6)

            LocusTagLocation = Location.X

            If Direction < 0 Then
                LocusTagLocation += (10 + HeadLength)
            End If

            Call Gr.DrawString(Product, Font, System.Drawing.Brushes.DarkOliveGreen, New System.Drawing.Point(LocusTagLocation, Location.Y + 5 + Height))

#If DEBUG Then
            Call Gr.DrawString(String.Format("{0} .. {1} KBp", Left / 1000, Right / 1000), Font, System.Drawing.Brushes.White, New Point(LocusTagLocation, Location.Y + 0.2 * Height))
#End If
            Return New Size(MaxLength, Height)
        End Function
    End Class
End Namespace
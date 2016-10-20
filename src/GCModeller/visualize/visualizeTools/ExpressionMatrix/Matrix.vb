#Region "Microsoft.VisualBasic::930a2837730ef461de6ed5d12a4af2da, ..\GCModeller\visualize\visualizeTools\ExpressionMatrix\Matrix.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.InteractionModel

Namespace ExpressionMatrix

    Public Module MatrixDrawing

        Dim Margin As Integer = 50

        Public Function InvokeDrawing(Data As SerialsData(), Conf As Configuration) As Image
            Data = Data.Skip(1).ToArray   '第一个元素为时间
            Dim RenderingColor = New GeneticClock.ColorRender(Data).GetColorRenderingProfiles
            Dim TagFont As Font = New Font("Ubuntu", 4)
            Dim TagSize = (From item In Data Select item.Tag Order By Len(Tag) Descending).First.MeasureString(TagFont)
            Dim Margin As Integer = 10
            Dim DotSize As New Size(20, If(TagSize.Height > 5, TagSize.Height, 5))
            Dim Device = New Size(Data.First.ChunkBuffer.Count * DotSize.Width + Margin * 2, Data.Count * DotSize.Height + Margin * 2).CreateGDIDevice
            Dim GR_DEVICE As Graphics = Device.Graphics

            For i As Integer = 0 To Data.Count - 1
                Dim Line = Data(i)
                Dim Height As Integer = Margin + i * TagSize.Height
                Call GR_DEVICE.DrawString(Line.Tag, TagFont, Brushes.Black, New Point(Margin, Height))
                Dim Left As Integer = Margin + TagSize.Width + 5
                Dim Colors = RenderingColor(i)

                For j As Integer = 0 To Line.ChunkBuffer.Count - 1
                    Dim Color = Colors.Profiles(j)
                    Call GR_DEVICE.FillRectangle(New SolidBrush(Color.Value), New Rectangle(New Point(Left, Height), DotSize))
                    Left += DotSize.Width
                Next
            Next

            Return Device.ImageResource
        End Function

        ''' <summary>
        '''      tag1 tag2 tag3 tag4 ...
        ''' tag1 ...  ...  ...  ...  ... 
        ''' tag2 ...  ...  ...  ...  ...
        ''' tag3 ...  ...  ...  ...  ...
        ''' tag4 ...  ...  ...  ...  ...
        ''' .... ...  ...  ...  ...  ...
        ''' </summary>
        ''' <param name="MAT"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function NormalMatrix(MAT As DocumentStream.File) As Image
            Dim Tags As String() = {(From row In MAT.Skip(1) Select row.First).ToArray, MAT.First.Skip(1).ToArray}.Unlist.Distinct.ToArray
            Dim TagDict = CreateAlphabetTagSerials(Tags)
            Dim DrawingFont As Font = New Font(FontFace.Ubuntu, 12)
            Dim MatrixValueString = (From s As String In (From row In MAT.Skip(1) Select row.Skip(1).ToArray).Unlist Select s Order By Len(s) Descending).ToArray
            Dim size = MatrixValueString.First.MeasureString(DrawingFont)
            Dim MatrixValues As Double() = (From s As String In MatrixValueString Select Val(s)).ToArray
            Dim ColorValues = GeneticClock.ColorRender.GetDesityRule(MatrixValues, 100)
            Dim DotSize = New Size(size.Width, size.Height)
            Dim Gr = (New Size(3 * Margin + DotSize.Width * MAT.Width, 3 * Margin + DotSize.Height * MAT.Count)).CreateGDIDevice

            Call Gr.ImageAddFrame()

            Dim x As Integer = Margin, y As Integer = Margin

            For Each col As String In MAT.First.Skip(1)
                x += size.Width
                Call Gr.Graphics.DrawString(TagDict(col), DrawingFont, Brushes.Black, New Point(x, y))
            Next

            y = Margin + size.Height

            For Each row In MAT.Skip(1)
                x = Margin

                Call Gr.Graphics.DrawString(TagDict(row.First), DrawingFont, Brushes.Black, New Point(x, y))

                x += size.Width

                For Each col As String In row.Skip(1)
                    Call Gr.Graphics.FillRectangle(New SolidBrush(ColorValues.GetValue(Val(col))), New Rectangle(New Point(x, y), DotSize))
                    Call Gr.Graphics.DrawString(col, DrawingFont, Brushes.Black, New Point(x, y))
                    x += size.Width
                Next

                y += size.Height
            Next

            Return Gr.ImageResource
        End Function


        ''' <summary>
        ''' 绘制上三角形
        ''' 
        '''      tag1 tag2 tag3 tag4 ...
        '''      ...  ...  ...  ...  ... tag1
        '''           ...  ...  ...  ... tag2
        '''                ...  ...  ... tag3
        '''                     ...  ... tag4
        '''                          ... tag5
        ''' </summary>
        ''' <param name="MAT"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function NormalMatrixTriangular(MAT As DocumentStream.File) As Image
            Dim Tags As String() = {(From row In MAT.Skip(1) Select row.First).ToArray, MAT.First.Skip(1).ToArray}.Unlist.Distinct.ToArray
            Dim TagDict As Dictionary(Of String, String) = CreateAlphabetTagSerials(Tags)
            Dim DrawingFont As Font = New Font(FontFace.Ubuntu, 12)
            Dim MatrixValueString = (From s As String In (From row In MAT.Skip(1) Select row.Skip(1)).Unlist Select s Order By Len(s) Descending).ToArray
            Dim size = MatrixValueString.First.MeasureString(DrawingFont)
            Dim MatrixValues As Double() = (From s As String In MatrixValueString Select Val(s)).ToArray
            Dim ColorValues = GeneticClock.ColorRender.GetDesityRule(MatrixValues, 100)
            Dim DotSize = New Size(size.Width, size.Height)
            Dim Gr = (New Size(3 * Margin + DotSize.Width * MAT.Width, 3 * Margin + DotSize.Height * MAT.Count * 2)).CreateGDIDevice

            Dim x As Integer = Margin, y As Integer = Margin

            For Each col As String In MAT.First.Skip(2) '绘制首行的标题
                x += size.Width
                Call Gr.Graphics.DrawString(TagDict(col), DrawingFont, Brushes.Black, New Point(x, y))
            Next

            y = Margin + size.Height

            Dim i As Integer = 1

            MAT = MAT.Skip(1).ToArray
            MAT = MAT.Take(MAT.Count - 1).ToArray

            For Each row In MAT
                x = Margin
                x += size.Width * i - i / 2

                For Each col As String In row.Skip(i + 1)
                    Call Gr.Graphics.FillRectangle(New SolidBrush(ColorValues.GetValue(Val(col))), New Rectangle(New Point(x, y), DotSize))
                    Call Gr.Graphics.DrawString(col, DrawingFont, Brushes.Black, New Point(x, y))
                    x += size.Width
                Next

                Call Gr.Graphics.DrawString(TagDict(row.First), DrawingFont, Brushes.Black, New Point(x, y))

                y += size.Height
                i += 1
            Next

            DrawingFont = New Font(FontFace.Ubuntu, 8)
            x = Margin
            Call ColorValues.DrawingDensityRule(Gr.Graphics, New Point(x, y - 50), DrawingFont, Gr.Width * 0.3)
            y += size.Height

            For Each item In TagDict
                Dim col As String = String.Format("{0}.  {1}", item.Value, item.Key)
                Call Gr.Graphics.DrawString(col, DrawingFont, Brushes.Black, New Point(x, y))
                y += size.Height
            Next

            Return Gr.ImageResource
        End Function

        Const ALPHABET As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="dat">数据必须是已经去除掉了重复的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateAlphabetTagSerials(dat As String()) As Dictionary(Of String, String)
            If dat.Count <= 26 Then
                Return (From I As Integer In dat.Sequence Let s As String = dat(I) Select Tag = ALPHABET(I).ToString, s).ToArray.ToDictionary(Function(item) item.s, Function(item) item.Tag)
            Else
                Dim ChunkBuffer = dat.CreateSlideWindows(26)
                Dim list = New List(Of KeyValuePair(Of String, String))
                Dim prefix As String = ""

                For i As Integer = 0 To ChunkBuffer.Count - 1
                    dat = ChunkBuffer(i).Elements
                    Call list.AddRange((From j As Integer In dat.Sequence Let s As String = dat(j) Select New KeyValuePair(Of String, String)(s, prefix & ALPHABET(j).ToString)).ToArray)
                    prefix &= ALPHABET(i)
                Next

                Return list.ToDictionary(Function(item) item.Key, elementSelector:=Function(item) item.Value)
            End If
        End Function
    End Module
End Namespace

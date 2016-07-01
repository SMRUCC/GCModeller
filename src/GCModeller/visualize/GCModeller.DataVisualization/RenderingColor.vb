#Region "Microsoft.VisualBasic::57d71857fcb25a93190aadb8a33283fe, ..\GCModeller\visualize\GCModeller.DataVisualization\RenderingColor.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.ComponentModel

Public Module RenderingColor

    Public Function NeutralizeColor(data As Color()) As Color
        Dim _r As Integer = CInt((From x As Color In data Select x.R).Average(Function(n) CInt(n)))
        Dim _g As Integer = CInt((From x As Color In data Select x.G).Average(Function(n) CInt(n)))
        Dim _b As Integer = CInt((From x As Color In data Select x.B).Average(Function(n) CInt(n)))
        Return Color.FromArgb(data.First.A, _r, _g, _b)
    End Function

    ''' <summary>
    ''' 材质映射
    ''' </summary>
    ''' <param name="categories">假若这个参数为空，则默认是使用COG分类的映射规则</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <Extension>
    Public Function CategoryMapsTextures(textures As Image(), Optional categories As String() = Nothing) As Dictionary(Of String, Brush)
        If categories.IsNullOrEmpty Then
            categories = LinqAPI.Exec(Of String) <=
                From category As COG.Category
                In COG.Function.Default.Categories
                Select From [class] As KeyValuePair
                       In category.SubClasses
                       Select [class].Key
            categories = categories.Distinct.ToArray
        End If

        Dim mapping As Dictionary(Of String, Brush) =
            If(categories.Length > textures.Length,
               __interpolateMapping(categories, textures), ' 材质不足，则会使用颜色来绘制
               __directlyMapping(categories, textures))    ' 直接映射

        Return mapping
    End Function

    ''' <summary>
    ''' 材质不足，则会使用颜色来绘制
    ''' </summary>
    ''' <returns></returns>
    Private Function __interpolateMapping(categories As String(), Textures As Image()) As Dictionary(Of String, Brush)
        Dim tmpBuf As String() = New String(Textures.Length - 1) {}
        Dim hash As Dictionary(Of String, Brush) = New Dictionary(Of String, Brush)

        Call Array.ConstrainedCopy(categories, 0, tmpBuf, 0, tmpBuf.Length)

        For i As Integer = 0 To tmpBuf.Length - 1
            Call hash.Add(tmpBuf(i), New TextureBrush(Textures(i)))
        Next

        '剩余的使用颜色
        Dim ColorList As List(Of Color) = AllDotNetPrefixColors.ToList
        categories = categories.Skip(tmpBuf.Count).ToArray
        Dim ChunkBuffer = categories.CreateSlideWindows(Textures.Count, Textures.Count)
        Dim J As Integer = 0

        Do While True
            For Each CatList In ChunkBuffer
                Dim Color As Color = ColorList(J)

                For i As Integer = 0 To CatList.Elements.Length - 1
                    Dim res = TextureResourceLoader.AdjustColor(Textures(i), Color)
                    Call hash.Add(CatList(i), New TextureBrush(res))
                Next

                J += 1
            Next
        Loop

        Return hash
    End Function

    ''' <summary>
    ''' 直接映射
    ''' </summary>
    ''' <param name="categories"></param>
    ''' <returns></returns>
    Private Function __directlyMapping(categories As String(), Textures As Image()) As Dictionary(Of String, Brush)
        Dim DictData As Dictionary(Of String, Brush) = New Dictionary(Of String, Brush)

        For i As Integer = 0 To categories.Count - 1
            Call DictData.Add(categories(i), New TextureBrush(Textures(i)))
        Next

        Return DictData
    End Function

    ''' <summary>
    ''' 这是一个很通用的颜色谱创建函数
    ''' </summary>
    ''' <param name="categories">当不为空的时候，会返回一个列表，其中空字符串会被排除掉，故而在返回值之中需要自己添加一个空值的默认颜色</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function InitCOGColors(categories As String()) As Dictionary(Of String, Color)
        If Not categories.IsNullOrEmpty Then
            Return GenerateColorProfiles(categories)
        End If

        Dim CogCategory As COG.Function =
            COG.Function.Default
        Dim f As Double = 255 / CogCategory.Categories.Length
        Dim R As Double = f
        Dim COGColors As New Dictionary(Of String, Color)
        Dim cl As Color

        For Each cata As COG.Category In CogCategory.Categories
            Dim f2 As Double = 255 / cata.SubClasses.Length
            Dim G As Double = f2

            For Each [class] As KeyValuePair In cata.SubClasses
                cl = Color.FromArgb(220, R, G, 255 * RandomDouble())
                G += f2

                Call COGColors.Add([class].Key, cl)
            Next

            R += f
        Next

        Return COGColors
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="categories"></param>
    ''' <param name="removeUsed">是否移除已经使用过的元素，这样子就会产生不重复的颜色</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function GenerateColorProfiles(categories As IEnumerable(Of String), Optional removeUsed As Boolean = True) As Dictionary(Of String, Color)
        Dim Colors As New Dictionary(Of String, Color)
        Dim Rs As New List(Of Integer)(255.Sequence.Randomize)
        Dim Gs As New List(Of Integer)(255.Sequence.Randomize)
        Dim Bs As New List(Of Integer)(255.Sequence.Randomize)
        Dim R, G, B As Integer

        For Each cl As String In From s As String
                                 In categories
                                 Where Not String.IsNullOrEmpty(s)
                                 Select s

            Call VBMath.Randomize() : R = RandomDouble() * (Rs.Count - 1)
            Call VBMath.Randomize() : G = RandomDouble() * (Gs.Count - 1)
            Call VBMath.Randomize() : B = RandomDouble() * (Bs.Count - 1)

            Call Colors.Add(cl, Color.FromArgb(Rs(R), Gs(G), Bs(B)))

            If removeUsed Then
                Call Rs.RemoveAt(R)
                Call Gs.RemoveAt(G)
                Call Bs.RemoveAt(B)
            End If
        Next

        Return Colors
    End Function
End Module

﻿#Region "Microsoft.VisualBasic::172ac1af12fc401fad035a14c83e9dbc, GCModeller.DataVisualization\RenderingColor.vb"

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

    ' Module RenderingColor
    ' 
    '     Function: __directlyMapping, __interpolateMapping, CategoryMapsTextures, GenerateColorProfiles, InitCOGColors
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI

Public Module RenderingColor

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
                From category As COG.Catalog
                In COG.Function.Default.Catalogs
                Select From [class] As KeyValuePair(Of Char, String)
                       In category.SubClasses
                       Select CStr([class].Key)
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
        Dim ColorList As New List(Of Color)(AllDotNetPrefixColors)
        Dim wins As SlideWindow(Of String)() = categories _
            .Skip(tmpBuf.Length) _
            .CreateSlideWindows(Textures.Length, Textures.Length)
        Dim J As Integer = 0

        Do While True
            For Each cats As SlideWindow(Of String) In wins
                Dim Color As Color = ColorList(J)

                For i As Integer = 0 To cats.Items.Length - 1
                    Dim res As Image = TextureResourceLoader.AdjustColor(Textures(i), Color)
                    Call hash.Add(cats(i), New TextureBrush(res))
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
    Private Function __directlyMapping(categories$(), Textures As Image()) As Dictionary(Of String, Brush)
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
    Public Function InitCOGColors(categories$(), Optional alpha% = 230) As Dictionary(Of String, Color)
        If Not categories.IsNullOrEmpty Then
            Return GenerateColorProfiles(categories)
        End If

        Dim CogCategory As COG.Function = COG.Function.Default
        Dim f As Double = 255 / CogCategory.Catalogs.Length
        Dim R As Double = f
        Dim COGColors As New Dictionary(Of String, Color)
        Dim cl As Color
        Dim rand As New Random

        For Each cata As COG.Catalog In CogCategory.Catalogs
            Dim f2 As Double = 255 / cata.SubClasses.Count
            Dim G As Double = f2

            For Each [class] As KeyValuePair(Of Char, String) In cata.SubClasses
                cl = Color.FromArgb(alpha, R, G, 255 * rand.NextDouble())
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
        Dim Rs As New List(Of Integer)(255.Sequence.Shuffles)
        Dim Gs As New List(Of Integer)(255.Sequence.Shuffles)
        Dim Bs As New List(Of Integer)(255.Sequence.Shuffles)
        Dim R, G, B As Integer
        Dim rand As New Random

        For Each cl As String In From s As String
                                 In categories
                                 Where Not String.IsNullOrEmpty(s)
                                 Select s

            R = rand.NextDouble() * (Rs.Count - 1)
            G = rand.NextDouble() * (Gs.Count - 1)
            B = rand.NextDouble() * (Bs.Count - 1)

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

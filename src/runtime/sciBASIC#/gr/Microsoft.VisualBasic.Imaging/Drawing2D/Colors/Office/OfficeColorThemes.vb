﻿#Region "Microsoft.VisualBasic::67a546b6e2cb80d94c4ec9f4a836c883, gr\Microsoft.VisualBasic.Imaging\Drawing2D\Colors\Office\OfficeColorThemes.vb"

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

    '     Module OfficeColorThemes
    ' 
    '         Properties: Aspect, Marquee, Office2010, Office2016, Paper
    '                     Slipstream, Themes
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetAccentColors
    ' 
    '         Sub: InternalLoadAllThemes
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace Drawing2D.Colors.OfficeAccent

    Public Module OfficeColorThemes

        Public ReadOnly Property Office2016 As OfficeColorTheme
        Public ReadOnly Property Office2010 As OfficeColorTheme
        Public ReadOnly Property Slipstream As OfficeColorTheme
        Public ReadOnly Property Marquee As OfficeColorTheme
        Public ReadOnly Property Aspect As OfficeColorTheme
        Public ReadOnly Property Paper As OfficeColorTheme

        Sub New()
            Office2016 = OfficeColorTheme.LoadFromXml(My.Resources.Default_Office)
            Office2010 = OfficeColorTheme.LoadFromXml(My.Resources.Default_Office2007_2010)
            Marquee = OfficeColorTheme.LoadFromXml(My.Resources.Default_Marquee)
            Aspect = OfficeColorTheme.LoadFromXml(My.Resources.Default_Aspect)
            Paper = OfficeColorTheme.LoadFromXml(My.Resources.Default_Paper)
            Slipstream = OfficeColorTheme.LoadFromXml(My.Resources.Default_Slipstream)

            Call InternalLoadAllThemes()
        End Sub

        ''' <summary>
        ''' All office color themes
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Themes As New Dictionary(Of OfficeColorTheme)

        Private Sub InternalLoadAllThemes()
            Dim resMgr As Type = GetType(My.Resources.Resources)
            Dim datas As IEnumerable(Of PropertyInfo) =
                DataFramework _
                .Schema(resMgr, PropertyAccess.Readable, BindingFlags.NonPublic Or BindingFlags.Static, True) _
                .Where(Function(k) InStr(k.Key, "Default_") = 1) _
                .Select(Function(x) x.Value) _
                .ToArray

            For Each theme As PropertyInfo In datas
                Dim xml As String = TryCast(theme.GetValue(Nothing, Nothing), String)
                Dim t As OfficeColorTheme = OfficeColorTheme.LoadFromXml(xml)

                t.name = t.name.Replace("Default_", "")
                Call Themes.Add(t) ' 顺序不能变换，否则键名就不一致了
            Next
        End Sub

        ''' <summary>
        ''' If found failure, default is reutrns the theme <see cref="Office2016"/>
        ''' </summary>
        ''' <param name="theme$"></param>
        ''' <returns></returns>
        Public Function GetAccentColors(theme$) As Color()
            If Themes.ContainsKey(theme) Then
                Return Themes(theme).GetAccentColors
            Else
                For Each t As OfficeColorTheme In Themes.Values
                    If t.name.TextEquals(theme) Then
                        Return t.GetAccentColors
                    End If
                Next
            End If

            Return Office2016.GetAccentColors
        End Function
    End Module
End Namespace

#Region "Microsoft.VisualBasic::a2938fa4acec5211c4584bf5e9d42815, ..\GNUplot\GNUplot\StoredPlot.vb"

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


Imports Microsoft.VisualBasic.Serialization.JSON

Public Class StoredPlot

    Public File As String = Nothing
    Public [Function] As String = Nothing
    Public X As Double()
    Public Y As Double()
    Public Z As Double()
    Public ZZ As Double(,)
    Public YSize As Integer
    Public Options As String
    Public PlotType As PlotTypes
    Public LabelContours As Boolean

    Public Sub New()
    End Sub

    Public Sub New(functionOrfilename As String, Optional options__1 As String = "", Optional plotType__2 As PlotTypes = PlotTypes.PlotFileOrFunction)
        If IsFile(functionOrfilename) Then
            File = functionOrfilename
        Else
            [Function] = functionOrfilename
        End If
        Options = options__1
        PlotType = plotType__2
    End Sub

    Public Sub New(y__1 As Double(), Optional options__2 As String = "")
        Y = y__1
        Options = options__2
        PlotType = PlotTypes.PlotY
    End Sub

    Public Sub New(x__1 As Double(), y__2 As Double(), Optional options__3 As String = "")
        X = x__1
        Y = y__2
        Options = options__3
        PlotType = PlotTypes.PlotXY
    End Sub

    '3D data
    Public Sub New(sizeY As Integer, z__1 As Double(), Optional options__2 As String = "", Optional plotType__3 As PlotTypes = PlotTypes.SplotZ)
        YSize = sizeY
        Z = z__1
        Options = options__2
        PlotType = plotType__3
    End Sub

    Public Sub New(x__1 As Double(), y__2 As Double(), z__3 As Double(), Optional options__4 As String = "", Optional plotType__5 As PlotTypes = PlotTypes.SplotXYZ)
        If x__1.Length < 2 Then
            YSize = 1
        Else
            For YSize = 1 To x__1.Length - 1
                If x__1(YSize) <> x__1(YSize - 1) Then
                    Exit For
                End If
            Next
        End If
        Z = z__3
        Y = y__2
        X = x__1
        Options = options__4
        PlotType = plotType__5
    End Sub

    Public Sub New(zz__1 As Double(,), Optional options__2 As String = "", Optional plotType__3 As PlotTypes = PlotTypes.SplotZZ)
        ZZ = zz__1
        Options = options__2
        PlotType = plotType__3
    End Sub

    Private Function IsFile(functionOrFilename As String) As Boolean
        Dim dot As Integer = functionOrFilename.LastIndexOf(".")
        If dot < 1 Then
            Return False
        End If
        If Char.IsLetter(functionOrFilename(dot - 1)) OrElse Char.IsLetter(functionOrFilename(dot + 1)) Then
            Return True
        End If
        Return False
    End Function

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

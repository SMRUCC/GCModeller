﻿#Region "Microsoft.VisualBasic::1e0b3808e202f7d750b04aa905341d6d, Microsoft.VisualBasic.Core\ApplicationServices\Terminal\Utility\EventProc.vb"

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

    '     Class EventProc
    ' 
    '         Properties: Capacity
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Tick, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports sys = System.Math

Namespace Terminal.Utility

    ''' <summary>
    ''' Generates the task progress for the console output.(处理任务进度)
    ''' </summary>
    Public Class EventProc

        ''' <summary>
        ''' The total <see cref="Tick"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Capacity As Integer
            Get
                Return _capacity
            End Get
            Set(value As Integer)
                _capacity = value
                delta = Capacity / 100
                current = CInt(_capacity * percentage)
            End Set
        End Property

        Dim _capacity As Integer
        Dim delta As Integer
        Dim tag As String
        Dim out As StreamWriter
        Dim current As VBInteger = 0

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="n"></param>
        ''' <param name="tag"></param>
        ''' <param name="out">Default is <see cref="Console"/></param>
        Sub New(n As Integer, <CallerMemberName> Optional tag As String = "", Optional out As StreamWriter = Nothing)
            Me.Capacity = n
            Me.tag = tag
            Me.out = If(out Is Nothing,
                New StreamWriter(Console.OpenStandardOutput),
                out)

            If String.IsNullOrEmpty(Me.tag) Then
                Me.tag = vbTab
            End If
        End Sub

        ''' <summary>
        ''' 会自动输出进度的
        ''' </summary>
        ''' <returns></returns>
        Public Function Tick() As Integer
            If delta = 0 Then
                Return 0
            End If

            If ++current Mod delta = 0 Then
                Call ToString.__DEBUG_ECHO
            Else
                Call out.Write(".")
            End If

            Return current
        End Function

        ''' <summary>
        ''' Current progress percentage.
        ''' </summary>
        Dim percentage As Double
        Dim sw As Stopwatch = Stopwatch.StartNew
        ''' <summary>
        ''' Previous <see cref="Stopwatch.ElapsedMilliseconds"/>
        ''' </summary>
        Dim preElapsedMilliseconds As Long

        ''' <summary>
        ''' Generates progress output
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            If Capacity = 0 Then
                Return ""
            End If

            Dim dt As Long = sw.ElapsedMilliseconds - preElapsedMilliseconds
            preElapsedMilliseconds = sw.ElapsedMilliseconds
            percentage = current / Capacity

            Return $" [{tag}, {dt}ms] * ...... {sys.Round(100 * percentage, 2)}%"
        End Function
    End Class
End Namespace

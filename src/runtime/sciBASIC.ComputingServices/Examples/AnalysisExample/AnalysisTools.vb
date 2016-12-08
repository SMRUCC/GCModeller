#Region "Microsoft.VisualBasic::3e29b77c6523cdb17354151e4fcb01f0, ..\sciBASIC.ComputingServices\Examples\AnalysisExample\AnalysisTools.vb"

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

Imports System.IO
Imports Microsoft.VisualBasic

Public Module API

    Public Function LongTest1(file As Stream, a2 As String) As String()
        Dim buf As Byte() = file.CopyStream
        Dim txt As String = System.Text.Encoding.Default.GetString(buf)
        Dim lines As String() = txt.lTokens
        For Each x In lines
            Call Console.WriteLine(x)
        Next

        Call MsgBox(a2, MsgBoxStyle.Critical)

        Return lines
    End Function
End Module

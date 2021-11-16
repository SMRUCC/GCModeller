#Region "Microsoft.VisualBasic::4573870a50c7860fe19f56447746e257, R#\bioconductor\RDotNET.vb"

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

    ' Module RDotNET
    ' 
    '     Function: push
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.Serialization
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("R")>
Module RDotNET

    ''' <summary>
    ''' push any .NET object into R runtime environment
    ''' </summary>
    ''' <param name="any"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("push")>
    Public Function push(any As Object, Optional env As Environment = Nothing) As String
        Dim codepage As Encoding = Encodings.UTF8WithoutBOM.CodePage

        If any Is Nothing Then
            Return base.c(x:=Nothing)
        ElseIf TypeOf any Is list Then
            Return SaveRda.Push(DirectCast(any, list).slots, codepage)
        ElseIf TypeOf any Is vector Then
            With DirectCast(any, vector)
                Dim vector As String = SaveRda.Push(.data, codepage)
                base.names(vector) = .getNames
                Return vector
            End With
        ElseIf TypeOf any Is vbObject Then
            Return SaveRda.Push(DirectCast(any, vbObject).target, codepage)
        ElseIf TypeOf any Is pipeline Then
            Return SaveRda.Push(DirectCast(any, pipeline).createVector(env).data, codepage)
        Else
            Return SaveRda.Push(any, codepage)
        End If
    End Function
End Module

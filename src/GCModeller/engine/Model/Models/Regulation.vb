﻿#Region "Microsoft.VisualBasic::9f59f965c601dfdd2e72060388513368, Model\Regulation.vb"

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

' Structure Regulation
' 
' 
' 
' Enum Processes
' 
'     Flux, Transcription, Translation
' 
'  
' 
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language

Public Structure Regulation

    Public name As String
    ''' <summary>
    ''' Compound / RNA / Proteins
    ''' </summary>
    Public regulator As String
    ''' <summary>
    ''' The regulated target process name
    ''' </summary>
    Public process As String
    ''' <summary>
    ''' The type of the target process
    ''' </summary>
    Public type As Processes
    ''' <summary>
    ''' + positive: accelerate
    ''' + negative: inhibition
    ''' </summary>
    Public effects As Double

    Public Overrides Function ToString() As String
        Dim effectString$ = "accelerate" Or "inhibition".When(effects < 0)
        Dim descript$ = $"[{type.ToString}] {regulator} {effectString} {process}"
        Return descript
    End Function

End Structure

''' <summary>
''' 过程类型
''' </summary>
Public Enum Processes
    ''' <summary>
    ''' 转录过程
    ''' </summary>
    Transcription
    ''' <summary>
    ''' 翻译过程
    ''' </summary>
    Translation
    ''' <summary>
    ''' 代谢过程
    ''' </summary>
    MetabolicProcess
End Enum

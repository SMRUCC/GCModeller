#Region "Microsoft.VisualBasic::73e16bb187697fae5299a8b14fdcdff8, analysis\SequenceToolkit\SmithWaterman\Extension\HSP.vb"

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

' Class HSP
' 
'     Properties: LengthHit, LengthQuery, Query, QueryLength, Subject
'                 SubjectLength
' 
'     Function: CreateHSP, CreateObject
' 
' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman

''' <summary>
''' 对<see cref="LocalHSPMatch(Of Char)"/>的XML文件保存文件格式对象
''' </summary>
Public Class HSP : Inherits Match

    Public Property Query As String
    Public Property Subject As String

    <XmlAttribute> Public Property QueryLength As Integer
    <XmlAttribute> Public Property SubjectLength As Integer

    Sub New()
    End Sub

    Sub New(local As LocalHSPMatch(Of Char))
        Call MyBase.New(local)

        Query = New String(local.seq1)
        Subject = New String(local.seq2)
        QueryLength = local.QueryLength
        SubjectLength = local.SubjectLength
    End Sub

End Class

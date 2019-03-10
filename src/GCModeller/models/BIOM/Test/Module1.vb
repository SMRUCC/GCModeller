﻿#Region "Microsoft.VisualBasic::373608423df5491d4c4912c3e9c129b8, models\BIOM\Test\Module1.vb"

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

' Module Module1
' 
'     Sub: BoxPlot, Main
' 
' Class DDDD
' 
'     Properties: d
' 
' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.MIME.application.netCDF
Imports Microsoft.VisualBasic.Serialization.JSON

Module Module1

    Const JsonLongTime$ = "\d+-\d+-\d+T\d+:\d+:\d+\.\d+"

    Sub Main()

        Call testCDFBIOM()

        '2016-10-31T17:30:49.768484

        Dim ddd = Regex.Match("2016-10-31T17:30:49.768484", JsonLongTime).Value
        Dim json = Date.Parse(ddd).GetJson

        Dim biom = SMRUCC.genomics.foundation.BIOM.v10.Json(Of Double).LoadFile("C:\Users\xieguigang\Desktop\predictions_ko.L3.biom.json")

        Call biom.GetJson(indent:=True).SaveTo("C:\Users\xieguigang\Desktop\predictions_ko.L3.biom.formatted.json")

        Pause()
    End Sub


    Sub testCDFBIOM()

        Dim file = "D:\GCModeller\src\GCModeller\models\EP418446_K40_BS1D.otu_table.biom"
        Dim reader As New HDF5Reader(file)

        Pause()
    End Sub

    Sub BoxPlot()

        '

    End Sub

End Module

Public Class DDDD


    Public Property d As Date

End Class

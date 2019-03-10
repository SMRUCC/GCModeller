﻿#Region "Microsoft.VisualBasic::e498829f11290d8836a2e1592cfab139, gr\Microsoft.VisualBasic.Imaging\test\ExifTest.vb"

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

    ' Module ExifTest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Imaging.Driver

Module ExifTest

    Sub Main()
        Dim adapter As New JpegMetadataAdapter("D:\smartnucl_integrative\dist\urine\biodeepMSMS\neg\plot\biodeepMSMS\85.0281@75_1,4-Butynediol.jpg")

        adapter.Metadata.Title = "Beach"
        adapter.Metadata.Subject = "Summer holiday 2014"
        adapter.Metadata.Rating = 4
        adapter.Metadata.Keywords.Add("beach")
        adapter.Metadata.Comments = "This is a comment."

        Console.WriteLine(adapter.Save())

        Pause()
    End Sub
End Module

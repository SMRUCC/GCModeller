#Region "Microsoft.VisualBasic::ebcb89d539221bfc564be60002ca733c, engine\IO\GCMarkupLanguage\Banner.vb"

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


    ' Code Statistics:

    '   Total Lines: 30
    '    Code Lines: 21 (70.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (30.00%)
    '     File Size: 929 B


    ' Module Banner
    ' 
    '     Sub: Print
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO

Public Module Banner

    Public Const BannerInformation As String = "

 Welcome To
      _____  _____ __  __           _      _ _           
     / ____|/ ____|  \/  |         | |    | | |          
    | |  __| |    | \  / | ___   __| | ___| | | ___ _ __ 
    | | |_ | |    | |\/| |/ _ \ / _` |/ _ \ | |/ _ \ '__|
    | |__| | |____| |  | | (_) | (_| |  __/ | |  __/ |   
     \_____|\_____|_|  |_|\___/ \__,_|\___|_|_|\___|_|   
                                                      
                      Virtual Cell Simulation Tool - 2025

                         https://biocad.innovation.ac.cn/
                         https://gcmodeller.org/

"

    Public Sub Print(console As TextWriter)
        For Each line As String In Banner.BannerInformation.LineTokens
            Call console.WriteLine(line)
        Next

        Call console.Flush()
    End Sub

End Module


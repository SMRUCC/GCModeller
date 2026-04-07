#Region "Microsoft.VisualBasic::b69db149cff14ea69bf2f5ccebf64e21, data\RegulonDatabase\test\Program.vb"

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

    '   Total Lines: 17
    '    Code Lines: 13 (76.47%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 4 (23.53%)
    '     File Size: 394 B


    ' Module Program
    ' 
    '     Sub: download_regprecise, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System
Imports SMRUCC.genomics.Data

Module Program
    Sub Main(args As String())
        download_regprecise()
        Console.WriteLine("Hello World!")
    End Sub

    Sub download_regprecise()
        Dim target = "F:\ecoli\regprecise"

        WebServiceUtils.Proxy = "http://127.0.0.1:10809"

        Call Regprecise.WebAPI.Download(target)
    End Sub
End Module

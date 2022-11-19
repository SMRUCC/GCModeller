#Region "Microsoft.VisualBasic::f7862c31903d3dfd4f5b482f485d1ce2, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayMap\PathwayMapDownloads.vb"

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

    '   Total Lines: 21
    '    Code Lines: 16
    ' Comment Lines: 0
    '   Blank Lines: 5
    '     File Size: 602 B


    '     Module PathwayMapDownloads
    ' 
    '         Sub: SetMapImage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http

Namespace Assembly.KEGG.DBGET.bGetObject

    Module PathwayMapDownloads

        <Extension>
        Public Sub SetMapImage(pathwayMap As PathwayMap, image As Image)
            Dim base64$ = Base64Codec.ToBase64String(image)
            Dim s As New List(Of String)

            For i As Integer = 1 To base64.Length Step 120
                s.Add(Mid(base64, i, 120))
            Next

            pathwayMap.Map = s.JoinBy(vbCrLf)
        End Sub
    End Module
End Namespace

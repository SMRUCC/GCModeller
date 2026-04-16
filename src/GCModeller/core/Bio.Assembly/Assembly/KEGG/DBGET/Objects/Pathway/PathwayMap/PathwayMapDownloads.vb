#Region "Microsoft.VisualBasic::2c37a1fdef085a46eb7ea2006fa8a3b8, core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayMap\PathwayMapDownloads.vb"

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

    '   Total Lines: 31
    '    Code Lines: 25 (80.65%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (19.35%)
    '     File Size: 984 B


    '     Module PathwayMapDownloads
    ' 
    '         Sub: SetMapImage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http

#If NET48 Then
Imports Image = System.Drawing.Image
#Else
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports LineCap = Microsoft.VisualBasic.Imaging.LineCap
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
#End If

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

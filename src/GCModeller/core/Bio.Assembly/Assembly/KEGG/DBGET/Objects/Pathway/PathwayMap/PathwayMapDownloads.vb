#Region "Microsoft.VisualBasic::9c53a9498ad11d150da752b3dfcf5779, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayMap\PathwayMapDownloads.vb"

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

    '   Total Lines: 153
    '    Code Lines: 121
    ' Comment Lines: 9
    '   Blank Lines: 23
    '     File Size: 7.33 KB


    '     Module PathwayMapDownloads
    ' 
    '         Function: DownloadAll, mapParserInternal, SolveEntries
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

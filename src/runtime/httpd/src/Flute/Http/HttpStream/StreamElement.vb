#Region "Microsoft.VisualBasic::95c54a79128dffdd239e152ce2fb7c76, G:/GCModeller/src/runtime/httpd/src/Flute//Http/HttpStream/StreamElement.vb"

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

    '   Total Lines: 15
    '    Code Lines: 12
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 497 B


    '     Class StreamElement
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Core.HttpStream

    Friend Class StreamElement

        Public ContentType As String
        Public Name As String
        Public Filename As String
        Public Start As Long
        Public Length As Long

        Public Overrides Function ToString() As String
            Return "ContentType " & ContentType & ", Name " & Name & ", Filename " & Filename & ", Start " & Start.ToString() & ", Length " & Length.ToString()
        End Function
    End Class
End Namespace

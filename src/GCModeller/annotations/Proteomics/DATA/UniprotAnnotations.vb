#Region "Microsoft.VisualBasic::1c5afc4c28ae858729de2d87ad3ec384, GCModeller\annotations\Proteomics\DATA\UniprotAnnotations.vb"

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

    '   Total Lines: 19
    '    Code Lines: 16
    ' Comment Lines: 0
    '   Blank Lines: 3
    '     File Size: 570 B


    ' Class UniprotAnnotations
    ' 
    '     Properties: Data, EC, fullName, geneName, GO
    '                 ID, KO, ORF, organism, uniprot
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Public Class UniprotAnnotations

    Public Property ID As String
    Public Property uniprot As String
    Public Property ORF As String
    Public Property geneName As String
    Public Property fullName As String
    Public Property GO As String()
    Public Property EC As String()
    Public Property KO As String
    Public Property organism As String
    Public Property Data As Dictionary(Of String, String)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

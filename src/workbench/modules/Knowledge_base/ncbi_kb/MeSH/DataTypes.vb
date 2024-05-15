#Region "Microsoft.VisualBasic::20308eb909ec98f34b930c64ba04fb41, modules\Knowledge_base\ncbi_kb\MeSH\DataTypes.vb"

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

    '   Total Lines: 24
    '    Code Lines: 16
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 525 B


    '     Class XmlString
    ' 
    '         Properties: [String]
    ' 
    '         Function: ToString
    ' 
    '     Class XmlDate
    ' 
    '         Properties: Day, Month, Year
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace MeSH

    Public Class XmlString

        Public Property [String] As String

        Public Overrides Function ToString() As String
            Return [String]
        End Function

    End Class

    Public Class XmlDate

        Public Property Year As Integer
        Public Property Month As Integer
        Public Property Day As Integer

        Public Overrides Function ToString() As String
            Return $"{Year}-{Month}-{Day}"
        End Function

    End Class
End Namespace

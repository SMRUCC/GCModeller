#Region "Microsoft.VisualBasic::0d57b73dcfb9dbeb76625fbd3e104ba2, engine\Model\LocalRepository\Reaction.vb"

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

    '   Total Lines: 41
    '    Code Lines: 31 (75.61%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (24.39%)
    '     File Size: 1.20 KB


    '     Class Reaction
    ' 
    '         Properties: guid, law, left, name, reaction
    '                     right
    ' 
    '         Function: ToString
    ' 
    '     Class Substrate
    ' 
    '         Properties: factor, location, molecule_id
    ' 
    '         Function: ToString
    ' 
    '     Class LawData
    ' 
    '         Properties: ec_number, lambda, metabolite_id, params
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace WebJSON

    Public Class Reaction

        Public Property guid As String
        Public Property name As String
        Public Property reaction As String
        Public Property left As Substrate()
        Public Property right As Substrate()
        Public Property law As LawData()

        Public Overrides Function ToString() As String
            Return $"{guid} - {name}"
        End Function
    End Class

    Public Class Substrate

        Public Property molecule_id As UInteger
        Public Property factor As Double
        Public Property location As UInteger

        Public Overrides Function ToString() As String
            Return $"{{{factor}}} {molecule_id}"
        End Function
    End Class

    Public Class LawData

        Public Property params As Dictionary(Of String, String)
        Public Property lambda As String
        Public Property metabolite_id As String
        Public Property ec_number As String

        Public Overrides Function ToString() As String
            Return $"{params.GetJson} -> {lambda}"
        End Function
    End Class
End Namespace

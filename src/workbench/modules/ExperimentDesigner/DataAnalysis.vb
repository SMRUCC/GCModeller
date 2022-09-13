#Region "Microsoft.VisualBasic::68cea5e4ceb35c7ce43d95de3e0f41a7, modules\ExperimentDesigner\DataAnalysis.vb"

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

    '   Total Lines: 40
    '    Code Lines: 32
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 893 B


    ' Class DataAnalysis
    ' 
    '     Properties: control, designs, experiment, size
    ' 
    ' Class DataGroup
    ' 
    '     Properties: color, sample_id, sampleGroup, shape
    ' 
    ' /********************************************************************************/

#End Region

Public Class DataAnalysis

    Public Property designs As DataGroup()

    Public ReadOnly Property size As Integer
        Get
            Return designs.Length
        End Get
    End Property

    Public ReadOnly Property experiment As DataGroup
        Get
            If size = 2 Then
                Return _designs(0)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public ReadOnly Property control As DataGroup
        Get
            If size = 2 Then
                Return _designs(1)
            Else
                Return Nothing
            End If
        End Get
    End Property

End Class

Public Class DataGroup

    Public Property sampleGroup As String
    Public Property sample_id As String()
    Public Property color As String
    Public Property shape As String

End Class

#Region "Microsoft.VisualBasic::9d2d972159659e739c26c7b4c45bcb88, GCModeller\engine\Dynamics\Drivers\OmicsTuple.vb"

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
    '    Code Lines: 15
    ' Comment Lines: 0
    '   Blank Lines: 4
    '     File Size: 517 B


    '     Class OmicsTuple
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Engine

    Public Class OmicsTuple(Of T)

        Public ReadOnly transcriptome As T
        Public ReadOnly proteome As T
        Public ReadOnly metabolome As T

        Sub New(transcriptome As T, proteome As T, metabolome As T)
            Me.transcriptome = transcriptome
            Me.proteome = proteome
            Me.metabolome = metabolome
        End Sub

        Public Overrides Function ToString() As String
            Return GetType(T).Name
        End Function
    End Class
End Namespace

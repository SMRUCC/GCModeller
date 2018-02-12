#Region "Microsoft.VisualBasic::2e792256bc2542ec845516160c8b1078, core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Files\Species.vb"

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

    '     Class Species
    ' 
    '         Properties: AttributeList
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.MetaCyc.File.DataFiles

    Public Class Species : Inherits DataFile(Of MetaCyc.File.DataFiles.Slots.Specie)

        Public Overrides ReadOnly Property AttributeList As String()
            Get
                Throw New NotImplementedException()
            End Get
        End Property
    End Class
End Namespace

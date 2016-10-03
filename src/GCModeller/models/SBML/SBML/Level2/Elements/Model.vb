#Region "Microsoft.VisualBasic::9d7a0a1d9375f9f910cb8851b0c42205, ..\GCModeller\models\SBML\SBML\Level2\Elements\Model.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports Microsoft.VisualBasic

Namespace Level2.Elements

    Public Class Model : Inherits Components.ModelBase

        Public Property listOfUnitDefinitions As List(Of Elements.unitDefinition)
        Public Property listOfCompartments As List(Of SBML.Components.Compartment)

        ''' <summary>
        ''' 在当前的这个SBML文件之中所定义的代谢物对象的列表 
        ''' </summary>
        ''' <remarks></remarks>
        Public Property listOfSpecies As List(Of Elements.Specie)
        Public Property listOfReactions As List(Of Elements.Reaction)

        Public Sub [AddHandler]()
            For i As Integer = 0 To listOfReactions.Count - 1
                listOfReactions(i).Handle = i
            Next
        End Sub
    End Class
End Namespace

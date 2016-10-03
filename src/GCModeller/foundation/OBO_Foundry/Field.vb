#Region "Microsoft.VisualBasic::7c27acd36a67306b9e08c12ab92dfd70, ..\GCModeller\foundation\OBO_Foundry\Field.vb"

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

Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' Obo term field or tabular indexed column
''' </summary>
<AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
Public Class Field : Inherits Attribute

    Friend _Name As String, _toLower As Boolean

    ''' <summary>
    ''' Tablular column index
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Index As Integer
    Public Shared ReadOnly Property TypeInfo As Type = GetType(Field)

    ''' <summary>
    ''' Init with a field name in the obo term.
    ''' </summary>
    ''' <param name="Name"></param>
    ''' <param name="toLower"></param>
    Sub New(Optional Name As String = "", Optional toLower As Boolean = True)
        Me._Name = Name
        Me._toLower = toLower
    End Sub

    ''' <summary>
    ''' Init with a field index in the table columns
    ''' </summary>
    ''' <param name="Index"></param>
    Sub New(Index As Integer)
        _Index = Index
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

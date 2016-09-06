#Region "Microsoft.VisualBasic::846fb53df83e41ad13bb26a50fb0c90d, ..\GCModeller\data\GO_gene-ontology\Reflections\Field.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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

<AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
Public Class Field : Inherits Attribute

    Friend _Name As String, _toLower As Boolean

    Public ReadOnly Property Index As Integer
    Public Shared ReadOnly Property TypeInfo As Type = GetType(Field)

    Sub New(Optional Name As String = "", Optional toLower As Boolean = True)
        Me._Name = Name
        Me._toLower = toLower
    End Sub

    Sub New(Index As Integer)
        _Index = Index
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

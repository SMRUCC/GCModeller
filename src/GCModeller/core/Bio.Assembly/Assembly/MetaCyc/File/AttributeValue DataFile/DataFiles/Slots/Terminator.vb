#Region "Microsoft.VisualBasic::bab7dea9791874662dd29995c0910bab, ..\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\Terminator.vb"

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

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    Public Class Terminator : Inherits MetaCyc.File.DataFiles.Slots.Object
        <MetaCycField()> Public Property ComponentsOf As String
        <MetaCycField()> Public Property LeftEndPosition As String
        <MetaCycField()> Public Property RightEndPosition As String

        Public Property DNASeq As String

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.terminators
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As Terminator
        '    Dim NewObj As Terminator = New Terminator

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of MetaCyc.File.DataFiles.Slots.Terminator) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Terminators.AttributeList, e), NewObj)

        '    If NewObj.Object.ContainsKey("COMPONENT-OF") Then NewObj.ComponentsOf = NewObj.Object("COMPONENT-OF") Else NewObj.ComponentsOf = String.Empty
        '    If NewObj.Object.ContainsKey("LEFT-END-POSITION") Then NewObj.ComponentsOf = NewObj.Object("LEFT-END-POSITION") Else NewObj.ComponentsOf = String.Empty
        '    If NewObj.Object.ContainsKey("RIGHT-END-POSITION") Then NewObj.ComponentsOf = NewObj.Object("RIGHT-END-POSITION") Else NewObj.ComponentsOf = String.Empty

        '    Return NewObj
        'End Operator
    End Class
End Namespace

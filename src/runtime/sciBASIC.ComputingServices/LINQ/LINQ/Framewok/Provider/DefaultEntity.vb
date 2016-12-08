#Region "Microsoft.VisualBasic::75af855e0de2d17e7e5726150bfa52c0, ..\sciBASIC.ComputingServices\LINQ\LINQ\Framewok\Provider\DefaultEntity.vb"

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

Namespace Framework.Provider

    Public Module DefaultEntity

        <LinqEntity("integer", GetType(Integer))>
        Public Function GetInt32(uri As String) As IEnumerable
            Dim lines As String() = IO.File.ReadAllLines(uri)
            Dim LQuery = (From line As String In lines Select Scripting.CastInteger(line))
            Return LQuery
        End Function

        <LinqEntity("long", GetType(Long))>
        Public Function GetInt64(uri As String) As IEnumerable
            Dim lines As String() = IO.File.ReadAllLines(uri)
            Dim LQuery = (From line As String In lines Select Scripting.CastLong(line))
            Return LQuery
        End Function

        <LinqEntity("double", GetType(Double))>
        Public Function GetDouble(uri As String) As IEnumerable
            Dim lines As String() = IO.File.ReadAllLines(uri)
            Dim LQuery = lines.Select(AddressOf Val)
            Return LQuery
        End Function
    End Module
End Namespace

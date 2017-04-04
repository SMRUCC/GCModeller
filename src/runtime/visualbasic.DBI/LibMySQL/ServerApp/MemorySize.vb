#Region "Microsoft.VisualBasic::8542177925a2b7aaae891852ede7bdd5, ..\visualbasic.DBI\LibMySQL\ServerApp\MemorySize.vb"

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

Imports System.Reflection
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.Text

Namespace ServerApp

    ''' <summary>
    ''' 分析某一个表实体对象的内存占用大小
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class MemorySize(Of T As SQLTable)

        Public ReadOnly Property Type As Type = GetType(T)
        ''' <summary>
        ''' ORM表对象的所有可读的属性集合
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Schema As PropertyInfo()

        Sub New()
            Schema = Type _
                .GetProperties(BindingFlags.Public Or BindingFlags.Instance) _
                .Where(Function(prop) prop.CanRead)
        End Sub

        Shared Sub New()
            Call __sizeOf()
        End Sub

        Shared ReadOnly sizeOf As New Dictionary(Of Type, Integer)
        Shared ReadOnly charSize As Integer = Marshal.SizeOf("$"c)

        Private Shared Sub __sizeOf()
            sizeOf(GetType(Long)) = Marshal.SizeOf(0&)
            sizeOf(GetType(Integer)) = Marshal.SizeOf(0%)
            sizeOf(GetType(Single)) = Marshal.SizeOf(0!)
            sizeOf(GetType(Boolean)) = Marshal.SizeOf(True)
            sizeOf(GetType(Date)) = Marshal.SizeOf(Now)
            sizeOf(GetType(Double)) = Marshal.SizeOf(0#)
            sizeOf(GetType(Decimal)) = Marshal.SizeOf(0@)
            ' sizeOf(GetType(Char)) = Marshal.SizeOf(ASCII.TAB)
            sizeOf(GetType(Short)) = Marshal.SizeOf(CShort(1))
        End Sub

        Public Function MeasureSize(o As T) As Long
            Dim obj As Object = o
            Dim size&

            For Each prop As PropertyInfo In Schema
                If sizeOf.ContainsKey(prop.PropertyType) Then
                    size += sizeOf(prop.PropertyType)
                ElseIf prop.PropertyType.Equals(GetType(String)) Then
                    Dim s$ = DirectCast(prop.GetValue(obj), String)
                    If Not s Is Nothing Then
                        size += s.Length * charSize
                    End If
                Else
                    size += Marshal.SizeOf(prop.GetValue(obj))
                End If
            Next

            Return size
        End Function

        Public Overrides Function ToString() As String
            Return $"sizeof({Type.FullName})"
        End Function
    End Class
End Namespace

#Region "Microsoft.VisualBasic::4a2f7d206952f013c1f892f49d68f767, GCModeller\data\RegulonDatabase\RegulonDB\Views\Database.vb"

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

    '   Total Lines: 44
    '    Code Lines: 37
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.88 KB


    '     Module Database
    ' 
    '         Function: __load, Load, SchemaParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace RegulonDB.Views

    Public Module Database

        Public Function SchemaParser(Of T As Class)() As PropertyInfo()
            Dim typeInfo As Type = GetType(T)
            Dim props As PropertyInfo() = typeInfo.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            Dim indexing = (From prop As PropertyInfo In props
                            Let index As Index = prop.GetCustomAttribute(Of Index)
                            Where Not index Is Nothing
                            Select index.value,
                                prop
                            Order By value Ascending).ToArray
            Return indexing.Select(Function(x) x.prop)
        End Function

        Public Function Load(Of T As Class)(path As String) As T()
            Dim lines As String() = IO.File.ReadAllLines(path)
            lines = (From line As String In lines
                     Where Not String.IsNullOrWhiteSpace(line) AndAlso
                         line.First <> "#"c
                     Select line).ToArray
            Dim schema As PropertyInfo() = SchemaParser(Of T)()
            Dim loadQuery = (From line As String In lines.AsParallel Select __load(Of T)(line, schema)).ToArray
            Return loadQuery
        End Function

        Private Function __load(Of T As Class)(line As String, schema As PropertyInfo()) As T
            Dim Tokens As String() = Strings.Split(line, vbTab)
            Dim obj As Object = Activator.CreateInstance(GetType(T))

            For i As Integer = 0 To schema.Length - 1
                Dim value As String = Tokens(i)
                Dim entry As PropertyInfo = schema(i)
                Call entry.SetValue(obj, value)
            Next

            Return DirectCast(obj, T)
        End Function
    End Module
End Namespace

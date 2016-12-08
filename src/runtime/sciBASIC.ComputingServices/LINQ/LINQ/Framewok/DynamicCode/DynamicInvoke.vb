#Region "Microsoft.VisualBasic::3af26b7fbc6d4690e1f6aef1dbf93306, ..\sciBASIC.ComputingServices\LINQ\LINQ\Framewok\DynamicCode\DynamicInvoke.vb"

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

Namespace Framework.DynamicCode

    Friend Module DynamicInvoke

        Public Function GetMethod(Type As System.Reflection.TypeInfo, Name As String) As System.Reflection.MethodInfo
            Dim LQuery = From Method In Type.GetMethods Where String.Equals(Method.Name, Name) Select Method '
            Return LQuery.First
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Assembly"></param>
        ''' <param name="TypeId">将要查找的目标对象</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function [GetType](Assembly As System.Reflection.Assembly, TypeId As String) As System.Reflection.TypeInfo()
            Dim LQuery = From Type As System.Reflection.TypeInfo In Assembly.DefinedTypes Where String.Equals(TypeId, Type.Name) Select Type '
            Return LQuery.ToArray
        End Function
    End Module
End Namespace

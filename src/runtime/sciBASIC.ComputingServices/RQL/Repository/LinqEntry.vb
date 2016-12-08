#Region "Microsoft.VisualBasic::60a78dbfa186f18eb31f36ba8f44049e, ..\sciBASIC.ComputingServices\RQL\Repository\LinqEntry.vb"

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

Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.ComputingServices.TaskHost

Namespace Linq

    ''' <summary>
    ''' 类型信息是所查询的对象的类型信息
    ''' </summary>
    Public Class LinqEntry : Inherits MetaData.TypeInfo

        ''' <summary>
        ''' 唯一标识当前的这个查询的哈希值
        ''' </summary>
        ''' <returns></returns>
        Public Property uid As String
        ''' <summary>
        ''' 除了使用上面的uid进行url查询，也可以使用这个地址来使用socket查询，具体的协议已经封装在<see cref="ILinq(Of T)"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Linq As IPEndPoint

        Sub New()
        End Sub

        Sub New(info As Type)
            Call MyBase.New(info)
        End Sub

        Public Function AsLinq(Of T)() As ILinq(Of T)
            Return New ILinq(Of T)(Linq)
        End Function

        Public Function LinqReader() As ILinqReader
            Return New ILinqReader(Linq, Me.GetType)
        End Function
    End Class
End Namespace

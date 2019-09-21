#Region "Microsoft.VisualBasic::c821ce1f9d944ce5de4380178d594d0f, RDotNET.Extensions.VisualBasic\Extensions\RScriptInvoke.vb"

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

    ' Class RScriptInvoke
    ' 
    '     Properties: [Call], STD_OUT
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: (+3 Overloads) Invoke, ToString
    ' 
    '     Sub: PrintSTDOUT
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.Serialization
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

''' <summary>
''' 推荐使用这个对象来执行R脚本
''' </summary>
Public Class RScriptInvoke

    ''' <summary>
    ''' The R script
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property [Call] As String
    ''' <summary>
    ''' R output from the script <see cref="[Call]"/>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property STD_OUT As String()

    ''' <summary>
    ''' Creates a R call from the script text
    ''' </summary>
    ''' <param name="script"></param>
    Sub New(script As String)
        Me.Call = script
    End Sub

    ''' <summary>
    ''' Creates a R call from the script builder
    ''' </summary>
    ''' <param name="script"></param>
    Sub New(script As IRProvider)
        Me.Call = script.RScript
    End Sub

    ''' <summary>
    ''' Display the output on the system console.
    ''' </summary>
    Public Sub PrintSTDOUT()
        For Each s As String In STD_OUT.SafeQuery
            Call Console.WriteLine(s)
        Next
    End Sub

    ''' <summary>
    ''' <see cref="[Call]"/>
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return [Call]
    End Function

    Public Function Invoke() As String()
        Return R.WriteLine([Call])
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="parser">提供了R数据输出解析的方法</param>
    ''' <returns></returns>
    Public Function Invoke(Of T)(parser As Func(Of String(), T)) As T
        Return parser(Invoke())
    End Function

    ''' <summary>
    ''' The R script <see cref="[Call]"/> should output a S4Object.
    ''' </summary>
    ''' <typeparam name="T">在R之中的类型必须是S4Object对象</typeparam>
    ''' <returns></returns>
    Public Function Invoke(Of T As Class)() As T
        Dim raw As RDotNET.SymbolicExpression = R.Evaluate([Call])
        Dim result As T = SerializationExtensions.S4Object(Of T)(raw)
        Return result
    End Function
End Class

#Region "Microsoft.VisualBasic::4909fcfd5a73d93456bbe45c37a1282d, RDotNET.Extensions.VisualBasic\ScriptBuilder\Abstract\IScriptProvider.vb"

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

    '     Class IRProvider
    ' 
    '         Properties: Requires
    ' 
    '         Function: ToString
    ' 
    '     Interface IScriptProvider
    ' 
    '         Function: RScript
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.ListExtensions

Namespace SymbolBuilder.Abstract

    ''' <summary>
    ''' A basic object model as a token in the R script.(一个提供脚本语句的最基本的抽象对象)
    ''' </summary>
    ''' <remarks>就只通过一个函数来提供脚本执行语句</remarks>
    Public MustInherit Class IRProvider
        Implements IScriptProvider

        Protected __requires As List(Of String)

        ''' <summary>
        ''' The package names that required of this script file.
        ''' (需要加载的R的包的列表)
        ''' </summary>
        ''' <returns></returns>
        <XmlIgnore> <Ignored> Public Overridable Property Requires As String()
            Get
                If __requires Is Nothing Then
                    __requires = New List(Of String)
                End If
                Return __requires.ToArray
            End Get
            Protected Set(value As String())
                If value Is Nothing Then
                    __requires = Nothing
                Else
                    __requires = value.AsList
                End If
            End Set
        End Property

        ''' <summary>
        ''' Get R Script text from this R script object build model.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public MustOverride Function RScript() As String Implements IScriptProvider.RScript

        Public Overrides Function ToString() As String
            Return RScript()
        End Function

        Public Shared Narrowing Operator CType(R As IRProvider) As String
            Return R.RScript
        End Operator
    End Class

    ''' <summary>
    ''' This abstract object provides a interface for generates the R script.
    ''' </summary>
    Public Interface IScriptProvider
        Function RScript() As String
    End Interface
End Namespace

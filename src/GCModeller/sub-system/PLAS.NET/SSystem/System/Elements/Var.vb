#Region "Microsoft.VisualBasic::8ef3f7f679afb9953a14a13455f0e8a2, ..\GCModeller\sub-system\PLAS.NET\SSystem\System\Elements\Var.vb"

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

Imports System.Xml.Serialization
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

Namespace Kernel.ObjectModels

    Public Class var : Inherits Variable

        <XmlAttribute> Public Property Title As String
        <XmlElement> Public Property Comment As String

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Comment) Then
                Return String.Format("{0}={1}", IIf(Len(Title) > 0, Title, UniqueId), Value)
            Else
                Return String.Format("{0}={1}; //{2}", IIf(Len(Title) > 0, Title, UniqueId), Value, Comment)
            End If
        End Function

        Public Shared Narrowing Operator CType(e As var) As Double
            Return e.Value
        End Operator

        Public Shared Narrowing Operator CType(e As var) As String
            Return IIf(Len(e.Title) > 0, e.Title, e.UniqueId)
        End Operator

        Public Shared Widening Operator CType(e As Specie) As var
            Return New var With {
                .UniqueId = e.ID,
                .Title = e.name,
                .Value = Val(e.InitialAmount)
            }
        End Operator

        Public Shared Function TryParse(strData As String) As var
            Return CType(strData, var)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s">Script line.(脚本行文本)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Widening Operator CType(s As String) As var
            Dim Tokens As String() = Mid(s, 6).Split(CChar("="))
            Return New var With {
                .UniqueId = Tokens.First.Trim,
                .Value = Val(Tokens.Last)
            }
        End Operator
    End Class
End Namespace

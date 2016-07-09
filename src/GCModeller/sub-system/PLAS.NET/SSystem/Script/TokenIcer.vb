#Region "Microsoft.VisualBasic::d863403ee815a0dadd8a6205c178545b, ..\GCModeller\sub-system\PLAS.NET\SSystem\Script\TokenIcer.vb"

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

Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace Script

    Public Module TokenIcer

        Public ReadOnly Property Tokens As IReadOnlyDictionary(Of String, Tokens) =
            New Dictionary(Of String, Tokens) From {
 _
            {Script.Tokens.Alias.Description, Script.Tokens.Alias},
            {Script.Tokens.Comment.Description, Script.Tokens.Comment},
            {Script.Tokens.Constant.Description, Script.Tokens.Constant},
            {Script.Tokens.InitValue.Description, Script.Tokens.InitValue},
            {Script.Tokens.Reaction.Description, Script.Tokens.Reaction},
            {Script.Tokens.Time.Description, Script.Tokens.Time},
            {Script.Tokens.Title.Description, Script.Tokens.Title}
        }

        Public Function TryParse(script As IEnumerable(Of String)) As Token(Of Tokens)()
            Dim LQuery As Token(Of Tokens)() =
                LinqAPI.Exec(Of Token(Of Tokens)) <=
 _
                    From line As String
                    In script
                    Let token As Token(Of Tokens) = line.Trim.__tokenParser()  ' 类型的前缀已经被切割掉了
                    Where Not token Is Nothing
                    Select token

            Return LQuery
        End Function

        ''' <summary>
        ''' 会忽略掉注释信息
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        <Extension> Private Function __tokenParser(line As String) As Token(Of Tokens)
            Dim x As String = line.Split.First.ToUpper

            If Not Tokens.ContainsKey(x) Then
                Return Nothing
            End If

            Dim type As Tokens = Tokens(x)
            If type = Script.Tokens.Comment Then
                Return Nothing
            End If
            Dim text As String = Mid(line, x.Length + 1).Trim
            Return New Token(Of Tokens)(type, text)
        End Function
    End Module
End Namespace

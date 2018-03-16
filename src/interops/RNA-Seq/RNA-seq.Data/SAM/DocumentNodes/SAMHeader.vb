#Region "Microsoft.VisualBasic::df37d3e36fcccb1522773c220115c5aa, RNA-Seq\RNA-seq.Data\SAM\DocumentNodes\SAMHeader.vb"

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

    '     Class SAMHeader
    ' 
    '         Properties: Tag, TagValue, TAGValues
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GenerateDocumentLine, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace SAM

    ''' <summary>
    ''' 实际上就相当于一个字典来的
    ''' </summary>
    Public Class SAMHeader

        Public Property TAGValues As Dictionary(Of String, String)
        Public Property Tag As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="str">
        ''' Each header line begins with character `@' followed by a two-letter record type code. In the header,
        ''' each line Is TAB-delimited And except the @CO lines, each data field follows a format `TAG:VALUE'
        ''' where TAG Is a two-letter String that defines the content And the format Of VALUE. Each header line
        ''' should match :  /^@[A-Za-z][A-Za-z](\t[A-Za-z][A-Za-z0-9]:[ -~]+)+$/ Or /^@CO\t.*/. Tags
        ''' containing lowercase letters are reserved For End users.
        ''' (每一行都是从@符号开始，后面跟随者两个字母的数据类型码，使用TAB进行分割除了@CO行，每一个域都以键值对的形式出现:  TAG:Value)
        ''' </param>
        Sub New(str As String)
            Dim tokens$() = Strings.Split(str, vbTab)
            Dim tuples = From s As String
                         In tokens.Skip(1)
                         Let arr As String() = s.Split(":"c)
                         Select (key:=arr(0), value:=arr(1))

            Tag = tokens(0)
            TAGValues = tuples _
                .ToDictionary(Function(t) t.key,
                              Function(t) t.value)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GenerateDocumentLine() As String
            Return Tag & vbTab & String.Join(vbTab, (From obj In Me.TAGValues Select $"{obj.Key}:{obj.Value}").ToArray)
        End Function

        Const TagsArray = "@HD@SQ@RG@PG@CO"

        Public ReadOnly Property TagValue As Tags
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return CByte(InStr(TagsArray, Tag) / 3)
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return TagValue.Description
        End Function
    End Class
End Namespace

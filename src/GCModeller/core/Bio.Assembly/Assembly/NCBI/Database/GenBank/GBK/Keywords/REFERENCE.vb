#Region "Microsoft.VisualBasic::3147f7849e390ed104879863ad78b913, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\REFERENCE.vb"

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

    '   Total Lines: 109
    '    Code Lines: 82
    ' Comment Lines: 8
    '   Blank Lines: 19
    '     File Size: 4.53 KB


    '     Class REFERENCE
    ' 
    '         Properties: ReferenceList
    ' 
    '         Function: InternalParser
    ' 
    '     Class RefInfo
    ' 
    '         Properties: Authors, BaseLocation, Consrtm, Id, Journal
    '                     Pubmed, Title
    ' 
    '         Function: Internal_subSectionParser, InternalParser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    ''' <summary>
    ''' 引用文献的解析模块，一个引用文献模块之中包含有若干篇引用文献<see cref="RefInfo"></see>
    ''' </summary>
    ''' <remarks></remarks>
    Public Class REFERENCE : Inherits KeyWord

        Public Property ReferenceList As RefInfo()

        Public Shared Function InternalParser(chunkText As String()) As REFERENCE
            Dim Heads As String() = (From strLine As String In chunkText.AsParallel Where InStr(strLine, "REFERENCE") = 1 Select strLine).ToArray
            Dim ChunkBuffer As List(Of String()) = New List(Of String())

            For Each Head As String In Heads
                Dim p As Integer = Array.IndexOf(chunkText, Head) + 1

                For i As Integer = p To chunkText.Count - 1
                    If Not chunkText(i).First = " "c Then
                        Dim Chunktemp As String() = New String(i - p) {}

                        Call Array.ConstrainedCopy(chunkText, p - 1, Chunktemp, 0, Chunktemp.Length)
                        Call ChunkBuffer.Add(Chunktemp)
                        Exit For
                    End If
                Next
            Next
#Const DEBUG = 1
            Dim refList As RefInfo()
#If DEBUG Then
            refList = (From chunk As String() In ChunkBuffer Select RefInfo.InternalParser(chunk)).ToArray
#Else
            refList = (From chunk As String() In ChunkBuffer.AsParallel Select L_Reference.InternalParser(chunk)).ToArray
#End If

            Return New REFERENCE With {.ReferenceList = refList}
        End Function
    End Class

    ''' <summary>
    ''' <see cref="REFERENCE"/>里面的一篇引用文献
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RefInfo

        Public Property BaseLocation As ComponentModel.Loci.Location
        Public Property Id As Integer
        Public Property Title As String
        Public Property Journal As String
        Public Property Pubmed As String
        Public Property Consrtm As String
        Public Property Authors As String()

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(Pubmed) Then
                Return String.Join("; ", Authors) & "  " & Journal
            End If
            Return String.Format("[PUBMED {0}] {1}  {2}. ({3})", Pubmed, String.Join("; ", Authors), Title, Journal)
        End Function

        Public Shared Function InternalParser(str As String()) As RefInfo
            Dim ref As String = str.First
            Dim subSections = Internal_subSectionParser(str.Skip(1).ToArray)
            Dim L_ref As RefInfo = New RefInfo
            Dim numbers As String() = (From m As Match In Regex.Matches(ref, "\d+") Select m.Value).ToArray
            L_ref.Id = numbers(0)
            If numbers.Count > 1 Then
                L_ref.BaseLocation = New ComponentModel.Loci.Location(Val(numbers(1)), Val(numbers(2)))
            Else
                L_ref.BaseLocation = New ComponentModel.Loci.Location
            End If

            Dim Internal_getValue = Function(key As String) If(subSections.ContainsKey(key), subSections(key), "")

            L_ref.Authors = Strings.Split(Internal_getValue("AUTHORS"), ", ")
            L_ref.Title = Internal_getValue("TITLE")
            L_ref.Journal = Internal_getValue("JOURNAL")
            L_ref.Consrtm = Internal_getValue("CONSRTM")
            L_ref.Pubmed = Internal_getValue("PUBMED")

            Return L_ref
        End Function

        Private Shared Function Internal_subSectionParser(str As String()) As Dictionary(Of String, String)
            Dim Dict As Dictionary(Of String, String) = New Dictionary(Of String, String)
            Dim segments As String() = {Mid(str.First, 3, 10).Trim, Mid(str.First, 11).Trim}
            Dim lastHead As String = segments(0)
            Dim Temp As String = segments(1)

            For Each line As String In str.Skip(1)
                segments = {Mid(line, 3, 10).Trim, Mid(line, 11).Trim}
                If String.IsNullOrEmpty(segments(0)) Then '当前的还没有结束
                    Temp = Temp & " " & segments(1)
                Else
                    Call Dict.Add(lastHead, Temp)
                    Temp = segments(1)
                    lastHead = segments(0)
                End If
            Next

            Call Dict.Add(lastHead, Temp)

            Return Dict
        End Function
    End Class
End Namespace

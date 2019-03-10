#Region "Microsoft.VisualBasic::7bb5a004140fe2bff5ae41558249c448, Bio.Assembly\Assembly\KEGG\DBGET\Objects\KEGGOrganism\Defines.vb"

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

    '     Class Organism
    ' 
    '         Properties: [Class], KEGGId, Kingdom, Phylum, RefSeq
    '                     Species
    ' 
    '         Function: __createObject, GetValue, ToString, Trim
    ' 
    '     Class Prokaryote
    ' 
    '         Properties: Year
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: GetValue, Trim
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.KEGG.DBGET.bGetObject.Organism

    Public Class Organism : Implements INamedValue

        <XmlAttribute> Public Property Kingdom As String
        <XmlAttribute> Public Property Phylum As String
        <XmlAttribute> Public Property [Class] As String
        ''' <summary>
        ''' 物种全称
        ''' </summary>
        ''' <returns></returns>
        Public Property Species As String

        ''' <summary>
        ''' KEGG里面的物种的简称代码
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property KEGGId As String Implements INamedValue.Key
        ''' <summary>
        ''' FTP url on NCBI
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RefSeq As String

        Public Const Field As String = "<td rowspan=\d+  align=[a-z]+>.+</td>"
        Public Const ClassType As String = "<td rowspan=\d+  align=[a+z]+><a href='.+'>.+</a></td>"
        Public Const CELL_ITEM As String = "<td align=[a-z]+><a href='.+'>[a-z0-9]+</a>"

        Protected Friend Shared __setValues As Action(Of Organism, String)() =
            New Action(Of Organism, String)() {
 _
                Sub(org, value) org.Kingdom = value,
                Sub(org, value) org.Phylum = value,
                Sub(org, value) org.Class = value,
                Sub(org, value) org.KEGGId = value,
                Sub(org, value) org.Species = value,
                Sub(org, value) org.RefSeq = value
        }

        Friend Overridable Function Trim() As Organism
            Phylum = GetValue(Phylum)
            [Class] = GetValue([Class])
            Species = GetValue(Species)
            KEGGId = GetValue(KEGGId)
            RefSeq = Regex.Match(RefSeq, """.+""").Value
            RefSeq = Mid(RefSeq, 2, Len(RefSeq) - 2)

            Return Me
        End Function

        Protected Friend Shared Function GetValue(str As String) As String
            If String.IsNullOrEmpty(str) Then
                Return ""
            End If
            str = Regex.Match(str, ">.+?<", RegexOptions.Singleline).Value
            str = Mid(str, 2, Len(str) - 2)
            Return str
        End Function

        Friend Shared Function __createObject(text As String) As Organism
            Dim Tokens As String() = Regex.Matches(text, "<a href=.+?</a>", RegexICSng).ToArray
            If Tokens.IsNullOrEmpty Then Return Nothing
            Dim Organism As Organism = New Organism
            Dim p As Integer = Organism.__setValues.Length - 1

            For i As Integer = Tokens.Length - 1 To 0 Step -1
                Call Organism.__setValues(p)(Organism, Tokens(i))
                p -= 1
            Next
            Return Organism
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", KEGGId, Species)
        End Function
    End Class

    Public Class Prokaryote : Inherits Organism

        <XmlAttribute> Public Property Year As String

        Public Sub New()
        End Sub

        Sub New(org As Organism)
            MyBase.Class = org.Class
            MyBase.KEGGId = org.KEGGId
            MyBase.Kingdom = org.Kingdom
            MyBase.Phylum = org.Phylum
            MyBase.RefSeq = org.RefSeq
            MyBase.Species = org.Species
        End Sub

        Protected Friend Shared Shadows SetValues As Action(Of Prokaryote, String)() =
            New Action(Of Prokaryote, String)() {
 _
                Sub(org, value) org.Kingdom = value,
                Sub(org, value) org.Phylum = value,
                Sub(org, value) org.Class = value,
                Sub(org, value) org.KEGGId = value,
                Sub(org, value) org.Species = value,
                Sub(org, value) org.Year = value,
                Sub(org, value) org.RefSeq = value
        }

        Protected Friend Sub New(text As String)
            Dim Tokens As String() = Regex.Matches(text, "<td.+?</td>", RegexICSng).ToArray
            If Tokens.IsNullOrEmpty Then
            Else
                Dim p As Integer = Prokaryote.SetValues.Length - 1

                For i As Integer = Tokens.Length - 1 To 0 Step -1
                    Call Prokaryote.SetValues(p)(Me, Tokens(i))
                    p -= 1
                Next
            End If
        End Sub

        Friend Overrides Function Trim() As Organism
            Phylum = GetValue(Phylum)
            [Class] = GetValue([Class])
            Species = GetValue(Species)
            KEGGId = GetValue(KEGGId)
            Year = GetValue(Year)

            If Not String.IsNullOrEmpty(RefSeq) Then
                RefSeq = Regex.Match(RefSeq, "<a href="".+?"">").Value
            End If
            If Not String.IsNullOrEmpty(RefSeq) Then
                RefSeq = Mid(RefSeq, 10, Len(RefSeq) - 11)
            End If

            Kingdom = GetValue(Kingdom)

            Return Me
        End Function

        Protected Friend Overloads Shared Function GetValue(str As String) As String
            If String.IsNullOrEmpty(str) Then
                Return ""
            End If
            Dim m = Regex.Match(str, "<a href="".+?"">.+?</a>")
            If m.Success Then
                str = Organism.GetValue(m.Value)
            Else
                str = Organism.GetValue(str)
            End If

            Return str
        End Function
    End Class
End Namespace

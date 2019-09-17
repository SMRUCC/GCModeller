#Region "Microsoft.VisualBasic::1892ad29306029de217967971bc060c3, analysis\ProteinTools\Interactions.BioGRID\API.vb"

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

    ' Module API
    ' 
    '     Function: AllIdentifierTypes, Selects
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' 
''' </summary>
Public Module API

    ''' <summary>
    ''' For debugs, no more functionals
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AllIdentifierTypes(source As IEnumerable(Of IDENTIFIERS)) As String()
        Dim out As New Dictionary(Of String, String)

        For Each x As IDENTIFIERS In source
            If Not out.ContainsKey(x.IDENTIFIER_TYPE) Then
                Call out.Add(x.IDENTIFIER_TYPE, Nothing)
                Call $"&{x.IDENTIFIER_TYPE} --> {x.GetJson}".__DEBUG_ECHO
            End If
        Next

        Return out.Keys.ToArray
    End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    Public Iterator Function Selects(source As IEnumerable(Of EntityObject), links As IEnumerable(Of ALLmitab)) As IEnumerable(Of EntityObject)
        Dim FromHash As Dictionary(Of String, ALLmitab()) = (
            From x As ALLmitab
            In links
            Select x,
                sid = x.A.Split(":"c).Last
            Group By sid Into Group) _
                 .ToDictionary(Function(x) x.sid,
                               Function(x) x.Group.Select(Function(o) o.x).ToArray)
        Dim ToHash As Dictionary(Of String, ALLmitab()) = (
            From x As ALLmitab
            In FromHash.Values.IteratesALL
            Select x,
                sid = x.B.Split(":"c).Last
            Group By sid Into Group) _
                 .ToDictionary(Function(x) x.sid,
                               Function(x) x.Group.Select(Function(o) o.x).ToArray)

        For Each x As EntityObject In source
            Dim key As String = x.ID
            Dim a = False, b = False

            If FromHash.ContainsKey(key) Then
                a = True

                For Each part As ALLmitab In FromHash(key)
                    Dim copy As EntityObject = x.Copy

                    copy.Properties.Add(NameOf(part.AliasA), part.AliasA)
                    copy.Properties.Add(NameOf(part.AliasB), part.AliasB)
                    copy.Properties.Add(NameOf(part.AltA), part.AltA)
                    copy.Properties.Add(NameOf(part.AltB), part.AltB)
                    copy.Properties.Add(NameOf(part.Database), part.Database)
                    copy.Properties.Add(NameOf(part.Author), part.Author)
                    copy.Properties.Add(NameOf(part.B), part.B)
                    copy.Properties.Add(NameOf(part.Confidence), part.Confidence)
                    copy.Properties.Add(NameOf(part.IDM), part.IDM)
                    copy.Properties.Add(NameOf(part.InteractType), part.InteractType)
                    copy.Properties.Add(NameOf(part.Publication), part.Publication)
                    copy.Properties.Add(NameOf(part.uid), part.uid)

                    Yield copy
                Next
            End If
            If ToHash.ContainsKey(key) Then
                b = True

                For Each part As ALLmitab In ToHash(key)
                    Dim copy As EntityObject = x.Copy

                    copy.Properties.Add(NameOf(part.AliasA), part.AliasA)
                    copy.Properties.Add(NameOf(part.AliasB), part.AliasB)
                    copy.Properties.Add(NameOf(part.AltA), part.AltA)
                    copy.Properties.Add(NameOf(part.AltB), part.AltB)
                    copy.Properties.Add(NameOf(part.Database), part.Database)
                    copy.Properties.Add(NameOf(part.Author), part.Author)
                    copy.Properties.Add(NameOf(part.B), part.B)
                    copy.Properties.Add(NameOf(part.Confidence), part.Confidence)
                    copy.Properties.Add(NameOf(part.IDM), part.IDM)
                    copy.Properties.Add(NameOf(part.InteractType), part.InteractType)
                    copy.Properties.Add(NameOf(part.Publication), part.Publication)
                    copy.Properties.Add(NameOf(part.uid), part.uid)

                    Yield copy
                Next
            End If

            If Not (a OrElse b) Then
                Yield x  ' 空白的，没有找到互作关系的
                Call Console.Write(".")
            End If
        Next
    End Function
End Module

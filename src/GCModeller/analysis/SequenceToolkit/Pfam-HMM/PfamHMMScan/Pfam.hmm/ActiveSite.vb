#Region "Microsoft.VisualBasic::a53da6383abd79a9cec201a962d5c52d, ..\GCModeller\analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\Pfam.hmm\ActiveSite.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Linq
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' active_site.dat
''' </summary>
Public Structure ActiveSite : Implements sIdEnumerable

    Public Property ID As String Implements sIdEnumerable.Identifier
    Public Property RE As Dictionary(Of String, RE)
    Public Property AL As Alignment()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Iterator Function LoadStream(path As String) As IEnumerable(Of ActiveSite)
        Dim lines As String() = path.ReadAllLines
        Dim tokens As IEnumerable(Of String()) = lines.Split("//")

        For Each token As String() In tokens
            Yield StreamParser(token)
        Next
    End Function

    Public Shared Function StreamParser(stream As String()) As ActiveSite
        Dim LQuery = (From s As String
                      In stream
                      Let tag As String = s.Substring(0, 2)
                      Let value As String = s.Substring(4)
                      Select tag,
                          value
                      Group By tag Into Group) _
                              .ToDictionary(Function(x) x.tag,
                                            Function(x) x.Group.ToArray(Function(o) o.value))
        Dim pfam As New ActiveSite With {
            .ID = LQuery.TryGetValue(NameOf(pfam.ID)).DefaultFirst,
            .RE = __RLhash(LQuery.TryGetValue(NameOf(pfam.RE))),
            .AL = (From s As String
                   In LQuery.TryGetValue(NameOf(pfam.AL)).AsParallel
                   Select New Alignment(s)).ToArray
        }

        Return pfam
    End Function

    Private Shared Function __RLhash(value As String()) As Dictionary(Of String, RE)
        Dim LQuery = (From x As String In value
                      Let tokens As String() = Strings.Split(x, "  ")
                      Let id As String = tokens(Scan0)
                      Let int As Integer = Scripting.CTypeDynamic(Of Integer)(tokens(1))
                      Select id,
                          int
                      Group By id Into Group) _
                              .ToDictionary(Function(x) x.id,
                                            Function(x) New RE With {
                                                .ID = x.id,
                                                .Value = x.Group.ToArray(Function(t) t.int)})
        Return LQuery
    End Function
End Structure

Public Structure RE : Implements sIdEnumerable
    Public Property ID As String Implements sIdEnumerable.Identifier
    Public Property Value As Integer()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure

Public Structure Alignment : Implements sIdEnumerable
    Public Property ID As String Implements sIdEnumerable.Identifier
    Public Property MAL As String

    Sub New(s As String)
        Dim tokens As String() = Regex.Split(s, "\s+")
        ID = tokens(Scan0)
        MAL = tokens(1)
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Structure

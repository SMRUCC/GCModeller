#Region "Microsoft.VisualBasic::50144e0271fecd8f0970b898f278009f, data\BASys\Project.vb"

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

    ' Class Project
    ' 
    '     Properties: Briefs, Ecards, Summary
    ' 
    '     Function: Parser, Write
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Class Project

    Public Property Summary As Summary
    Public Property Briefs As TableBrief()
    Public Property Ecards As Ecard()

    Public Function Write(EXPORT As String) As Boolean
        Dim i As Pointer = 0

        Call Summary.GetJson.SaveTo(EXPORT & "/" & NameOf(Summary) & ".json")
        Call Briefs.SaveTo(EXPORT & "/" & NameOf(Briefs) & ".Csv")

        For Each x As Ecard In Ecards
            Call x.WriteLargeJson(EXPORT & $"/{NameOf(Ecards)}/BASys{ZeroFill(++i, 4)}.json")
        Next

        Call Me.ExportPTT.Save(EXPORT & $"/{Summary.chrId}.PTT")
        Call Me.ExportCOG.SaveTo(EXPORT & $"/{Summary.chrId}.MyvaCOG.Csv")
        Call Me.ExportFaa.Save(EXPORT & $"/{Summary.chrId}.faa", Encoding.ASCII)
        Call Me.ExportFfn.Save(EXPORT & $"/{Summary.chrId}.ffn", Encoding.ASCII)

        Return True
    End Function

    Public Shared Function Parser(DIR As String, Optional skipCards As Boolean = False) As Project
        Dim details As String = DIR & "/basys_text_final/"
        Dim loads As IEnumerable(Of String) = ls - l - r - wildcards("*.ecard") <= details
        Dim proj As New Project With {
            .Summary = Summary.IndexParser(DIR & "/index.html"),
            .Briefs = TableBrief.TableParser(DIR & "/table.html")
        }

        If Not skipCards Then
            Dim ecards As Ecard() = LinqAPI.Exec(Of Ecard) <=
 _
                From file As String
                In loads.AsParallel
                Select Ecard.Parser(file)

            proj.Ecards = ecards.ToArray
        End If

        Return proj
    End Function
End Class

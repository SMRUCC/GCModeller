#Region "Microsoft.VisualBasic::19daa772c20f8944a71f4c901e1ca290, GCModeller\engine\Dynamics.Debugger\Summary.vb"

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

    '   Total Lines: 92
    '    Code Lines: 74
    ' Comment Lines: 6
    '   Blank Lines: 12
    '     File Size: 3.16 KB


    ' Module Summary
    ' 
    '     Function: summaryOf, summaryReport
    ' 
    '     Sub: summary
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text.Xml.HtmlBuilder
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.ModelLoader
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports DynamicsEngine = SMRUCC.genomics.GCModeller.ModellingEngine.BootstrapLoader.Engine.Engine

''' <summary>
''' model summary report generator
''' </summary>
Public Module Summary

    <Extension>
    Public Sub summary(inits As Definition, model As CellularModule, dir As String)
        ' do initialize of the virtual cell engine
        ' and then load virtual cell model into 
        ' engine kernel
        Dim loader As Loader = Nothing
        Dim vcell As Vessel = New DynamicsEngine(
                def:=inits,
                dynamics:=New FluxBaseline,
                iterations:=0,
                showProgress:=False
            ) _
            .LoadModel(model, getLoader:=loader) _
            .getCore
        Dim fluxIndex = loader.GetFluxIndex
        Dim channels As Dictionary(Of String, Channel) = vcell.Channels.ToDictionary(Function(r) r.ID)

        For Each region In fluxIndex
            Call channels _
                .Takes(region.Value) _
                .summaryReport(region.Key) _
                .SaveTo($"{dir}/{region.Key}.html")
        Next
    End Sub

    <Extension>
    Private Function summaryReport(reactions As IEnumerable(Of Channel), regionName$) As String
        Dim summaryList As New List(Of String)

        For Each reaction As Channel In reactions
            summaryList.Add(reaction.summaryOf)
        Next

        Return sprintf(<html>
                           <head>
                               <title><%= regionName %></title>
                           </head>
                           <body>
                               <h1><%= regionName %></h1>
                               <hr/>

                               <div>%s</div>
                           </body>
                       </html>, summaryList.JoinBy(vbCrLf))
    End Function

    <Extension>
    Private Function summaryOf(rxn As Channel) As String
        Dim forward As New List(Of String)
        Dim reverse As New List(Of String)

        For Each m In rxn.GetReactants
            forward.Add((<li><%= m.mass.ID %></li>).ToString)
        Next

        For Each m In rxn.GetProducts
            reverse.Add((<li><%= m.mass.ID %></li>).ToString)
        Next

        Return sprintf(
            <div>
                <h2 id=<%= rxn.ID %>><%= rxn.ID %></h2>

                <div>
                    <h3>left</h3>
                    <ul>
                    %s
                    </ul>
                </div>
                <div>
                    <h3>right</h3>
                    <ul>
                    %s
                    </ul>
                </div>
            </div>, rxn.ID, forward.JoinBy("<br />"), reverse.JoinBy("<br />"))
    End Function
End Module

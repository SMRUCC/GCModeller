Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Xml.HtmlBuilder
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine.Definitions
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine.ModelLoader
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports DynamicsEngine = SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Engine.Engine

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

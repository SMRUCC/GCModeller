#Region "Microsoft.VisualBasic::620294729ac601e608fd6f2a8003d921, engine\Dynamics\test\Module1.vb"

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

' Module Module1
' 
'     Function: mass, reactions
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports Dynamics.Debugger
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Core

Module Module1

    Sub Main()
        Dim massTable = mass.ToDictionary(Function(m) m.ID)
        Dim envir As New Vessel With {
            .Mass = massTable.Values.ToArray,
            .Channels = reactions(massTable).ToArray
        }

        Dim snapshots As New List(Of DataSet)
        Dim flux As New List(Of DataSet)

        Call envir.Initialize()

        For i As Integer = 0 To 10000
            flux += New DataSet With {.ID = i, .Properties = envir.ContainerIterator().ToDictionary.FlatTable}
            snapshots += New DataSet With {
                .ID = i,
                .Properties = massTable.ToDictionary(Function(m) m.Key, Function(m) m.Value.Value)
            }
        Next

        Call snapshots.SaveTo("./test_mass.csv")
        Call flux.SaveTo("./test_flux.csv")
        Call envir.ToGraph.Tabular.Save("./test_network/")

        Pause()
    End Sub

    Private Iterator Function mass() As IEnumerable(Of Factor)
        Yield New Factor With {.ID = "A", .Value = 1000}
        Yield New Factor With {.ID = "B", .Value = 1000}
        Yield New Factor With {.ID = "C", .Value = 1000}
        Yield New Factor With {.ID = "D", .Value = 1000}
        Yield New Factor With {.ID = "E", .Value = 1000}
        Yield New Factor With {.ID = "F", .Value = 1000}
        Yield New Factor With {.ID = "G", .Value = 1000}
        Yield New Factor With {.ID = "H", .Value = 1000}
        Yield New Factor With {.ID = "I", .Value = 1000}
    End Function

    ''' <summary>
    ''' Build a test network
    ''' </summary>
    ''' <param name="massTable"></param>
    ''' <returns></returns>
    Private Iterator Function reactions(massTable As Dictionary(Of String, Factor)) As IEnumerable(Of Channel)
        Dim pop = Iterator Function(names As String()) As IEnumerable(Of Variable)
                      For Each ref In names
                          Yield New Variable(massTable(ref), 0.05)
                      Next
                  End Function

        Yield New Channel(pop({"A", "B"}), pop({"C", "D"})) With {
            .bounds = {5, 50},
            .ID = "ABCD",
            .Forward = New Controls,
            .Reverse = New Controls With {.Activation = pop({"B", "D"}).ToArray}}

        Yield New Channel(pop({"E", "F"}), pop({"A", "G"})) With {
            .bounds = {5, 50},
            .ID = "EFAG",
            .Forward = New Controls,
            .Reverse = New Controls With {.Activation = pop({"B"}).ToArray}
        }

        Yield New Channel(pop({"B"}), pop({"A", "D"})) With {
            .bounds = {5, 50},
            .ID = "BAD",
            .Forward = New Controls With {.Activation = pop({"C", "G", "B"}).ToArray},
            .Reverse = New Controls With {.Activation = pop({"E"}).ToArray}
        }

        Yield New Channel(pop({"G"}), pop({"E"})) With {
            .bounds = {5, 50},
            .ID = "GE",
            .Forward = New Controls With {.Activation = pop({"F"}).ToArray}
        }
        Yield New Channel(pop({"E"}), pop({"G", "D", "C"})) With {
            .bounds = {5, 50},
            .ID = "EGDC",
            .Forward = New Controls With {.Activation = pop({"E"}).ToArray},
            .Reverse = New Controls With {.Activation = pop({"C", "D"}).ToArray}
        }

        Yield New Channel(pop({"B", "F"}), pop({"H"})) With {
            .bounds = {5, 50},
            .ID = "BFH",
            .Forward = New Controls With {.Activation = pop({"B"}).ToArray},
            .Reverse = New Controls With {.Activation = pop({"I", "D"}).ToArray}
        }

        Yield New Channel(pop({"D", "F"}), pop({"H"})) With {
            .bounds = {5, 50},
            .ID = "DFH",
            .Forward = New Controls With {.Activation = pop({"B"}).ToArray},
            .Reverse = New Controls With {.Activation = pop({"I", "D"}).ToArray}
        }

        Yield New Channel(pop({"I"}), pop({"G"})) With {
            .bounds = {5, 50},
            .ID = "IG",
           .Forward = New Controls With {.Activation = pop({"B"}).ToArray},
           .Reverse = New Controls With {.Activation = pop({"G", "D"}).ToArray}
       }

        Yield New Channel(pop({"H"}), pop({"I", "D"})) With {
            .bounds = {5, 50},
            .ID = "HID",
           .Forward = New Controls With {.Activation = pop({"B", "H"}).ToArray},
           .Reverse = New Controls With {.Activation = pop({"A"}).ToArray}
       }
    End Function
End Module


Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
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
            .ID = "ABCD",
            .Forward = New Controls,
            .Reverse = New Controls With {.Activation = pop({"B", "D"}).ToArray}}

        Yield New Channel(pop({"E", "F"}), pop({"A", "G"})) With {
            .ID = "EFAG",
            .Forward = New Controls,
            .Reverse = New Controls With {.Activation = pop({"B"}).ToArray}
        }

        Yield New Channel(pop({"B"}), pop({"A", "D"})) With {
            .ID = "BAD",
            .Forward = New Controls With {.Activation = pop({"C", "G", "B"}).ToArray},
            .Reverse = New Controls With {.Activation = pop({"E"}).ToArray}
        }

        Yield New Channel(pop({"G"}), pop({"E"})) With {
            .ID = "GE",
            .Forward = New Controls With {.Activation = pop({"F"}).ToArray}
        }
        Yield New Channel(pop({"E"}), pop({"G", "D", "C"})) With {
            .ID = "EGDC",
            .Forward = New Controls With {.Activation = pop({"E"}).ToArray},
            .Reverse = New Controls With {.Activation = pop({"C", "D"}).ToArray}
        }

        Yield New Channel(pop({"B", "F"}), pop({"H"})) With {
            .ID = "BFH",
            .Forward = New Controls With {.Activation = pop({"B"}).ToArray},
            .Reverse = New Controls With {.Activation = pop({"I", "D"}).ToArray}
        }

        Yield New Channel(pop({"D", "F"}), pop({"H"})) With {
            .ID = "DFH",
            .Forward = New Controls With {.Activation = pop({"B"}).ToArray},
            .Reverse = New Controls With {.Activation = pop({"I", "D"}).ToArray}
        }

        Yield New Channel(pop({"I"}), pop({"G"})) With {
            .ID = "IG",
           .Forward = New Controls With {.Activation = pop({"B"}).ToArray},
           .Reverse = New Controls With {.Activation = pop({"G", "D"}).ToArray}
       }

        Yield New Channel(pop({"H"}), pop({"I", "D"})) With {
            .ID = "HID",
           .Forward = New Controls With {.Activation = pop({"B", "H"}).ToArray},
           .Reverse = New Controls With {.Activation = pop({"A"}).ToArray}
       }
    End Function
End Module

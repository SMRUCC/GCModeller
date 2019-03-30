Imports Dynamics
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language

Module Module1

    Sub Main()
        Dim massTable = mass.ToDictionary(Function(m) m.ID)
        Dim envir As New Vessel With {
            .Mass = massTable.Values.ToArray,
            .Channels = reactions(massTable).ToArray
        }

        Dim snapshots As New List(Of DataSet)

        For i As Integer = 0 To 10000
            envir.ContainerIterator()
            snapshots += New DataSet With {
                .ID = i,
                .Properties = massTable.ToDictionary(Function(m) m.Key, Function(m) m.Value.Value)
            }
        Next

        Call snapshots.SaveTo("./test_mass.csv")

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
            .Forward = New Regulation,
            .Reverse = New Regulation With {.Activation = pop({"B", "D"}).ToArray}}

        Yield New Channel(pop({"E", "F"}), pop({"A", "G"})) With {
            .Forward = New Regulation,
            .Reverse = New Regulation With {.Activation = pop({"B"}).ToArray}
        }

        Yield New Channel(pop({"B"}), pop({"A", "D"})) With {
            .Forward = New Regulation With {.Activation = pop({"C", "G"}).ToArray},
            .Reverse = New Regulation With {.Activation = pop({"E"}).ToArray}
        }

        Yield New Channel(pop({"G"}), pop({"E"})) With {
            .Forward = New Regulation With {.Activation = pop({"F"}).ToArray}
        }
        Yield New Channel(pop({"E"}), pop({"G", "D", "C"})) With {
            .Forward = New Regulation With {.Activation = pop({"E"}).ToArray},
            .Reverse = New Regulation With {.Activation = pop({"C", "D"}).ToArray}
        }

        Yield New Channel(pop({"B", "F"}), pop({"H"})) With {
            .Forward = New Regulation With {.Activation = pop({"B"}).ToArray},
            .Reverse = New Regulation With {.Activation = pop({"I", "D"}).ToArray}
        }

        Yield New Channel(pop({"I"}), pop({"G"})) With {
           .Forward = New Regulation With {.Activation = pop({"B"}).ToArray},
           .Reverse = New Regulation With {.Activation = pop({"G", "D"}).ToArray}
       }

        Yield New Channel(pop({"H"}), pop({"I", "D"})) With {
           .Forward = New Regulation With {.Activation = pop({"B"}).ToArray},
           .Reverse = New Regulation With {.Activation = pop({"A"}).ToArray}
       }
    End Function
End Module

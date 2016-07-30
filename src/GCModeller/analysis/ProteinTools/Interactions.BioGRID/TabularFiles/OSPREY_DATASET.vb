
Imports Microsoft.VisualBasic.Serialization.JSON
''' <summary>
''' ``BIOGRID-OSPREY_DATASETS-3.4.138.osprey.zip``
''' 
''' This zip archive contains multiple files formatted In Osprey Network Visualization 
''' System Custom Network format containing all interaction And associated annotation 
''' data from the BIOGRID separated into seperate Organisms And Experimental Systems 
''' To be used In Osprey.
''' </summary>
Public Class OSPREY_DATASET

    Public Property GeneA As String
    Public Property GeneB As String
    Public Property ScreenNameA As String
    Public Property ScreenNameB As String
    Public Property ExperimentalSystem As String
    Public Property Source As String
    Public Property PubmedID As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

Imports LANS.SystemsBiology.DatabaseServices.Regprecise
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.DataVisualization.Network
Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' Regulons object that reconstruct from RegPrecise database by using ``bbh`` method.
''' </summary>
Public Class RegPreciseRegulon
    Public Property Regulator As String
    Public Property Family As String
    Public Property Pathway As String
    Public Property BiologicalProcess As String
    Public Property Members As String()
    Public Property hits As String()
    Public Property Effectors As String()
    Public Property Sites As String()

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function ToNetwork(source As IEnumerable(Of RegPreciseRegulon)) As FileStream.Network
        Dim Nodes As Dictionary(Of String, FileStream.Node) =
            New Dictionary(Of String, FileStream.Node)
        For Each x As RegPreciseRegulon In source
            If Not Nodes.ContainsKey(x.Regulator) Then
                Dim TF As New FileStream.Node With {
                    .Identifier = x.Regulator,
                    .NodeType = "TF"
                }
                Call Nodes.Add(x.Regulator, TF)
            End If
            For Each member In x.Members
                If Not Nodes.ContainsKey(member) Then
                    Dim Target As New FileStream.Node With {
                        .Identifier = member,
                        .NodeType = "Member"
                    }
                    Call Nodes.Add(member, Target)
                End If
            Next
        Next

        Dim Regulations = (From x As RegPreciseRegulon
                           In source
                           Select x.Regulator,
                               x.Members
                           Group By Regulator Into Group).ToArray
        Dim edges As New List(Of NetworkEdge)

        For Each xGroup In Regulations
            Dim toNodes As String() =
                xGroup.Group.ToArray(Function(x) x.Members).MatrixToList.Distinct.ToArray
            For Each member As String In toNodes
                Dim edge As New NetworkEdge With {
                    .FromNode = xGroup.Regulator,
                    .ToNode = member,
                    .InteractionType = "Regulates"
                }
                Call edges.Add(edge)
            Next
        Next

        Return New FileStream.Network With {
            .Edges = edges.ToArray,
            .Nodes = Nodes.Values.ToArray
        }
    End Function

    Public Shared Function Merge(source As IEnumerable(Of Regulator)) As RegPreciseRegulon()
        Dim Groups = (From x In source Select x Group x By x.Family.ToLower Into Group).ToArray
        Dim LQuery = (From x In Groups Select __merge(x.Group)).ToArray
        Return LQuery
    End Function

    Private Shared Function __merge(source As IEnumerable(Of Regulator)) As RegPreciseRegulon
        Dim __1st = source.First
        Dim regulates = (From x In source Select x.Regulates.ToArray(Function(xx) xx.LocusId)).MatrixToList
        Dim effectors = (From x In source Select x.Effector Distinct).ToArray
        Dim hits = (From x In source Select x.LocusTag.Value Distinct Order By Value Ascending).ToArray
        Dim sites = (From x In source Select x.RegulatorySites.ToArray(Function(xx) xx.UniqueId)).MatrixToList
        Dim regulon As New RegPreciseRegulon With {
            .Family = __1st.Family,
            .BiologicalProcess = __1st.BiologicalProcess,
            .Members = (From sId As String In regulates Select sId Distinct Order By sId).ToArray,
            .Pathway = __1st.Pathway,
            .Regulator = __1st.LocusId,
            .Effectors = effectors,
            .hits = hits,
            .Sites = (From sId As String In sites Select sId Distinct Order By sId Ascending).ToArray
        }
        Return regulon
    End Function

    Public Shared Function Merges(inDIR As String) As RegPreciseRegulon()
        Dim loads = (From xml As String
                     In FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.xml").AsParallel
                     Select xml.LoadXml(Of BacteriaGenome)).ToArray
        Dim regulons = (From x As BacteriaGenome In loads
                        Where x.NumOfRegulons > 0
                        Select x.Regulons.Regulators).MatrixToList
        Dim Groups = (From xx In (From x As Regulator
                                  In regulons
                                  Select x,
                                      uid = x.BiologicalProcess.Replace(" ", "").ToLower
                                  Group By x.LocusId Into Group).AsParallel
                      Select xx.LocusId,
                          parts = (From x In xx.Group
                                   Select x
                                   Group x By x.uid Into Group) _
                                        .ToDictionary(Function(x) x.uid,
                                                      Function(x) x.Group.ToArray(Function(xxx) xxx.x))).ToArray
        Dim LQuery = (From x
                      In Groups.AsParallel
                      Select x.parts.Values.ToArray(
                          Function(xx) RegPreciseRegulon.Merge(xx))).MatrixToList.MatrixToVector
        Return LQuery
    End Function
End Class
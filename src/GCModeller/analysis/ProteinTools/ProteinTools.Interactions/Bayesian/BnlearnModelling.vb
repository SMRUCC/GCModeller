Imports System.Text
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Public Class BnlearnModelling : Inherits RDotNET.Extensions.VisualBasic.IRScript

    Dim Tokens As String()
    Dim TempData As String

    Sub New(ExperimentalInteractionAssemblies As String())
        Dim csvData As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File = New DocumentFormat.Csv.DocumentStream.File
        Dim rowQuery = (From item In ExperimentalInteractionAssemblies Select Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject.CreateObject((From n In item Select CStr(n)).ToArray)).ToArray
        For i As Long = 0 To rowQuery.First.Count - 1
            Dim p = i
            Dim LQuery = (From row In rowQuery.AsParallel Let ch = row(p) Select ch Distinct).ToArray

            If LQuery.Count = 1 Then '仅有一种状态，则无法进行计算，则必须要替换新字符
                Dim strData As String = LQuery.First
                Console.Write("Replace character at location {0} :  ", p)
                If String.Equals(strData, "-") Then
                    rowQuery.First()(p) = "A"
                    Console.WriteLine("""-"" --> 'A'")
                Else
                    Console.WriteLine("""{0}"" --> '-'", rowQuery.First()(p))
                    rowQuery.First()(p) = "-"
                End If
            End If
        Next

        Tokens = (From handle As Long In rowQuery.First.Count.Sequence Select "aa_" & handle).ToArray
        Call csvData.Add(Tokens)

        For Each row In rowQuery
            Call csvData.AppendLine(row)
        Next

        TempData = My.Computer.FileSystem.SpecialDirectories.Temp & "\bnlearn_modelling_#DEBUG.csv"
        Call csvData.Save(TempData, False)
    End Sub

    Protected Overrides Function __R_script() As String
        Dim scriptBuilder As StringBuilder = New StringBuilder(4096)
        Call scriptBuilder.AppendLine("library(bnlearn)")
        'Call scriptBuilder.AppendLine(createNetwork(Tokens))
        Call scriptBuilder.AppendLine(String.Format("dip_data <- read.csv(""{0}"");", TempData.Replace("\", "/")))
        Call scriptBuilder.AppendLine("dip_network <- iamb(dip_data);")
        Call scriptBuilder.AppendLine("dip_network_model <- bn.fit(dip_network, dip_data, method=""bayes"");")
        Call scriptBuilder.AppendLine("dip_network_model")

        Return scriptBuilder.ToString
    End Function

    ''' <summary>
    ''' 创建一个空网络，并构建好初始的网络结构
    ''' </summary>
    ''' <param name="Tokens"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Friend Shared Function createNetwork(Tokens As String()) As String
        Dim vecBuilder As StringBuilder = New StringBuilder(4096)
        For Each Token As String In Tokens
            Call vecBuilder.Append(String.Format("""{0}"", ", Token))
        Next
        Call vecBuilder.Remove(vecBuilder.Length - 2, 2)

        Dim scriptBuilder As StringBuilder = New StringBuilder(String.Format("dip_network <- empty.graph(c({0}));" & vbCrLf, vecBuilder.ToString), 1024 * 128)
        Call vecBuilder.Clear()
        Call vecBuilder.Append(String.Format("""{0}"", ", Tokens.First))
        For Each Token As String In Tokens.Skip(1)
            Call vecBuilder.Append(String.Format("""{0}"", ""{1}"", ", Token, Token))
        Next
        Call vecBuilder.Remove(vecBuilder.Length - 2 - Len(Tokens.Last) - 4, 6 + Len(Tokens.Last))

        Call scriptBuilder.AppendLine(String.Format("arc.set <- matrix(c({0}), ncol=2, byrow=TRUE, dimnames=list(NULL, c(""from"", ""to"")));", vecBuilder.ToString))
        Call scriptBuilder.AppendLine("arcs(dip_network) <- arc.set")

        Return scriptBuilder.ToString
    End Function

    Public Shared Function Convert(Sequence As Char()) As Integer()
        Dim LQuery = (From c As Char In Sequence Select Asc(c)).ToArray
        Return LQuery
    End Function
End Class

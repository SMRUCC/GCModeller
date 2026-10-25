
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.GPS
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports SMRUCC.genomics.Analysis.GPS.Training
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports SMRUCC.genomics.Analysis.SGrid.GRN
Imports SMRUCC.genomics.Analysis.SGrid.IO

<Package("ssGrid")>
<RTypeExport("ssgrid_network", GetType(SSystemNetwork))>
Module ssGrid

    Sub Main()
        Call RInternal.generic.add("writeBin", GetType(SSystemNetwork), AddressOf saveModel)
        Call RInternal.generic.add("readBin.ssgrid_network", GetType(Stream), AddressOf loadModel)
    End Sub

    Private Function saveModel(model As SSystemNetwork, args As list, env As Environment) As Object
        Dim con As Stream = args!con

        Try
            Call model.Save(con)
            Call con.Flush()
        Catch ex As Exception
            Call App.LogException(ex)
            Call ex.Message.warning
        End Try

        Return True
    End Function

    Private Function loadModel(s As Stream, args As list, env As Environment) As Object
        Return ZipPersists.Load(s)
    End Function
End Module

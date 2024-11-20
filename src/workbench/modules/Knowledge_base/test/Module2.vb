Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI.BITS

Module Module2

    Sub Main()
        Dim doc = "C:\Users\Administrator\Downloads\livertox_NBK547852\Pegcetacoplan.nxml".LoadXml(Of BookPartWrapper)

        Call Console.WriteLine(doc.GetXml)

        doc = "C:\Users\Administrator\Downloads\livertox_NBK547852\Acitretin.nxml".LoadXml(Of BookPartWrapper)

        Call Console.WriteLine(doc.GetXml)

        Pause()
    End Sub
End Module

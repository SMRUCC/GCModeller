Imports Microsoft.VisualBasic.Scripting.Runtime

Module roc_test2

    Sub Main()
        Dim predicts = "E:\biodeep\biodeepdb_v3\biodeepdb_v3\workspace\202410-mslearn\networking_pos_roc\predicts.txt".ReadAllLines.AsDouble
        Dim labels = "E:\biodeep\biodeepdb_v3\biodeepdb_v3\workspace\202410-mslearn\networking_pos_roc\label.txt".ReadAllLines.AsDouble


    End Sub
End Module
